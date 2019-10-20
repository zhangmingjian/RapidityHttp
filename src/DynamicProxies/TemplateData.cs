using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Rapidity.Http.DynamicProxies
{
    /// <summary>
    /// 模板数据
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
        public string Name { get; set; }

        public TypeTemplate ParameterType { get; set; }

        public override string ToString()
        {
            return $"{ParameterType} {Name}";
        }
    }
}