using System;

namespace Rapidity.Http
{
    public interface IRequestBuilderFactory
    {
        IHttpRequestBuilder GetBuilder(Type type);
    }
}