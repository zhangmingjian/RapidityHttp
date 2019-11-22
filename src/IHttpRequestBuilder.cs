namespace Rapidity.Http
{
    public interface IHttpRequestBuilder
    {
        HttpRequest GetRequest(RequestDescriptor description);
    }
}