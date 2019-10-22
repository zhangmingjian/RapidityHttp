using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

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
        private readonly MethodInfo _method;

        public MethodTemplate(MethodInfo method)
        {
            this._method = method;
            Name = method.Name;
            IsAsync = method.ReturnType == typeof(Task) || method.ReturnType.Name == "Task`1";
            ReturnType = new TypeTemplate(method.ReturnType);
            foreach (var attr in CustomAttributeData.GetCustomAttributes(method))
                AttributeList.Add(attr.ToString());
            //方法参数
            foreach (var parameter in method.GetParameters())
                Parameters.Add(new ParameterTemplate(parameter));

            if (method.IsGenericMethod)
            {
                var genericMethod = method.GetGenericMethodDefinition();
                foreach (var argumentType in genericMethod.GetGenericArguments())
                {
                    GenericArguments.Add(argumentType.Name);
                    Type[] constraints = argumentType.GetGenericParameterConstraints();
                    if (argumentType.GenericParameterAttributes == GenericParameterAttributes.None) continue;
                    this.GenericConstraints[argumentType.Name] = new Collection<string>();
                    if (argumentType.GenericParameterAttributes.HasFlag(GenericParameterAttributes
                        .ReferenceTypeConstraint))
                        this.GenericConstraints[argumentType.Name].Add("class");
                    if (argumentType.GenericParameterAttributes.HasFlag(GenericParameterAttributes.NotNullableValueTypeConstraint))
                        this.GenericConstraints[argumentType.Name].Add("struct");

                    if (argumentType.GenericParameterAttributes.HasFlag(GenericParameterAttributes.DefaultConstructorConstraint))
                        this.GenericConstraints[argumentType.Name].Add("new()");
                }

            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 标签列表
        /// </summary>
        public ICollection<string> AttributeList { get; } = new Collection<string>();

        /// <summary>
        /// 返回类型
        /// </summary>
        public TypeTemplate ReturnType { get; }

        /// <summary>
        /// 是否异步方法
        /// </summary>
        public bool IsAsync { get; set; }

        /// <summary>
        /// 参数模板
        /// </summary>
        public ICollection<ParameterTemplate> Parameters { get; } = new Collection<ParameterTemplate>();

        /// <summary>
        /// 泛型参数
        /// </summary>
        public ICollection<string> GenericArguments { get; } = new Collection<string>();

        /// <summary>
        /// 泛型约束
        /// </summary>
        public IDictionary<string, ICollection<string>> GenericConstraints { get; } = new Dictionary<string, ICollection<string>>();

    }

    /// <summary>
    /// 类型模板
    /// </summary>
    public class TypeTemplate
    {
        public string Name { get; }

        /// <summary>
        /// 泛型参数
        /// </summary>
        public ICollection<TypeTemplate> GenericArguments { get; } = new Collection<TypeTemplate>();

        public TypeTemplate(Type type)
        {
            foreach (var genericType in type.GenericTypeArguments)
                GenericArguments.Add(new TypeTemplate(genericType));

            Name = GenericArguments.Count <= 0
                ? type.FullName ?? type.Name  //当为泛型类型时，fullname为null
                : type.FullName.Substring(0, type.FullName.IndexOf('`'));
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

    /// <summary>
    /// 方法参数模板
    /// </summary>
    public class ParameterTemplate
    {
        private ParameterInfo _parameter;

        public ParameterTemplate(ParameterInfo info)
        {
            if (info.Attributes.HasFlag(ParameterAttributes.Out)) throw new NotSupportedException("不支持定义out参数");
            if (info.Attributes.HasFlag(ParameterAttributes.In)) throw new NotSupportedException("不支持定义in参数");
            this._parameter = info;

            this.Name = info.Name;
            this.ParameterType = new TypeTemplate(info.ParameterType);
            this.AttributeList = CustomAttributeData.GetCustomAttributes(info)
                .Where(x => x.AttributeType != typeof(OptionalAttribute))
                .Select(x => x.ToString()).ToList();
        }

        /// <summary>
        /// 参数名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 参数类型
        /// </summary>
        public TypeTemplate ParameterType { get; }

        /// <summary>
        /// 标签列表
        /// </summary>
        public ICollection<string> AttributeList { get; }

        private string DefaultValueToString()
        {
            string value = string.Empty;
            if (_parameter.IsOptional)
            {
                try
                {
                    if (_parameter.DefaultValue != null)
                    {
                        if (_parameter.ParameterType == typeof(string))
                            value = $"\"{_parameter.DefaultValue}\"";
                        else if (_parameter.ParameterType == typeof(char))
                            value = $"'{_parameter.DefaultValue}'";
                        else
                            value = _parameter.DefaultValue.ToString();
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
            var paramsFlag = _parameter.CustomAttributes
                                 .FirstOrDefault(x => x.AttributeType == typeof(ParamArrayAttribute)) != null
                                ? "params " : string.Empty;
            return $"{attrs}{paramsFlag}{ParameterType} {Name}{value}";
        }
    }
}