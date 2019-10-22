using System.Threading.Tasks;
using Rapidity.Http.Attributes;

namespace Sample.Service
{
    /// <summary>
    /// 
    /// </summary>
    [HttpService]
    public interface ITestService
    {
        Task<string> GetAsync<TInput>([Query]TInput input, [Header]string token) where TInput : class, new();
    }
}