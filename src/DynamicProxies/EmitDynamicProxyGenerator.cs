using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Rapidity.Http.Extensions;

namespace Rapidity.Http.DynamicProxies
{
    /// <summary>
    /// 使用emit生成服务代理
    /// </summary>
    public class EmitDynamicProxyGenerator : IProxyGenerator
    {
        public Type Generate(Type type)
        {
            if (!type.IsInterface)
                throw new NotSupportedException("当前不支持非接口类型的生成");

            //创建程序集
            AssemblyName assemblyName = new AssemblyName(Guid.NewGuid().ToString("n"));
            AssemblyBuilder dynamicAssembly = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
            //创建模块
            ModuleBuilder mb = dynamicAssembly.DefineDynamicModule(assemblyName.Name);

            var className = $"{assemblyName.Name}.{type.Name.TrimStart('I')}DynamicGenerated";
            //创建类
            TypeBuilder typeBuilder = mb.DefineType(className, TypeAttributes.Public | TypeAttributes.Class, typeof(object));
            //实现接口
            typeBuilder.AddInterfaceImplementation(type);

            //复制接口上的Attribute
            foreach (var attr in CustomAttributeData.GetCustomAttributes(type))
                typeBuilder.SetCustomAttribute(ToAttributeBuilder(attr));

            //定义私有变量
            var clientField = typeBuilder.DefineField("_httpClient", typeof(IHttpClientWrapper), FieldAttributes.Private);
            var builderField = typeBuilder.DefineField("_descriptionBuilder", typeof(IRequestDescriptionBuilder), FieldAttributes.Private);

            BuildConstructor(typeBuilder, builderField, clientField);

            foreach (var @event in type.GetRuntimeEvents())
                BuildEvents(typeBuilder, @event);

            foreach (var property in type.GetRuntimeProperties())
                BuildProperties(typeBuilder, property);

            foreach (var method in type.GetRuntimeMethods())
            {
                if (method.Attributes.HasFlag(MethodAttributes.SpecialName))
                    continue;
                BuildMethods(typeBuilder, method, builderField, clientField);
            }

            //创建类型
            Type classType = typeBuilder.CreateTypeInfo();
            return classType;
        }

        //构造函数
        private void BuildConstructor(TypeBuilder typeBuilder, FieldBuilder builderField, FieldBuilder clientField)
        {
            //构造函数
            Type[] constructorArgs = { typeof(IRequestDescriptionBuilder), typeof(IHttpClientWrapper) };
            var constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, constructorArgs);

            ILGenerator ilOfCtor = constructorBuilder.GetILGenerator();
            //设置字段 _descriptionBuilder
            ilOfCtor.Emit(OpCodes.Ldarg_0);
            ilOfCtor.Emit(OpCodes.Ldarg_2);
            ilOfCtor.Emit(OpCodes.Stfld, builderField);

            //设置字段 _httpClient
            ilOfCtor.Emit(OpCodes.Ldarg_0);
            ilOfCtor.Emit(OpCodes.Ldarg_1);
            ilOfCtor.Emit(OpCodes.Stfld, clientField);

