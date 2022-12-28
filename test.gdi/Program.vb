Imports System.Drawing
Imports System.Windows.Forms
Imports jp.co.systembase.barcode

Friend Module Program

    <STAThread()>
    Friend Sub Main(args As String())
        Dim ppd As New PrintPreviewDialog
        ppd.Document = pd
        ppd.ShowDialog()
    End Sub

    Private WithEvents pd As New System.Drawing.Printing.PrintDocument
    Private page As Integer = 1

    Private Sub pd_PrintPage(ByVal sender As Object, ByVal e As System.Drawing.Printing.PrintPageEventArgs) Handles pd.PrintPage
        Dim g As Graphics = e.Graphics
        g.PageUnit = GraphicsUnit.Point
        Select Case page
            Case 1
                With Nothing
                    Dim b As New Code128
                    Render(g, b.CreateShape(20, 30, 150, 40, "0123456789CODE"))
                    Render(g, b.CreateShape(20, 100, 150, 40, "!@#$%^&*()-"))
                    Render(g, b.CreateShape(20, 170, 150, 40, "[]{};':"""))
                    b.WithText = False
                    Render(g, b.CreateShape(20, 230, 150, 40, "abcd12!"))
                End With
                With Nothing
                    Dim b As New Ean8
                    Render(g, b.CreateShape(220, 30, 150, 40, "4901234"))
                    Render(g, b.CreateShape(220, 100, 150, 40, "5678901"))
                    Render(g, b.CreateShape(220, 170, 150, 40, "5678901"))
                    b.WithText = False
                    Render(g, b.CreateShape(220, 230, 150, 40, "8888888"))
                End With
                With Nothing
                    Dim b As New Ean13
                    Render(g, b.CreateShape(420, 30, 150, 40, "490123456789"))
                    Render(g, b.CreateShape(420, 100, 150, 40, "192205502800"))
                    Render(g, b.CreateShape(420, 170, 150, 40, "978488337649"))
                    b.WithText = False
                    Render(g, b.CreateShape(420, 230, 150, 40, "390123456789"))
                End With
                With Nothing
                    Dim b As New Code39
                    Render(g, b.CreateShape(20, 330, 150, 40, "0123456789"))
                    Render(g, b.CreateShape(20, 400, 150, 40, "ABCDEFGHIJ"))
                    b.GenerateCheckSum = True
                    Render(g, b.CreateShape(20, 470, 150, 40, "KLMNOPQRSTU"))
                    b.WithText = False
                    Render(g, b.CreateShape(20, 530, 150, 40, "VWXYZ-. $/+%"))
                End With
                With Nothing
                    Dim b As New Codabar
                    Render(g, b.CreateShape(220, 330, 150, 40, "A123456A"))
                    Render(g, b.CreateShape(220, 400, 150, 40, "B987653B"))
                    b.GenerateCheckSum = True
                    Render(g, b.CreateShape(220, 470, 150, 40, "C82-$:/.+34C"))
                    b.WithText = False
                    Render(g, b.CreateShape(220, 530, 150, 40, "D98-$:/.+21D"))
                End With
                With Nothing
                    Dim b As New Itf
                    Render(g, b.CreateShape(420, 330, 150, 40, "12345678901234"))
                    b.GenerateCheckSum = True
                    Render(g, b.CreateShape(420, 400, 150, 40, "432109876543"))
                    b.WithCheckSumText = True
                    Render(g, b.CreateShape(420, 470, 150, 40, "1234567890123"))
                    b.WithText = False
                    Render(g, b.CreateShape(420, 530, 150, 40, "1234567890123"))
                End With
                e.HasMorePages = True
            Case 2
                With Nothing
                    Dim b As New Yubin
                    Render(g, b.CreateShape(20, 30, 300, 40, "1234567890-"))
                    Render(g, b.CreateShape(20, 130, 300, 40, "1112222ABCDEFGHIJK"))
                    Render(g, b.CreateShape(20, 230, 300, 40, "UVWXYZ"))
                    Render(g, b.CreateShape(20, 330, 300, 40, "024007315-10-3"))
                    Render(g, b.CreateShape(20, 430, 300, 40, "91000673-80-25J1-2B"))
                End With
                With Nothing
                    Dim b As New Gs1_128
                    Render(g, b.CreateShape(350, 30, 200, 40, "(00)123456789012345678"))
                    Render(g, b.CreateShape(350, 130, 200, 40, "(11)ABCDEF{99}!""%&'()*+,-./"))
                    b.WithText = False
                    Render(g, b.CreateShape(350, 230, 200, 40, "(01)04912345123459{10}ABC123"))
                    b.WithText = True
                    b.ConveniFormat = True
                    Render(g, b.CreateShape(350, 330, 200, 40, "(91)91234512345678901234567892110203310123456"))
                End With
                e.HasMorePages = False
            Case Else
        End Select

        If e.HasMorePages Then
            page += 1
        Else
            page = 1
        End If
    End Sub

    Private Sub Render(g As Graphics, shape As Barcode.Shape)
        If shape Is Nothing Then
            Exit Sub
        End If
        For Each bar In shape.Bars
            g.FillRectangle(Brushes.Black, bar.X, bar.Y, bar.W, bar.H)
        Next
        If shape.Texts.Count > 0 Then
            Dim f As New Font("Arial", shape.FontSize)
            For Each t In shape.Texts
                If t.W > 0 Then
                    g.DrawString(t.Text, f, Brushes.Black,
                                 New RectangleF(t.X, t.Y, t.W, t.H),
                                 New StringFormat With {.Alignment = StringAlignment.Center})
                Else
                    g.DrawString(t.Text, f, Brushes.Black, t.X, t.Y)
                End If
            Next
        End If

    End Sub

End Module
