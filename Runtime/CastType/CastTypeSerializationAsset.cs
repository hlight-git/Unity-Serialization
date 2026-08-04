using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace Hlight.Serialization.Serializer
{
    /// <summary>
    /// Plain <see cref="Convert"/>/<see cref="TypeConverter"/> backend for primitive and simple
    /// types. See <see cref="CastTypeSerializer"/>.
    /// </summary>
    [CreateAssetMenu(fileName = "CastTypeSerializer", menuName = "Hlight/Serialization/Cast Type Serializer")]
    public class CastTypeSerializationAsset : ASerializationAsset
    {
        [SerializeField] private CastTypeSerializer serializer = new();

        public CastTypeSerializer Serializer => serializer;

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

        /// <summary>
        /// Lightweight serializer for primitive/simple types (int, float, bool, string,
        /// enum, DateTime, Guid, etc.) using <see cref="Convert.ChangeType(object,Type)"/>
        /// and <see cref="TypeConverter"/>. Not suitable for collections, nested objects,
        /// or custom classes — use BuiltIn/Newtonsoft/Odin for those.
        /// </summary>
        [Serializable]
        public class CastTypeSerializer : ISerializer, IDeserializer
        {
            public string Serialize(object objectToSerialize)
            {
                if (objectToSerialize == null) return string.Empty;
                return Convert.ToString(objectToSerialize, CultureInfo.InvariantCulture);
            }

            public byte[] SerializeToBytes(object objectToSerialize)
                => Encoding.UTF8.GetBytes(Serialize(objectToSerialize));

            public object Deserialize(string serializedObject, Type type)
            {
                if (type == null) throw new ArgumentNullException(nameof(type));
                if (string.IsNullOrEmpty(serializedObject)) return GetDefault(type);

                if (type == typeof(string)) return serializedObject;
                if (type.IsEnum) return Enum.Parse(type, serializedObject, ignoreCase: true);

                var converter = TypeDescriptor.GetConverter(type);
                if (converter != null && converter.CanConvertFrom(typeof(string)))
                    return converter.ConvertFromInvariantString(serializedObject);

                return Convert.ChangeType(serializedObject, type, CultureInfo.InvariantCulture);
            }

            public T Deserialize<T>(string serializedObject)
                => (T)Deserialize(serializedObject, typeof(T));

            public object Deserialize(byte[] serializedObject, Type type)
                => Deserialize(Encoding.UTF8.GetString(serializedObject), type);

            public T Deserialize<T>(byte[] serializedObject)
                => Deserialize<T>(Encoding.UTF8.GetString(serializedObject));

            private static object GetDefault(Type t)
                => t.IsValueType ? Activator.CreateInstance(t) : null;
        }
    }
}
