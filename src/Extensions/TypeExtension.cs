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
        public static MethodInfo GetMethodInfo(this Type type, string name, BindingFlags flags, bool isGeneric, params Type[] parameterTypes)
        {
            var methods = type.GetMethods(flags).Where(m => m.Name == name && m.IsGenericMethod == isGeneric);
            foreach (MethodInfo method in methods)
            {
                var parameters = method.GetParameters();
                if (parameters.Length != parameterTypes.Length)
                    continue;
                bool flag = true;
                for (var i = 0; i < parameters.Length; i++)
                {
                    if (parameters[i].ParameterType != parameterTypes[i])
                        flag = false;
                }
                if (flag) return method;
            }
            return null;
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