﻿using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Rapidity.Http.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Rapidity.Http
{
    /// <summary>
    /// 重试/熔断降级Processor
    /// </summary>
    internal class DefaultRetryPolicyProcessor : IRetryPolicyProcessor
    {
        private readonly ILogger<DefaultRetryPolicyProcessor> _logger;
        private readonly IMemoryCache _cache;

        public DefaultRetryPolicyProcessor(ILogger<DefaultRetryPolicyProcessor> logger, IMemoryCache cache)
        {
            _logger = logger;
            _cache = cache;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        public async Task<ResponseWrapperResult> ProcessAsync(RetryPolicyArgument argument)
        {
            var fusedResult = TryGetFuseResult(argument.Service, argument.Module);
            if (fusedResult != null) return fusedResult;

            var records = new List<RequestRecord>();
            var option = argument.Option;
            await HandleException(argument.Request, async () =>
             {
                 var timeoutToken = (option?.TotalTimeout ?? 0) > 0
                     ? new CancellationTokenSource(TimeSpan.FromMilliseconds(option.TotalTimeout))
                     : new CancellationTokenSource();
                 return await SendWithRetryAsync(option, argument.Request, argument.Sending, records, timeoutToken.Token);
             }, record =>
             {
                 //只有执行超时才会抛ExecutionHttpException，意味着重试结束
                 if (record.Exception is ExecutionHttpException executionEx)
                 {
                     record.Request = executionEx.Request;
                     record.Exception = executionEx.InnerException;
                     record.Duration = GetDuration(record.Request.TimeStamp);
                     records.Add(record);
                 }
                 return record;
             });
            var result = new ResponseWrapperResult
            {
                Request = argument.Request,
                Records = records,
                Duration = GetDuration(argument.Request.TimeStamp),
                RetryCount = records.Count() > 1 ? records.Count() - 1 : 0
            };
            //获取有效请求记录
            RequestRecord vaildRecord = result.Records.LastOrDefault(x => x.Response != null)
                                            ?? result.Records.LastOrDefault();
            result.Response = vaildRecord?.Response;
            if (result.Response != null)
                result.RawResponse = await result.Response.Content.ReadAsStringAsync();
            result.Exception = vaildRecord?.Exception;
            _logger.LogInformation($"请求{argument.Request.RequestUri}执行完毕，用时:{result.Duration}ms");
            SetFunseResult(argument, result);
            return result;
        }

        private ResponseWrapperResult TryGetFuseResult(string service, string module)
        {
            var key = $"{service}.{module}";

            if (_cache.TryGetValue<FuseEntry>(key, out FuseEntry entry))
            {
                if (entry.Result != null)
                    return entry.Result;
            }
            return null;
        }

        public void SetFunseResult(RetryPolicyArgument argument, ResponseWrapperResult result)
        {
            if (argument.Option == null || !argument.Option.FuseEnabled || argument.IsSuccessResponse(result.Response)) 
                return;

            var key = $"{argument.Service}.{argument.Module}";
            FuseEntry entry = default;
            if (_cache.TryGetValue<FuseEntry>(key, out entry))
            {
                if (entry.FailedCount >= argument.Option.FuseEnabledWhenFailedCount - 1)
                {
                    entry.Result = result;
                    entry.Result.Fused = true;
                }
            }
            else entry = new FuseEntry();
            entry.FailedCount++;
            _cache.Set<FuseEntry>(key, entry, TimeSpan.FromMilliseconds(argument.Option.FuseDuration));
        }

        /// <summary>
        /// 重试逻辑
        /// </summary>
        /// <param name="option"></param>
        /// <param name="request"></param>
        /// <param name="sending"></param>
        /// <param name="records"></param>
        /// <param name="timeoutToken"></param>
        /// <param name="retryCount"></param>
        /// <returns></returns>
        private async Task<RequestRecord> SendWithRetryAsync(RetryOption option, HttpRequest request,
            Func<HttpRequest, Task<HttpResponse>> sending,
            List<RequestRecord> records, CancellationToken timeoutToken, int retryCount = 0)
        {
            if (timeoutToken.IsCancellationRequested)
                throw new ExecutionHttpException(request, new TimeoutException($"请求超时，在{option.TotalTimeout}ms内未获取到结果"));
            records = records ?? new List<RequestRecord>();
            var record = await HandleException(request, async () =>
             {
                 var response = await sending(request);
                 var requestRecord = new RequestRecord
                 {
                     Request = request,
                     Response = response,
                     Duration = response.Duration
                 };
                 return requestRecord;
             }, requestRecord =>
             {
                 records.Add(requestRecord);
                 return requestRecord;
             });

            if (!CanRetry(option, record, retryCount)) return record;
            var waitTime = option.WaitIntervals[retryCount];
            await Waiting(request, waitTime, option.TotalTimeout, timeoutToken);
            request = request.Clone();
            return await SendWithRetryAsync(option, request, sending, records, timeoutToken, ++retryCount);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="waitMilliseconds"></param>
        /// <param name="totalTimeout"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task Waiting(HttpRequest request, int waitMilliseconds, int totalTimeout, CancellationToken token)
        {
            if (waitMilliseconds <= 0)
                await Task.FromResult(0);
            try
            {
                await Task.Delay(waitMilliseconds, token);
            }
            catch //将OperationCanceledException异常转换为timeoutException
            {
                throw new ExecutionHttpException(request, new TimeoutException($"请求超时，在{totalTimeout}ms内未获取到结果"));
            }
        }

        /// <summary>
        /// 检查是否满足重试条件
        /// </summary>
        /// <param name="option"></param>
        /// <param name="record"></param>
        /// <param name="retryCount"></param>
        /// <returns></returns>
        private bool CanRetry(RetryOption option, RequestRecord record, int retryCount)
        {
            if (option == null
                || option.RetryCount <= 0
                || retryCount >= option.RetryCount)
                return false;

            if (option.CanRetry != null)
                return option.CanRetry(record);

            if ((option.TransientErrorRetry ?? false) && record.Exception != null)
            {
                if (record.Exception is TimeoutException
                    || record.Exception is OperationCanceledException
                    || record.Exception is HttpRequestException)
                    return true;
                return false;
            }

            var statusCode = (int)record.Response.StatusCode;
            var method = record.Response.RequestMessage.Method.ToString();
            if (option.RetryStatusCodes?.Contains(statusCode) ?? false
                && (option.RetryMethods?.Contains(method, StringComparer.CurrentCultureIgnoreCase) ?? false))
                return true;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="running"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        private async Task<RequestRecord> HandleException(HttpRequest request, Func<Task<RequestRecord>> running, Func<RequestRecord, RequestRecord> callback)
        {
            RequestRecord record = default;
            try
            {
                record = await running();
            }
            catch (Exception ex)
            {
                record = new RequestRecord
                {
                    Request = request,
                    Exception = ex,
                    Duration = GetDuration(request.TimeStamp)
                };
            }
            return callback(record);
        }

        private long GetDuration(long beginTicks)
        {
            return (long)TimeSpan.FromTicks(DateTime.Now.Ticks - beginTicks).TotalMilliseconds;
        }
    }
}