Imports System.Drawing
Imports System.Math
Imports SkiaSharp

Public MustInherit Class Barcode

    Public Shared BarWidth As Single = 1.0F

    Public MarginX As Single = 2.0F
    Public MarginY As Single = 2.0F

    Public WithText As Boolean = True

    'Public Function GetFontSize(ByVal g As Graphics, ByVal txt As String, ByVal w As Single, ByVal h As Single) As Single
    '    Dim r As Single = g.MeasureString("0", GetFont(10)).Height * 0.08
    '    Dim ret As Single = h * 0.2F
    '    ret = Math.Min(ret, ((w * 0.9F) / (txt.Length * r)) * 2.0F)
    '    ret = Math.Max(ret, 6.0F)
    '    Return ret
    'End Function

    'Public Function GetFont(ByVal fontSize As Single) As Font
    '    Return New Font("Arial", fontSize)
    'End Function

    Public Sub Render(canvas As SKCanvas, x As Single, y As Single, w As Single, h As Single, data As String)
        Render(canvas, New SKRect(x, y, x + w, y + h), data)
    End Sub

    Public MustOverride Sub Render(canvas As SKCanvas, r As SKRect, data As String)

End Class
