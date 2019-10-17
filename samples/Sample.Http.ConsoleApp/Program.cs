using Rapidity.Http.DynamicProxies;
using System;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.Reflection;
using System.IO;

namespace Sample.Http.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("started===============");

            var dotnetCoreDirectory = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();

            var compilation = CSharpCompilation.Create("a")
    .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            .AddReferences(MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location))
                       .AddReferences(MetadataReference.CreateFromFile(typeof(System.Console).GetTypeInfo().Assembly.Location))
                       //.AddReferences(MetadataReference.CreateFromFile(Path.Combine(dotnetCoreDirectory, "mscorlib.dll")))
                       //.AddReferences(MetadataReference.CreateFromFile(Path.Combine(dotnetCoreDirectory, "netstandard.dll")))
                       .AddReferences(MetadataReference.CreateFromFile(Path.Combine(dotnetCoreDirectory, "System.Runtime.dll")))

            .AddSyntaxTrees(CSharpSyntaxTree.ParseText(
        @"
using System;

public static class C
{
    public static string Test(string input)
    {
        System.Console.WriteLine(input);
        return input;
    }
}"));
            Type target = null;
            using (var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms);
                if (!result.Success)
                {
                    var error = result.Diagnostics[0];
                    Console.WriteLine(error.GetMessage());
                }
                else
                {
                    var assembly = Assembly.Load(ms.GetBuffer());
                    target = assembly.GetType("C", true, true);
                }
            }
            var text = target.GetMethod("Test").Invoke(null, new object[] { "你好而法国微软微软微软为" });

            Console.ReadKey();
        }
    }
}
