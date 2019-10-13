using System;

namespace Rapidity.Http.Attributes
{
    /// <summary>
    /// 重试
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RetryAttribute : Attribute
    {
        /// <summary>
        /// 瞬间网络异常重试
        /// </summary>
        public bool? TransientErrorRetry { get; set; } = true;

        /// <summary>
        /// 重试等待间隔时间 ms
        /// </summary>
        public int[] WaitIntervals { get; set; }

        /// <summary>
        /// 当遇到以下状态码时执行重试
        /// </summary>
        public int[] RetryStatusCodes { get; set; }

        /// <summary>
        /// 总超时时间(ms)
        /// </summary>
        public int TotalTimeout { get; set; }

        public RetryAttribute()
        {
        }

        public RetryAttribute(params int[] waitIntervals)
        {
            this.WaitIntervals = waitIntervals;
        }
    }
}