using Microsoft.Extensions.DependencyInjection;
using System;

namespace Rapidity.Http
{
    /// <summary>
    /// 
    /// </summary>
    internal class RequestBuilderFactory : IRequestBuilderFactory
    {
        private readonly IServiceProvider _service;

        public RequestBuilderFactory(IServiceProvider service)
        {
            _service = service;
        }

        public IHttpRequestBuilder GetBuilder(Type type)
        {
            if (type == null)
                return _service.GetService<DefaultHttpRequestBuilder>();
            return (IHttpRequestBuilder)_service.GetService(type);
        }
    }
}