// Odin Inspector ships Sirenix.Serialization, so ODIN_INSPECTOR alone is enough.
// Standalone OdinSerializer users define ODIN_SERIALIZER themselves.
#if ODIN_INSPECTOR && !ODIN_SERIALIZER
#define ODIN_SERIALIZER
#endif

using System;
using UnityEngine;

#if ODIN_SERIALIZER
using Sirenix.Serialization;
#endif

namespace Hlight.Serialization.Serializer
{
    /// <summary>
    /// Sirenix OdinSerializer backend. Handles Unity types, dictionaries and polymorphism, in
    /// Binary/JSON/Nodes formats. See Documentation~/odin.md.
    /// </summary>
    [CreateAssetMenu(fileName = "OdinSerializer", menuName = "Hlight/Serialization/Odin Serializer")]
    public class OdinSerializationAsset : ASerializationAsset
    {
        [SerializeField] private OdinSerializer serializer = new();

        public OdinSerializer Serializer => serializer;

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
        public class OdinSerializer : ISerializer, IDeserializer
        {
            private const string MissingPackage =
                "Import Odin Inspector or the standalone OdinSerializer, and add the `ODIN_SERIALIZER` " +
                "scripting symbol, to use OdinSerializationAsset.";

            [SerializeField] private DataFormatProxy dataFormat;

            public OdinSerializer() { }

            public OdinSerializer(DataFormatProxy dataFormat)
            {
                this.dataFormat = dataFormat;
            }

            public string Serialize(object objectToSerialize)
            {
#if ODIN_SERIALIZER
                return Encode(SerializeToBytes(objectToSerialize));
#else
                throw new NotSupportedException(MissingPackage);
#endif
            }

            public byte[] SerializeToBytes(object objectToSerialize)
            {
#if ODIN_SERIALIZER
                return SerializationUtility.SerializeValue(objectToSerialize, (DataFormat)dataFormat);
#else
                throw new NotSupportedException(MissingPackage);
#endif
            }

            public object Deserialize(string serializedObject, Type type)
                => Deserialize(Decode(serializedObject), type);

            public T Deserialize<T>(string serializedObject)
                => Deserialize<T>(Decode(serializedObject));

            public object Deserialize(byte[] serializedObject, Type type)
            {
#if ODIN_SERIALIZER
                // Odin embeds the concrete type in the payload; `type` is only a hint here.
                return SerializationUtility.DeserializeValueWeak(serializedObject, (DataFormat)dataFormat);
#else
                throw new NotSupportedException(MissingPackage);
#endif
            }

            public T Deserialize<T>(byte[] serializedObject)
            {
#if ODIN_SERIALIZER
                return SerializationUtility.DeserializeValue<T>(serializedObject, (DataFormat)dataFormat);
#else
                throw new NotSupportedException(MissingPackage);
#endif
            }

            /// <summary>Binary payloads are Base64 so they survive a round-trip through string.</summary>
            private string Encode(byte[] bytes) => dataFormat == DataFormatProxy.Binary
                ? Convert.ToBase64String(bytes)
                : System.Text.Encoding.UTF8.GetString(bytes);

            private byte[] Decode(string text) => dataFormat == DataFormatProxy.Binary
                ? Convert.FromBase64String(text)
                : System.Text.Encoding.UTF8.GetBytes(text);

            public enum DataFormatProxy
            {
                Binary,
                JSON,
                Nodes,
            }
        }
    }
}
