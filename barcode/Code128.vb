Imports System.Buffers
Imports System.Drawing
Imports System.Net.Mime.MediaTypeNames

Public Class Code128
    Inherits Barcode

    Private Shared CODE_PATTERNS(,) As Byte =
      {{2, 1, 2, 2, 2, 2},
       {2, 2, 2, 1, 2, 2},
       {2, 2, 2, 2, 2, 1},
       {1, 2, 1, 2, 2, 3},
       {1, 2, 1, 3, 2, 2},
       {1, 3, 1, 2, 2, 2},
       {1, 2, 2, 2, 1, 3},
       {1, 2, 2, 3, 1, 2},
       {1, 3, 2, 2, 1, 2},
       {2, 2, 1, 2, 1, 3},
       {2, 2, 1, 3, 1, 2},
       {2, 3, 1, 2, 1, 2},
       {1, 1, 2, 2, 3, 2},
       {1, 2, 2, 1, 3, 2},
       {1, 2, 2, 2, 3, 1},
       {1, 1, 3, 2, 2, 2},
       {1, 2, 3, 1, 2, 2},
       {1, 2, 3, 2, 2, 1},
       {2, 2, 3, 2, 1, 1},
       {2, 2, 1, 1, 3, 2},
       {2, 2, 1, 2, 3, 1},
       {2, 1, 3, 2, 1, 2},
       {2, 2, 3, 1, 1, 2},
       {3, 1, 2, 1, 3, 1},
       {3, 1, 1, 2, 2, 2},
       {3, 2, 1, 1, 2, 2},
       {3, 2, 1, 2, 2, 1},
       {3, 1, 2, 2, 1, 2},
       {3, 2, 2, 1, 1, 2},
       {3, 2, 2, 2, 1, 1},
       {2, 1, 2, 1, 2, 3},
       {2, 1, 2, 3, 2, 1},
       {2, 3, 2, 1, 2, 1},
       {1, 1, 1, 3, 2, 3},
       {1, 3, 1, 1, 2, 3},
       {1, 3, 1, 3, 2, 1},
       {1, 1, 2, 3, 1, 3},
       {1, 3, 2, 1, 1, 3},
       {1, 3, 2, 3, 1, 1},
       {2, 1, 1, 3, 1, 3},
       {2, 3, 1, 1, 1, 3},
       {2, 3, 1, 3, 1, 1},
       {1, 1, 2, 1, 3, 3},
       {1, 1, 2, 3, 3, 1},
       {1, 3, 2, 1, 3, 1},
       {1, 1, 3, 1, 2, 3},
       {1, 1, 3, 3, 2, 1},
       {1, 3, 3, 1, 2, 1},
       {3, 1, 3, 1, 2, 1},
       {2, 1, 1, 3, 3, 1},
       {2, 3, 1, 1, 3, 1},
       {2, 1, 3, 1, 1, 3},
       {2, 1, 3, 3, 1, 1},
       {2, 1, 3, 1, 3, 1},
       {3, 1, 1, 1, 2, 3},
       {3, 1, 1, 3, 2, 1},
       {3, 3, 1, 1, 2, 1},
       {3, 1, 2, 1, 1, 3},
       {3, 1, 2, 3, 1, 1},
       {3, 3, 2, 1, 1, 1},
       {3, 1, 4, 1, 1, 1},
       {2, 2, 1, 4, 1, 1},
       {4, 3, 1, 1, 1, 1},
       {1, 1, 1, 2, 2, 4},
       {1, 1, 1, 4, 2, 2},
       {1, 2, 1, 1, 2, 4},
       {1, 2, 1, 4, 2, 1},
       {1, 4, 1, 1, 2, 2},
       {1, 4, 1, 2, 2, 1},
       {1, 1, 2, 2, 1, 4},
       {1, 1, 2, 4, 1, 2},
       {1, 2, 2, 1, 1, 4},
       {1, 2, 2, 4, 1, 1},
       {1, 4, 2, 1, 1, 2},
       {1, 4, 2, 2, 1, 1},
       {2, 4, 1, 2, 1, 1},
       {2, 2, 1, 1, 1, 4},
       {4, 1, 3, 1, 1, 1},
       {2, 4, 1, 1, 1, 2},
       {1, 3, 4, 1, 1, 1},
       {1, 1, 1, 2, 4, 2},
       {1, 2, 1, 1, 4, 2},
       {1, 2, 1, 2, 4, 1},
       {1, 1, 4, 2, 1, 2},
       {1, 2, 4, 1, 1, 2},
       {1, 2, 4, 2, 1, 1},
       {4, 1, 1, 2, 1, 2},
       {4, 2, 1, 1, 1, 2},
       {4, 2, 1, 2, 1, 1},
       {2, 1, 2, 1, 4, 1},
       {2, 1, 4, 1, 2, 1},
       {4, 1, 2, 1, 2, 1},
       {1, 1, 1, 1, 4, 3},
       {1, 1, 1, 3, 4, 1},
       {1, 3, 1, 1, 4, 1},
       {1, 1, 4, 1, 1, 3},
       {1, 1, 4, 3, 1, 1},
       {4, 1, 1, 1, 1, 3},
       {4, 1, 1, 3, 1, 1},
       {1, 1, 3, 1, 4, 1},
       {1, 1, 4, 1, 3, 1},
       {3, 1, 1, 1, 4, 1},
       {4, 1, 1, 1, 3, 1},
       {2, 1, 1, 4, 1, 2},
       {2, 1, 1, 2, 1, 4},
       {2, 1, 1, 2, 3, 2}}

    Private Shared STOP_PATTERN() As Byte = {2, 3, 3, 1, 1, 1, 2}

    Private Const TO_C As Integer = 99
    Private Const TO_B As Integer = 100
    Private Const TO_A As Integer = 101
    Private Const FNC_1 As Integer = 102
    Private Const START_A As Integer = 103
    Private Const START_B As Integer = 104
    Private Const START_C As Integer = 105

    Public Enum ECodeType
        NO_CHANGE
        A
        B
        C
    End Enum

    Public ParseFnc1 As Boolean = False

    Public Function Encode(ByVal codePoints As List(Of Integer)) As Byte()
        Dim cs As New List(Of Byte)
        For Each p As Integer In codePoints
            Me.addCodes(cs, p)
        Next
        addCodes(cs, Me.calcCheckDigit(codePoints))
        cs.AddRange(STOP_PATTERN)
        Return cs.ToArray
    End Function

    Public Sub Validate(ByVal data As String)
        For Each c As Char In data
            If Asc(c) > &H7F Then
                Throw New ArgumentException("(code128)不正なデータです: " & data)
            End If
        Next
    End Sub

    Public Function GetCodePoints(ByVal data As String) As List(Of Integer)
        Return Me.GetCodePoints(data, Me.getStartCodeType(data))
    End Function

    Public Function GetCodePoints(ByVal data As String, ByVal startCodeType As ECodeType) As List(Of Integer)
        Dim ret As New List(Of Integer)
        Dim _data As String = data
        Dim codeType As ECodeType = startCodeType
        Select Case codeType
            Case ECodeType.A
                ret.Add(START_A)
            Case ECodeType.B
                ret.Add(START_B)
            Case ECodeType.C
                ret.Add(START_C)
        End Select
        Do While (_data.Length > 0)
            If Me.ParseFnc1 AndAlso _data.StartsWith("{1}") Then
                ret.Add(FNC_1)
                _data = _data.Substring(3)
            Else
                Select Case Me.getNextCodeType(_data, codeType)
                    Case ECodeType.A
                        ret.Add(TO_A)
                        codeType = ECodeType.A
                    Case ECodeType.B
                        ret.Add(TO_B)
                        codeType = ECodeType.B
                    Case ECodeType.C
                        ret.Add(TO_C)
                        codeType = ECodeType.C
                End Select
                Select Case codeType
                    Case ECodeType.A
                        ret.Add(Me.getCodePointA(_data))
                        _data = _data.Substring(1)
                    Case ECodeType.B
                        ret.Add(Me.getCodePointB(_data))
                        _data = _data.Substring(1)
                    Case ECodeType.C
                        ret.Add(Me.getCodePointC(_data))
                        _data = _data.Substring(2)
                End Select
            End If
        Loop
        Return ret
    End Function

    Private Sub addCodes(ByVal l As List(Of Byte), ByVal p As Integer)
        l.Add(CODE_PATTERNS(p, 0))
        l.Add(CODE_PATTERNS(p, 1))
        l.Add(CODE_PATTERNS(p, 2))
        l.Add(CODE_PATTERNS(p, 3))
        l.Add(CODE_PATTERNS(p, 4))
        l.Add(CODE_PATTERNS(p, 5))
    End Sub

    Private Function getStartCodeType(ByVal data As String) As ECodeType
        If data.Length >= 2 Then
            If Char.IsDigit(data(0)) And Char.IsDigit(data(1)) Then
                Return ECodeType.C
            End If
        End If
        If Asc(data(0)) <= &H1F Then
            Return ECodeType.A
        End If
        Return ECodeType.B
    End Function

    Private Function getNextCodeType(ByVal data As String, ByVal codeType As ECodeType) As ECodeType
        If codeType <> ECodeType.C Then
            If data.Length >= 4 Then
                If Char.IsDigit(data(0)) And Char.IsDigit(data(1)) And Char.IsDigit(data(2)) And Char.IsDigit(data(3)) Then
                    Return ECodeType.C
                End If
            End If
        End If
        If codeType <> ECodeType.A Then
            If Asc(data(0)) <= &H1F Then
                Return ECodeType.A
            End If
        End If
        If codeType <> ECodeType.B Then
            If Asc(data(0)) >= &H60 Then
                Return ECodeType.B
            End If
        End If
        If codeType = ECodeType.C Then
            If data.Length < 2 OrElse (Not Char.IsDigit(data(0)) Or Not Char.IsDigit(data(1))) Then
                Return ECodeType.B
            End If
        End If
        Return ECodeType.NO_CHANGE
    End Function

    Private Function getCodePointA(ByVal data As String) As Integer
        Dim c As Integer = Asc(data(0))
        If c <= &H1F Then
            Return c + &H40
        Else
            Return c - &H20
        End If
    End Function

    Private Function getCodePointB(ByVal data As String) As Integer
        Return Asc(data(0)) - &H20
    End Function

    Private Function getCodePointC(ByVal data As String) As Integer
        Return Integer.Parse(data.Substring(0, 2))
    End Function

    Private Function calcCheckDigit(ByVal ps As List(Of Integer)) As Byte
        Dim t As Integer = ps(0)
        For i As Integer = 1 To ps.Count - 1
            t += i * ps(i)
        Next
        Return t Mod 103
    End Function

    Public Overrides Function CreateShape(x As Single, y As Single, w As Single, h As Single, data As String) As Shape
        If data Is Nothing OrElse data.Length = 0 Then
            Return Nothing
        End If
        If w <= 0 Or h <= 0 Then
            Return Nothing
        End If
        Me.Validate(data)
        Dim _h As Single = h
        If Me.WithText Then
            _h *= 0.7F
        End If
        Dim text As String = data
        Dim ret As New Shape()
        With Nothing
            Dim cps = GetCodePoints(data)
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
            ret.FontSize = GetFontSize(text, w, h * 0.25)
            ret.Texts.Add(New Shape.Text(text, x, y + _h, w, h))
        End If
        Return ret
    End Function

End Class