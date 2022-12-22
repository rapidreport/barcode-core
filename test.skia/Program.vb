Imports System.IO
Imports SkiaSharp
Imports jp.co.systembase.barcode.skia

Module Program
    Sub Main(args As String())
        With Nothing
            Dim b As New Code128
            Write(b, "0123456789CODE", "code128_0", 500, 200)
            Write(b, "!@#$%^&*()-", "code128_1", 500, 200)
            Write(b, "!@#$%^&*()-", "code128_2", 500, 200)
            Write(b, "[]{};':""", "code128_3", 500, 200)
            b.WithText = False
            Write(b, "abcd12!", "code128_4", 500, 200)
        End With
        With Nothing
            Dim b As New Ean8
            Write(b, "4901234", "ean8_0", 500, 200)
            Write(b, "5678901", "ean8_1", 500, 200)
            Write(b, "5678901", "ean8_2", 500, 200)
            b.WithText = False
            Write(b, "8888888", "ean8_3", 500, 200)
        End With
        With Nothing
            Dim b As New Ean13
            Write(b, "490123456789", "ean13_0", 500, 200)
            Write(b, "192205502800", "ean13_1", 500, 200)
            Write(b, "978488337649", "ean13_2", 500, 200)
            b.WithText = False
            Write(b, "390123456789", "ean13_3", 500, 200)
        End With
        With Nothing
            Dim b As New Code39
            Write(b, "490123456789", "code39_0", 500, 200)
            Write(b, "0123456789", "code39_1", 500, 200)
            Write(b, "ABCDEFGHIJ", "code39_2", 500, 200)
            b.GenerateCheckSum = True
            Write(b, "KLMNOPQRSTU", "code39_3", 500, 200)
            b.WithText = False
            Write(b, "VWXYZ-. $/+%", "code39_4", 500, 200)
        End With
        With Nothing
            Dim b As New Codabar
            Write(b, "A123456A", "codabar_0", 500, 200)
            Write(b, "B987653B", "codabar_1", 500, 200)
            b.GenerateCheckSum = True
            Write(b, "C82-$:/.+34C", "codabar_2", 500, 200)
            b.WithText = False
            Write(b, "D98-$:/.+21D", "codabar_3", 500, 200)
        End With
        With Nothing
            Dim b As New Itf
            Write(b, "12345678901234", "itf_0", 500, 200)
            b.GenerateCheckSum = True
            Write(b, "432109876543", "itf_1", 500, 200)
            b.WithCheckSumText = True
            Write(b, "1234567890123", "itf_2", 500, 200)
            b.WithText = False
            Write(b, "1234567890123", "itf_3", 500, 200)
        End With
        With Nothing
            Dim b As New Yubin
            Write(b, "1234567890-", "yubin_0", 300, 100)
            Write(b, "1112222ABCDEFGHIJK", "yubin_1", 300, 100)
            Write(b, "UVWXYZ", "yubin_2", 300, 100)
            Write(b, "024007315-10-3", "yubin_3", 300, 100)
            Write(b, "91000673-80-25J1-2B", "yubin_4", 300, 100)
        End With
        With Nothing
            Dim b As New Gs1_128
            Write(b, "(00)123456789012345678", "gs1_128_0", 800, 200)
            Write(b, "(11)ABCDEF{99}!""%&'()*+,-./", "gs1_128_1", 800, 200)
            b.WithText = False
            Write(b, "(01)04912345123459{10}ABC123", "gs1_128_2", 800, 200)
            b.WithText = True
            b.ConveniFormat = True
            Write(b, "(91)91234512345678901234567892110203310123456", "gs1_128_3", 800, 200)
        End With
    End Sub

    Sub Write(barcode As Barcode, data As String, fileName As String, w As Single, h As Single)
        Const MARGIN = 200
        With Nothing
            Using bmp As New SKBitmap(w + MARGIN * 2, h + MARGIN * 2)
                Using canvas As New SKCanvas(bmp)
                    Dim paint As New SKPaint With {
                        .Color = SKColors.White,
                        .Style = SKPaintStyle.Fill
                    }
                    canvas.DrawRect(0, 0, w + MARGIN * 2, h + MARGIN * 2, paint)
                    barcode.Render(canvas, MARGIN, MARGIN, w, h, data)
                End Using
                File.WriteAllBytes(Path.Combine("out", fileName & ".png"),
                    SKImage.FromBitmap(bmp).Encode(SKEncodedImageFormat.Png, 100).ToArray())
            End Using
        End With
    End Sub

End Module
