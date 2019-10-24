using System;
using System.Linq;
using System.Reflection;

namespace Rapidity.Http.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class TypeExtension
    {
        /// <summary>
        /// 查找方法
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="flags"></param>
        /// <param name="isGeneric"></param>
        /// <param name="genericParameterCount"></param>
        /// <param name="parameterTypes"></param>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo(this Type type, string name, BindingFlags? flags, bool isGeneric, int genericParameterCount = 0, params Type[] parameterTypes)
        {
            var methods = flags == null ? type.GetMethods() : type.GetMethods(flags.Value);
            var method = methods.FirstOrDefault(m =>
            {
                if (m.Name != name || m.IsGenericMethod != isGeneric
                    || m.IsGenericMethod && m.GetGenericArguments().Length != genericParameterCount)
                    return false;
                var parameters = m.GetParameters();
                if (parameters.Length != parameterTypes.Length)
                    return false;
                for (var i = 0; i < parameters.Length; i++)
                {
                    if (parameters[i].ParameterType != parameterTypes[i])
                        return false;
                }
                return true;
            });
            return method;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsSimpleType(this Type type)
        {
            var code = Type.GetTypeCode(type);
            switch (code)
            {
                case TypeCode.Object: return false;
                default: return true;
            }
        }
    }
}