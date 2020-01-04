using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Rapidity.Http.Attributes;

namespace Rapidity.Http
{
    /// <summary>
    /// 
    /// </summary>
    public class StringTemplate
    {
        private readonly string _template;
        private const char _startMark = '{';
        private const char _endMark = '}';
        private ICollection<string> _variables;

        public StringTemplate(string template)
        {
            _template = template;
        }

        /// <summary>
        /// 是否有变量
        /// </summary>
        public bool HaveVariable => IsContainVariables(_template);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        private bool IsContainVariables(string template)
        {
            if (string.IsNullOrWhiteSpace(template)) return false;
            var leftBracketIndex = template.IndexOf(_startMark);
            var rightBracketIndex = template.IndexOf(_endMark);
            return leftBracketIndex > -1 && rightBracketIndex > -1 && leftBracketIndex < rightBracketIndex - 1;
        }

        public ICollection<string> Variables => _variables ?? (_variables = GetVariables(_template));

        private ICollection<string> GetVariables(string template)
        {
            var variables = new Collection<string>();
            if (string.IsNullOrWhiteSpace(template))
                return variables;
            var enumer = template.GetEnumerator();
            StringBuilder sb = new StringBuilder();
            bool flag = false;
            while (enumer.MoveNext())
            {
                var current = enumer.Current;
                if (flag && current == _startMark)
                    throw new InvalidOperationException("模板变量当前不支持嵌套");
                if (current == _startMark)
                {
                    flag = true;
                    continue;
                }
                if (current == _endMark)
                {
                    flag = false;
                    if (sb.Length > 0)
                    {
                        variables.Add(sb.ToString());
                        sb.Clear();
                    }
                    continue;
                }
                if (flag) sb.Append(current);
            }
            return variables;
        }

        /// <summary>
        /// 替换变量,字典中找不到变量key时，跳过该变量的替换
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        private string TryReplaceVariable(NameValueCollection values)
        {
            string temp = _template;
            if (values == null || values.Count <= 0)
                return temp;
            foreach (var variable in Variables)
            {
                var key = values.AllKeys.FirstOrDefault(x => x.Equals(variable, StringComparison.CurrentCultureIgnoreCase));
                if (key == null) continue;
                temp = temp.Replace($"{_startMark}{variable}{_endMark}", values[key] ?? string.Empty);
                Trace.TraceInformation($"变量{variable}替换成功，当前值：{temp}");
            }
            return temp;
        }

        /// <summary>
        /// 替换变量,字典中找不到变量key时，跳过该变量的替换 //IEnumerable<KeyValuePair<TKey, TValue>>
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        private string TryReplaceVariable<TValue>(IEnumerable<KeyValuePair<string, TValue>> values)
        {
            string temp = _template;
            if (values == null || !values.Any())
                return temp;
            foreach (var variable in Variables)
            {
                var keyValue = values.FirstOrDefault(x => x.Key != null && x.Key.Equals(variable, StringComparison.CurrentCultureIgnoreCase));
                if (string.IsNullOrWhiteSpace(keyValue.Key)) continue;
                temp = temp.Replace($"{_startMark}{variable}{_endMark}", keyValue.Value?.ToString() ?? string.Empty);
                Trace.TraceInformation($"变量{variable}替换成功，当前值：{temp}");
            }
            return temp;
        }

        private string TryReplaceVariable(StringKeyValues values)
        {
            string temp = _template;
            if (values == null || !values.Any())
                return temp;
            foreach (var variable in Variables)
            {
                var keyValue = values.FirstOrDefault(x => x.Key != null 
                                && x.Key.Equals(variable, StringComparison.CurrentCultureIgnoreCase));
                if (string.IsNullOrWhiteSpace(keyValue.Key)) continue;
                foreach (var value in keyValue.Value)
                {
                    temp = temp.Replace($"{_startMark}{variable}{_endMark}", value);
                    Trace.TraceInformation($"变量{variable}替换成功，当前值：{temp}");
                }
            }
            return temp;
        }

        /// <summary>
        /// 替换变量,替换变量,对象中找不到变量属性时，跳过该变量的替换
        /// </summary>
        /// <param name="values"></param>
        /// <param name="throwWhenNotFoundVariable"></param>
        /// <returns></returns>
        public string TryReplaceVariable(object values, bool throwWhenNotFoundVariable = false)
        {
            string temp = _template;
            if (values == null)
            {
                if (throwWhenNotFoundVariable && IsContainVariables(temp))
                    throw new Exception($"未匹配到的所有变量,{string.Join(",", GetVariables(temp))}");
                return temp;
            }

            if (values is StringKeyValues keyValues)
                temp = TryReplaceVariable(keyValues);
            else if (values is NameValueCollection namValue)
                temp = TryReplaceVariable(namValue);
            else if (values is IEnumerable<KeyValuePair<string, object>> list1)
                temp = TryReplaceVariable(list1);
            else if (values is IEnumerable<KeyValuePair<string, string>> list2)
                temp = TryReplaceVariable(list2);
            else if (values is IEnumerable<KeyValuePair<string, IEnumerable<string>>> list3)
                temp = TryReplaceVariable(list3);
            else
            {
                var properties = values.GetType().GetProperties();
                foreach (var variable in Variables)
                {
                    var property = properties.FirstOrDefault(x =>
                    {
                        var namedAttr = x.GetCustomAttribute<NamedAttribute>();
                        var name = namedAttr != null ? namedAttr.Name : x.Name;
                        return name.Equals(variable, StringComparison.CurrentCultureIgnoreCase);
                    });
                    if (property == null) continue;
                    var value = property.GetValue(values);
                    temp = temp.Replace($"{_startMark}{variable}{_endMark}", value.ToString());
                    Trace.TraceInformation($"变量{variable}替换成功，当前值：{temp}");
                }
            }
            if (throwWhenNotFoundVariable && IsContainVariables(temp))
                throw new Exception($"未匹配到的所有变量,{string.Join(",", GetVariables(temp))}");
            return temp;
        }
    }
}