using Rapidity.Http.Configurations;
using System;
using System.Threading.Tasks;

namespace Rapidity.Http
{
    /// <summary>
    /// 
    /// </summary>
    public struct RetryPolicyArgument
    {
        public string Service { get; set; }
        public string Module { get; set; }
        public RetryOption Option { get; set; }
        public HttpRequest Request { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Func<HttpRequest, Task<HttpResponse>> Sending { get; set; }
        /// <summary>
        /// 是否为成功响应
        /// </summary>
        public Func<HttpResponse, bool> IsSuccessResponse { get; set; }
    }

    /// <summary>
    /// 熔断缓存项
    /// </summary>
    internal struct FuseEntry
    {
        public int FailedCount { get; set; }

        public ResponseWrapperResult Result { get; set; }
    }
}