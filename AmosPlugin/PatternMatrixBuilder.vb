Imports Amos
Imports System.Text.RegularExpressions
Imports MiscAmosTypes

<ComponentModel.Composition.Export(GetType(IPlugin))>
Public Class PatternMatrixBuilder
    Implements Amos.IPlugin

    Private Property factors As List(Of Factor) = New List(Of Factor)
    Private Property items As List(Of Item) = New List(Of Item)
    Private Property pageWidth As Double = pd.PageWidth
    Private Property pageHeight As Double = pd.PageHeight
    Private Property verticalSeparation As Double
    Private Property fontSize As Double
    Private Property estimatesChecked As Boolean

    Private Sub ProcessButton_Click(sender As Object, e As EventArgs) Handles ProcessButton.Click
        If IsNullOrWhiteSpace(Me.MatrixInput.Text()) Then
            MsgBox("Please paste your matrix into the box.")
            Return
        End If

        Me.ProcessButton.Enabled = False
        Me.CancelButtonControl.Enabled = False
        Me.MatrixInput.Enabled = False

        Me.ParseData(MatrixInput.Text())
        Me.Close()
    End Sub

    Public Function Description() As String Implements IPlugin.Description
        Return "Draws an Amos model from an SPSS pattern matrix."
    End Function

    Public Function MainSub() As Integer Implements IPlugin.MainSub
        While DataFileIsEmpty() = True
            If MsgBox("Please specify a data file to continue.", MsgBoxStyle.OkCancel) = 2 Then
                Return 0
            End If
            pd.FileDataFiles()
        End While

        Me.ShowDialog()
        Me.MatrixInput.Focus()
        Return 1
    End Function

    Public Function Name1() As String Implements IPlugin.Name
        Return "Pattern Matrix Model Builder"
    End Function

    Private Sub PatternMatrixBuilder_Closed(ByVal sender As Object,
                                            ByVal e As System.EventArgs) Handles MyBase.FormClosed
        Me.Dispose()
    End Sub

    Private Sub CancelButtonControl_Click(sender As Object,
                                          e As EventArgs) Handles CancelButtonControl.Click
        Me.Close()
    End Sub

    Private Function DataFileIsEmpty() As Boolean
        Dim dbFormat As cDatabaseFormat
        Dim fileName As String = ""
        Dim tableName As String = ""
        Dim groupingVariable As String = ""
        Dim groupingValue As Object = vbNull
        pd.GetDataFile(1, dbFormat, fileName, tableName, groupingVariable, groupingValue)

        If IsNullOrWhiteSpace(fileName) Then
            Return True
        Else
            Return False
        End If
    End Function

    Private Function IsNullOrWhiteSpace(inputString As String) As Boolean
        If String.IsNullOrEmpty(inputString) Then
            Return True
        End If

        If inputString.Trim().Length = 0 Then
            Return True
        End If

        Return False
    End Function

    Private Sub CheckMeansAndIntercepts()
        estimatesChecked = pd.GetCheckBox("AnalysisPropertiesForm", "MeansInterceptsCheck").Checked
        pd.GetCheckBox("AnalysisPropertiesForm", "MeansInterceptsCheck").Checked = True
    End Sub

    Private Sub ResetMeansAndIntercepts()
        pd.GetCheckBox("AnalysisPropertiesForm", "MeansInterceptsCheck").Checked = estimatesChecked
    End Sub

    Private Sub ParseData(matrixInput As String)
        CheckMeansAndIntercepts()
        ' It's necessary to encapsulate the inputData into a StringReader so that the 
        ' TextFieldParser will recognize it.
        Using textParser As New Microsoft.VisualBasic.FileIO.TextFieldParser(New System.IO.
                                                                     StringReader(matrixInput))
            textParser.TextFieldType = FileIO.FieldType.Delimited
            textParser.SetDelimiters(vbTab)

            Dim currentRow As String()
            While Not textParser.EndOfData
                Try
                    ' Process the current row.
                    currentRow = textParser.ReadFields()

                    ' Check if this row contains the factors themselves.
                    If factors.Count = 0 And IsFactorRow(currentRow) Then
                        ' Use the cells (after cell 1) to create the dependent variables
                        Dim arrayLength As Integer = currentRow.Length - 1
                        For index As Integer = 1 To arrayLength

                            factors.Add(New Factor(currentRow(index)))
                        Next index
                    End If

                    ' Check if this row is at the end of the table
                    Dim regexObject As Regex = New Regex("^Extraction")
                    If (regexObject.IsMatch(currentRow(0)) Or currentRow.Length < 0) Then
                        If factors.Count = 0 Then
                            MsgBox("It doesn't look like there were any factors. Please try again.")
                        End If
                        Exit While
                    End If

                    'If we've found the factor row, we'll assume we're into the items
                    If factors.Count > 0 And Not (IsFactorRow(currentRow)) Then
                        ' Find the largest loading in the list of factors
                        Dim maxLoading As Double = 0
                        Dim maxLoadingIndex As Integer
                        Dim i As Integer
                        For i = 1 To currentRow.Length - 1
                            If Not (IsNullOrWhiteSpace(currentRow(i))) Then
                                If CDbl(currentRow(i)) > maxLoading Then
                                    maxLoadingIndex = i
                                    maxLoading = CDbl(currentRow(i))
                                End If
                            End If
                        Next

                        Dim loadingFactor As Factor = factors(maxLoadingIndex - 1)
                        Dim item As Item = New Item(currentRow(0), loadingFactor)
                        loadingFactor.linkedItems.Add(item)
                        items.Add(item)
                    End If
                Catch ex As Exception

                End Try
            End While

            ClearCanvas()

            CreateUnobservedVarsFromFactors()
            CreateObservedVars()

            resizeObserved()
            drawCovariances()
            touchUpAll()

            linkData()
            ResetMeansAndIntercepts()
            pd.EditSelect()
        End Using
    End Sub

    Private Sub ClearCanvas()
        While pd.PDElements.Count > 0
            pd.EditErase(pd.PDElements(1))
        End While
    End Sub

    Private Sub CreateUnobservedVarsFromFactors()
        verticalSeparation = (pageHeight - 72) / (items.Count + factors.Count - 1)
        fontSize = verticalSeparation / 2
        If fontSize > 20 Then
            fontSize = 20
        End If

        Dim horizontalPosition As Double = pageWidth * 0.5

        ' Start the spacing half an inch below the top of the page
        Dim vPos As Double = 72 / 2

        ' Draw each variable
        For index As Integer = 0 To factors.Count - 1
            Dim verticalPosition As Double = (verticalSeparation * factors(index).
                                              linkedItems.Count) / 2 + vPos
            Dim unobservedElement As PDElement = pd.DiagramDrawUnobserved(horizontalPosition,
                                                         verticalPosition, 0.5, 0.7)
            unobservedElement.NameOrCaption = factors(index).Name
            factors(index).pdElement = unobservedElement
            unobservedElement.Width = fontSize * 4.5
            unobservedElement.Height = fontSize * 3

            unobservedElement.NameFontSize = CSng(fontSize)

            vPos = vPos + verticalSeparation * (factors(index).linkedItems.Count + 1)

            pd.Refresh()
        Next index
    End Sub

    Private Sub linkData()
        Dim amosEngine As New AmosEngineLib.AmosEngine

        pd.SpecifyModel(amosEngine)

        amosEngine.FitModel()
        amosEngine.Dispose()
    End Sub

    Private Function inchesToPoints(inches As Double) As Double
        Return inches * 72
    End Function

    Private Function pointsToInches(points As Double) As Double
        Return points / 72
    End Function

    Private Sub CreateObservedVars()
        Dim horizontalPosition As Double = pageWidth * 0.25
        Dim errorCounter As Integer = 1
        Dim errorhPos As Double = pageWidth * (0.1 + ((1 - ((fontSize * 6) / 100)) * 0.1))

        ' Loop through each factor
        For factorIndex As Integer = 0 To factors.Count - 1
            ' Set up the positioning for the factors
            Dim currentFactor As Factor = factors(factorIndex)
            Dim factorPosition As Double = currentFactor.pdElement.originY
            Dim startVerticalPosition As Double = factorPosition - ((currentFactor.linkedItems.Count - 1) *
                                                          verticalSeparation) / 2
            ' Loop through each item in the current factor
            For itemIndex As Integer = 0 To currentFactor.linkedItems.Count - 1
                ' Position the element
                Dim verticalPosition As Double = startVerticalPosition + (itemIndex * verticalSeparation)

                ' Create the element
                Dim observedElement As PDElement = pd.DiagramDrawObserved(horizontalPosition,
                                                         verticalPosition, 0.2, 0.1)
                observedElement.Height = fontSize
                observedElement.Width = fontSize * 4

                observedElement.NameOrCaption = currentFactor.linkedItems(itemIndex).Name
                observedElement.NameFontSize = CSng(fontSize)
                currentFactor.linkedItems(itemIndex).pdElement = observedElement


                Dim errorElement As PDElement = pd.DiagramDrawUnobserved(
                                                        errorhPos,
                                                        verticalPosition,
                                                        fontSize * 1.75,
                                                        fontSize * 1.75)
                Dim errorPath As PDElement = pd.DiagramDrawPath(errorElement, observedElement)
                errorPath.Value1 = CType(1, String)
                errorElement.NameFontSize = CSng(fontSize)
                errorElement.NameOrCaption = "e" & errorCounter
                errorCounter = errorCounter + 1

                ' Add the path.
                Dim path As PDElement = pd.DiagramDrawPath(currentFactor.pdElement, observedElement)
                ' If this is the first item, add a regression weight of 1
                If itemIndex = 0 Then
                    path.Value1 = CType(1, String)
                End If
            Next
        Next
        pd.Refresh()
    End Sub

    Private Function IsFactorRow(checkRow As Array) As Boolean
        ' Check if the row has variables in every row (except the first)
        Dim arraySize As Integer = checkRow.Length - 1
        For index As Integer = 0 To arraySize
            Dim currentRow As String = CType(checkRow.GetValue(index), String)
            If index = 0 And IsNullOrWhiteSpace(currentRow) = False Then
                Return False
            End If
            If index <> 0 And IsNullOrWhiteSpace(currentRow) Then
                Return False
            End If
        Next index

        Return True
    End Function

    Private Sub touchUpAll()
        Dim element As PDElement
        For Each element In pd.PDElements
            pd.EditTouchUp(element)
        Next
    End Sub

    Private Sub drawCovariances()
        For index As Integer = 0 To factors.Count - 1
            Dim currentFactor As Factor = factors(index)
            For innerIndex As Integer = index + 1 To factors.Count - 1
                Dim linkFactor As Factor = factors(innerIndex)
                pd.DiagramDrawCovariance()
                pd.DragMouse(currentFactor.pdElement,
                             linkFactor.pdElement.originX, linkFactor.pdElement.originY)
                pd.DiagramDrawCovariance()
            Next
        Next
    End Sub

    Private Sub resizeObserved()
        Dim x As PDElement
        Dim LargestWidth As Single, LargestHeight As Single
        LargestWidth = 0
        LargestHeight = 0
        For Each x In pd.PDElements
            If x.IsObservedVariable Then
                If x.NameWidth > LargestWidth Then LargestWidth = CSng(x.NameWidth)
                If x.NameHeight > LargestHeight Then LargestHeight = CSng(x.NameHeight)
            End If
        Next
        LargestWidth = CSng(LargestWidth * 1.4)
        LargestHeight = CSng(LargestHeight * 1.4)
        pd.UndoToHere()
        If LargestWidth > 0.2 And LargestHeight > 0.1 Then
            For Each x In pd.PDElements
                If x.IsObservedVariable Then
                    x.Width = LargestWidth
                    x.Height = LargestHeight
                End If
            Next
            pd.Refresh()
        End If
        pd.DiagramRedrawDiagram()
        pd.UndoResume()
    End Sub
End Class