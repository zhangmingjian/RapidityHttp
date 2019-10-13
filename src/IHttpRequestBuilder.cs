namespace Rapidity.Http
{
    public interface IHttpRequestBuilder
    {
        HttpRequest GetRequest(RequestDescription description);
    }
}