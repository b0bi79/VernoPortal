using System;
using System.IO;
using System.Net.Mime;
using System.Text;
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
            string headerValue = ContentDispositionUtil.GetHeaderValue(ContentDisposition.Inline, ContentDisposition.FileName ?? FileDownloadName);
            var response = context.HttpContext.Response;
            response.Headers.Add(ContentDispositionHeaderName, headerValue);

            return base.ExecuteResultAsync(context);
        }

        internal static class ContentDispositionUtil
        {
            private const string HexDigits = "0123456789ABCDEF";

            private static void AddByteToStringBuilder(byte b, StringBuilder builder)
            {
                builder.Append('%');

                int i = b;
                AddHexDigitToStringBuilder(i >> 4, builder);
                AddHexDigitToStringBuilder(i % 16, builder);
            }

            private static void AddHexDigitToStringBuilder(int digit, StringBuilder builder)
            {
                builder.Append(HexDigits[digit]);
            }

            private static string CreateRfc2231HeaderValue(bool inline, string filename)
            {
                StringBuilder builder = new StringBuilder(inline ? "inline;": "attachment;");
                builder.Append(" filename*=UTF-8''");

                byte[] filenameBytes = Encoding.UTF8.GetBytes(filename);
                foreach (byte b in filenameBytes)
                {
                    if (IsByteValidHeaderValueCharacter(b))
                    {
                        builder.Append((char)b);
                    }
                    else
                    {
                        AddByteToStringBuilder(b, builder);
                    }
                }

                return builder.ToString();
            }

            public static string GetHeaderValue(bool inline, string fileName)
            {
                // If fileName contains any Unicode characters, encode according
                // to RFC 2231 (with clarifications from RFC 5987)
                foreach (char c in fileName)
                {
                    if ((int)c > 127)
                    {
                        return CreateRfc2231HeaderValue(inline, fileName);
                    }
                }

                // Knowing there are no Unicode characters in this fileName, rely on
                // ContentDisposition.ToString() to encode properly.
                // In .Net 4.0, ContentDisposition.ToString() throws FormatException if
                // the file name contains Unicode characters.
                // In .Net 4.5, ContentDisposition.ToString() no longer throws FormatException
                // if it contains Unicode, and it will not encode Unicode as we require here.
                // The Unicode test above is identical to the 4.0 FormatException test,
                // allowing this helper to give the same results in 4.0 and 4.5.         
                ContentDisposition disposition = new ContentDisposition() { Inline = inline, FileName = fileName };
                return disposition.ToString();
            }

            // Application of RFC 2231 Encoding to Hypertext Transfer Protocol (HTTP) Header Fields, sec. 3.2
            // http://greenbytes.de/tech/webdav/draft-reschke-rfc2231-in-http-latest.html
            private static bool IsByteValidHeaderValueCharacter(byte b)
            {
                if ((byte)'0' <= b && b <= (byte)'9')
                {
                    return true; // is digit
                }
                if ((byte)'a' <= b && b <= (byte)'z')
                {
                    return true; // lowercase letter
                }
                if ((byte)'A' <= b && b <= (byte)'Z')
                {
                    return true; // uppercase letter
                }

                switch (b)
                {
                    case (byte)'-':
                    case (byte)'.':
                    case (byte)'_':
                    case (byte)'~':
                    case (byte)':':
                    case (byte)'!':
                    case (byte)'$':
                    case (byte)'&':
                    case (byte)'+':
                        return true;
                }

                return false;
            }
        }
    }
}