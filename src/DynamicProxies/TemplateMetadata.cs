using Rapidity.Http.Attributes;
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
    /// </summary>
    internal class TemplateMetadata
    {
        public SortedSet<string> UsingList { get; set; } = new SortedSet<string>();

        public ICollection<ClassTemplate> ClassList { get; set; } = new Collection<ClassTemplate>();
    }

    internal class ClassTemplate
    {
        /// <summary>
        /// 实现的接口
        /// </summary>
        public Type InterfaceType { get; set; }

        /// <summary>
        /// 类名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 命名空间
        /// </summary>
        public string Namespace { get; set; }


        public string FullName => $"{Namespace}.{Name}";

        /// <summary>
        /// using语句
        /// </summary>
        public SortedSet<string> UsingList { get; set; } = new SortedSet<string>();

        /// <summary>
        /// 标签列表
        /// </summary>
        public ICollection<string> AttributeList { get; set; } = new Collection<string>();

        /// <summary>
        /// 泛型参数
        /// </summary>
        public ICollection<GenericArgumentTemplate> GenericArguments { get; } = new Collection<GenericArgumentTemplate>();

        /// <summary>
        /// 方法列表
        /// </summary>
        public ICollection<MethodTemplate> MethodList { get; set; } = new Collection<MethodTemplate>();

        /// <summary>
        /// 属性列表
        /// </summary>
        public ICollection<PropertyTemplate> PropertyList { get; set; } = new Collection<PropertyTemplate>();
    }
    /// <summary>
    /// 方法信息
    /// </summary>
    internal class MethodTemplate
    {
        public MethodInfo Method { get; }

        public MethodTemplate(MethodInfo method)
        {
            Method = method;
            Name = method.Name;
            IsAsync = method.ReturnType.FullName.StartsWith(typeof(Task).FullName);
            ReturnType = new TypeTemplate(method.ReturnType);
            foreach (var attr in CustomAttributeData.GetCustomAttributes(method))
                AttributeList.Add(new CustomAttributeTemplate(attr).ToString());
            //方法参数
            foreach (var parameter in method.GetParameters())
                Parameters.Add(new ParameterTemplate(parameter));

            if (method.IsGenericMethod)
            {
                foreach (var argumentType in method.GetGenericArguments())
                {
                    var argument = new GenericArgumentTemplate()
                    {
                        Name = argumentType.Name
                    };
                    GenericArguments.Add(argument);

                    if (argumentType.GenericParameterAttributes == GenericParameterAttributes.None
                        && argumentType.GetGenericParameterConstraints().Length == 0) continue;

                    //class约束
                    if (argumentType.GenericParameterAttributes.HasFlag(GenericParameterAttributes.ReferenceTypeConstraint))
                        argument.Constraints.Add("class");
                    //值类型约束
                    if (argumentType.GenericParameterAttributes.HasFlag(GenericParameterAttributes.NotNullableValueTypeConstraint))
                        argument.Constraints.Add("struct");

                    foreach (var type in argumentType.GetGenericParameterConstraints())
                        argument.Constraints.Add(new TypeTemplate(type).ToString());

                    //无参构造函数
                    if (argumentType.GenericParameterAttributes.HasFlag(GenericParameterAttributes.DefaultConstructorConstraint))
                        argument.Constraints.Add("new()");


                }
            }
        }

        /// <summary>
        /// 方法名
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Attribute标签列表
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
        public ICollection<GenericArgumentTemplate> GenericArguments { get; } = new Collection<GenericArgumentTemplate>();

    }

    internal class PropertyTemplate
    {
        public PropertyTemplate(PropertyInfo property)
        {
            this.Name = property.Name;
            this.PropertyType = new TypeTemplate(property.PropertyType);
            this.HasSet = property.CanWrite;
            this.HasGet = property.CanRead;
        }

        public string Name { get; set; }

        public TypeTemplate PropertyType { get; set; }

        public bool HasGet { get; set; }

        public bool HasSet { get; set; }

        public override string ToString()
        {
            return $"public {PropertyType} {Name} {{ {(HasGet ? "get;" : string.Empty)} {(HasSet ? "set;" : string.Empty)} }}";
        }
    }

    /// <summary>
    /// 类型模板
    /// </summary>
    internal class TypeTemplate
    {
        public Type Type { get; }
        public string Name { get; }

        //public bool IsIn { get; }

        //public bool IsOut { get; }

        /// <summary>
        /// 泛型参数
        /// </summary>
        public ICollection<TypeTemplate> GenericArguments { get; } = new Collection<TypeTemplate>();

        public TypeTemplate(Type type)
        {
            this.Type = type;
            var typeInfo = type as TypeInfo;
            foreach (var genericType in typeInfo.GenericTypeArguments)
                GenericArguments.Add(new TypeTemplate(genericType));
            //if (typeInfo.IsGenericParameter)
            //{
            //    //协变
            //    if (typeInfo.GenericParameterAttributes.HasFlag(GenericParameterAttributes.Covariant))
            //        IsOut = true;
            //    //逆变
            //    if (typeInfo.GenericParameterAttributes.HasFlag(GenericParameterAttributes.Contravariant))
            //        IsIn = true;
            //}
            var typeNamespace = typeInfo.Namespace;
            var typeName = typeInfo.Name;

            var fullName = typeInfo.FullName ?? (typeInfo.IsGenericParameter ? typeName : $"{typeNamespace}.{typeName}"); //当为泛型类型时，fullname为null                                                                                                                         //当为ref参数时需要移除类型后面的&标识
            //type.Namespace 需要考虑是否加上namespace
            if (typeInfo.IsByRef)
                fullName = fullName.TrimEnd('&');
            var index = fullName.IndexOf('`');
            Name = index == -1 ? fullName : fullName.Substring(0, index);
        }

        public override string ToString()
        {
            if (this.Type == typeof(void)) return "void";
            switch (Type.GetTypeCode(this.Type))
            {
                case TypeCode.Boolean: return "bool";
                case TypeCode.String: return "string";
                case TypeCode.Int16: return "short";
                case TypeCode.Int32: return "int";
                case TypeCode.Int64: return "long";
            }
            var name = this.Name;
            if (GenericArguments.Count > 0)
                name = $"{name}<{string.Join(",", GenericArguments.Select(x => x.ToString()))}>";
            return name;
        }
    }

    /// <summary>
    /// 方法参数模板
    /// </summary>
    internal class ParameterTemplate
    {
        private ParameterInfo _parameter;

        public ParameterTemplate(ParameterInfo info)
        {
            if (info.IsOut) throw new NotSupportedException("不支持定义out参数");
            this._parameter = info;

            this.Name = info.Name;
            this.ParameterType = new TypeTemplate(info.ParameterType);
            this.AttributeList = CustomAttributeData.GetCustomAttributes(info)
                                    .Where(x => x.AttributeType != typeof(OptionalAttribute) && x.AttributeType != typeof(ParamArrayAttribute))
                                    .Select(x => new CustomAttributeTemplate(x).ToString()).ToList();
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
        /// Attribute标签列表
        /// </summary>
        public ICollection<string> AttributeList { get; }
        /// <summary>
        /// 是否可变数组参数
        /// </summary>
        public bool IsParamArray => _parameter.GetCustomAttribute<ParamArrayAttribute>() != null;

        public bool IsRef => _parameter.ParameterType.IsByRef;

        public string DefauleValue
        {
            get
            {
                string value = string.Empty;
                //可选参数
                if (_parameter.IsOptional)
                {
                    try
                    {
                        if (_parameter.DefaultValue != null)
                        {
                            switch (Type.GetTypeCode(_parameter.ParameterType))
                            {
                                case TypeCode.String:
                                    value = $"\"{_parameter.DefaultValue}\"";
                                    break;
                                case TypeCode.Char:
                                    value = $"'{_parameter.DefaultValue}'";
                                    break;
                                case TypeCode.Boolean:
                                    value = _parameter.DefaultValue.ToString().ToLower();
                                    break;
                                default:
                                    value = _parameter.DefaultValue.ToString();
                                    break;
                            }
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
        }

        public override string ToString()
        {
            var value = string.IsNullOrEmpty(DefauleValue) ? DefauleValue : " = " + DefauleValue;
            var attrs = string.Join("", AttributeList);
            var paramsFlag = IsParamArray ? "params " : string.Empty;
            var refFlag = IsRef ? "ref " : string.Empty;
            return $"{attrs}{refFlag}{paramsFlag}{ParameterType} {Name}{value}";
        }
    }

    /// <summary>
    /// 泛型参数信息
    /// </summary>
    internal class GenericArgumentTemplate
    {
        /// <summary>
        /// 泛型参数名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 泛型约束
        /// </summary>
        public ICollection<string> Constraints { get; set; } = new Collection<string>();
    }

    internal class CustomAttributeTemplate
    {
        private CustomAttributeData _attributeData;

        public CustomAttributeTemplate(CustomAttributeData attributeData)
        {
            _attributeData = attributeData;
        }

        public string AttributeName
        {
            get
            {
                if (_attributeData.AttributeType.Namespace == typeof(HttpServiceAttribute).Namespace)
                {
                    return _attributeData.AttributeType.Name.Replace(nameof(Attribute), string.Empty);
                }
                return _attributeData.AttributeType.FullName;
            }
        }

        public ICollection<string> ConstructorArguments
        {
            get
            {
                var arguments = new Collection<string>();
                foreach (var arg in _attributeData.ConstructorArguments)
                {
                    if (arg.Value == null) continue;
                    switch (Type.GetTypeCode(arg.ArgumentType))
                    {
                        case TypeCode.String:
                            arguments.Add($"\"{arg.Value}\"");
                            break;
                        case TypeCode.Char:
                            arguments.Add($"'{arg.Value}'");
                            break;
                        case TypeCode.Boolean:
                            arguments.Add($"{arg.Value.ToString().ToLower()}");
                            break;
                        default:
                            arguments.Add(arg.ToString());
                            break;
                    }
                }
                return arguments;
            }
        }

        public IDictionary<string, string> NamedArguments
        {
            get
            {
                var arguments = new Dictionary<string, string>();
                foreach (var arg in _attributeData.NamedArguments)
                {
                    switch (Type.GetTypeCode(arg.TypedValue.ArgumentType))
                    {
                        case TypeCode.String:
                            arguments.Add(arg.MemberName, $"\"{arg.TypedValue.Value}\"");
                            break;
                        case TypeCode.Char:
                            arguments.Add(arg.MemberName, $"'{arg.TypedValue.Value}'");
                            break;
                        case TypeCode.Boolean:
                            arguments.Add(arg.MemberName, $"{arg.TypedValue.Value?.ToString().ToLower()}");
                            break;
                        default:
                            arguments.Add(arg.MemberName, $"{arg.TypedValue.Value}");
                            break;
                    }
                }
                return arguments;
            }
        }


        public override string ToString()
        {
            if (ConstructorArguments.Count <= 0 && NamedArguments.Count <= 0)
                return $"[{AttributeName}]";
            return $"[{AttributeName}({string.Join(",", ConstructorArguments)}{(ConstructorArguments.Count > 0 && NamedArguments.Count > 0 ? "," : string.Empty)}{string.Join(",", NamedArguments.Select(x => $"{x.Key}={x.Value}"))})]";
        }
    }
}