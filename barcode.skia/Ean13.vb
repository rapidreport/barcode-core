Imports System.Buffers
Imports System.Drawing
Imports System.Net.Mime.MediaTypeNames
Imports SkiaSharp

Public Class Ean13
    Inherits Ean

    Protected Shared PREF_PARITY(,) As Byte =
      {{0, 0, 0, 0, 0, 0},
       {0, 0, 1, 0, 1, 1},
       {0, 0, 1, 1, 0, 1},
       {0, 0, 1, 1, 1, 0},
       {0, 1, 0, 0, 1, 1},
       {0, 1, 1, 0, 0, 1},
       {0, 1, 1, 1, 0, 0},
       {0, 1, 0, 1, 0, 1},
       {0, 1, 0, 1, 1, 0},
       {0, 1, 1, 0, 1, 0}}

    Private Shared GUARDS() As Integer = {0, 2, 28, 30, 56, 58}
    Private Shared CHARPOS() As Single = {4, 14, 21, 28, 35, 42, 49, 61, 68, 75, 81, 88, 95}

    Public Function Encode(ByVal data As List(Of Byte)) As Byte()
        Dim cs As New List(Of Byte)
        cs.AddRange(START_PATTERN)
        For i As Integer = 1 To 6
            If PREF_PARITY(data(0), i - 1) Then
                addCodesEven(cs, data(i))
            Else
                addCodes(cs, data(i))
            End If
        Next
        cs.AddRange(CENTER_PATTERN)
        For i As Integer = 7 To 12
            addCodes(cs, data(i))
        Next
        cs.AddRange(STOP_PATTERN)
        Return cs.ToArray
    End Function

    Public Function PreprocessData(ByVal data As String) As List(Of Byte)
        Dim ret As List(Of Byte) = pack(data)
        If ret.Count = 12 Then
            ret.Add(Me.CalcCheckDigit(ret))
        End If
        If ret.Count <> 13 Then
            Throw New ArgumentException("(ean13)データは12桁(チェックディジットを含めるなら13桁)でなければいけません: " & data)
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
        Dim mw As Single
        With Nothing
            Dim cs() As Byte = Me.Encode(_data)
            Dim x As Single
            If Me.WithText Then
                mw = w / (12 * 7 + 18)
                x = r.Left + MarginX + mw * 7
            Else
                mw = w / (12 * 7 + 11)
                x = r.Left + MarginX
            End If
            Dim y As Single = r.Top + MarginY
            Dim draw As Boolean = True
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
            Dim fs = GetFontSize("0000000000000", w, h * 0.2)
            Dim paint As New SKPaint With {
              .TextSize = fs,
              .Color = SKColors.Black,
              .Style = SKPaintStyle.Fill,
              .IsAntialias = True,
              .Typeface = Me.Typeface
            }
            For i As Integer = 0 To 12
                canvas.DrawText(_data(i), r.Left + MarginX + CHARPOS(i) * mw - fs / 4, r.Top + MarginY + _h1 + fs * 0.8, paint)
            Next
        End If
    End Sub

End Class