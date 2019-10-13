using System;
using Rapidity.Http.Configurations;

namespace Rapidity.Http.Attributes
{
    /// <summary>
    /// 
    /// </summary>
    public static class AttributeExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static CacheOption GetCacheOption(this CacheAttribute attribute)
        {
            return new CacheOption
            {
                Enabled = attribute.Enabled,
                ExpireIn = attribute.ExpireIn
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static RetryOption GetRetryOption(this RetryAttribute attribute)
        {
            return new RetryOption
            {
                RetryStatusCodes = attribute.RetryStatusCodes,
                TotalTimeout = attribute.TotalTimeout,
                TransientErrorRetry = attribute.TransientErrorRetry,
                WaitIntervals = attribute.WaitIntervals
            };
        }
    }
}