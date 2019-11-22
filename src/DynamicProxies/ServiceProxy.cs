using Microsoft.Extensions.DependencyInjection;
using Rapidity.Http.Extensions;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Rapidity.Http.DynamicProxies
{
    /// <summary>
    /// 服务代理调用逻辑
    /// </summary>
    public class ServiceProxy
    {
        private IServiceProvider _provider { get; set; }

        protected virtual object Invoke(MethodInfo targetMethod, object[] args)
        {
            var builder = _provider.GetService<IRequestDescriptorBuilder>();
            var wrapper = _provider.GetService<IHttpClientWrapper>();

            var description = builder.Build(targetMethod, args);

            var isAsync = false;
            Type returnType = targetMethod.ReturnType;
            if (targetMethod.ReturnType == typeof(Task) || targetMethod.ReturnType.Name == "Task`1")
            {
                isAsync = true;
                if (targetMethod.ReturnType.IsGenericType)
                    returnType = targetMethod.ReturnType.GetGenericArguments()[0];
            }

            object result = null;

            var sendMethod = GetInvokeMethod(returnType);
            if (sendMethod.IsGenericMethod)
            {
                var genericType = returnType;
                //如果是ResponseWrapper<>包装 还需要剥离掉ResponseWrapper
                if (returnType.Namespace == typeof(ResponseWrapper).Namespace && returnType.Name == "ResponseWrapper`1")
                {
                    genericType = returnType.GetGenericArguments()[0];
                }
                sendMethod = sendMethod.MakeGenericMethod(genericType);
            }

            var token = args.FirstOrDefault(x => x is CancellationToken);

            result = sendMethod.ReflectedType == typeof(HttpClientWrapperExtension)
                ? sendMethod.Invoke(null, new[] { wrapper, description, token })
                : sendMethod.Invoke(wrapper, new[] { description, token });

            if (isAsync) return result;

            var task = (Task)result;
            task.ConfigureAwait(false).GetAwaiter().GetResult();
            return task.GetType().GetProperty("Result")?.GetValue(task);
        }

        private MethodInfo GetInvokeMethod(Type returnType)
        {

            if (returnType == typeof(ResponseWrapper))
            {
                return typeof(IHttpClientWrapper).GetMethodInfo(nameof(IHttpClientWrapper.SendAndWrapAsync),
                    BindingFlags.Public | BindingFlags.Instance, false, 0, typeof(RequestDescriptor), typeof(CancellationToken));
            }

            if (returnType.Namespace == typeof(ResponseWrapper).Namespace && returnType.Name == "ResponseWrapper`1")
            {
                return typeof(IHttpClientWrapper).GetMethodInfo(nameof(IHttpClientWrapper.SendAndWrapAsync),
                    BindingFlags.Public | BindingFlags.Instance, true, 1, typeof(RequestDescriptor), typeof(CancellationToken));
            }

            if (returnType == typeof(void))
            {
                return typeof(HttpClientWrapperExtension).GetMethodInfo(nameof(HttpClientWrapperExtension.SendAsync),
                    BindingFlags.Static | BindingFlags.Public,
                    false, 0, typeof(IHttpClientWrapper), typeof(RequestDescriptor), typeof(CancellationToken));
            }

            return typeof(HttpClientWrapperExtension).GetMethodInfo(nameof(HttpClientWrapperExtension.SendAsync),
                BindingFlags.Static | BindingFlags.Public,
                true, 1, typeof(IHttpClientWrapper), typeof(RequestDescriptor), typeof(CancellationToken));
        }

        /// <summary>
        /// 服务生成
        /// </summary>
        /// <param name="type"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static ServiceProxy Create(Type type, IServiceProvider provider)
        {
            var proxy = (ServiceProxy)ServiceProxyGenerator.CreateProxyInstance(typeof(ServiceProxy), type);
            proxy._provider = provider;
            return proxy;
        }
    }
}