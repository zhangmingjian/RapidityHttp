using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;

namespace Rapidity.Http.DynamicProxies
{
    /// <summary>
    /// 
    /// </summary>
    public class ProxyGenerator
    {
        public static Type Generate(Type type)
        {
            //var options = new Dictionary<string, string> {
            //    { "CompilerVersion", "v4.5" }
            //};
            //CSharpCodeProvider provider = new CSharpCodeProvider();
            var provider = CodeDomProvider.CreateProvider("CSharp");
            var referencedDll = new string[]
            {
                "mscorlib.dll",
                "System.dll",
                "System.Core.dll",
            };
            var parameters = new CompilerParameters(referencedDll)
            {
                IncludeDebugInformation = true,
                GenerateInMemory = true,
                TreatWarningsAsErrors = true
            };
            var source = new[] { "class Test { static void Foo() {}}" };
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