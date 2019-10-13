namespace Rapidity.Http.Serializes
{
    public static class ContentDeserializerExtension
    {
        public static T Deserialize<T>(this IContentDeserializer deserializer, string content)
        {
            return (T)deserializer.Deserialize(content, typeof(T));
        }
    }
}