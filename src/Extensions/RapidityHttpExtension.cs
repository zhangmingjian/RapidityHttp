﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Rapidity.Http.Configurations;
using Rapidity.Http.Serializes;
using System;
using System.Linq;
using Rapidity.Http.Attributes;
using System.Reflection;
using Rapidity.Http.DynamicProxies;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Extensions.DependencyInjection;

namespace Rapidity.Http.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class RapidityHttpExtension
    {
        public static IServiceCollection AddRapidityHttp(this IServiceCollection services)
        {
            services.AddTransient<IHttpClientWrapper, HttpClientWrapper>()
                .AddTransient<IRequestBuilderFactory, RequestBuilderFactory>()
                .AddTransient<IResponseResolverFactory, ResponseResolverFactory>()
                .AddTransient<DefaultHttpRequestBuilder>()
                .AddTransient<JsonResponseResolver>()
                .AddTransient<XmlResponseResolver>()
                .AddTransient<IHttpContentGenerator, HttpContentGenerator>()
                .AddSingleton<IRequestDescriptorBuilder, DefaultRequestDescriptorBuilder>()
                .AddTransient<IRetryPolicyProcessor, DefaultRetryPolicyProcessor>()
                .AddTransient<ICacheOperator, DefaultCacheOperator>()
                .AddTransient<IInvokeRecordStore, NullInvokeRecordStore>();

            services.AddHttpClient()
                    .AddMemoryCache();
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
        public static HttpServiceConfigureItem AddHttpService(this IServiceCollection services, HttpServiceConfigureItem configure)
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
                    foreach (var key in config.Option.DefaultHeaders.Keys)
                        client.DefaultRequestHeaders.Add(key, config.Option.DefaultHeaders[key]);
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
        public static HttpServiceConfigureItem AddHttpService(this IServiceCollection services, string name)
        {
            var configure = new HttpServiceConfigureItem
            {
                ServiceName = name
            };
            return services.AddHttpService(configure);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="name"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static HttpServiceConfigureItem AddHttpService(this IServiceCollection services, string name, Action<HttpServiceConfigureItem> action)
        {
            var configure = new HttpServiceConfigureItem
            {
                ServiceName = name
            };
            action?.Invoke(configure);
            return services.AddHttpService(configure);
        }

        /// <summary>
        /// 为IHttpService创建服务实现
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection BuildProxy(this IServiceCollection services, CodeGeneratorOption option = default)
        {
            var configuration = services.ServiceConfigure();
            var types = new Collection<Type>();
            foreach (var config in configuration)
            {
                foreach (var type in config.ForTypes)
                {
                    if (!type.IsInterface) continue;
                    if (typeof(IHttpService).IsAssignableFrom(type)
                         || (type.GetCustomAttribute<HttpServiceAttribute>()?.GenerateProxy ?? false))
                    {
                        types.Add(type);
                    }
                }
            }
            var assembly = ProxyGenerator.Generate(types.ToArray(), option);
            foreach (var type in types)
            {
                var proxyType = assembly.ExportedTypes.First(x => type.IsAssignableFrom(x));
                services.AddTransient(type, proxyType);
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

        /// <summary>
        /// 配置默认日志记录器
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection ConfigDefaultRecordStore(this IServiceCollection services)
        {
            return services.ConfigRecordStore<TextInvokeRecordStore>();
        }
    }
}