using System.Net.Http;

namespace Rapidity.Http
{
    internal class DefaultHttpRequestBuilder : IHttpRequestBuilder
    {
        private readonly IUriGenerator _uriGenerator;
        private readonly IRequestHeaderSetter _headerSetter;
        private readonly IHttpContentGenerator _contentGenerator;

        public DefaultHttpRequestBuilder(IUriGenerator uriGenerator,
            IRequestHeaderSetter headerSetter,
            IHttpContentGenerator contentGenerator)
        {
            _uriGenerator = uriGenerator;
            _headerSetter = headerSetter;
            _contentGenerator = contentGenerator;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        public HttpRequest GetRequest(RequestDescription description)
        {
            var request = new HttpRequest
            {
                Method = description.Method ?? HttpMethod.Post,
                Content = _contentGenerator.GetContent(description)
            };
            _headerSetter.SetHeader(description, request.Headers);
            request.RequestUri = _uriGenerator.GetUri(description);
            return request;
        }
    }
}