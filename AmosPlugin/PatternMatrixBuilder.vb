Imports AmosGraphics

Public Class PatternMatrixBuilder
    Implements AmosGraphics.IPlugin

    Private Sub InputDialog_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub ProcessButton_Click(sender As Object, e As EventArgs) Handles ProcessButton.Click

    End Sub

    Public Function Description() As String Implements IPlugin.Description
        Return "Draws an Amos model from an SPSS pattern matrix."
    End Function

    Public Function MainSub() As Integer Implements IPlugin.MainSub
        Me.Show()
    End Function

    Public Function Name1() As String Implements IPlugin.Name
        Return "Pattern Matrix Model Builder"
    End Function

    Private Sub PatternMatrixBuilder_Closed(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.FormClosed
        Me.Dispose()
    End Sub

    Private Sub CancelButtonControl_Click(sender As Object, e As EventArgs) Handles CancelButtonControl.Click
        Me.Close()
    End Sub
End Class