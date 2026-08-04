using System;
using UnityEngine;

namespace Hlight.Serialization.Serializer
{
    /// <summary>
    /// Base class for serializer assets. Create one asset per configuration and drop it into
    /// any field typed as <see cref="ASerializationAsset"/> to swap backends without code changes.
    /// </summary>
    public abstract class ASerializationAsset : ScriptableObject, ISerializer, IDeserializer
    {
        public abstract string Serialize(object objectToSerialize);

        public abstract byte[] SerializeToBytes(object objectToSerialize);

        public abstract object Deserialize(string serializedObject, Type type);

        public abstract T Deserialize<T>(string serializedObject);

        public abstract object Deserialize(byte[] serializedObject, Type type);

        public abstract T Deserialize<T>(byte[] serializedObject);
    }
}
