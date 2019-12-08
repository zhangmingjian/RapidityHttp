using System.Net.Http;

namespace Rapidity.Http
{
    /// <summary>
    /// 
    /// </summary>
    public interface IHttpContentGenerator
    {
        HttpContent GetContent(RequestDescriptor descriptor);
    }
}