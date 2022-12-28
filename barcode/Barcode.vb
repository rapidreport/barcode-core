Imports System.Drawing
Imports System.Math
Imports System.Reflection

Public MustInherit Class Barcode

    Public Class Shape

        Public Class Bar
            Public X As Single
            Public Y As Single
            Public W As Single
            Public H As Single
            Public Sub New(x As Single, y As Single, w As Single, h As Single)
                Me.X = x
                Me.Y = y
                Me.W = w
                Me.H = h
            End Sub
        End Class

        Public Class Text
            Public Text As String
            Public X As Single
            Public Y As Single
            Public W As Single
            Public H As Single
            Public Sub New(text As String, x As Single, y As Single)
                Me.Text = text
                Me.X = x
                Me.Y = y
                Me.W = 0
                Me.H = 0
            End Sub
            Public Sub New(text As String, x As Single, y As Single, w As Single, h As Single)
                Me.Text = text
                Me.X = x
                Me.Y = y
                Me.W = w
                Me.H = h
            End Sub

        End Class

        Public Bars As New List(Of Bar)
        Public Texts As New List(Of Text)
        Public FontSize As Single = 0

    End Class

    Public Shared BarWidth As Single = 1.0F

    Public MarginX As Single = 2.0F
    Public MarginY As Single = 2.0F

    Public WithText As Boolean = True

    Public Function GetFontSize(text As String, w As Single, h As Single) As Single
        Return Math.Max(Math.Min((w / (text.Length + 0.5)) * 2, h), h * 0.2)
    End Function

    Public MustOverride Function CreateShape(x As Single, y As Single, w As Single, h As Single, data As String) As Shape

End Class
