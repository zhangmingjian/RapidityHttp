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

        public async Task WriteAsync(RequestDescriptor descriptor, ResponseWrapperResult result)
        {
            var sb = new StringBuilder();
            sb.Append(DateTime.Now + ", ");
            sb.Append($"Url:{GetUriPath(descriptor, result.Request.RequestUri)}, ");
            sb.Append($"HasHitCache:{result.HasHitCache},");
            sb.Append($"Duration:{result.Duration}ms, ");
            sb.Append($"RetryCount:{result.RetryCount}, ");
            sb.Append($"StatusCode:{(result.Response != null ? ((int)result.Response.StatusCode).ToString() : string.Empty)}, ");

            var rawResponse = await result.Response?.Content.ReadAsStringAsync();
            var content = rawResponse != null && rawResponse.Length > 2000
                ? rawResponse?.Substring(0, 2000) + "..."
                : rawResponse;
            sb.Append($"RawResponse:{content}, "); //最多记录2000个字符

            if (result.Exception != null)
                sb.Append($"Exception:{result.Exception.Message}:{result.Exception}");
            sb.AppendLine();

            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);

            var fileName = Path.Combine(filePath, $"{descriptor.ServiceName}_{DateTime.Today.ToString("yyyyMMdd")}.log");
            var fileInfo = new FileInfo(fileName);

            lock (_lock)
            {
                using (var write = fileInfo.Open(FileMode.Append, FileAccess.Write, FileShare.Read))
                {
                    var buffer = Encoding.UTF8.GetBytes(sb.ToString());
                    write.Write(buffer, 0, buffer.Length);
                }
            }
        }

        private string GetUriPath(RequestDescriptor descriptor, Uri uri)
        {
            if (uri.IsAbsoluteUri) return uri.ToString();
            var baseUri = descriptor.HttpOption.Uri;
            return uri.ToString();
        }
    }
}