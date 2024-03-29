﻿Imports System.Drawing

Public Class Itf
    Inherits Barcode

    Private Shared CODE_PATTERNS As New Dictionary(Of Char, Byte()) From
        {{"0", {0, 0, 1, 1, 0}},
         {"1", {1, 0, 0, 0, 1}},
         {"2", {0, 1, 0, 0, 1}},
         {"3", {1, 1, 0, 0, 0}},
         {"4", {0, 0, 1, 0, 1}},
         {"5", {1, 0, 1, 0, 0}},
         {"6", {0, 1, 1, 0, 0}},
         {"7", {0, 0, 0, 1, 1}},
         {"8", {1, 0, 0, 1, 0}},
         {"9", {0, 1, 0, 1, 0}}}

    Private Shared START_PATTERN As Byte() = {0, 0, 0, 0}
    Private Shared STOP_PATTERN As Byte() = {1, 0, 0}

    Public GenerateCheckSum As Boolean = False
    Public WithCheckSumText As Boolean = False

    Public Function Encode(ByVal data As String) As List(Of Byte)
        Dim ret As New List(Of Byte)
        ret.AddRange(START_PATTERN)
        For i As Integer = 0 To data.Length - 1 Step 2
            Dim c1 As Byte() = CODE_PATTERNS(data(i))
            Dim c2 As Byte() = CODE_PATTERNS(data(i + 1))
            For j As Integer = 0 To c1.Length - 1
                ret.Add(c1(j))
                ret.Add(c2(j))
            Next
        Next
        ret.AddRange(STOP_PATTERN)
        Return ret
    End Function

    Public Function RegularizeData(ByVal data As String) As String
        Dim ret As String = data
        If Me.GenerateCheckSum Then
            If ret.Length Mod 2 = 0 Then
                ret = "0" & ret
            End If
        Else
            If ret.Length Mod 2 = 1 Then
                ret = "0" & ret
            End If
        End If
        Return ret
    End Function

    Public Sub Validate(ByVal data As String)
        For Each c As Char In data
            If Not CODE_PATTERNS.ContainsKey(c) Then
                Throw New ArgumentException("(itf)不正なデータです: " & data)
            End If
        Next
    End Sub

    Public Function CalcCheckDigit(ByVal data As String) As Byte
        Dim sum As Integer = 0
        For i As Integer = data.Length - 1 To 0 Step -1
            Dim n As Integer = data.Substring(i, 1)
            If i Mod 2 <> 0 Then
                sum += n
            Else
                sum += n * 3
            End If
        Next
        Const checkNum = 10
        Dim cd As Byte = checkNum - (sum Mod checkNum)
        If cd = checkNum Then
            cd = 0
        End If
        Return cd
    End Function

    Public Overrides Function CreateShape(x As Single, y As Single, w As Single, h As Single, data As String) As Shape
        If data Is Nothing OrElse data.Length = 0 Then
            Return Nothing
        End If
        If w <= 0 Or h <= 0 Then
            Return Nothing
        End If
        Validate(data)
        Dim _h As Single = h
        If Me.WithText Then
            _h *= 0.7F
        End If
        Dim ret As New Shape
        Dim _data As String = RegularizeData(data)
        Dim text As String = _data
        If Me.GenerateCheckSum Then
            _data &= Me.CalcCheckDigit(_data)
            If Me.WithCheckSumText Then
                text = _data
            End If
        End If
        With Nothing
            Dim cs As List(Of Byte) = Encode(_data)
            Dim uw As Single = w / (_data.Length * 7 + 8)
            Dim _x As Single = x
            Dim _y As Single = y
            Dim draw As Boolean = True
            For Each c As Byte In cs
                Dim bw As Single = uw * (c + 1)
                If draw Then
                    ret.Bars.Add(New Shape.Bar(_x, _y, bw * BarWidth, _h))
                End If
                draw = Not draw
                _x += bw
            Next
        End With
        If Me.WithText Then
            ret.FontSize = GetFontSize(text, w, h * 0.25)
            ret.Texts.Add(New Shape.Text(text, x, y + _h, w, h))
        End If
        Return ret
    End Function


End Class

