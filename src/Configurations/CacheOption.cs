using System;

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
        public TimeSpan? ExpireIn { get; set; }

        public CacheOption Union(CacheOption other)
        {
            if (other == null) return this;
            var option = new CacheOption();
            option.Enabled = this.Enabled;
            if (this.ExpireIn == null)
                option.ExpireIn = other.ExpireIn;
            return option;
        }
    }
}