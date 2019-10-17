using System;
using System.Collections.Specialized;
using System.Net.Http;

namespace Rapidity.Http.Configurations
{
    /// <summary>
    /// 当前请求配置参数
    /// </summary>
    public class HttpConfigureItem
    {
        /// <summary>
        /// 请求url
        /// </summary>
        public string Uri { get; set; }
        /// <summary>
        /// 请求方法
        /// </summary>
        public HttpMethod Method { get; set; }

        /// <summary>
        /// ContentType
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// 编码方式
        /// </summary>
        public string Encoding { get; set; }

        /// <summary>
        /// 请求headers
        /// </summary>
        public NameValueCollection DefaultHeaders { get; } = new NameValueCollection();

        /// <summary>
        /// 缓存配置
        /// </summary>
        public CacheOption CacheOption { get; set; }

        /// <summary>
        /// 重试配置
        /// </summary>
        public RetryOption RetryOption { get; set; }

        /// <summary>
        /// 请求构造器类型
        /// </summary>
        public Type RequestBuilderType { get; set; }

        /// <summary>
        /// 响应解析器类型
        /// </summary>
        public Type ResponseResolverType { get; set; }

        /// <summary>
        /// 两个配置项做交集，根据优先级
        /// </summary>
        /// <param name="other"></param>
        /// <param name="isRoot"></param>
        /// <returns></returns>
        public virtual HttpConfigureItem Union(HttpConfigureItem other, bool isRoot = false)
        {
            var option = new HttpConfigureItem
            {
                Uri = this.Uri,
                Method = this.Method,
                ContentType = this.ContentType,
                Encoding = this.Encoding,
                RequestBuilderType = this.RequestBuilderType,
                ResponseResolverType = this.ResponseResolverType,
                CacheOption = this.CacheOption?.Union(other.CacheOption) ?? other.CacheOption,
                RetryOption = this.RetryOption?.Union(other.RetryOption) ?? other.RetryOption
            };
            option.DefaultHeaders.Add(this.DefaultHeaders);
            option.DefaultHeaders.Add(other.DefaultHeaders);
            if (!isRoot && !string.IsNullOrEmpty(other.Uri))
            {
                var delimiter = !other.Uri.Contains("?") && !other.Uri.Contains("&") ? "/" : string.Empty;
                option.Uri = $"{other.Uri.TrimEnd('/')}{delimiter}{option.Uri?.TrimStart('/')}";
            }
            if (string.IsNullOrEmpty(option.ContentType)) option.ContentType = other.ContentType;
            if (string.IsNullOrEmpty(option.Encoding)) option.Encoding = other.Encoding;
            if (option.Method == default) option.Method = other.Method;
            if (option.RequestBuilderType == null) option.RequestBuilderType = other.RequestBuilderType;
            if (option.ResponseResolverType == null) option.ResponseResolverType = other.ResponseResolverType;
            return option;
        }
    }
}