using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.Extensions.Logging;
using Rapidity.Http.Extensions;

namespace Rapidity.Http
{
    /// <summary>
    /// 
    /// </summary>
    public class UriGenerator : IUriGenerator
    {
        private readonly ILogger<UriGenerator> _logger;

        public UriGenerator(ILogger<UriGenerator> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        public Uri GetUri(RequestDescriptor description)
        {
            var tempUri = description.HttpOption.Uri;
            var template = new StringTemplate(tempUri);
            if (template.HaveVariable) //使用query替换变量
            {
                tempUri = template.TryReplaceVariable(description.UriQuery);
                template = new StringTemplate(tempUri);
                if (template.HaveVariable) //使用context.ExtendData替换变量
                {
                    _logger.LogInformation($"uri:{tempUri} 使用{nameof(description.UriQuery)}未完全替换");
                    tempUri = template.TryReplaceVariable(description.ExtendData);
                    template = new StringTemplate(tempUri);
                    if (template.HaveVariable) //使用body替换变量
                    {
                        _logger.LogInformation($"uri:{tempUri}使用{nameof(description.ExtendData)}未完全替换");
                        tempUri = template.TryReplaceVariable(description.Body, true);
                    }
                }
            }
            if (description.UriQuery.Count > 0)
                tempUri = $"{tempUri}{(tempUri.Contains("?") ? "&" : "?")}{description.UriQuery.ToQueryString()}";

            tempUri = Uri.EscapeUriString(tempUri);
            return new Uri(tempUri, UriKind.RelativeOrAbsolute);
        }
    }
}