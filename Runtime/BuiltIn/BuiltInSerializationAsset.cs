using System;
using System.Text;
using UnityEngine;

namespace Hlight.Serialization.Serializer
{
    /// <summary>
    /// <see cref="JsonUtility"/> backend. No dependencies, fastest, but fields only — no
    /// properties, dictionaries or polymorphism. See Documentation~/builtin.md.
    /// </summary>
    [CreateAssetMenu(fileName = "BuiltInSerializer", menuName = "Hlight/Serialization/Built-In Serializer")]
    public class BuiltInSerializationAsset : ASerializationAsset
    {
        [SerializeField] private BuiltInSerializer serializer = new();

        public BuiltInSerializer Serializer => serializer;

        public override string Serialize(object objectToSerialize)
            => serializer.Serialize(objectToSerialize);

        public override byte[] SerializeToBytes(object objectToSerialize)
            => serializer.SerializeToBytes(objectToSerialize);

        public override object Deserialize(string serializedObject, Type type)
            => serializer.Deserialize(serializedObject, type);

        public override T Deserialize<T>(string serializedObject)
            => serializer.Deserialize<T>(serializedObject);

        public override object Deserialize(byte[] serializedObject, Type type)
            => serializer.Deserialize(serializedObject, type);

        public override T Deserialize<T>(byte[] serializedObject)
            => serializer.Deserialize<T>(serializedObject);

        [Serializable]
        public class BuiltInSerializer : ISerializer, IDeserializer
        {
            [SerializeField] private bool prettyPrintSerialize;

            public BuiltInSerializer() { }

            public BuiltInSerializer(bool prettyPrintSerialize)
            {
                this.prettyPrintSerialize = prettyPrintSerialize;
            }

            public string Serialize(object objectToSerialize)
                => JsonUtility.ToJson(objectToSerialize, prettyPrintSerialize);

            public byte[] SerializeToBytes(object objectToSerialize)
                => Encoding.UTF8.GetBytes(Serialize(objectToSerialize));

            public object Deserialize(string serializedObject, Type type)
                => JsonUtility.FromJson(serializedObject, type);

            public T Deserialize<T>(string serializedObject)
                => JsonUtility.FromJson<T>(serializedObject);

            public object Deserialize(byte[] serializedObject, Type type)
                => Deserialize(Encoding.UTF8.GetString(serializedObject), type);

            public T Deserialize<T>(byte[] serializedObject)
                => Deserialize<T>(Encoding.UTF8.GetString(serializedObject));
        }
    }
}
