Imports AmosGraphics
Imports Microsoft.VisualBasic.FileIO.TextFieldParser

Public Class PatternMatrixBuilder
    Implements AmosGraphics.IPlugin

    Private Sub InputDialog_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub ProcessButton_Click(sender As Object, e As EventArgs) Handles ProcessButton.Click
        Me.ParseData(TextBox1.Text())
        Me.Close()
    End Sub

    Public Function Description() As String Implements IPlugin.Description
        Return "Draws an Amos model from an SPSS pattern matrix."
    End Function

    Public Function MainSub() As Integer Implements IPlugin.MainSub
        Me.Show()
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

    Private Sub ParseData(inputData As String)
        Using InputString As New System.IO.StringReader(inputData)
            Using textParser As New Microsoft.VisualBasic.FileIO.TextFieldParser(New System.IO.StringReader(inputData))
                textParser.TextFieldType = FileIO.FieldType.Delimited
                textParser.SetDelimiters(vbTab)

                Dim currentRow As String()
                While Not textParser.EndOfData
                    Try
                        currentRow = textParser.ReadFields()
                        Dim currentField As String
                        For Each currentField In currentRow
                            MsgBox(currentField)
                        Next
                    Catch ex As Exception

                    End Try
                End While
            End Using
        End Using
    End Sub
End Class