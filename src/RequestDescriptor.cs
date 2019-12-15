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
    public class RequestDescriptor
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
        /// url查询参数
        /// </summary>
        public UriQueryValues UriQuery { get; } = new UriQueryValues();

        ///// <summary>
        /////  请求headers
        ///// </summary>
        public HttpHeaderValues Headers => HttpOption.DefaultHeaders;

        /// <summary>
        /// 报文体数据
        /// </summary>
        public object Body { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public HttpOption HttpOption { get; set; } = new HttpOption();

        /// <summary>
        /// 扩展数据
        /// </summary>
        public IDictionary<string, object> ExtendData { get; } = new Dictionary<string, object>();
    }
}