using System.Threading;
using System.Threading.Tasks;

namespace Rapidity.Http
{
    public interface IHttpClientWrapper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="descriptor"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<ResponseWrapper> SendAndWrapAsync(RequestDescriptor descriptor, CancellationToken token = default);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TOutData"></typeparam>
        /// <param name="descriptor"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<ResponseWrapper<TOutData>> SendAndWrapAsync<TOutData>(RequestDescriptor descriptor, CancellationToken token = default);

    }
}