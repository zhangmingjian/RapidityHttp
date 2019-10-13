using System.Threading;
using System.Threading.Tasks;

namespace Rapidity.Http
{
    public interface IHttpClientWrapper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="description"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<ResponseWrapper> SendAndWrapAsync(RequestDescription description, CancellationToken token = default);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TOutData"></typeparam>
        /// <param name="description"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<ResponseWrapper<TOutData>> SendAndWrapAsync<TOutData>(RequestDescription description, CancellationToken token = default);

    }
}