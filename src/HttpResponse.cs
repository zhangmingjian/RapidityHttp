using Rapidity.Http.Extensions;
using System;
using System.Net.Http;

namespace Rapidity.Http
{
    public class HttpResponse : HttpResponseMessage
    {
        /// <summary>
        /// 本次请求id
        /// </summary>
        public string RequestId => base.RequestMessage?.Properties.ValueOrDefault(ConstantValue.RequestId) as string;
        /// <summary>
        /// RequestHashValue
        /// </summary>
        public string RequestHashValue => base.RequestMessage?.Properties.ValueOrDefault(ConstantValue.RequestHashCode) as string;
        /// <summary>
        /// 请求开始时间
        /// </summary>
        public long RequestTimeStamp => base.RequestMessage?.Properties.ValueOrDefault(ConstantValue.RequestTimeStamp) as long? ?? 0;
        /// <summary>
        /// 请求结束时间
        /// </summary>
        public long ResponseTimeStamp { get; }
        /// <summary>
        /// 执行时间
        /// </summary>
        public long Duration => (long)TimeSpan.FromTicks(ResponseTimeStamp - RequestTimeStamp).TotalMilliseconds;

        public HttpResponse(HttpResponseMessage response)
        {
            this.Content = response.Content;
            this.RequestMessage = response.RequestMessage;
            this.ReasonPhrase = response.ReasonPhrase;
            this.StatusCode = response.StatusCode;
            this.Version = response.Version;
            foreach (var header in response.Headers)
            {
                this.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
            ResponseTimeStamp = DateTime.Now.Ticks;
        }
    }
}