using System.Threading.Tasks;

namespace Rapidity.Http
{
    /// <summary>
    /// 
    /// </summary>
    public interface IInvokeRecordStore
    {
        Task WriteAsync(RequestDescription description, ResponseWrapperResult result);
    }


    internal class NullInvokeRecordStore : IInvokeRecordStore
    {
        public async Task WriteAsync(RequestDescription description, ResponseWrapperResult result)
        {
            await Task.CompletedTask;
        }
    }

}