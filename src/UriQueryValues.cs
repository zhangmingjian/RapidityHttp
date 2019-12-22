using Microsoft.Extensions.Primitives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Rapidity.Http
{
    /// <summary>
    /// 
    /// </summary>
    public class UriQueryValues : StringKeyValues
    {
        public UriQueryValues() { }

        public UriQueryValues(string key, string value) : base(key, value) { }

        public UriQueryValues(string key, IEnumerable<string> values) : base(key, values) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static UriQueryValues FromUri(string uri)
        {
            return FromUri(new Uri(uri));
        }

        public static UriQueryValues FromUri(Uri uri)
        {
            string queryValue = uri.GetComponents(UriComponents.Query, UriFormat.SafeUnescaped);
            if (string.IsNullOrWhiteSpace(queryValue)) return new UriQueryValues();
            var values = queryValue.Split('&');
            var uriQuery = new UriQueryValues();
            foreach (var value in values)
            {
                var arrays = value.Split('=');
                uriQuery.Add(arrays[0], arrays[1]);
            }
            return uriQuery;
        }

        /// <summary>
        /// 连接一个uri地址
        /// </summary>
        /// <param name="uriString"></param>
        /// <returns></returns>
        public Uri Concat(string uriString)
        {
            if (Count == 0)  return new Uri(uriString, UriKind.RelativeOrAbsolute);
            var uriStr = uriString.TrimEnd('&').TrimEnd('?');
            uriStr = $"{uriStr}{(uriStr.Contains('?') ? "&" + ToString() : "?" + ToString())}";
            return new Uri(uriStr, UriKind.RelativeOrAbsolute);
        }

        /// <summary>
        /// 连接一个uri地址
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public Uri Concat(Uri uri)
        {
            return Concat(uri.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Count == 0) return string.Empty;
            StringBuilder builder = new StringBuilder();
            foreach (var key in Keys)
            {
                foreach (var value in this[key])
                {
                    if (builder.Length > 0) builder.Append("&");
                    builder.AppendFormat("{0}={1}", key, value);
                }
            }
            var url = builder.ToString();
            return Uri.EscapeUriString(url);
        }
    }
}