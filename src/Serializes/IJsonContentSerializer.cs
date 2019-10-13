namespace Rapidity.Http.Serializes
{
    /// <summary>
    /// json序列化器
    /// </summary>
    public interface IJsonContentSerializer : IContentSerializer, IContentDeserializer
    {
    }

    /// <summary>
    /// xml序列化器
    /// </summary>
    public interface IXmlContentSerializer : IContentSerializer, IContentDeserializer
    {
    }
}