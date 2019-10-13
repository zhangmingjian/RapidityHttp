using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http.Headers;

namespace Rapidity.Http
{
    /// <summary>
    /// 响应解析器工厂
    /// </summary>
    public class ResponseResolverFactory : IResponseResolverFactory
    {
        private readonly IServiceProvider _provider;

        public ResponseResolverFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public IHttpResponseResolver GetResolver(Type type, HttpContentHeaders headers)
        {
            if (type == null)
            {
                var contentType = headers?.ContentType.MediaType;
                if (contentType == MimeTypes.Application.Json)
                    return _provider.GetService<JsonResponseResolver>();
                if (contentType == MimeTypes.Application.Xml)
                    return _provider.GetService<XmlResponseResolver>();
                throw new Exception("找不到ResponseResolverType，请检查配置");
            }
            return (IHttpResponseResolver)_provider.GetService(type);
        }
    }
}