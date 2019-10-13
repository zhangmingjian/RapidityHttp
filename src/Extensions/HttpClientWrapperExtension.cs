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
        /// <param name="description"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task SendAsync(this IHttpClientWrapper client, RequestDescription description, CancellationToken token = default)
        {
            await client.SendAndWrapAsync(description, token);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TOutData"></typeparam>
        /// <param name="client"></param>
        /// <param name="description"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<TOutData> SendAsync<TOutData>(this IHttpClientWrapper client, RequestDescription description, CancellationToken token = default)
        {
            var response = await client.SendAndWrapAsync<TOutData>(description, token);
            return response.Data;
        }
    }
}