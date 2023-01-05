Imports System.Drawing
Imports System.Net.NetworkInformation
Imports System.Text
Imports System.Text.RegularExpressions

Public Class Gs1_128
    Inherits Code128

    Public ConveniFormat As Boolean = False

    Public Sub New()
        Me.ParseFnc1 = True
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

    Public Overrides Function CreateShape(x As Single, y As Single, w As Single, h As Single, data As String) As Shape
        If data Is Nothing OrElse data.Length = 0 Then
            Return Nothing
        End If
        If w <= 0 Or h <= 0 Then
            Return Nothing
        End If
        Dim _h As Single = h
        If Me.WithText Then
            If Me.ConveniFormat Then
                _h *= 0.5F
            Else
                _h *= 0.7F
            End If
        End If
        Dim ret As New Shape
        Dim _data As String = data
        Me.Validate(_data)
        If Me.ConveniFormat Then
            _data = Me.PreprocessConveniData(_data)
        End If
        With Nothing
            Dim cps = GetCodePoints(Me.TrimData(_data), ECodeType.C)
            Dim mw As Single = w / ((cps.Count + 1) * 11 + 13)
            Dim draw As Boolean = True
            Dim _x As Single = x
            For Each c As Byte In Me.Encode(cps)
                Dim dw As Single = c * mw
                If draw Then
                    ret.Bars.Add(New Shape.Bar(_x, y, dw * BarWidth, _h))
                End If
                draw = Not draw
                _x += dw
            Next
        End With
        If Me.WithText Then
            If Me.ConveniFormat Then
                Dim t As String = Me.ConveniDisplayFormat(_data)
                Dim t1 As String = t.Substring(0, 33)
                Dim t2 As String = t.Substring(33)
                ret.FontSize = GetFontSize(t1, w, h * 0.3)
                ret.Texts.Add(New Shape.Text(t1, x, y + _h))
                ret.Texts.Add(New Shape.Text(t2, x, y + _h + ret.FontSize))
            Else
                Dim t As String = Me.DisplayFormat(_data)
                ret.FontSize = GetFontSize(t, w, h * 0.3)
                ret.Texts.Add(New Shape.Text(t, x, y + _h, w, _h))
            End If
        End If
        Return ret
    End Function

End Class
