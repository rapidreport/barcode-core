Imports System.Buffers
Imports System.Drawing
Imports System.Reflection

Public Class Codabar
    Inherits Barcode

    Private Shared CODE_PATTERNS(,) As Byte =
      {{0, 0, 0, 0, 0, 1, 1},
       {0, 0, 0, 0, 1, 1, 0},
       {0, 0, 0, 1, 0, 0, 1},
       {1, 1, 0, 0, 0, 0, 0},
       {0, 0, 1, 0, 0, 1, 0},
       {1, 0, 0, 0, 0, 1, 0},
       {0, 1, 0, 0, 0, 0, 1},
       {0, 1, 0, 0, 1, 0, 0},
       {0, 1, 1, 0, 0, 0, 0},
       {1, 0, 0, 1, 0, 0, 0},
       {0, 0, 0, 1, 1, 0, 0},
       {0, 0, 1, 1, 0, 0, 0},
       {1, 0, 0, 0, 1, 0, 1},
       {1, 0, 1, 0, 0, 0, 1},
       {1, 0, 1, 0, 1, 0, 0},
       {0, 0, 1, 0, 1, 0, 1},
       {0, 0, 1, 1, 0, 1, 0},
       {0, 1, 0, 1, 0, 0, 1},
       {0, 0, 0, 1, 0, 1, 1},
       {0, 0, 0, 1, 1, 1, 0}}

    Private Const CHARS As String = "0123456789-$:/.+ABCD"
    Private Const START_STOP_POINT As Integer = 16

    Public GenerateCheckSum As Boolean = False
    Public WithCheckSumText As Boolean = False
    Public WithStartStopText As Boolean = False

    Public Function Encode(ByVal codePoints As List(Of Integer)) As Byte()
        Dim ret As New List(Of Byte)
        For Each p As Integer In codePoints
            Me.addCodes(ret, p)
        Next
        Return ret.ToArray
    End Function

    Public Function GetCodePoints(ByVal data As String) As List(Of Integer)
        Dim ret As New List(Of Integer)
        For i As Integer = 0 To data.Length - 1
            Dim p As Integer = CHARS.IndexOf(Char.ToUpper(data(i)))
            If p >= 0 Then
                If i = 0 Or i = data.Length - 1 Then
                    If p < START_STOP_POINT Then
                        Throw New ArgumentException("(codabar)スタート/ストップ文字が含まれていません: " & data)
                    End If
                Else
                    If p >= START_STOP_POINT Then
                        Throw New ArgumentException("(codabar)不正なデータです: " & data)
                    End If
                End If
                ret.Add(p)
            Else
                Throw New ArgumentException("(codabar)不正なデータです: " & data)
            End If
        Next
        If ret.Count < 2 Then
            Throw New ArgumentException("(codabar)不正なデータです: " & data)
        End If
        Return ret
    End Function

    Public Function CalcCheckDigit(ByVal ps As List(Of Integer)) As Integer
        Dim s As Integer = 0
        For Each p As Integer In ps
            s += p
        Next
        Return (16 - (s Mod 16)) Mod 16
    End Function

    Public Sub AddCheckDigit(ByVal codePoints As List(Of Integer), ByVal cd As Integer)
        codePoints.Insert(codePoints.Count - 1, cd)
    End Sub

    Public Function AddCheckDigit(ByVal txt As String, ByVal cd As Integer) As String
        Dim ret As String = txt.Substring(0, txt.Length - 1)
        ret &= CHARS(cd)
        ret &= txt(txt.Length - 1)
        Return ret
    End Function

    Public Function TrimStartStopText(ByVal txt As String) As String
        Return txt.Substring(1, txt.Length - 2)
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
    End Sub

    Public Overrides Function CreateShape(x As Single, y As Single, w As Single, h As Single, data As String) As Shape
        If data Is Nothing OrElse data.Length = 0 Then
            Return Nothing
        End If
        Dim _w As Single = w - Me.MarginX * 2
        Dim _h As Single = h - Me.MarginY * 2
        Dim __h As Single = _h
        If Me.WithText Then
            __h *= 0.7F
        End If
        If _w <= 0 Or _h <= 0 Then
            Return Nothing
        End If
        Dim ret As New Shape()
        Dim ps As List(Of Integer) = Me.GetCodePoints(data)
        Dim txt As String = data
        If Me.GenerateCheckSum Then
            Dim cd As Integer = Me.CalcCheckDigit(ps)
            Me.AddCheckDigit(ps, cd)
            If Me.WithCheckSumText Then
                txt = Me.AddCheckDigit(txt, cd)
            End If
        End If
        If Not Me.WithStartStopText Then
            txt = Me.TrimStartStopText(txt)
        End If
        Dim cs As Byte() = Me.Encode(ps)
        Dim mw As Single
        With Nothing
            Dim l As Integer = 0
            For Each c As Integer In cs
                l += c + 1
            Next
            mw = _w / l
        End With
        With Nothing
            Dim draw As Boolean = True
            Dim _x As Single = x + MarginX
            Dim _y As Single = y + MarginY
            For i As Integer = 0 To cs.Length - 1
                Dim dw As Single = (cs(i) + 1) * mw
                If draw Then
                    ret.Bars.Add(New Shape.Bar(_x, _y, dw * BarWidth, __h))
                End If
                draw = Not draw
                _x += dw
            Next
        End With
        If Me.WithText Then
            ret.FontSize = GetFontSize(txt, _w, _h * 0.25)
            ret.Texts.Add(New Shape.Text(txt, x + MarginX, y + MarginY + __h, _w, _h))
        End If
        Return ret
    End Function

End Class
