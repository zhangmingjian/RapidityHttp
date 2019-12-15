using Microsoft.Extensions.Logging;
using Rapidity.Http.Extensions;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace Rapidity.Http
{
    public class DefaultHttpRequestBuilder : IHttpRequestBuilder
    {
        private readonly IHttpContentGenerator _contentGenerator;
        private readonly ILogger<DefaultHttpRequestBuilder> _logger;

        public DefaultHttpRequestBuilder(
            IHttpContentGenerator contentGenerator,
            ILogger<DefaultHttpRequestBuilder> logger)
        {
            _contentGenerator = contentGenerator;
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="descriptor"></param>
        /// <returns></returns>
        public HttpRequest GetRequest(RequestDescriptor descriptor)
        {
            var request = new HttpRequest
            {
                Method = descriptor.HttpOption.Method ?? HttpMethod.Post,
                Content = _contentGenerator.GetContent(descriptor)
            };
            SetHeader(descriptor, request.Headers);
            request.RequestUri = GetUri(descriptor);
            request.RequestHash = ComputeRequestHash(descriptor, request);
            return request;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="descriptor"></param>
        /// <returns></returns>
        protected virtual Uri GetUri(RequestDescriptor descriptor)
        {
            var tempUri = descriptor.HttpOption.Uri;
            var template = new StringTemplate(tempUri);
            if (template.HaveVariable) //使用query替换变量
            {
                tempUri = template.TryReplaceVariable(descriptor.UriQuery);
                template = new StringTemplate(tempUri);
                if (template.HaveVariable) //使用context.ExtendData替换变量
                {
                    _logger.LogInformation($"uri:{tempUri} 使用{nameof(descriptor.UriQuery)}未完全替换");
                    tempUri = template.TryReplaceVariable(descriptor.ExtendData);
                    template = new StringTemplate(tempUri);
                    if (template.HaveVariable) //使用body替换变量
                    {
                        _logger.LogInformation($"uri:{tempUri}使用{nameof(descriptor.ExtendData)}未完全替换");
                        tempUri = template.TryReplaceVariable(descriptor.Body, true);
                    }
                }
            }
            return descriptor.UriQuery.Concat(tempUri);
        }

        protected virtual void SetHeader(RequestDescriptor descriptor, HttpRequestHeaders header)
        {
            var headers = descriptor.Headers;
            foreach (var key in headers.Keys)
            {
                foreach (var v in headers[key])
                {
                    var value = v;
                    var template = new StringTemplate(value);
                    if (!template.HaveVariable)
                    {
                        header.Add(key, value);
                        continue;
                    }
                    _logger.LogInformation($"{key}:{value}需要进行模板替换");
                    //尝试使用uri参数进行替换
                    value = template.TryReplaceVariable(descriptor.UriQuery);
                    template = new StringTemplate(value);
                    if (!template.HaveVariable)
                    {
                        header.Add(key, value);
                        continue;
                    }
                    //尝试使用ExtendData进行替换
                    value = template.TryReplaceVariable(descriptor.ExtendData);
                    template = new StringTemplate(value);
                    if (!template.HaveVariable)
                    {
                        header.Add(key, value);
                        continue;
                    }
                    //尝试使用Body进行替换
                    value = template.TryReplaceVariable(descriptor.Body, true);
                    header.Add(key, value);
                }
            }
        }

        /// <summary>
        /// 计算请求hash值
        /// </summary>
        /// <param name="descriptor"></param>
        /// <returns></returns>
        protected virtual string ComputeRequestHash(RequestDescriptor descriptor, HttpRequest request)
        {
            var uri = request.RequestUri.ToString();
            using (var md5 = MD5.Create())
            {
                var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(uri));
                StringBuilder sb = new StringBuilder();
                foreach (var b in bytes)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }
    }
}