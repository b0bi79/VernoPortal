using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Image = System.Drawing.Image;
using Rectangle = System.Drawing.Rectangle;

namespace Verno.Images
{
    public static class PdfHelper
    {
        public static void ReducePdf(Stream inStream, string outFile, float dpi, InterpolationMode interpolation)
        {
            using (var outStream = File.OpenWrite(outFile))
            {
                var document = new Document();
                document.SetPageSize(PageSize.A4);
                document.SetMargins(0.0f, 0.0f, 0.0f, 0.0f);
                var pageW = (float)((document.PageSize.Width - (double)document.LeftMargin - document.RightMargin) / 72.0);
                var pageH = (float)((document.PageSize.Height - (double)document.TopMargin - document.BottomMargin) / 72.0);

                PdfWriter.GetInstance(document, outStream).SetLinearPageMode();
                document.Open();

                var reader = new PdfReader(inStream);
                for (var i = 1; i <= reader.XrefSize; i++)
                {
                    var stream = reader.GetPdfObject(i) as PRStream;
                    if (stream == null)
                        continue;

                    if (stream.Get(PdfName.SUBTYPE)?.ToString() != PdfName.IMAGE.ToString()) continue;

                    byte[] buffer;
                    try
                    {
                        buffer = PdfReader.GetStreamBytes(stream);
                    }
                    catch
                    {
                        buffer = PdfReader.GetStreamBytesRaw(stream);
                    }
                    using (var image = Image.FromStream(new MemoryStream(buffer)))
                    {
                        var k = Math.Min(pageH * dpi / image.Height, pageW * dpi / image.Width);
                        var newH = (int)Math.Round(image.Height * (double)k);
                        var newW = (int)Math.Round(image.Width * (double)k);
                        using (var newimg = new Bitmap(newW, newH))
                        {
                            using (var g = Graphics.FromImage(newimg))
                            {
                                g.InterpolationMode = interpolation;
                                g.DrawImage(image, new Rectangle(0, 0, newW, newH), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel);
                            }
                            using (var ms = new MemoryStream())
                            {
                                newimg.Save(ms, ImageFormat.Jpeg);
                                var pdfimg = iTextSharp.text.Image.GetInstance(ms.ToArray());
                                pdfimg.ScaleToFit(pageW * 72f, pageH * 72f);
                                document.Add(pdfimg);
                            }
                        }
                    }
                }
                document.Close();
            }
        }
    }
}
