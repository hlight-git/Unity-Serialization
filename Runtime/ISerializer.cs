namespace Hlight.Serialization.Serializer
{
    public interface ISerializer
    {
        string Serialize(object objectToSerialize);
        byte[] SerializeToBytes(object objectToSerialize);
    }
}
