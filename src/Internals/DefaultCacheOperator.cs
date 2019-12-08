using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using Rapidity.Http.Configurations;

namespace Rapidity.Http
{
    public class DefaultCacheOperator : ICacheOperator
    {
        private readonly IMemoryCache _caching;

        public DefaultCacheOperator(IMemoryCache caching)
        {
            this._caching = caching;
        }

        public ResponseWrapperResult Read(HttpRequest request)
        {
            var result = new ResponseWrapperResult
            {
                Request = request
            };
            var flag = _caching.TryGetValue<HttpResponse>(request.RequestHash, out HttpResponse response);
            if (flag)
            {
                result.HasHitCache = true;
                result.Response = response;
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        /// <param name="option"></param>
        public void Write(HttpResponse response, CacheOption option)
        {
            if (option == null || !option.Enabled || !option.CanCached(response)) return;
            var cacheOption = new MemoryCacheEntryOptions();
            var type = option.ExpireType ?? ExpireType.Absolute;
            switch (type)
            {
                case ExpireType.Absolute:
                    cacheOption.AbsoluteExpirationRelativeToNow = option.ExpireIn;
                    break;
                case ExpireType.Sliding:
                    cacheOption.SlidingExpiration = option.ExpireIn;
                    break;
            }
            _caching.Set<HttpResponse>(response.RequestHashValue, response, cacheOption);
        }
    }
}
