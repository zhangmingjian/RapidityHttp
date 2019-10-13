using System.Net.Http.Headers;

namespace Rapidity.Http
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRequestHeaderSetter
    {
        void SetHeader(RequestDescription description, HttpRequestHeaders header);
    }
}