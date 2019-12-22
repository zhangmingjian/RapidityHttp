using System;

namespace Rapidity.Http.Serializes
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class ContentSerializer
    {
        public abstract string Serialize(object content);

        public abstract object Deserialize(string content, Type type);

        public virtual T Deserialize<T>(string content)
        {
            return (T)Deserialize(content, typeof(T));
        }
    }
}