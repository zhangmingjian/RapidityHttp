using System;
using Rapidity.Http.Attributes;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Rapidity.Http;
using Rapidity.Http.Extensions;

namespace Sample.Service
{
    /// <summary>
    /// 
    /// </summary>
    [HttpService]
    public interface ITestService
    {
        Task<string> GetAsync<TInput>([Query]IList<TInput> input, [Header]string token) where TInput : MessageTemplate, new();
    }

    [HttpService]
    public interface IGenericService<in TData, TResult>
        where TData : ICollection<MessageTemplate>, new()
        where TResult : class, ITestService
    {
        [Get("/send/{data}")]
        [Header("afee:jfkejlfe")]
        [Cache(true, 10000)]
        Task Send(IEnumerable<TData> list, bool flag = true);
    }
}

