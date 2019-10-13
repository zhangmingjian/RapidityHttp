using System;

namespace Rapidity.Http.Serializes
{
    public interface IContentDeserializer
    {
        object Deserialize(string content, Type type);
    }
}