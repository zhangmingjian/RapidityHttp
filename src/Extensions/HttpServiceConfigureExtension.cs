using System;
using System.Linq;
using System.Reflection;
using Rapidity.Http.Attributes;
using Rapidity.Http.Configurations;

namespace Rapidity.Http.Extensions
{
    public static class HttpServiceConfigureExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceConfigure"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public static HttpServiceConfigureItem ForTypes(this HttpServiceConfigureItem serviceConfigure, params Type[] types)
        {
            var list = serviceConfigure.ForTypes.ToList();
            list.AddRange(types);
            serviceConfigure.ForTypes = list;
            return serviceConfigure;
        }

        /// <summary>
        /// 批量注册 用于整个项目对接一个服务时
        /// </summary>
        /// <param name="serviceConfigure"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static HttpServiceConfigureItem ForTypes(this HttpServiceConfigureItem serviceConfigure, Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            var types = assembly.GetTypes()
                .Where(type =>typeof(IHttpService).IsAssignableFrom(type) || type.GetCustomAttribute<HttpServiceAttribute>() != null);
            return serviceConfigure.ForTypes(types.ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="serviceConfigure"></param>
        /// <returns></returns>
        public static HttpServiceConfigureItem For<TService>(this HttpServiceConfigureItem serviceConfigure)
        {
            serviceConfigure.ForTypes.Add(typeof(TService));
            return serviceConfigure;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceConfigure"></param>
        /// <param name="moduleName"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public static HttpServiceConfigureItem AddModule(this HttpServiceConfigureItem serviceConfigure, string moduleName, HttpOption option)
        {
            serviceConfigure.ModuleOptions[moduleName] = option;
            return serviceConfigure;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceConfigure"></param>
        /// <param name="moduleName"></param>
        /// <param name="configAction"></param>
        /// <returns></returns>
        public static HttpServiceConfigureItem AddModule(this HttpServiceConfigureItem serviceConfigure, string moduleName, Action<HttpOption> configAction)
        {
            var option = new HttpOption();
            configAction?.Invoke(option);
            serviceConfigure.ModuleOptions[moduleName] = option;
            return serviceConfigure;
        }
    }
}