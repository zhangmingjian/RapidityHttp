using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Rapidity.Http.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class RequestDescriptionBuilderExtension
    {
        public static RequestDescription Build(this IRequestDescriptionBuilder builder, Type entryType, params object[] parameters)
        {
            int maxDeep = 10;
            MethodInfo method = null;
            var stack = new StackTrace(1);
            for (int deep = 0; deep < stack.FrameCount && deep < maxDeep; deep++)
            {
                var frame = stack.GetFrame(deep);
                method = (MethodInfo)frame.GetMethod();
                if (method.ReflectedType == entryType) break;
                if (entryType.IsInterface && entryType.IsAssignableFrom(method.ReflectedType))
                {
                    //转换成接口方法
                    var parametTypes = method.GetParameters().Select(x => x.ParameterType).ToArray();
                    var genericCount = method.GetGenericArguments().Length;
                    method = entryType.GetMethodInfo(method.Name, null, method.IsGenericMethod, genericCount, parametTypes);
                    break;
                }
                method = null;
            }
            if (method == null)
                throw new Exception($"获取MethodInfo失败，请确保{entryType}在当前调用堆栈中");

            return builder.Build(method, parameters);
        }
    }
}