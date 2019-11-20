using Rapidity.Http.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Rapidity.Http.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    internal static class NameValueCollectionExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static TValue ValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            return dictionary != null && dictionary.ContainsKey(key) ? dictionary[key] : default(TValue);
        }


        public static string ToQueryString(this NameValueCollection collection)
        {
            if (collection == null || collection.Count <= 0)
                return string.Empty;
            StringBuilder sb = new StringBuilder();
            foreach (var key in collection.AllKeys)
            {
                var value = collection[key];
                if (!string.IsNullOrEmpty(value))
                {
                    var delimiter = sb.Length > 0 ? "&" : string.Empty;
                    sb.AppendFormat($"{delimiter}{key}={collection[key]}");
                }
            }
            return sb.ToString();
        }


        public static NameValueCollection ToNameValueCollection(this object obj, bool ignoreNullValue = true)
        {
            var collection = new NameValueCollection();
            var dic = ToKeyValuePairs(obj, ignoreNullValue);
            foreach (var item in dic)
            {
                if (item.Key != null)
                {
                    collection.Add(item.Key, item.Value);
                }
            }
            return collection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="ignoreNullValue"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, string>> ToKeyValuePairs(this object obj, bool ignoreNullValue = true)
        {
            if (obj == null)
                yield return default;
            var type = obj.GetType();
            //字典类型解析
            if (type.HasImplementedRawGeneric(typeof(IDictionary<,>)))
            {
                var keys = type.GetProperty("Keys")?.GetValue(obj) as IEnumerable<object>;
                var values = type.GetProperty("Values")?.GetValue(obj) as IEnumerable<object>;
                if (keys != null && values != null)
                {
                    var keyList = keys.ToList();
                    var valueList = values.ToList();
                    for (int i = 0; i < keyList.Count; i++)
                    {
                        if (ignoreNullValue && valueList[i] == null) continue;
                        yield return new KeyValuePair<string, string>(keyList[i].ToString(), valueList[i]?.ToString());
                    }
                }
                yield return default;
            }
            //todo IEnumerable类型的数据不应该被支持
            else
            {
                var properties = type.GetProperties();
                foreach (var property in properties)
                {
                    //如果是索引器则跳过
                    if (property.GetGetMethod().GetParameters().Length > 0) continue;
                    var attr = property.GetCustomAttribute<NamedAttribute>();
                    var name = attr?.Name ?? property.Name;
                    var value = property.GetValue(obj);
                    if (ignoreNullValue && value == null) continue;
                    yield return new KeyValuePair<string, string>(name, value.ToString());
                }
                yield return default;
            }
        }

        private static bool HasImplementedRawGeneric(this Type type, Type generic)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (generic == null) throw new ArgumentNullException(nameof(generic));

            // 测试接口。
            var isTheRawGenericType = type.GetInterfaces().Any(IsTheRawGenericType);
            if (isTheRawGenericType) return true;

            // 测试类型。
            while (type != null && type != typeof(object))
            {
                isTheRawGenericType = IsTheRawGenericType(type);
                if (isTheRawGenericType) return true;
                type = type.BaseType;
            }

            // 没有找到任何匹配的接口或类型。
            return false;

            // 测试某个类型是否是指定的原始接口。
            bool IsTheRawGenericType(Type test)
                => generic == (test.IsGenericType ? test.GetGenericTypeDefinition() : test);
        }
    }
}