using System;
using System.Net.Http.Headers;

namespace Rapidity.Http
{
    public interface IResponseResolverFactory
    {
        IHttpResponseResolver GetResolver(Type type, HttpContentHeaders headers);
    }
}