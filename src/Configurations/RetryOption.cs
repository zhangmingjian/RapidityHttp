using System;

namespace Rapidity.Http.Configurations
{
    /// <summary>
    /// 重试配置
    /// </summary>
    public class RetryOption
    {
        /// <summary>
        /// 重试次数
        /// </summary>
        public int RetryCount => WaitIntervals?.Length ?? 0;

        /// <summary>
        /// 重试等待间隔时间 ms
        /// </summary>
        public int[] WaitIntervals { get; set; }

        /// <summary>
        /// 瞬间网络异常重试
        /// </summary>
        public bool? TransientErrorRetry { get; set; } = true;

        /// <summary>
        /// 当遇到以下状态码时执行重试
        /// </summary>
        public int[] RetryStatusCodes { get; set; } = { 408, 500, 501, 502, 503, 504, 505 };

        /// <summary>
        /// 允许重试的methods
        /// </summary>
        public string[] RetryMethods { get; set; } = { "get", "delete" };

        /// <summary>
        /// 总超时时间(ms)
        /// </summary>
        public int TotalTimeout { get; set; } = 30000;

        /// <summary>
        /// 
        /// </summary>
        public Func<RequestRecord, bool> CanRetry { get; set; }

        /// <summary>
        /// 是否开启熔断
        /// </summary>
        public bool FuseEnabled => FuseEnabledWhenFailedCount > 0;

        /// <summary>
        /// 当失败次数达到多少次时启用熔断
        /// </summary>
        public int FuseEnabledWhenFailedCount { get; set; } = 3;

        /// <summary>
        /// 熔断时间
        /// </summary>
        public int FuseDuration { get; set; } = 60000;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public RetryOption Union(RetryOption other)
        {
            if (other == null) return this;
            var option = new RetryOption
            {
                WaitIntervals = this.WaitIntervals,
                RetryStatusCodes = this.RetryStatusCodes,
                TotalTimeout = this.TotalTimeout,
                TransientErrorRetry = this.TransientErrorRetry,
                RetryMethods = this.RetryMethods,
                CanRetry = this.CanRetry,
                FuseEnabledWhenFailedCount = this.FuseEnabledWhenFailedCount,
                FuseDuration = this.FuseDuration,
            };
            if (option.WaitIntervals == null || option.WaitIntervals.Length <= 0)
                option.WaitIntervals = other.WaitIntervals;
            if (option.TransientErrorRetry == null)
                option.TransientErrorRetry = other.TransientErrorRetry;
            if (option.RetryStatusCodes == null)
                option.RetryStatusCodes = other.RetryStatusCodes;
            if (option.TotalTimeout <= 0)
                option.TotalTimeout = other.TotalTimeout;
            if (option.RetryMethods == null || option.RetryMethods.Length <= 0)
                option.RetryMethods = other.RetryMethods;
            if (option.CanRetry == null)
                option.CanRetry = other.CanRetry;
            if (option.FuseEnabledWhenFailedCount <= 0)
                option.FuseEnabledWhenFailedCount = other.FuseEnabledWhenFailedCount;
            if (option.FuseDuration <= 0)
                option.FuseDuration = other.FuseDuration;
            return other;
        }
    }
}