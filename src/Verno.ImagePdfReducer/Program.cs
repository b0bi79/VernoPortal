using System;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using iTextSharp.text.exceptions;
using Verno.Images;

namespace Verno.ImagePdfReducer
{
    public class Program
    {
        private static int _cursorTop;

        public static void Main(string[] args)
        {
            var outDir = Path.Combine(Environment.CurrentDirectory, "out");
            _cursorTop = Console.CursorTop;
            if (!Directory.Exists(outDir))
                Directory.CreateDirectory(outDir);

            var files = Directory.GetFiles(Environment.CurrentDirectory).ToArray();
            for (var i = 0; i < files.Length; i++)
            {
                var top = Console.CursorTop;
                Console.SetCursorPosition(0, top);
                Console.Write($"{i + 1}/{files.Length}");
                Reduce(files[i]);
            }
        }

        private static void Reduce(string filePath)
        {
            var fileDir = Path.GetDirectoryName(filePath);
            var fileName = Path.GetFileName(filePath);
            var file = new FileInfo(filePath);
            var outPath = Path.Combine(fileDir, "out", fileName);

            bool converted = false;
            try
            {
                using (var stream = file.OpenRead())
                {
                    PdfHelper.ReducePdf(stream, outPath, 100, InterpolationMode.HighQualityBilinear);
                    converted = true;
                }
            }
            catch (ArgumentException)
            {
                LogError($"Ошибка сжатия файла {filePath}.");
            }
            catch (InvalidPdfException)
            {
                LogError($"Неверный формат файла {filePath}.");
            }
            if (!converted)
                using (var stream = file.OpenRead())
                {
                    var jpgPath = Path.ChangeExtension(outPath, ".jpg");
                    try
                    {
                        ImageHelper.Reduce(stream, jpgPath, 100, InterpolationMode.HighQualityBilinear);
                        File.Delete(outPath);
                        File.Move(jpgPath, outPath);
                        converted = true;
                    }
                    catch (ArgumentException)
                    {
                        LogError($"Ошибка сжатия файла {filePath}.");
                    }
                }

            if (converted)
            {
                File.SetAttributes(filePath, FileAttributes.Normal);
                file.Delete();
            }
            else
            {
                File.Delete(outPath);
                File.Move(filePath, outPath);
            }
        }

        private static void LogError(string message)
        {
            Console.WriteLine(message);
        }
    }
}

