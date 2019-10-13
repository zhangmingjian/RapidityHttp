using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rapidity.Http
{
    public interface IHttpResponseResolver
    {
        Task<TData> Resolve<TData>(HttpResponse response, IDictionary<string, object> properties);
    }
}