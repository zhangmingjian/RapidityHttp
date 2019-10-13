using System;

namespace Rapidity.Http.DynamicProxies
{
    /// <summary>
    /// 代理生成器
    /// </summary>
    public interface IProxyGenerator
    {
        Type Generate(Type type);
    }
}
