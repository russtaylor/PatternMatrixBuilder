Imports Microsoft.VisualBasic
Imports Amos

Public Class Factor
    Property Name As String
    Property linkedItems As List(Of Item) = New List(Of Item)
    Property pdElement As PDElement

    Public Sub New(name As String)
        Me.Name = name
    End Sub
End Class
