using System;
using System.Net.Http;

namespace Rapidity.Http
{
    public class HttpRequest : HttpRequestMessage
    {
        /// <summary>
        /// 本次请求标识
        /// </summary>
        public string Id
        {
            get => Properties.ContainsKey(ConstantValue.RequestId)
                    ? (string)Properties[ConstantValue.RequestId]
                    : default;
            set => Properties[ConstantValue.RequestId] = value;
        }

        /// <summary>
        /// 当前请求的hash值，用来标识是否相同内容的请求，作为缓存key
        /// </summary>
        public string HashCode
        {
            get => Properties.ContainsKey(ConstantValue.RequestHashCode)
                ? (string)Properties[ConstantValue.RequestHashCode]
                : default;
            set => Properties[ConstantValue.RequestHashCode] = value;
        }

        public long TimeStamp
        {
            get => Properties.ContainsKey(ConstantValue.RequestTimeStamp)
                ? (long)Properties[ConstantValue.RequestTimeStamp]
                : default;
            set => Properties[ConstantValue.RequestTimeStamp] = value;
        }

        public HttpRequest()
        {
            Id = Guid.NewGuid().ToString("n");
            TimeStamp = DateTime.Now.Ticks;
        }

        public HttpRequest Clone()
        {
            var request = new HttpRequest
            {
                Method = this.Method,
                RequestUri = this.RequestUri,
                Content = this.Content,
                Version = this.Version
            };

            foreach (var header in this.Headers)
            {
                request.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            foreach (var property in this.Properties)
            {
                if (property.Key == ConstantValue.RequestTimeStamp) continue;
                request.Properties[property.Key] = property.Value;
            }
            return request;
        }
    }
}