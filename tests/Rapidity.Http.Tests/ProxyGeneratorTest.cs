﻿using Rapidity.Http.DynamicProxies;
using Sample.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            var types = new Type[] { typeof(IGenericService<,>), typeof(ITestService), typeof(ITokenService), typeof(IUserService) };
            var assembly = ProxyGenerator.Generate(types);
            var generateTypes = assembly.ExportedTypes;
            Assert.Equal(types.Length, generateTypes.Count());
        }

        [Fact]
        public void BuildTemplateTest()
        {
            var types = new Type[] { typeof(ITokenService) };
            var template = ProxyGenerator.BuildTemplate(types);
        }

        [Fact]
        public void Service_Template_Build_Test()
        {
            var types = new Type[] { typeof(ITestService) };
            var template = ProxyGenerator.BuildTemplate(types);

            Assert.NotNull(template);
            Assert.Equal(types.Length, template.ClassList.Count);
            var className = template.ClassList.First().Name;
            Assert.Equal("AutoGeneratedTestService", className);
        }


        [Fact]
        public void TypeTemplateTest()
        {
            var type = typeof(IDictionary<string, Func<object, DateTime, bool, Task<Guid>>>);
            var template = new TypeTemplate(type);
            var name = template.ToString();
            Assert.Equal("System.Collections.Generic.IDictionary<System.String,System.Func<System.Object,System.DateTime,System.Boolean,System.Threading.Tasks.Task<System.Guid>>>", name);
        }
    }
}