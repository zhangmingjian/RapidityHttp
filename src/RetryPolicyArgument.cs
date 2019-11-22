using Rapidity.Http.Configurations;

namespace Rapidity.Http
{
    /// <summary>
    /// 
    /// </summary>
    public class RetryPolicyArgument
    {
        public string Service { get; set; }
        public string Module { get; set; }
        public RetryOption Option { get; set; }
        public HttpRequest Request { get; set; }
    }
}