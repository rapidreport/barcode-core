Imports System.Drawing
Imports System.Text
Imports System.Text.RegularExpressions

Public Class Gs1_128
    Inherits Code128

    Public ConveniFormat As Boolean = False

    Public Sub New()
        Me.ParseFnc1 = True
    End Sub

    Public Overrides Sub Render(ByVal g As System.Drawing.Graphics, ByVal r As RectangleF, ByVal data As String)
        If data Is Nothing OrElse data.Length = 0 Then
            Exit Sub
        End If
        Dim w As Single = r.Width - Me.MarginX * 2
        Dim h As Single = r.Height - Me.MarginY * 2
        Dim _h As Single = h
        If Me.WithText Then
            If Me.ConveniFormat Then
                _h *= 0.5F
            Else
                _h *= 0.7F
            End If
        End If
        If w <= 0 Or h <= 0 Then
            Exit Sub
        End If
        Dim _data As String = data
        Me.Validate(_data)
        If Me.ConveniFormat Then
            _data = Me.PreprocessConveniData(_data)
        End If
        Me.renderBars( _
            g, _
            Me.GetCodePoints(Me.TrimData(_data), ECodeType.C), _
            r.X + Me.MarginX, _
            r.Y + Me.MarginY, _
            w, _
            _h)
        If Me.WithText Then
            If Me.ConveniFormat Then
                Dim t As String = Me.ConveniDisplayFormat(_data)
                Dim t1 As String = t.Substring(0, 33)
                Dim t2 As String = t.Substring(33)
                Dim f As Font = Me.GetFont(GetFontSize(g, t1, w, h))
                Dim format As StringFormat = New StringFormat()
                format.Alignment = StringAlignment.Near
                g.DrawString(t1, f, Brushes.Black, r.X + Me.MarginX, r.Y + _h + Me.MarginY - f.Size * 0.1, format)
                g.DrawString(t2, f, Brushes.Black, r.X + Me.MarginX, r.Y + _h + Me.MarginY + f.Size, format)
            Else
                Dim t As String = Me.DisplayFormat(_data)
                Dim f As Font = Me.GetFont(GetFontSize(g, t, w, h))
                Dim format As StringFormat = New StringFormat()
                format.Alignment = StringAlignment.Center
                g.DrawString(t, f, Brushes.Black, r.X + w / 2 + Me.MarginX, r.Y + _h + Me.MarginY, format)
            End If
        End If
    End Sub

    Public Function PreprocessConveniData(ByVal data As String) As String
        Dim _data As String = data
        If Not _data.StartsWith("(91)") Then
            Throw New ArgumentException("(gs1_128)データの先頭が'(91)'でなければいけません: " & data)
        End If
        If _data.Length = 45 Then
            _data = data & Me.calcConveniCheckDigit(_data)
        ElseIf _data.Length <> 46 Then
            Throw New ArgumentException("(gs1_128)データの'(91)'以降が41桁(チェックディジットを含めるなら42桁)でなければいけません: " & data)
        End If
        Return _data
    End Function

    Public Function TrimData(ByVal data As String) As String
        Dim ret As String = data
        If Not ret.StartsWith("{1}") Then
            ret = "{1}" & ret
        End If
        ret = ret.Replace("(", "")
        ret = ret.Replace(")", "")
        Return ret
    End Function

    Public Function DisplayFormat(ByVal data As String) As String
        Dim ret As String = data
        ret = ret.Replace("{1}", "")
        Return ret
    End Function

    Public Function ConveniDisplayFormat(ByVal data As String) As String
        Dim ret As String = data
        ret = ret.Replace("{1}", "")
        Return ret.Substring(0, 10) & "-" & _
            ret.Substring(10, 28) & "-" & _
            ret.Substring(38, 1) & "-" & _
            ret.Substring(39, 6) & "-" & _
            ret.Substring(45, 1)
    End Function

    Private Function calcConveniCheckDigit(ByVal data As String) As String
        Dim _data As String = data
        _data = _data.Replace("(", "")
        _data = _data.Replace(")", "")
        Dim s As Integer = 0
        For i As Integer = 0 To _data.Length - 1
            Dim t As Integer = _data.Substring(_data.Length - i - 1, 1)
            If i Mod 2 = 0 Then
                s += t * 3
            Else
                s += t
            End If
        Next
        Return (10 - (s Mod 10)) Mod 10
    End Function

End Class
