namespace Rapidity.Http.Serializes
{
    public interface IContentSerializer
    {
        string Serialize(object content);
    }
}