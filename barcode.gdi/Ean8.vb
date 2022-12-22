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
            Throw New ArgumentException("(ean13)データは7桁(チェックディジットを含めるなら8桁)でなければいけません: " & data)
        End If
        Return ret
    End Function

    Public Sub Render(ByVal g As Graphics,
                      ByVal x As Single, ByVal y As Single, ByVal w As Single, ByVal h As Single,
                      ByVal data As String)
        Me.Render(g, New RectangleF(x, y, w, h), data)
    End Sub

    Public Sub Render(ByVal g As Graphics, ByVal r As RectangleF, ByVal data As String)
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
        Dim cs() As Byte = Encode(_data)
        Dim mw As Single = w / (8 * 7 + 11)
        Dim draw As Boolean = True
        Dim x As Single = MarginX
        For i As Integer = 0 To cs.Length - 1
            Dim dw As Single = cs(i) * mw
            If draw Then
                Dim __h As Single = _h1
                If Array.IndexOf(GUARDS, i) >= 0 Then
                    __h = _h2
                End If
                g.FillRectangle(Brushes.Black, New RectangleF(r.X + x, r.Y + MarginY, dw * BarWidth, __h))
            End If
            draw = Not draw
            x += dw
        Next
        If Me.WithText Then
            Dim f As Font = Me.GetFont(GetFontSize(g, "00000000", h, w))
            Dim format As StringFormat = New StringFormat()
            format.Alignment = StringAlignment.Center
            For i As Integer = 0 To 7
                g.DrawString(_data(i), f, Brushes.Black, r.X + CHARPOS(i) * mw + MarginX, r.Y + _h1 + MarginY, format)
            Next
        End If
    End Sub

End Class
