using System;
using System.Net.Http;

namespace Rapidity.Http.Configurations
{
    /// <summary>
    /// 缓存配置项
    /// </summary>
    public class CacheOption
    {
        /// <summary>
        /// 缓存开关
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public TimeSpan? ExpireIn { get; set; } = TimeSpan.FromMinutes(1);

        /// <summary>
        /// 过期类型
        /// </summary>
        public ExpireType? ExpireType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Func<HttpResponse, bool> CanCached { get; set; }

        public CacheOption()
        {
            CanCached = response =>
            {
                //只有成功的响应，且get,delete请求才允许缓存
                if (response == null
                    || response.StatusCode < System.Net.HttpStatusCode.OK
                    || response.StatusCode >= System.Net.HttpStatusCode.Ambiguous)
                    return false;
                var method = response.RequestMessage.Method;
                return method == HttpMethod.Get || method == HttpMethod.Delete;
            };
        }

        public CacheOption Union(CacheOption other)
        {
            if (other == null) return this;
            var option = new CacheOption();
            option.Enabled = this.Enabled;
            if (this.ExpireIn == null)
                option.ExpireIn = other.ExpireIn;
            if (!this.ExpireType.HasValue)
                this.ExpireType = other.ExpireType;
            return option;
        }
    }

    public enum ExpireType
    {
        Absolute,   //相对于当前时间的固定过期
        Sliding     //滑动窗口过期时间
    }
}