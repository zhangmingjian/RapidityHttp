using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Rapidity.Http
{
    /// <summary>
    /// 
    /// </summary>
    internal class HttpClientWrapper : IHttpClientWrapper
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IRequestBuilderFactory _builderFactory;
        private readonly IResponseResolverFactory _resolverFactory;
        private readonly ILogger<HttpClientWrapper> _logger;
        private readonly IRetryPolicyProcessor _retryProcessor;
        private readonly IInvokeRecordStore _recordStore;

        public HttpClientWrapper(IHttpClientFactory clientFactory,
            IRequestBuilderFactory builderFactory,
            IResponseResolverFactory resolverFactory,
            ILogger<HttpClientWrapper> logger,
            IRetryPolicyProcessor retryProcessor,
            IInvokeRecordStore recordStore)
        {
            _clientFactory = clientFactory;
            _builderFactory = builderFactory;
            _resolverFactory = resolverFactory;
            _logger = logger;
            _retryProcessor = retryProcessor;
            _recordStore = recordStore;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="descriptor"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<ResponseWrapper> SendAndWrapAsync(RequestDescriptor descriptor, CancellationToken token = default)
        {
            var requestBuilder = _builderFactory.GetBuilder(descriptor.HttpOption.RequestBuilderType);
            var request = requestBuilder.GetRequest(descriptor);

            var result = new ResponseWrapperResult
            {
                Request = request
            };
            var client = _clientFactory.CreateClient(descriptor.ServiceName);
            var argument = new RetryPolicyArgument
            {
                Service = descriptor.ServiceName,
                Module = descriptor.ModuleName,
                Option = descriptor.HttpOption.RetryOption,
                Request = request,
                IsSuccessResponse = descriptor.HttpOption.IsSuccessResponse,
                Sending = async message =>
                {
                    var responseMessage = await client.SendAsync(message, token);
                    return new HttpResponse(responseMessage);
                }
            };
            result = await _retryProcessor.ProcessAsync(argument);
            //调用日志记录器，写缓存（如果符合条件）
            try
            {
                await _recordStore.WriteAsync(descriptor, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"日志记录器{_recordStore.GetType().Name},写入日志时出错：{ex.Message}");
            }

            if (result.Exception != null)
                throw result.Exception;

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TOutData"></typeparam>
        /// <param name="descriptor"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<ResponseWrapper<TOutData>> SendAndWrapAsync<TOutData>(RequestDescriptor descriptor, CancellationToken token = default)
        {
            var wrapper = await SendAndWrapAsync(descriptor, token);
            var response = wrapper.Response;
            var responseResolver = _resolverFactory.GetResolver(descriptor.HttpOption.ResponseResolverType, response?.Content?.Headers);
            var outData = await responseResolver.Resolve<TOutData>(response, descriptor.ExtendData);
            return new ResponseWrapper<TOutData>(wrapper)
            {
                Data = outData
            };
        }
    }
}