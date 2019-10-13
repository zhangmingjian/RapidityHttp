using System.Reflection;

namespace Rapidity.Http
{
    public interface IRequestDescriptionBuilder
    {
        RequestDescription Build(MethodInfo method, params object[] parameters);
    }
}