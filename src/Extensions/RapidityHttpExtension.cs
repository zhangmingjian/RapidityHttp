using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Rapidity.Http.Configurations;
using Rapidity.Http.Serializes;
using System;
using System.Linq;
using Rapidity.Http.Attributes;
using System.Reflection;
using Rapidity.Http.DynamicProxies;

namespace Rapidity.Http.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class RapidityHttpExtension
    {
        public static IServiceCollection UseRapidHttp(this IServiceCollection services)
        {
            services.TryAddTransient<IJsonContentSerializer, NewtonsoftJsonSerializer>();
            services.TryAddTransient<IXmlContentSerializer, XmlContentSerializer>();
            services.TryAddTransient(typeof(Lazy<>));

            services.AddTransient<IHttpClientWrapper, HttpClientWrapper>()
                .AddTransient<IRequestBuilderFactory, RequestBuilderFactory>()
                .AddTransient<IResponseResolverFactory, ResponseResolverFactory>()
                .AddTransient<DefaultHttpRequestBuilder>()
                .AddTransient<JsonResponseResolver>()
                .AddTransient<XmlResponseResolver>()
                .AddTransient<IUriGenerator, UriGenerator>()
                .AddTransient<IRequestHeaderSetter, RequestHeaderSetter>()
                .AddTransient<IHttpContentGenerator, HttpContentGenerator>()
                .AddSingleton<IRequestDescriptionBuilder, DefaultRequestDescriptionBuilder>()
                .AddTransient<IRetryPolicyProcessor, DefaultRetryPolicyProcessor>()
                .AddTransient<IInvokeRecordStore, NullInvokeRecordStore>();
            services.AddHttpClient();
            return services;
        }

        /// <summary>
        /// 获取服务配置（如未初始化则先初始化）
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IHttpServiceConfiguration ServiceConfigure(this IServiceCollection services)
        {
            var configuration = (IHttpServiceConfiguration)services
                .FirstOrDefault(x => x.ServiceType == typeof(IHttpServiceConfiguration))?.ImplementationInstance;
            if (configuration == null)
            {
                configuration = new HttpServiceConfiguration();
                services.AddSingleton(configuration);
            }
            return configuration;
        }


        /// <summary>
        /// 添加服务配置，并添加对应的httpClient
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static HttpServiceConfigure AddService(this IServiceCollection services, HttpServiceConfigure configure)
        {
            var configuration = services.ServiceConfigure();

            var config = configuration.AddConfigure(configure);
            services.AddHttpClient(configure.ServiceName, client =>
            {
                //设置默认baseAddress
                if (!string.IsNullOrEmpty(config.BaseAddress))
                    client.BaseAddress = new Uri(config.BaseAddress, UriKind.Absolute);
                //设置默认headers
                if (config.Option.DefaultHeaders.Count > 0)
                {
                    foreach (var key in config.Option.DefaultHeaders.AllKeys)
                        client.DefaultRequestHeaders.Add(key, config.Option.DefaultHeaders.Get(key));
                }

                //设置请求超时时间(默认30秒)
                client.Timeout = TimeSpan.FromSeconds(config.Timeout > 0 ? config.Timeout : 30);
            });
            services.Replace(ServiceDescriptor.Singleton(configuration));
            return config;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static HttpServiceConfigure AddService(this IServiceCollection services, string name)
        {
            var configure = new HttpServiceConfigure
            {
                ServiceName = name
            };
            return services.AddService(configure);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="name"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static HttpServiceConfigure AddService(this IServiceCollection services, string name, Action<HttpServiceConfigure> action)
        {
            var configure = new HttpServiceConfigure
            {
                ServiceName = name
            };
            action?.Invoke(configure);
            return services.AddService(configure);
        }

        /// <summary>
        /// 为HttpService创建动态代理
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection BuildProxy(this IServiceCollection services)
        {
            var configuration = services.ServiceConfigure();
            foreach (var config in configuration)
            {
                foreach (var type in config.ForTypes)
                {
                    if (!type.IsInterface) continue;
                    if (typeof(IHttpService).IsAssignableFrom(type)
                         || (type.GetCustomAttribute<HttpServiceAttribute>()?.GenerateProxy ?? false))
                    {
                        services.AddSingleton(type, provider => ServiceProxy.Create(type, provider));
                    }
                }
            }

            return services;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TRecordStore"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection ConfigRecordStore<TRecordStore>(this IServiceCollection services) where TRecordStore : IInvokeRecordStore
        {
            return services.Replace(ServiceDescriptor.Transient(typeof(IInvokeRecordStore), typeof(TRecordStore)));
        }
    }
}