            //方法结束
            ilOfCtor.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// 复制事件
        /// </summary>
        /// <param name="typeBuilder"></param>
        /// <param name="event"></param>
        private void BuildEvents(TypeBuilder typeBuilder, EventInfo @event)
        {
            var eventBuilder = typeBuilder.DefineEvent(@event.Name, @event.Attributes, @event.EventHandlerType);
            //复制Attribute
            foreach (var attr in CustomAttributeData.GetCustomAttributes(@event))
                eventBuilder.SetCustomAttribute(ToAttributeBuilder(attr));

            var field = typeBuilder.DefineField($"_{@event.Name.ToLower()}", @event.EventHandlerType, FieldAttributes.Private);

            if (@event.AddMethod != null)
            {
                var parameters = @event.AddMethod.GetParameters().Select(x => x.ParameterType).ToArray();
                var addMethod = typeBuilder.DefineMethod(@event.AddMethod.Name, MethodAttributes.Public | MethodAttributes.Virtual,
                    @event.AddMethod.CallingConvention, @event.AddMethod.ReturnType, parameters);
                var generator = addMethod.GetILGenerator();
                var combine = typeof(Delegate).GetMethod(nameof(Delegate.Combine), new[] { typeof(Delegate), typeof(Delegate) });
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldfld, field);
                generator.Emit(OpCodes.Ldarg_1);
                generator.Emit(OpCodes.Call, combine);
                generator.Emit(OpCodes.Castclass, @event.EventHandlerType);
                generator.Emit(OpCodes.Stfld, field);
                generator.Emit(OpCodes.Ret);
                eventBuilder.SetAddOnMethod(addMethod);
            }
            if (@event.RemoveMethod != null)
            {
                var parameters = @event.RemoveMethod.GetParameters().Select(x => x.ParameterType).ToArray();
                var removeMethod = typeBuilder.DefineMethod(@event.RemoveMethod.Name, MethodAttributes.Public | MethodAttributes.Virtual,
                    @event.RemoveMethod.CallingConvention, @event.RemoveMethod.ReturnType, parameters);
                var remove = typeof(Delegate).GetMethod(nameof(Delegate.Remove), new[] { typeof(Delegate), typeof(Delegate) });
                var generator = removeMethod.GetILGenerator();
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldfld, field);
                generator.Emit(OpCodes.Ldarg_1);
                generator.Emit(OpCodes.Call, remove);
                //generator.Emit(OpCodes.Castclass, typeof(PropertyChangedEventHandler));
                generator.Emit(OpCodes.Stfld, field);
                generator.Emit(OpCodes.Ret);
                eventBuilder.SetRemoveOnMethod(removeMethod);
            }
        }

        /// <summary>
        /// 复制属性
        /// </summary>
        /// <param name="typeBuilder"></param>
        /// <param name="baseProperty"></param>
        private void BuildProperties(TypeBuilder typeBuilder, PropertyInfo baseProperty)
        {
            var parameterTypes = baseProperty.GetIndexParameters().Select(x => x.ParameterType).ToArray();
            var propertyBuilder = typeBuilder.DefineProperty(baseProperty.Name, PropertyAttributes.None, baseProperty.PropertyType, parameterTypes);
            //复制属性上的Attribute
            foreach (var attr in CustomAttributeData.GetCustomAttributes(baseProperty))
                propertyBuilder.SetCustomAttribute(ToAttributeBuilder(attr));

            var field = typeBuilder.DefineField($"_{baseProperty.Name.ToLower()}", baseProperty.PropertyType, FieldAttributes.Private);
            //设置getMethod
            if (baseProperty.CanRead)
            {
                var baseGetMethod = baseProperty.GetMethod;
                var parameters = baseGetMethod.GetParameters().Select(x => x.ParameterType).ToArray();
                var getMethod = typeBuilder.DefineMethod(baseGetMethod.Name, MethodAttributes.Public | MethodAttributes.Virtual,
                    baseGetMethod.CallingConvention, baseGetMethod.ReturnType, parameters);
                var ilGenerator = getMethod.GetILGenerator();
                ilGenerator.Emit(OpCodes.Ldarg_0); // this
                ilGenerator.Emit(OpCodes.Ldfld, field);
                ilGenerator.Emit(OpCodes.Ret);

                propertyBuilder.SetGetMethod(getMethod);
            }
            //设置SetMethod
            if (baseProperty.CanWrite)
            {
                var baseSetMethod = baseProperty.SetMethod;
                var parameters = baseSetMethod.GetParameters().Select(x => x.ParameterType).ToArray();
                var setMethod = typeBuilder.DefineMethod(baseSetMethod.Name, MethodAttributes.Public | MethodAttributes.Virtual,
                    baseSetMethod.CallingConvention, baseSetMethod.ReturnType, parameters);
                var ilGenerator = setMethod.GetILGenerator();
                ilGenerator.Emit(OpCodes.Ldarg_0); // this
                ilGenerator.Emit(OpCodes.Ldarg_1); //
                ilGenerator.Emit(OpCodes.Stfld, field);
                ilGenerator.Emit(OpCodes.Ret);

                propertyBuilder.SetGetMethod(setMethod);
            }
        }

        /// <summary>
        /// 实现方法
        /// </summary>
        /// <param name="typeBuilder"></param>
        /// <param name="method"></param>
        /// <param name="builderField"></param>
        /// <param name="clientField"></param>
        private void BuildMethods(TypeBuilder typeBuilder, MethodInfo method, FieldBuilder builderField, FieldBuilder clientField)
        {
            var parameters = method.GetParameters().Select(x => x.ParameterType).ToArray();
            //创建方法 实现接口方法必须要有MethodAttributes.Virtual 否则报未实现接口
            MethodBuilder methodBuild = typeBuilder.DefineMethod(method.Name, MethodAttributes.Public | MethodAttributes.Virtual, method.CallingConvention, method.ReturnType, parameters);
            //复制方法上的Attribute
            foreach (var attr in CustomAttributeData.GetCustomAttributes(method))
                methodBuild.SetCustomAttribute(ToAttributeBuilder(attr));

            //生成指令
            ILGenerator iLGenerator = methodBuild.GetILGenerator();

            iLGenerator.Emit(OpCodes.Nop);
            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Ldfld, builderField);
            iLGenerator.Emit(OpCodes.Castclass, method.ReflectedType);
            for (int index = 1; index <= parameters.Length; index++)
                iLGenerator.Emit(OpCodes.Ldarg, index);

            var buildMethod = typeof(RequestDescriptionBuilderExtension).GetMethod(nameof(RequestDescriptionBuilderExtension.Build));
            iLGenerator.Emit(OpCodes.Call, buildMethod);
            iLGenerator.Emit(OpCodes.Pop);

            iLGenerator.Emit(OpCodes.Ldarg_1);
            iLGenerator.Emit(OpCodes.Ret);
        }

        private CustomAttributeBuilder ToAttributeBuilder(CustomAttributeData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            var constructorArguments = new List<object>();
            foreach (var ctorArg in data.ConstructorArguments)
            {
                constructorArguments.Add(ctorArg.Value);
            }

            var propertyArguments = new List<PropertyInfo>();
            var propertyArgumentValues = new List<object>();
            var fieldArguments = new List<FieldInfo>();
            var fieldArgumentValues = new List<object>();
            foreach (var namedArg in data.NamedArguments)
            {
                var fi = namedArg.MemberInfo as FieldInfo;
                var pi = namedArg.MemberInfo as PropertyInfo;

                if (fi != null)
                {
                    fieldArguments.Add(fi);
                    fieldArgumentValues.Add(namedArg.TypedValue.Value);
                }
                else if (pi != null)
                {
                    propertyArguments.Add(pi);
                    propertyArgumentValues.Add(namedArg.TypedValue.Value);
                }
            }
            return new CustomAttributeBuilder(
                data.Constructor,
                constructorArguments.ToArray(),
                propertyArguments.ToArray(),
                propertyArgumentValues.ToArray(),
                fieldArguments.ToArray(),
                fieldArgumentValues.ToArray());
        }
    }
}