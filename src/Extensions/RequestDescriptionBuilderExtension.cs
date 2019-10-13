using System;
using System.Diagnostics;
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
                if (method.ReflectedType == entryType
                    || entryType.IsInterface && entryType.IsAssignableFrom(method.ReflectedType))
                    break;
                method = null;
            }
            if (method == null)
                throw new Exception($"获取MethodInfo失败，请确保{entryType}在当前调用堆栈中");

            return builder.Build(method, parameters);
        }
    }
}