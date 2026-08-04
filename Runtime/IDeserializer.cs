using System;

namespace Hlight.Serialization.Serializer
{
    public interface IDeserializer
    {
        object Deserialize(string serializedObject, Type type);
        T Deserialize<T>(string serializedObject);

        object Deserialize(byte[] serializedObject, Type type);
        T Deserialize<T>(byte[] serializedObject);
    }
}
