using System;

namespace Rapidity.Http
{
    /// <summary>
    /// 请求记录
    /// </summary>
    public class RequestRecord
    {
        public HttpRequest Request { get; set; }

        public HttpResponse Response { get; set; }
        /// <summary>
        /// 调用时间
        /// </summary>
        public long Duration { get; set; }

        public Exception Exception { get; set; }

        private bool? _isTimeout;
        /// <summary>
        /// 请求是否超时
        /// </summary>
        public bool IsTimeout
        {
            set => _isTimeout = value;
            get
            {
                if (_isTimeout == null)
                {
                    if (Exception != null)
                        _isTimeout = Exception is TimeoutException;
                    else
                        _isTimeout = false;
                }
                return _isTimeout ?? false;
            }
        }
        /// <summary>
        /// 是否有异常
        /// </summary>
        public bool HasException => Exception != null;
    }
}