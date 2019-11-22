using System.Reflection;

namespace Rapidity.Http
{
    public interface IRequestDescriptorBuilder
    {
        RequestDescriptor Build(MethodInfo method, params object[] arguments);
    }
}