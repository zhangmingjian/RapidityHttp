using System.Threading;
using System.Threading.Tasks;

namespace Rapidity.Http.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class HttpClientWrapperExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="descriptor"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task SendAsync(this IHttpClientWrapper client, RequestDescriptor descriptor, CancellationToken token = default)
        {
            await client.SendAndWrapAsync(descriptor, token);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TOutData"></typeparam>
        /// <param name="client"></param>
        /// <param name="descriptor"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<TOutData> SendAsync<TOutData>(this IHttpClientWrapper client, RequestDescriptor descriptor, CancellationToken token = default)
        {
            var response = await client.SendAndWrapAsync<TOutData>(descriptor, token);
            return response.Data;
        }
    }
}