using Rapidity.Http.Attributes;
using Rapidity.Http.Configurations;
using Rapidity.Http.Extensions;
using System;
using System.Reflection;

namespace Rapidity.Http
{
    internal class DefaultRequestDescriptorBuilder : IRequestDescriptorBuilder
    {
        private readonly IHttpServiceConfiguration _config;

        public DefaultRequestDescriptorBuilder(IHttpServiceConfiguration config)
        {
            _config = config;
        }

        public RequestDescriptor Build(MethodInfo method, params object[] arguments)
        {
            var methodOption = method.GetConfigureItem();
            var moduleOption = method.ReflectedType.GetConfigureItem(methodOption);
            //从config中获取
            var configure = string.IsNullOrEmpty(moduleOption.Service)
                ? _config.Get(method.ReflectedType, true)
                : _config.Get(moduleOption.Service);
            if (configure == null)
                throw new Exception($"ServiceName:{moduleOption.Service}或{method.ReflectedType}应至少有一项在配置中");
            var option = moduleOption.Option.Union(configure.GetHttpOption(moduleOption.Module), true);

            var descriptor = new RequestDescriptor
            {
                ServiceName = configure.ServiceName,
                ModuleName = moduleOption.Module ?? method.Name,
                HttpOption = option
            };
            return Initialize(descriptor, method, arguments);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="descriptor"></param>
        /// <param name="method"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        private RequestDescriptor Initialize(RequestDescriptor descriptor, MethodInfo method, object[] arguments)
        {
            arguments = arguments ?? new object[0];
            var methodParameters = method.GetParameters();
            if (methodParameters.Length != arguments.Length)
                throw new Exception($"参数{nameof(arguments)}个数与方法{method.Name}的参数个数不一致");
            //header转换
            var headerAttrOnMethod = method.GetCustomAttributes<HeaderAttribute>();
            foreach (var header in headerAttrOnMethod)
                descriptor.Headers.Add(header.Name, header.Value);
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
                    if (parameterValue.GetType().IsSimpleType())
                    {
                        var name = string.IsNullOrEmpty(queryAttr.Name) ? parameter.Name : queryAttr.Name;
                        descriptor.UriQuery.Add(name, parameterValue?.ToString());
                    }
                    else
                    {
                        descriptor.UriQuery.Add(parameterValue.ToNameValueCollection());
                    }
                }
                if (headerAttr != null)
                {
                    if (!headerAttr.CanNull && parameterValue == null)
                        throw new ArgumentNullException(parameter.Name);
                    if (parameterValue.GetType().IsSimpleType())
                    {
                        var name = string.IsNullOrEmpty(headerAttr.Name) ? parameter.Name : headerAttr.Name;
                        descriptor.Headers.Add(name, parameterValue?.ToString());
                    }
                    else
                    {
                        descriptor.Headers.Add(parameterValue.ToNameValueCollection());
                    }
                }
                if (bodyAttr != null)
                {
                    if (!bodyAttr.CanNull && parameterValue == null)
                        throw new ArgumentNullException(parameter.Name);
                    descriptor.Body = parameterValue;
                }
                if (queryAttr == null && headerAttr == null && bodyAttr == null)
                    descriptor.ExtendData.Add(parameter.Name, parameterValue);
            }
            return descriptor;
        }
    }
}