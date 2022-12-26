Imports System.Drawing
Imports SkiaSharp

Public Class Ean8
    Inherits Ean

    Private Shared GUARDS() As Integer = {0, 2, 20, 22, 40, 42}
    Private Shared CHARPOS() As Single = {7, 14, 21, 28, 39, 46, 53, 60}

    Public Function Encode(ByVal data As List(Of Byte)) As Byte()
        Dim cs As New List(Of Byte)
        cs.AddRange(START_PATTERN)
        For i As Integer = 0 To 3
            addCodes(cs, data(i))
        Next
        cs.AddRange(CENTER_PATTERN)
        For i As Integer = 4 To 7
            addCodes(cs, data(i))
        Next
        cs.AddRange(STOP_PATTERN)
        Return cs.ToArray
    End Function

    Public Function PreprocessData(ByVal data As String) As List(Of Byte)
        Dim ret As List(Of Byte) = pack(data)
        If ret.Count = 7 Then
            ret.Add(Me.CalcCheckDigit(ret))
        End If
        If ret.Count <> 8 Then
            Throw New ArgumentException("(ean8)データは7桁(チェックディジットを含めるなら8桁)でなければいけません: " & data)
        End If
        Return ret
    End Function

    Public Overrides Sub Render(canvas As SKCanvas, r As SKRect, data As String)
        If data Is Nothing OrElse data.Length = 0 Then
            Exit Sub
        End If
        Dim w As Single = r.Width - Me.MarginX * 2
        Dim h As Single = r.Height - Me.MarginY * 2
        Dim _h1 As Single = h
        Dim _h2 As Single = h
        If Me.WithText Then
            _h1 *= 0.7F
            _h2 *= 0.8F
        End If
        If w <= 0 Or h <= 0 Then
            Exit Sub
        End If
        Dim _data As List(Of Byte) = Me.PreprocessData(data)
        Dim mw As Single = w / (8 * 7 + 11)
        With Nothing
            Dim cs() As Byte = Encode(_data)
            Dim draw As Boolean = True
            Dim x As Single = r.Left + MarginX
            Dim y As Single = r.Top + MarginY
            Dim paint As New SKPaint With {
              .Color = SKColors.Black,
              .Style = SKPaintStyle.Fill
            }
            For i As Integer = 0 To cs.Length - 1
                Dim dw As Single = cs(i) * mw
                If draw Then
                    canvas.DrawRect(x, y, dw * BarWidth, IIf(GUARDS.Contains(i), _h2, _h1), paint)
                End If
                draw = Not draw
                x += dw
            Next
        End With
        If Me.WithText Then
            Dim fs = GetFontSize("00000000", w, h * 0.2)
            Dim paint As New SKPaint With {
              .TextSize = fs,
              .Color = SKColors.Black,
              .Style = SKPaintStyle.Fill,
              .IsAntialias = True,
              .Typeface = Me.Typeface
            }
            For i As Integer = 0 To 7
                canvas.DrawText(_data(i), r.Left + MarginX + CHARPOS(i) * mw - fs / 4, r.Top + MarginY + _h1 + fs * 0.8, paint)
            Next
        End If
    End Sub

End Class
