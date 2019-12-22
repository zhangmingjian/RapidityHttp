using Rapidity.Http.Serializes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rapidity.Http
{
    /// <summary>
    /// 
    /// </summary>
    internal class XmlResponseResolver : IHttpResponseResolver
    {

        public async Task<TData> Resolve<TData>(HttpResponse response, IDictionary<string, object> properties)
        {
            var content = await response.Content.ReadAsStringAsync();
            var _serializer = new XmlContentSerializer();
            return _serializer.Deserialize<TData>(content);
        }
    }
}