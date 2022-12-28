Imports System.Buffers
Imports System.Drawing
Imports System.Net.Mime.MediaTypeNames

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

    Public Overrides Function CreateShape(x As Single, y As Single, w As Single, h As Single, data As String) As Shape
        If data Is Nothing OrElse data.Length = 0 Then
            Return Nothing
        End If
        Dim _w As Single = w - Me.MarginX * 2
        Dim _h As Single = h - Me.MarginY * 2
        Dim __h1 As Single = _h
        Dim __h2 As Single = _h
        If Me.WithText Then
            __h1 *= 0.7F
            __h2 *= 0.8F
        End If
        If _w <= 0 Or _h <= 0 Then
            Return Nothing
        End If
        Dim ret As New Shape()
        Dim _data As List(Of Byte) = Me.PreprocessData(data)
        Dim mw As Single
        With Nothing
            Dim cs() As Byte = Me.Encode(_data)
            Dim _x As Single
            If Me.WithText Then
                mw = _w / (12 * 7 + 18)
                _x = x + MarginX + mw * 7
            Else
                mw = _w / (12 * 7 + 11)
                _x = x + MarginX
            End If
            Dim _y As Single = y + MarginY
            Dim draw As Boolean = True
            For i As Integer = 0 To cs.Length - 1
                Dim dw As Single = cs(i) * mw
                If draw Then
                    ret.Bars.Add(New Shape.Bar(_x, _y, dw * BarWidth, IIf(GUARDS.Contains(i), __h2, __h1)))
                End If
                draw = Not draw
                _x += dw
            Next
        End With
        If Me.WithText Then
            ret.FontSize = GetFontSize("0000000000000", _w, _h * 0.25)
            For i As Integer = 0 To 12
                ret.Texts.Add(New Shape.Text(_data(i), x + MarginX + CHARPOS(i) * mw - ret.FontSize / 2, y + MarginY + __h1))
            Next
        End If
        Return ret
    End Function

End Class