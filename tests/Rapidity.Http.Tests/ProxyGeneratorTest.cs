using Rapidity.Http.DynamicProxies;
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
            var type = ProxyGenerator.Generate(typeof(string));
        }
    }
}