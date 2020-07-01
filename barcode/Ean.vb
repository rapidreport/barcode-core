Public Class Ean
    Inherits Barcode

    Protected Shared CODE_PATTERNS(,) As Byte = _
      {{3, 2, 1, 1}, _
       {2, 2, 2, 1}, _
       {2, 1, 2, 2}, _
       {1, 4, 1, 1}, _
       {1, 1, 3, 2}, _
       {1, 2, 3, 1}, _
       {1, 1, 1, 4}, _
       {1, 3, 1, 2}, _
       {1, 2, 1, 3}, _
       {3, 1, 1, 2}}

    Protected Shared START_PATTERN() As Byte = {1, 1, 1}
    Protected Shared STOP_PATTERN() As Byte = {1, 1, 1}
    Protected Shared CENTER_PATTERN() As Byte = {1, 1, 1, 1, 1}

    Public Function CalcCheckDigit(ByVal data As List(Of Byte)) As Byte
        Dim s As Integer = 0
        For i As Integer = 0 To data.Count - 1
            If i Mod 2 = 0 Then
                s += data(data.Count - i - 1) * 3
            Else
                s += data(data.Count - i - 1)
            End If
        Next
        Return (10 - (s Mod 10)) Mod 10
    End Function

    Protected Function pack(ByVal data As String) As List(Of Byte)
        Dim ret As New List(Of Byte)
        For Each c As Char In data
            If Not Char.IsDigit(c) Then
                Throw New ArgumentException("(ean)不正なデータです: " & data)
            End If
            ret.Add(Byte.Parse(c))
        Next
        Return ret
    End Function

    Protected Sub addCodes(ByVal l As List(Of Byte), ByVal p As Integer)
        l.Add(CODE_PATTERNS(p, 0))
        l.Add(CODE_PATTERNS(p, 1))
        l.Add(CODE_PATTERNS(p, 2))
        l.Add(CODE_PATTERNS(p, 3))
    End Sub

    Protected Sub addCodesEven(ByVal l As List(Of Byte), ByVal p As Integer)
        l.Add(CODE_PATTERNS(p, 3))
        l.Add(CODE_PATTERNS(p, 2))
        l.Add(CODE_PATTERNS(p, 1))
        l.Add(CODE_PATTERNS(p, 0))
    End Sub

End Class
