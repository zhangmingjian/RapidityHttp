using System;
using System.Collections.Generic;

namespace Rapidity.Http
{
    /// <summary>
    /// 响应结果完整信息
    /// </summary>
    public class ResponseWrapper
    {
        /// <summary>
        /// 响应结果
        /// </summary>
        public HttpResponse Response { get; set; }
        /// <summary>
        /// 异常
        /// </summary>
        public Exception Exception { get; set; }
        /// <summary>
        /// 是否进行了重试
        /// </summary>
        public bool HasRetry => RetryCount > 0;
        /// <summary>
        /// 重试次数
        /// </summary>
        public int RetryCount { get; set; }
        /// <summary>
        /// 是否命中缓存
        /// </summary>
        public bool HasHitCache { get; set; }
        /// <summary>
        /// 执行时间 ms
        /// </summary>
        public long Duration { get; set; }

        /// <summary>
        /// 触发熔断
        /// </summary>
        public bool Fused { get; set; }
    }

    /// <summary>
    /// 反序列化后的响应数据
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public class ResponseWrapper<TData> : ResponseWrapper
    {
        public TData Data { get; set; }

        public ResponseWrapper() { }

        public ResponseWrapper(ResponseWrapper wrapper)
        {
            this.Response = wrapper.Response;
            this.Exception = wrapper.Exception;
            this.HasHitCache = wrapper.HasHitCache;
            this.RetryCount = wrapper.RetryCount;
            this.Duration = wrapper.Duration;
        }
    }


    /// <summary>
    /// 包含重试过程的完整请求数据
    /// </summary>
    public class ResponseWrapperResult : ResponseWrapper
    {
        /// <summary>
        /// 原始请求
        /// </summary>
        public HttpRequest Request { get; set; }

        /// <summary>
        /// 请求过程记录
        /// </summary>
        public IEnumerable<RequestRecord> Records { get; set; }
    }
}