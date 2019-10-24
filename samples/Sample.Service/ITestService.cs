using System;
using Rapidity.Http.Attributes;
using System.Collections.Generic;
using System.Threading.Tasks;

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
    public interface IGenericService<in TData> where TData : ICollection<MessageTemplate>, new()
    {
        [Get("/send/{data}")]
        [Header("afee:jfkejlfe")]
        [Rapidity.Http.Attributes.CacheAttribute((Boolean)true, (Int32)10000)]
        Task Send(IEnumerable<TData> list, params string[] data);
    }
}