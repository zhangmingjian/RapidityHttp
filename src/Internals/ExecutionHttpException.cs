using System;

namespace Rapidity.Http
{
    /// <summary>
    /// 
    /// </summary>
    internal class ExecutionHttpException : Exception
    {
        public HttpRequest Request { get; set; }

        public ExecutionHttpException() { }

        public ExecutionHttpException(HttpRequest request, Exception innerException) : base("", innerException)
        {
            this.Request = request;
        }
    }
}