using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Rapidity.Http.DynamicProxies
{
    /// <summary>
    /// 模板数据
    /// todo: 1.泛型约束生成 2.泛型方法生成
    /// </summary>
    public class TemplateData
    {
        public ICollection<string> UsingList { get; set; } = new Collection<string>();

        public ICollection<ClassTemplate> ClassList { get; set; } = new Collection<ClassTemplate>();
    }

    public class ClassTemplate
    {
        /// <summary>
        /// 命名空间
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// using语句
        /// </summary>
        public ICollection<string> UsingList { get; set; } = new Collection<string>();

        /// <summary>
        /// 标签列表
        /// </summary>
        public ICollection<string> AttributeList { get; set; } = new Collection<string>();

        /// <summary>
        /// 类名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 实现父类
        /// </summary>
        public TypeTemplate BaseClass { get; set; }

        /// <summary>
        /// 实现的接口
        /// </summary>
        public ICollection<TypeTemplate> ImplementInterfaces { get; set; } = new Collection<TypeTemplate>();
        /// <summary>
        /// 是否为泛型接口
        /// </summary>
        public bool IsGenericInterface => (GenericArguments?.Count ?? 0) > 0;
        /// <summary>
        /// 泛型参数
        /// </summary>
        public ICollection<string> GenericArguments { get; set; }
        /// <summary>
        /// 泛型约束
        /// </summary>
        public string GenericConstraint { get; set; }

        /// <summary>
        /// 方法列表
        /// </summary>
        public ICollection<MethodTemplate> MethodList { get; set; } = new Collection<MethodTemplate>();
    }

    public class MethodTemplate
    {
        /// <summary>
        /// 标签列表
        /// </summary>
        public ICollection<string> AttributeList { get; set; } = new Collection<string>();
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 返回类型
        /// </summary>
        public TypeTemplate ReturnType { get; set; }
        /// <summary>
        /// 是否异步方法
        /// </summary>
        public bool IsAsync { get; set; }

        /// <summary>
        /// 参数模板
        /// </summary>
        public ICollection<ParameterTemplate> Parameters { get; set; } = new Collection<ParameterTemplate>();

        /// <summary>
        /// 泛型参数
        /// </summary>
        public ICollection<string> GenericArguments { get; set; } = new Collection<string>();

        /// <summary>
        /// 泛型约束
        /// </summary>
        public string GenericConstraint { get; set; }

    }

    public class TypeTemplate
    {
        public string Name { get; set; }

        /// <summary>
        /// 泛型参数
        /// </summary>
        public ICollection<TypeTemplate> GenericArguments { get; set; } = new Collection<TypeTemplate>();

        public TypeTemplate(Type type)
        {
            foreach (var genericType in type.GenericTypeArguments)
            {
                GenericArguments.Add(new TypeTemplate(genericType));
            }
            Name = GenericArguments.Count <= 0 ? type.FullName : type.FullName.Substring(0, type.FullName.IndexOf('`'));
        }

        public override string ToString()
        {
            var subName = string.Empty;
            if (GenericArguments != null && GenericArguments.Count > 0)
            {
                subName = $"<{string.Join(",", GenericArguments.Select(x => x.ToString()))}>";
            }
            return $"{Name}{subName}";
        }
    }

    public class ParameterTemplate
    {
        public ParameterInfo ParameterInfo { get; }

        public ParameterTemplate(ParameterInfo info)
        {
            if (info.Attributes.HasFlag(ParameterAttributes.Out)) throw new NotSupportedException("不支持定义out参数");
            if (info.Attributes.HasFlag(ParameterAttributes.In)) throw new NotSupportedException("不支持定义in参数");
            ParameterInfo = info;
        }

        /// <summary>
        /// 参数名称
        /// </summary>
        public string Name => ParameterInfo.Name;

        /// <summary>
        /// 参数类型
        /// </summary>
        public TypeTemplate ParameterType => new TypeTemplate(ParameterInfo.ParameterType);

        /// <summary>
        /// 标签列表
        /// </summary>
        public ICollection<string> AttributeList
        {
            get
            {
                return CustomAttributeData.GetCustomAttributes(ParameterInfo)
                        .Where(x => x.AttributeType != typeof(OptionalAttribute))
                        .Select(x => x.ToString()).ToList();
            }
        }

        private string DefaultValueToString()
        {
            string value = string.Empty;
            if (ParameterInfo.IsOptional)
            {
                try
                {
                    if (ParameterInfo.DefaultValue != null)
                    {
                        if (ParameterInfo.ParameterType == typeof(string))
                            value = $"\"{ParameterInfo.DefaultValue}\"";
                        else if (ParameterInfo.ParameterType == typeof(char))
                            value = $"'{ParameterInfo.DefaultValue}'";
                        else
                            value = ParameterInfo.DefaultValue.ToString();
                    }
                    else value = $"default({ParameterType})";
                }
                catch
                {
                    value = $"default({ParameterType})";
                }
            }
            return value;
        }

        public override string ToString()
        {
            var value = DefaultValueToString();
            value = string.IsNullOrEmpty(value) ? value : " = " + value;
            var attrs = string.Join("", AttributeList);
            var paramsFlag = ParameterInfo.CustomAttributes
                                 .FirstOrDefault(x => x.AttributeType == typeof(ParamArrayAttribute)) != null 
                                ? "params " : string.Empty;
            return $"{attrs}{paramsFlag}{ParameterType} {Name}{value}";
        }
    }
}