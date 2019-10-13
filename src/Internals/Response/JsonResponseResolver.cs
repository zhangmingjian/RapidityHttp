using Rapidity.Http.Serializes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rapidity.Http
{
    /// <summary>
    /// 
    /// </summary>
    public class JsonResponseResolver : IHttpResponseResolver
    {
        private readonly IJsonContentSerializer _serializer;

        public JsonResponseResolver(IJsonContentSerializer serializer)
        {
            _serializer = serializer;
        }

        public async Task<TData> Resolve<TData>(HttpResponse response, IDictionary<string, object> properties)
        {
            var content = await response.Content.ReadAsStringAsync();
            return _serializer.Deserialize<TData>(content);
        }
    }
}