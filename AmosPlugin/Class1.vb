Imports System.Windows.Forms

Public Class Class1
    Implements AmosGraphics.IPlugin
    Dim WithEvents pd As AmosGraphics.pd

    Public Function Description() As String Implements AmosGraphics.IPlugin.Description
        Return "A simple example of a plugin."
    End Function

    Public Function MainSub() As Integer Implements AmosGraphics.IPlugin.MainSub
        MsgBox("Installing a simple plugin")
        AddHandler AmosGraphics.pd.MouseUp, AddressOf pd_MouseUp
    End Function

    Public Function Name() As String Implements AmosGraphics.IPlugin.Name
        Return "A simple plugin"
    End Function

    Private Sub pd_MouseUp(button As MouseButtons, shift As Integer, x As Double, y As Double) Handles pd.MouseUp
        MsgBox("You released a mouse button.")
        RemoveHandler AmosGraphics.pd.MouseUp, AddressOf pd_MouseUp
    End Sub
End Class
