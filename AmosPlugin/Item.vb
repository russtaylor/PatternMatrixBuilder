Imports Microsoft.VisualBasic
Imports AmosGraphics

Public Class Item
    Property Name As String
    Property Factor As Factor
    Property pdElement As PDElement

    Public Sub New(name As String, factor As Factor)
        Me.Name = name
        Me.Factor = factor
    End Sub
End Class
