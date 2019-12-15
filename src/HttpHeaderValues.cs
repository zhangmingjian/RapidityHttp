using Microsoft.Extensions.Primitives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Rapidity.Http
{
    /// <summary>
    /// HttpHeaders
    /// </summary>
    public class HttpHeaderValues : StringKeyValues
    {
        public HttpHeaderValues() { }
        public HttpHeaderValues(string key, string value) : base(key, value) { }
        public HttpHeaderValues(string key, IEnumerable<string> values) : base(key, values) { }

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
                if (builder.Length > 0) builder.AppendLine();
                builder.AppendFormat("{0}: {1}", key, string.Join(";", this[key]));
            }
            return builder.ToString();
        }
    }
}
