using System;
using System.Net.Http;

namespace Rapidity.Http.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class HttpServiceAttribute : Attribute
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public string Service { get; set; }
        /// <summary>
        /// 模块名称
        /// </summary>
        public string Module { get; set; }

        public string Uri { get; set; }

        public virtual HttpMethod HttpMethod { get; protected set; }

        public string Method
        {
            get => this.HttpMethod?.ToString();
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException(nameof(value));
                HttpMethod = new HttpMethod(value);
            }
        }

        public string ContentType { get; set; }

        public string Encoding { get; set; }

        public Type RequestBuilderType { get; set; }

        public Type ResponseResolverType { get; set; }

        /// <summary>
        /// 是否生成代理类（仅当该标签使用在接口上有效）
        /// </summary>
        public bool GenerateProxy { get; set; } = true;

        public HttpServiceAttribute()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        public HttpServiceAttribute(string service) : this()
        {
            if (string.IsNullOrEmpty(service))
                throw new ArgumentNullException(nameof(service));
            this.Service = service;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        /// <param name="module"></param>
        public HttpServiceAttribute(string service, string module) : this(service)
        {
            if (string.IsNullOrEmpty(module))
                throw new ArgumentNullException(nameof(module));
            this.Module = module;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class GetAttribute : HttpServiceAttribute
    {
        public GetAttribute(string uri)
        {
            this.Uri = uri;
        }

        public override HttpMethod HttpMethod => HttpMethod.Get;
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class PostAttribute : HttpServiceAttribute
    {
        public PostAttribute(string uri)
        {
            this.Uri = uri;
        }

        public override HttpMethod HttpMethod => HttpMethod.Post;
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class PutAttribute : HttpServiceAttribute
    {
        public PutAttribute(string uri)
        {
            this.Uri = uri;
        }

        public override HttpMethod HttpMethod => HttpMethod.Put;
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class DeleteAttribute : HttpServiceAttribute
    {
        public DeleteAttribute(string uri)
        {
            this.Uri = uri;
        }

        public override HttpMethod HttpMethod => HttpMethod.Delete;
    }
}