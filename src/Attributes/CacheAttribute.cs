using System;

namespace Rapidity.Http.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Method)]
    public class CacheAttribute : Attribute
    {
        /// <summary>
        /// 缓存开关
        /// </summary>
        public bool Enabled { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        public TimeSpan? ExpireIn { get; }

        public CacheAttribute(bool enabled)
        {
            this.Enabled = enabled;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enabled"></param>
        /// <param name="expireInSenonds">过期时间（ms）</param>
        public CacheAttribute(bool enabled, int expireInSenonds) : this(enabled)
        {
            if (expireInSenonds > 0)
            {
                this.ExpireIn = TimeSpan.FromSeconds(expireInSenonds);
            }
        }
    }
}