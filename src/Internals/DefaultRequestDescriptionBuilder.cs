using Rapidity.Http.Attributes;
using Rapidity.Http.Configurations;
using Rapidity.Http.Extensions;
using System;
using System.Reflection;

namespace Rapidity.Http
{
    internal class DefaultRequestDescriptionBuilder : IRequestDescriptionBuilder
    {
        private readonly IHttpServiceConfiguration _config;

        public DefaultRequestDescriptionBuilder(IHttpServiceConfiguration config)
        {
            _config = config;
        }

        public RequestDescription Build(MethodInfo method, params object[] arguments)
        {
            var methodOption = method.GetConfigureItem();
            var moduleOption = method.ReflectedType.GetConfigureItem(methodOption);
            //从config中获取
            var configure = string.IsNullOrEmpty(moduleOption.Service)
                ? _config.Get(method.ReflectedType, true)
                : _config.Get(moduleOption.Service);
            if (configure == null)
                throw new Exception($"ServiceName:{moduleOption.Service}或{method.ReflectedType}应至少有一项在配置中");
            var option = moduleOption.Option.Union(configure.GetConfigureItem(moduleOption.Module), true);

            var description = new RequestDescription
            {
                ServiceName = configure.ServiceName,
                ModuleName = moduleOption.Module ?? method.Name,
                Uri = option.Uri,
                Method = option.Method,
                Encoding = option.Encoding,
                ContentType = option.ContentType,
                CacheOption = option.CacheOption,
                RetryOption = option.RetryOption,
                RequestBuilderType = option.RequestBuilderType,
                ResponseResolverType = option.ResponseResolverType
            };
            description.Headers.Add(option.DefaultHeaders);
            return Initialize(description, method, arguments);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="description"></param>
        /// <param name="method"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        private RequestDescription Initialize(RequestDescription description, MethodInfo method, object[] arguments)
        {
            arguments = arguments ?? new object[0];
            var methodParameters = method.GetParameters();
            if (methodParameters.Length != arguments.Length)
                throw new Exception($"参数{nameof(arguments)}个数与方法{method.Name}的参数个数不一致");
            //header转换
            var headerAttrOnMethod = method.GetCustomAttributes<HeaderAttribute>();
            foreach (var header in headerAttrOnMethod)
                description.Headers.Add(header.Name, header.Value);
            //方法参数转换
            for (int index = 0; index < methodParameters.Length; index++)
            {
                var parameter = methodParameters[index];
                var parameterValue = arguments[index];
                var queryAttr = parameter.GetCustomAttribute<QueryAttribute>();
                var headerAttr = parameter.GetCustomAttribute<HeaderAttribute>();
                var bodyAttr = parameter.GetCustomAttribute<BodyAttribute>();
                if (queryAttr != null)
                {
                    if (!queryAttr.CanNull && parameterValue == null)
                        throw new ArgumentNullException(parameter.Name);
                    if (parameter.ParameterType.IsSimpleType())
                    {
                        var name = string.IsNullOrEmpty(queryAttr.Name) ? parameter.Name : queryAttr.Name;
                        description.UriQuery.Add(name, parameterValue?.ToString());
                    }
                    else
                    {
                        description.UriQuery.Add(parameterValue.ToNameValueCollection());
                    }
                }
                if (headerAttr != null)
                {
                    if (!headerAttr.CanNull && parameterValue == null)
                        throw new ArgumentNullException(parameter.Name);
                    if (parameter.ParameterType.IsSimpleType())
                    {
                        var name = string.IsNullOrEmpty(headerAttr.Name) ? parameter.Name : headerAttr.Name;
                        description.Headers.Add(name, parameterValue?.ToString());
                    }
                    else
                    {
                        description.Headers.Add(parameterValue.ToNameValueCollection());
                    }
                }
                if (bodyAttr != null)
                {
                    if (!bodyAttr.CanNull && parameterValue == null)
                        throw new ArgumentNullException(parameter.Name);
                    description.Body = parameterValue;
                }
                if (queryAttr == null && headerAttr == null && bodyAttr == null)
                    description.ExtendData.Add(parameter.Name, parameterValue);
            }
            return description;
        }
    }
}