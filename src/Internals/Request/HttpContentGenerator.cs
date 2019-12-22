using Rapidity.Http.Extensions;
using Rapidity.Http.Serializes;
using System;
using System.IO;
using System.Net.Http;
using System.Text;

namespace Rapidity.Http
{
    /// <summary>
    /// 
    /// </summary>
    internal class HttpContentGenerator : IHttpContentGenerator
    {

        //public HttpContentGenerator()
        //{
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="descriptor"></param>
        /// <returns></returns>
        public HttpContent GetContent(RequestDescriptor descriptor)
        {
            if (descriptor.Body == null) return null;

            //如果未设置contentType则按stringcontent发送
            if (string.IsNullOrEmpty(descriptor.HttpOption.ContentType))
            {
                var encoding = Encoding.GetEncoding(descriptor.HttpOption.Encoding ?? "utf-8");
                return new StringContent(descriptor.Body.ToString(), encoding, MimeTypes.Text.Plain);
            }

            if (descriptor.HttpOption.ContentType.Equals(MimeTypes.Application.Json,
                StringComparison.CurrentCultureIgnoreCase))
            {
                return JsonContent(descriptor.Body, descriptor.HttpOption.Encoding);
            }
            if (descriptor.HttpOption.ContentType.Equals(MimeTypes.Application.Xml,
                StringComparison.CurrentCultureIgnoreCase))
            {
                return XmlContent(descriptor.Body, descriptor.HttpOption.Encoding);
            }
            if (descriptor.HttpOption.ContentType.Equals(MimeTypes.Application.XWwwFormUrlencoded,
                StringComparison.CurrentCultureIgnoreCase))
            {
                return FormUrlContent(descriptor.Body, descriptor.HttpOption.Encoding);
            }
            if (descriptor.HttpOption.ContentType.Equals(MimeTypes.Application.OctetStream,
                StringComparison.CurrentCultureIgnoreCase))
            {
                return ByteContent(descriptor.Body);
            }
            //MultipartFormDataContent

            throw new Exception($"不支持的ContentType{descriptor.HttpOption.ContentType}");
        }

        public HttpContent JsonContent(object body, string encode)
        {
            var serializer = new NewtonsoftJsonSerializer();
            var json = serializer.Serialize(body);
            var encoding = Encoding.GetEncoding(encode ?? "utf-8");
            return new StringContent(json, encoding, MimeTypes.Application.Json);
        }

        public HttpContent XmlContent(object body, string encode)
        {
            var serializer = new XmlContentSerializer();
            var json = serializer.Serialize(body);
            var encoding = Encoding.GetEncoding(encode ?? "utf-8");
            return new StringContent(json, encoding, MimeTypes.Application.Xml);
        }

        public HttpContent FormUrlContent(object body, string encode)
        {
            StringBuilder sb = new StringBuilder();
            var dic = body.ToKeyValuePairs();
            foreach (var item in dic)
            {
                if (item.Key == null) continue;
                var value = item.Value;
                if (!string.IsNullOrEmpty(value))
                {
                    var delimiter = sb.Length > 0 ? "&" : string.Empty;
                    sb.AppendFormat($"{delimiter}{item.Key}={value}");
                }
            }
            var formUrl = Uri.EscapeUriString(sb.ToString());
            var encoding = Encoding.GetEncoding(encode ?? "utf-8");
            return new StringContent(formUrl, encoding, MimeTypes.Application.XWwwFormUrlencoded);
        }


        public HttpContent ByteContent(object body)
        {
            if (body is byte[] byteArray)
            {
                return new ByteArrayContent(byteArray);
            }
            if (body is Stream stream)
            {
                //if (!stream.CanRead) throw new Exception("当前流不可读");
                //stream.Seek(0, SeekOrigin.Begin);
                //var bytes = new byte[stream.Length];
                //stream.Read(bytes, 0, bytes.Length);
                //stream.Seek(0, SeekOrigin.Begin);
                //return new ByteArrayContent(bytes);
                //MultipartFormDataContent
                return new StreamContent(stream);
            }
            throw new Exception($"不支持的body类型{body.GetType()}");
        }
    }
}