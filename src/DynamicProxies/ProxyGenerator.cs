﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Rapidity.Http.DynamicProxies
{
    /// <summary>
    /// https://stackoverflow.com/questions/37526165/compiling-and-running-code-at-runtime-in-net-core-1-0
    /// https://benohead.com/three-options-to-dynamically-execute-csharp-code/
    /// </summary>
    public static class ProxyGenerator
    {
        public static Type Generate(Type type)
        {
            var source = "";
            using (var template = typeof(ProxyGenerator).Assembly.GetManifestResourceStream("Rapidity.Http.DynamicProxies.AutoGenerated.template"))
            using (var stream = new MemoryStream())
            {
                template.CopyTo(stream);
                source = Encoding.UTF8.GetString(stream.ToArray());
            }

            var assemblyName = "AutoGenerated";
            var dotnetCoreDirectory = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();
            var compilation = CSharpCompilation.Create(assemblyName).WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddReferences(MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location))
                .AddReferences(MetadataReference.CreateFromFile(type.GetTypeInfo().Assembly.Location))
                .AddReferences(MetadataReference.CreateFromFile(typeof(ProxyGenerator).GetTypeInfo().Assembly.Location))
                .AddReferences(MetadataReference.CreateFromFile(Path.Combine(dotnetCoreDirectory, "netstandard.dll")))
                .AddReferences(MetadataReference.CreateFromFile(Path.Combine(dotnetCoreDirectory, "System.Runtime.dll")))
                .AddSyntaxTrees(CSharpSyntaxTree.ParseText(source));

            using (var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms);
                if (!result.Success)
                {
                    var errorMsg = result.Diagnostics[0].GetMessage();
                    Trace.TraceError(errorMsg);
                    throw new Exception(errorMsg);
                }
                var assembly = Assembly.Load(ms.GetBuffer());
                return assembly.GetType("AutoGenerated.Service.TokenServiceProxy", true, true);
            }
        }

        public static TemplateData BuildTemplate(Type type)
        {
            var template = new TemplateData() { };
            template.UsingList.Add(typeof(Task).Namespace);
            template.UsingList.Add(typeof(IHttpService).Namespace);
            template.UsingList.Add(type.Namespace);

            var classTemplate = new ClassTemplate()
            {
                Namespace = $"{DateTime.Now.Ticks}.Service",
                Name = $"AutoGenerated{type.Name.TrimStart('I')}"
            };
            classTemplate.UsingList.Add(type.Namespace);
            foreach (var attr in CustomAttributeData.GetCustomAttributes(type))
            {
                classTemplate.AttributeList.Add(attr.ToString());
            }

            classTemplate.ImplementInterfaces.Add(new TypeTemplate(type));

            foreach (var method in type.GetRuntimeMethods())
            {
                if (method.Attributes.HasFlag(MethodAttributes.SpecialName))
                    continue;
                classTemplate.MethodList.Add(new MethodTemplate(method));
            }

            template.ClassList.Add(classTemplate);
            return template;
        }
    }
}