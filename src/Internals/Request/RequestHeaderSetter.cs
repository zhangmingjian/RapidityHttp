using System;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;

namespace Rapidity.Http
{
    /// <summary>
    /// 
    /// </summary>
    public class RequestHeaderSetter : IRequestHeaderSetter
    {
        private readonly ILogger<RequestHeaderSetter> _logger;

        public RequestHeaderSetter(ILogger<RequestHeaderSetter> logger)
        {
            _logger = logger;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="description"></param>
        /// <param name="header"></param>
        public void SetHeader(RequestDescription description, HttpRequestHeaders header)
        {
            var headers = description.Headers;
            foreach (var key in headers.AllKeys)
            {
                var value = headers.Get(key);
                var template = new StringTemplate(value);
                if (!template.HaveVariable)
                {
                    header.Add(key, value);
                    continue;
                }
                _logger.LogInformation($"{key}:{value}需要进行模板替换");
                //尝试使用uri参数进行替换
                value = template.TryReplaceVariable(description.UriQuery);
                template = new StringTemplate(value);
                if (!template.HaveVariable)
                {
                    header.Add(key, value);
                    continue;
                }
                //尝试使用ExtendData进行替换
                value = template.TryReplaceVariable(description.ExtendData);
                template = new StringTemplate(value);
                if (!template.HaveVariable)
                {
                    header.Add(key, value);
                    continue;
                }
                //尝试使用Body进行替换
                value = template.TryReplaceVariable(description.Body, true);
                header.Add(key, value);
            }
        }
    }
}