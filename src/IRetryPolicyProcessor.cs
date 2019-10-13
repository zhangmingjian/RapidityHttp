using System;
using System.Threading.Tasks;

namespace Rapidity.Http
{
    /// <summary>
    ///  重试/熔断降级Processor
    /// </summary>
    public interface IRetryPolicyProcessor
    {
        Task<ResponseWrapperResult> ProcessAsync(RetryPolicyContext context, Func<HttpRequest, Task<HttpResponse>> sending);
    }
}