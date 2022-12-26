Imports System.Drawing
Imports System.Net.NetworkInformation
Imports SkiaSharp

Public Class Code39
    Inherits Barcode

    Private Shared CODE_PATTERNS(,) As Byte =
      {{0, 0, 0, 1, 1, 0, 1, 0, 0},
       {1, 0, 0, 1, 0, 0, 0, 0, 1},
       {0, 0, 1, 1, 0, 0, 0, 0, 1},
       {1, 0, 1, 1, 0, 0, 0, 0, 0},
       {0, 0, 0, 1, 1, 0, 0, 0, 1},
       {1, 0, 0, 1, 1, 0, 0, 0, 0},
       {0, 0, 1, 1, 1, 0, 0, 0, 0},
       {0, 0, 0, 1, 0, 0, 1, 0, 1},
       {1, 0, 0, 1, 0, 0, 1, 0, 0},
       {0, 0, 1, 1, 0, 0, 1, 0, 0},
       {1, 0, 0, 0, 0, 1, 0, 0, 1},
       {0, 0, 1, 0, 0, 1, 0, 0, 1},
       {1, 0, 1, 0, 0, 1, 0, 0, 0},
       {0, 0, 0, 0, 1, 1, 0, 0, 1},
       {1, 0, 0, 0, 1, 1, 0, 0, 0},
       {0, 0, 1, 0, 1, 1, 0, 0, 0},
       {0, 0, 0, 0, 0, 1, 1, 0, 1},
       {1, 0, 0, 0, 0, 1, 1, 0, 0},
       {0, 0, 1, 0, 0, 1, 1, 0, 0},
       {0, 0, 0, 0, 1, 1, 1, 0, 0},
       {1, 0, 0, 0, 0, 0, 0, 1, 1},
       {0, 0, 1, 0, 0, 0, 0, 1, 1},
       {1, 0, 1, 0, 0, 0, 0, 1, 0},
       {0, 0, 0, 0, 1, 0, 0, 1, 1},
       {1, 0, 0, 0, 1, 0, 0, 1, 0},
       {0, 0, 1, 0, 1, 0, 0, 1, 0},
       {0, 0, 0, 0, 0, 0, 1, 1, 1},
       {1, 0, 0, 0, 0, 0, 1, 1, 0},
       {0, 0, 1, 0, 0, 0, 1, 1, 0},
       {0, 0, 0, 0, 1, 0, 1, 1, 0},
       {1, 1, 0, 0, 0, 0, 0, 0, 1},
       {0, 1, 1, 0, 0, 0, 0, 0, 1},
       {1, 1, 1, 0, 0, 0, 0, 0, 0},
       {0, 1, 0, 0, 1, 0, 0, 0, 1},
       {1, 1, 0, 0, 1, 0, 0, 0, 0},
       {0, 1, 1, 0, 1, 0, 0, 0, 0},
       {0, 1, 0, 0, 0, 0, 1, 0, 1},
       {1, 1, 0, 0, 0, 0, 1, 0, 0},
       {0, 1, 1, 0, 0, 0, 1, 0, 0},
       {0, 1, 0, 1, 0, 1, 0, 0, 0},
       {0, 1, 0, 1, 0, 0, 0, 1, 0},
       {0, 1, 0, 0, 0, 1, 0, 1, 0},
       {0, 0, 0, 1, 0, 1, 0, 1, 0},
       {0, 1, 0, 0, 1, 0, 1, 0, 0}}

    Private Const CHARS As String = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ-. $/+%*"
    Private Const START_STOP_POINT As Integer = 43

    Public GenerateCheckSum As Boolean = False
    Public WithCheckSumText As Boolean = False

    Public Function Encode(ByVal codePoints As List(Of Integer)) As Byte()
        Dim ret As New List(Of Byte)
        For Each p As Integer In codePoints
            Me.addCodes(ret, p)
        Next
        Return ret.ToArray
    End Function

    Public Function GetCodePoints(ByVal data As String) As List(Of Integer)
        Dim ret As New List(Of Integer)
        For Each c As Char In data
            Dim p As Integer = CHARS.IndexOf(c)
            If p >= 0 Then
                ret.Add(p)
            Else
                Throw New ArgumentException("(code39)不正なデータです: " & data)
            End If
        Next
        Return ret
    End Function

    Public Function CalcCheckDigit(ByVal ps As List(Of Integer)) As Integer
        Dim s As Integer = 0
        For Each p As Integer In ps
            s += p
        Next
        Return s Mod 43
    End Function

    Public Sub AddStartStopPoint(ByVal codePoints As List(Of Integer))
        codePoints.Add(START_STOP_POINT)
    End Sub

    Public Function AddStartStopText(ByVal txt As String) As String
        Return "*" & txt & "*"
    End Function

    Private Sub addCodes(ByVal l As List(Of Byte), ByVal p As Integer)
        If l.Count > 0 Then
            l.Add(0)
        End If
        l.Add(CODE_PATTERNS(p, 0))
        l.Add(CODE_PATTERNS(p, 1))
        l.Add(CODE_PATTERNS(p, 2))
        l.Add(CODE_PATTERNS(p, 3))
        l.Add(CODE_PATTERNS(p, 4))
        l.Add(CODE_PATTERNS(p, 5))
        l.Add(CODE_PATTERNS(p, 6))
        l.Add(CODE_PATTERNS(p, 7))
        l.Add(CODE_PATTERNS(p, 8))
    End Sub

    Public Overrides Sub Render(canvas As SKCanvas, r As SKRect, data As String)
        If data Is Nothing OrElse data.Length = 0 Then
            Exit Sub
        End If
        Dim w As Single = r.Width - Me.MarginX * 2
        Dim h As Single = r.Height - Me.MarginY * 2
        Dim _h As Single = h
        If Me.WithText Then
            _h *= 0.7F
        End If
        If w <= 0 Or h <= 0 Then
            Exit Sub
        End If
        Dim ps As New List(Of Integer)
        Dim txt As String = data
        With Nothing
            Me.AddStartStopPoint(ps)
            Dim _ps As List(Of Integer) = Me.GetCodePoints(data)
            ps.AddRange(_ps)
            If Me.GenerateCheckSum Then
                Dim cd As Integer = Me.CalcCheckDigit(_ps)
                ps.Add(cd)
                If Me.WithCheckSumText Then
                    txt &= CHARS(cd)
                End If
            End If
            Me.AddStartStopPoint(ps)
            txt = Me.AddStartStopText(txt)
        End With
        With Nothing
            Dim cs As Byte() = Me.Encode(ps)
            Dim mw As Single = w / (ps.Count * 13 - 1)
            Dim draw As Boolean = True
            Dim x As Single = r.Left + Me.MarginX
            Dim y As Single = r.Top + Me.MarginY
            Dim paint As New SKPaint With {
              .Color = SKColors.Black,
              .Style = SKPaintStyle.Fill
            }
            For i As Integer = 0 To cs.Length - 1
                Dim dw As Single = (cs(i) + 1) * mw
                If draw Then
                    canvas.DrawRect(x, y, dw * BarWidth, _h, paint)
                End If
                draw = Not draw
                x += dw
            Next
        End With
        If Me.WithText Then
            Dim fs = GetFontSize(txt, w, h * 0.2)
            Dim paint As New SKPaint With {
              .TextSize = fs,
              .Color = SKColors.Black,
              .Style = SKPaintStyle.Fill,
              .IsAntialias = True,
              .Typeface = Me.Typeface
            }
            canvas.DrawText(txt, r.Left + MarginX + (r.Width - paint.MeasureText(txt)) / 2, r.Top + MarginY + _h + fs * 0.8, paint)
        End If
    End Sub

End Class
