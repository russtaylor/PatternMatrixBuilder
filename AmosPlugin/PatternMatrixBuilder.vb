﻿Imports AmosGraphics
Imports Microsoft.VisualBasic.FileIO.TextFieldParser
Imports System.Text.RegularExpressions
Imports PatternMatrixBuilder.Factor

Public Class PatternMatrixBuilder
    Implements AmosGraphics.IPlugin

    Private Property factors As List(Of Factor) = New List(Of Factor)
    Private Property items As List(Of Item) = New List(Of Item)
    Private Property pageWidth As Double = pd.PageWidth
    Private Property pageHeight As Double = pd.PageHeight

    Private Sub InputDialog_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub ProcessButton_Click(sender As Object, e As EventArgs) Handles ProcessButton.Click
        If String.IsNullOrWhiteSpace(Me.MatrixInput.Text()) Then
            MsgBox("Please paste your matrix into the box.")
            Return
        End If

        Me.ProcessButton.Enabled = False
        Me.CancelButtonControl.Enabled = False
        Me.MatrixInput.Enabled = False
        Me.InputFilePath.Enabled = False

        Me.ParseData(InputFilePath.Text(), MatrixInput.Text())
        Me.Close()
    End Sub

    Public Function Description() As String Implements IPlugin.Description
        Return "Draws an Amos model from an SPSS pattern matrix."
    End Function

    Public Function MainSub() As Integer Implements IPlugin.MainSub
        Me.Show()
        Me.InputFilePath.Focus()
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

    Private Sub ParseData(inputPath As String, matrixInput As String)
        ' It's necessary to encapsulate the inputData into a StringReader so that the 
        ' TextFieldParser will recognize it.
        Using textParser As New Microsoft.VisualBasic.FileIO.TextFieldParser(New System.IO.StringReader(matrixInput))
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
                    Dim regexObject = New Regex("^Extraction")
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
                        For i = 1 To currentRow.Length - 1
                            If Not (String.IsNullOrWhiteSpace(currentRow(i))) Then
                                If currentRow(i) > maxLoading Then
                                    maxLoadingIndex = i
                                    maxLoading = currentRow(i)
                                End If
                            End If
                        Next

                        Dim loadingFactor = factors(maxLoadingIndex - 1)
                        Dim item As Item = New Item(currentRow(0), loadingFactor)
                        loadingFactor.linkedItems.Add(item)
                        items.Add(item)
                    End If
                Catch ex As Exception

                End Try
            End While

            CreateUnobservedVarsFromFactors()
            CreateObservedVars()

            resizeObserved()
            moveUniqueVariables()
            drawCovariances()
            touchUpAll()

            linkData(inputPath)
        End Using
    End Sub

    Private Sub CreateUnobservedVarsFromFactors()
        Dim verticalSeparation As Double = pageHeight / factors.Count
        Dim horizontalPosition As Double = pageWidth * 0.5

        ' Draw each variable
        For index As Integer = 0 To factors.Count - 1
            Dim verticalPosition As Double = verticalSeparation * index + verticalSeparation * 0.5
            Dim unobservedElement = pd.DiagramDrawUnobserved(horizontalPosition,
                                                         verticalPosition, 0.5, 0.7)
            unobservedElement.NameOrCaption = factors(index).Name
            factors(index).pdElement = unobservedElement
            pd.Refresh()
        Next index
    End Sub

    Private Sub linkData(inputPath As String)
        Dim amosEngine As New AmosEngineLib.AmosEngine

        If Not (String.IsNullOrWhiteSpace(inputPath)) Then
            pd.SetDataFile(1, MiscAmosTypes.cDatabaseFormat.mmSPSS, inputPath, "", "", 0)
        End If

        pd.SpecifyModel(amosEngine)

        amosEngine.FitModel()
        amosEngine.Dispose()
    End Sub

    Private Sub CreateObservedVars()
        Dim originalVerticalSeparation As Double = pageHeight / items.Count
        Dim verticalSeparation = originalVerticalSeparation * 0.65
        Dim horizontalPosition As Double = pageWidth * 0.25

        ' Loop through each factor
        For factorIndex As Integer = 0 To factors.Count - 1
            ' Set up the positioning for the factors
            Dim currentFactor As Factor = factors(factorIndex)
            Dim factorPosition As Double = currentFactor.pdElement.originY
            Dim startVerticalPosition = factorPosition - ((currentFactor.linkedItems.Count - 1) *
                                                          verticalSeparation) / 2
            ' Loop through each item in the current factor
            For itemIndex As Integer = 0 To currentFactor.linkedItems.Count - 1
                ' Position the element
                Dim verticalPosition = startVerticalPosition + (itemIndex * verticalSeparation)

                ' Create the element
                Dim observedElement = pd.DiagramDrawObserved(horizontalPosition,
                                                         verticalPosition, 0.7, 0.5)
                observedElement.NameOrCaption = currentFactor.linkedItems(itemIndex).Name
                currentFactor.linkedItems(itemIndex).pdElement = observedElement

                pd.DiagramDrawUniqueVariable(observedElement)

                ' Add the path.
                Dim path = pd.DiagramDrawPath(currentFactor.pdElement, observedElement)
                ' If this is the first item, add a regression weight of 1
                If itemIndex = 0 Then
                    path.Value1 = 1
                End If
            Next
        Next
    End Sub

    Private Function IsFactorRow(checkRow As Array)
        ' Check if the row has variables in every row (except the first)
        Dim arraySize = checkRow.Length - 1
        For index As Integer = 0 To arraySize
            If index = 0 And String.IsNullOrWhiteSpace(checkRow(index)) = False Then
                Return False
            End If
            If index <> 0 And String.IsNullOrWhiteSpace(checkRow(index)) Then
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

    Private Sub moveUniqueVariables()
        Dim element As PDElement
        Dim uniquePrefix As String = "e"
        Dim uniqueCounter As Integer = 1
        For Each element In pd.PDElements
            If element.IsUniqueVariable Then
                element.originX = pageWidth * 0.1
                element.Width = 25
                element.Height = 25
                element.NameFontSize = 10
                element.NameOrCaption = uniquePrefix & uniqueCounter
                uniqueCounter += 1
            End If
        Next
    End Sub

    Private Sub drawCovariances()
        For index As Integer = 0 To factors.Count - 1
            Dim currentFactor = factors(index)
            For innerIndex As Integer = index + 1 To factors.Count - 1
                Dim linkFactor = factors(innerIndex)
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
                If x.NameWidth > LargestWidth Then LargestWidth = x.NameWidth
                If x.NameHeight > LargestHeight Then LargestHeight = x.NameHeight
            End If
        Next
        LargestWidth = LargestWidth * 1.4
        LargestHeight = LargestHeight * 1.4
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

    Private Sub Label2_Click(sender As Object, e As EventArgs) Handles Label2.Click

    End Sub
End Class