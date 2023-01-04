Imports System.Drawing
Imports System.Net.NetworkInformation

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
        Dim ret As New Shape
        Dim ps As New List(Of Integer)
        Dim text As String = data
        With Nothing
            Me.AddStartStopPoint(ps)
            Dim _ps As List(Of Integer) = Me.GetCodePoints(data)
            ps.AddRange(_ps)
            If Me.GenerateCheckSum Then
                Dim cd As Integer = Me.CalcCheckDigit(_ps)
                ps.Add(cd)
                If Me.WithCheckSumText Then
                    text &= CHARS(cd)
                End If
            End If
            Me.AddStartStopPoint(ps)
            text = Me.AddStartStopText(text)
        End With
        With Nothing
            Dim cs As Byte() = Me.Encode(ps)
            Dim mw As Single = w / (ps.Count * 13 - 1)
            Dim draw As Boolean = True
            Dim _x As Single = x + Me.MarginX
            Dim _y As Single = y + Me.MarginY
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
            ret.FontSize = GetFontSize(text, _w, _h * 0.25)
            ret.Texts.Add(New Shape.Text(text, x + MarginX, y + MarginY + __h, _w, _h))
        End If
        Return ret
    End Function

End Class
