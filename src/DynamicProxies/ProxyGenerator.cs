using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;

namespace Rapidity.Http.DynamicProxies
{
    /// <summary>
    /// https://stackoverflow.com/questions/37526165/compiling-and-running-code-at-runtime-in-net-core-1-0
    /// https://benohead.com/three-options-to-dynamically-execute-csharp-code/
    /// </summary>
    public class ProxyGenerator
    {
        public static Type Generate(Type type)
        {
            CSharpCodeProvider provider = new CSharpCodeProvider();
            var referencedDll = new string[]
            {
                "System.dll"
            };
            var parameters = new CompilerParameters(referencedDll)
            {
                IncludeDebugInformation = false,
                GenerateInMemory = true,
                TreatWarningsAsErrors = true
            };
            var source = "namespace Sample.test{ class Test { static void Foo() {}} }";
            var result = provider.CompileAssemblyFromSource(parameters, source);
            if (result.Errors.HasErrors)
            {
                var error = result.Errors[0];
                throw new Exception(error.ToString());
            }
            
            var typerest = result.CompiledAssembly.GetType("Test");
            return typerest;
        }
    }
}