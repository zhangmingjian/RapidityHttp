using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using Rapidity.Http.Configurations;

namespace Rapidity.Http
{
    /// <summary>
    /// 当前请求参数信息
    /// </summary>
    public class RequestDescription
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// ModuleName
        /// </summary>
        public string ModuleName { get; set; }

        /// <summary>
        /// 请求路径
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
        /// url查询参数
        /// </summary>
        public NameValueCollection UriQuery { get; } = new NameValueCollection();

        /// <summary>
        ///  请求headers
        /// </summary>
        public NameValueCollection Headers { get; } = new NameValueCollection();

        /// <summary>
        /// 报文体数据
        /// </summary>
        public object Body { get; set; }

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
        /// 扩展数据
        /// </summary>
        public IDictionary<string, object> ExtendData { get; } = new Dictionary<string, object>();
    }
}