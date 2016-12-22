using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace Verno.ActionResults
{
    public class MemoryStreamResult: FileStreamResult
    {
        public MemoryStreamResult(string fileName, Action<Stream> action):base(new MemoryStream(), GetContentType(fileName))
        {
            FileDownloadName = fileName;
            action(FileStream);
            FileStream.Position = 0;
        }

        private static string GetContentType(string fileName)
        {
            string contentType;
            new FileExtensionContentTypeProvider().TryGetContentType(fileName, out contentType);
            return contentType;
        }

        /*public static ActionResult Create(string fileName, Action<Stream> action)
        {
            var file = new MemoryStream();
            action(file);
            file.Position = 0;

            string contentType;
            new FileExtensionContentTypeProvider().TryGetContentType(fileName, out contentType);
            return new FileStreamResult(file, contentType) { FileDownloadName = fileName };
        }*/
    }
}