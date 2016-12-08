using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Verno.ActionResults
{
    public class FileContentResultWithContentDisposition : FileContentResult
    {
        private const string ContentDispositionHeaderName = "Content-Disposition";

        public FileContentResultWithContentDisposition(byte[] fileContents, string contentType, ContentDisposition contentDisposition)
            : base(fileContents, contentType)
        {
            // check for null or invalid ctor arguments
            ContentDisposition = contentDisposition;
        }

        public ContentDisposition ContentDisposition { get; private set; }

        public override Task ExecuteResultAsync(ActionContext context)
        {
            ContentDisposition.FileName = ContentDisposition.FileName ?? FileDownloadName;
            var response = context.HttpContext.Response;
            response.Headers.Add(ContentDispositionHeaderName, ContentDisposition.ToString());

            return base.ExecuteResultAsync(context);
        }
    }

    public class PhysicalFileResultWithContentDisposition : PhysicalFileResult
    {
        private const string ContentDispositionHeaderName = "Content-Disposition";

        public PhysicalFileResultWithContentDisposition(string fileName, string contentType, ContentDisposition contentDisposition)
            : base(fileName, contentType)
        {
            // check for null or invalid ctor arguments
            ContentDisposition = contentDisposition;
        }

        public ContentDisposition ContentDisposition { get; private set; }

        public override Task ExecuteResultAsync(ActionContext context)
        {
            ContentDisposition.FileName = ContentDisposition.FileName ?? FileDownloadName;
            var response = context.HttpContext.Response;
            response.Headers.Add(ContentDispositionHeaderName, ContentDisposition.ToString());

            return base.ExecuteResultAsync(context);
        }
    }
}