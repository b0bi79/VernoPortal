using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Verno.Images
{
    public static class ImageHelper
    {
        public static void Reduce(Stream stream, string outFile, float dpi, InterpolationMode interpolation)
        {
            using (var image = Image.FromStream(stream))
            {
                var pageH = 11.69; // A4 height, inches
                var k = Math.Min(1, Math.Min(pageH * dpi / image.Height, pageH * dpi / image.Width));
                var newH = (int) Math.Round(image.Height * k);
                var newW = (int) Math.Round(image.Width * k);
                using (var newimg = new Bitmap(newW, newH))
                {
                    using (var g = Graphics.FromImage(newimg))
                    {
                        g.InterpolationMode = interpolation;
                        g.DrawImage(image, new Rectangle(0, 0, newW, newH), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel);
                    }
                    newimg.Save(outFile, GetImageFormat(outFile));
                }
            }
        }

        public static ImageFormat GetImageFormat(string fileName)
        {
            string extension = Path.GetExtension(fileName);
            if (string.IsNullOrEmpty(extension))
                throw new ArgumentException(
                    string.Format("Unable to determine file extension for fileName: {0}", fileName));

            switch (extension.ToLower())
            {
                case @".bmp":
                    return ImageFormat.Bmp;

                case @".gif":
                    return ImageFormat.Gif;

                case @".ico":
                    return ImageFormat.Icon;

                case @".jpg":
                case @".jpeg":
                    return ImageFormat.Jpeg;

                case @".png":
                    return ImageFormat.Png;

                case @".tif":
                case @".tiff":
                    return ImageFormat.Tiff;

                case @".wmf":
                    return ImageFormat.Wmf;

                default:
                    return null;
            }
        }
    }
}