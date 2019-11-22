using System.Net.Http.Headers;

namespace Rapidity.Http
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRequestHeaderSetter
    {
        void SetHeader(RequestDescriptor description, HttpRequestHeaders header);
    }
}