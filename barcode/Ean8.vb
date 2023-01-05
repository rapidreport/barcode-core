Imports System.Drawing

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

    Public Overrides Function CreateShape(x As Single, y As Single, w As Single, h As Single, data As String) As Shape
        If data Is Nothing OrElse data.Length = 0 Then
            Return Nothing
        End If
        If w <= 0 Or h <= 0 Then
            Return Nothing
        End If
        Dim __h1 As Single = h
        Dim __h2 As Single = h
        If Me.WithText Then
            __h1 *= 0.7F
            __h2 *= 0.8F
        End If
        Dim ret As New Shape()
        Dim _data As List(Of Byte) = Me.PreprocessData(data)
        Dim mw As Single = w / (8 * 7 + 11)
        With Nothing
            Dim cs() As Byte = Encode(_data)
            Dim draw As Boolean = True
            Dim _x As Single = x
            For i As Integer = 0 To cs.Length - 1
                Dim dw As Single = cs(i) * mw
                If draw Then
                    ret.Bars.Add(New Shape.Bar(_x, y, dw * BarWidth, IIf(GUARDS.Contains(i), __h2, __h1)))
                End If
                draw = Not draw
                _x += dw
            Next
        End With
        If Me.WithText Then
            ret.FontSize = GetFontSize("00000000000", w, h * 0.25)
            For i As Integer = 0 To 7
                ret.Texts.Add(New Shape.Text(_data(i), x + CHARPOS(i) * mw - ret.FontSize / 2, y + __h1))
            Next
        End If
        Return ret
    End Function

End Class
