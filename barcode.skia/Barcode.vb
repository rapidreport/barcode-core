Imports System.Drawing
Imports System.Math
Imports System.Reflection
Imports SkiaSharp

Public MustInherit Class Barcode

    Public Shared BarWidth As Single = 1.0F

    Public MarginX As Single = 2.0F
    Public MarginY As Single = 2.0F

    Public WithText As Boolean = True

    Public Typeface As SKTypeface = SKTypeface.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("jp.co.systembase.barcode.skia.NotoSans-Regular.ttf"))

    Public Function GetFontSize(ByVal txt As String, ByVal w As Single, ByVal h As Single) As Single
        Dim p As New SKPaint With {
          .TextSize = 1,
          .Typeface = Me.Typeface
        }
        Return Math.Max(Math.Min(h, w / p.MeasureText(txt)), h * 0.2F)
    End Function

    Public Sub Render(canvas As SKCanvas, x As Single, y As Single, w As Single, h As Single, data As String)
        Render(canvas, New SKRect(x, y, x + w, y + h), data)
    End Sub

    Public MustOverride Sub Render(canvas As SKCanvas, r As SKRect, data As String)

End Class
