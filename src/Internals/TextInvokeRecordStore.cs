using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Rapidity.Http
{
    /// <summary>
    /// 
    /// </summary>
    public class TextInvokeRecordStore : IInvokeRecordStore
    {
        private static object _lock = new object();

        public Task WriteAsync(RequestDescription description, ResponseWrapperResult result)
        {
            var sb = new StringBuilder();
            sb.Append(DateTime.Now + ", ");
            sb.Append($"Url:{GetUriPath(result.Request.RequestUri.ToString())}, ");
            sb.Append($"Duration:{result.Duration}ms, ");
            sb.Append($"RetryCount:{result.RetryCount}, ");

            var content = result.RawResponse != null && result.RawResponse.Length > 2000
                ? result.RawResponse?.Substring(0, 2000) + "..."
                : result.RawResponse;
            sb.Append($"RawResponse:{content}, "); //最多记录2000个字符

            if (result.Exception != null)
                sb.Append($"Exception:{result.Exception.Message}:{result.Exception}");
            sb.AppendLine();

            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", DateTime.Today.ToString("yyyyMMdd"));
            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);
            var fileName = Path.Combine(filePath, description.ServiceName + ".log");
            var fileInfo = new FileInfo(fileName);

            lock (_lock)
            {
                using (var write = fileInfo.Open(FileMode.Append, FileAccess.Write, FileShare.Read))
                {
                    var buffer = Encoding.UTF8.GetBytes(sb.ToString());
                    write.Write(buffer, 0, buffer.Length);
                    return Task.CompletedTask;
                }
            }
        }

        private string GetUriPath(string uri)
        {
            if (string.IsNullOrEmpty(uri)) return uri;
            var index = uri.IndexOf('?');
            if (index <= -1) return uri;
            return uri.Substring(0, index);
        }
    }
}