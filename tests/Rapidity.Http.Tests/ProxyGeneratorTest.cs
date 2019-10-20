using System.Collections.Generic;
using Rapidity.Http.DynamicProxies;
using Sample.Service;
using Xunit;

namespace Rapidity.Http.Tests
{
    /// <summary>
    /// 
    /// </summary>
    public class ProxyGeneratorTest
    {
        [Fact]
        public void GeneratorTest()
        {
            var type = ProxyGenerator.Generate(typeof(ITokenService));
        }

        [Fact]
        public void BuildTemplateTest()
        {
            var type = typeof(ITokenService);
            var template = ProxyGenerator.BuildTemplate(type);
        }

        [Fact]
        public void TypeTemplateTest()
        {
            var type = typeof(IDictionary<string, IList<int>>);
            var template = new TypeTemplate(type);
            var str = template.ToString();
        }
    }
}