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
                if(flag) return method;
            }
            return null;
        }
    }
}