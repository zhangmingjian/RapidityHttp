using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Rapidity.Http
{
    /// <summary>
    /// 
    /// </summary>
    public class EmptyContent : HttpContent
    {
        public EmptyContent() : this(MimeTypes.Text.Html, "utf-8")
        {
        }

        public EmptyContent(string contentType, string encoding)
        {
            Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
            Headers.ContentType.CharSet = encoding;
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            return Task.FromResult(0);
        }

        protected override bool TryComputeLength(out long length)
        {
            length = 0;
            return true;
        }
    }
}