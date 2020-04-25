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
        /// <summary>
        /// 从当前调用链向上查找entryType类的方法
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="entryType"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static RequestDescriptor Build(this IRequestDescriptorBuilder builder, Type entryType, params object[] arguments)
        {
            int maxDeep = 10; //设置最大查找层级
            MethodInfo method = null;
            var stack = new StackTrace(1);
            for (int deep = 0; deep < stack.FrameCount && deep < maxDeep; deep++)
            {
                var frame = stack.GetFrame(deep);
                var temp = (MethodInfo)frame.GetMethod();
                if (temp.ReflectedType != entryType) continue;
                method = temp;
                Debug.WriteLine($"在第{deep + 1}层中追踪到方法{entryType}.{method.Name},总调用层级：{stack.FrameCount}");
                break;
                //if (entryType.IsInterface && entryType.IsAssignableFrom(method.ReflectedType))
                //{
                //    //转换成接口方法
                //    var parametTypes = method.GetParameters().Select(x => x.ParameterType).ToArray();
                //    var genericCount = method.GetGenericArguments().Length;
                //    method = entryType.GetMethodInfo(method.Name, null, method.IsGenericMethod, genericCount, parametTypes);
                //    break;
                //}
            }
            if (method == null)
                throw new Exception($"获取MethodInfo失败，请确保{entryType}在当前调用链中");

            return builder.Build(method, arguments);
        }
    }
}