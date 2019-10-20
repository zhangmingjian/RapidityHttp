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
            var valueTypes = new[]
            {
                typeof(string),
                typeof(char),typeof(char?),
                typeof(int), typeof(int?),
                typeof(byte),typeof(byte?),
                typeof(sbyte),typeof(sbyte?),
                typeof(short),typeof(short?),
                typeof(ushort),typeof(ushort?),
                typeof(uint), typeof(uint?),
                typeof(long),typeof(long?),
                typeof(ulong),typeof(ulong?),
                typeof(float),typeof(float?),
                typeof(double),typeof(double?),
                typeof(decimal), typeof(decimal?),
                typeof(Guid),typeof(Guid?),
                typeof(DateTime),typeof(DateTime?),
                typeof(bool),typeof(bool?)
            };

            return valueTypes.Contains(type);
        }
    }
}