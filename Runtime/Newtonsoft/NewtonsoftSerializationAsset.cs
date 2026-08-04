using System;
using System.Text;
using UnityEngine;

#if USE_NEWTONSOFT_JSON
using Newtonsoft.Json;
#endif

namespace Hlight.Serialization.Serializer
{
    /// <summary>
    /// Newtonsoft.Json backend. Requires the <c>com.unity.nuget.newtonsoft-json</c> package;
    /// the <c>USE_NEWTONSOFT_JSON</c> symbol is defined automatically when it is installed.
    /// See Documentation~/newtonsoft.md.
    /// </summary>
    [CreateAssetMenu(fileName = "NewtonsoftSerializer", menuName = "Hlight/Serialization/Newtonsoft Serializer")]
    public class NewtonsoftSerializationAsset : ASerializationAsset
    {
        [SerializeField] private NewtonsoftSerializer serializer = new();

        public NewtonsoftSerializer Serializer => serializer;

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

#if USE_NEWTONSOFT_JSON
        private void OnValidate() => serializer?.InvalidateSettings();
#endif

        [Serializable]
        public class NewtonsoftSerializer : ISerializer, IDeserializer
        {
            [SerializeField] private FormattingProxy _formatting = FormattingProxy.None;
            [SerializeField] private DateFormatHandlingProxy _dateFormatHandling = DateFormatHandlingProxy.IsoDateFormat;
            [SerializeField] private DateTimeZoneHandlingProxy _dateTimeZoneHandling = DateTimeZoneHandlingProxy.RoundtripKind;
            [SerializeField] private DateParseHandlingProxy _dateParseHandling = DateParseHandlingProxy.DateTime;
            [SerializeField] private FloatFormatHandlingProxy _floatFormatHandling = FloatFormatHandlingProxy.String;
            [SerializeField] private FloatParseHandlingProxy _floatParseHandling = FloatParseHandlingProxy.Double;
            [SerializeField] private StringEscapeHandlingProxy _stringEscapeHandling = StringEscapeHandlingProxy.Default;
            [SerializeField] private bool _checkAdditionalContent;
            [SerializeField] private int _maxDepth = 64;
            [SerializeField] private string _dateFormatString = @"yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK";

            [SerializeField]
            private TypeNameAssemblyFormatHandlingProxy _typeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandlingProxy.Simple;

            [SerializeField] private DefaultValueHandlingProxy _defaultValueHandling = DefaultValueHandlingProxy.Include;

            [SerializeField]
            private PreserveReferencesHandlingProxy _preserveReferencesHandling = PreserveReferencesHandlingProxy.None;

            [SerializeField] private NullValueHandlingProxy _nullValueHandling = NullValueHandlingProxy.Include;
            [SerializeField] private ObjectCreationHandlingProxy _objectCreationHandling = ObjectCreationHandlingProxy.Auto;
            [SerializeField] private MissingMemberHandlingProxy _missingMemberHandling = MissingMemberHandlingProxy.Ignore;
            [SerializeField] private ReferenceLoopHandlingProxy _referenceLoopHandling = ReferenceLoopHandlingProxy.Error;
            [SerializeField] private ConstructorHandlingProxy _constructorHandling = ConstructorHandlingProxy.Default;
            [SerializeField] private TypeNameHandlingProxy _typeNameHandling = TypeNameHandlingProxy.None;
            [SerializeField] private MetadataPropertyHandlingProxy _metadataPropertyHandling = MetadataPropertyHandlingProxy.Default;

#if USE_NEWTONSOFT_JSON
            private JsonSerializerSettings settings;

            public JsonSerializerSettings Settings => settings ??= NewSettings;

            private JsonSerializerSettings NewSettings => new()
            {
                Formatting = (Formatting)_formatting,
                DateFormatHandling = (DateFormatHandling)_dateFormatHandling,
                DateTimeZoneHandling = (DateTimeZoneHandling)_dateTimeZoneHandling,
                DateParseHandling = (DateParseHandling)_dateParseHandling,
                FloatFormatHandling = (FloatFormatHandling)_floatFormatHandling,
                FloatParseHandling = (FloatParseHandling)_floatParseHandling,
                StringEscapeHandling = (StringEscapeHandling)_stringEscapeHandling,
                CheckAdditionalContent = _checkAdditionalContent,
                MaxDepth = _maxDepth > 0 ? _maxDepth : (int?)null,
                DateFormatString = _dateFormatString,
                TypeNameAssemblyFormatHandling = (TypeNameAssemblyFormatHandling)_typeNameAssemblyFormatHandling,
                DefaultValueHandling = (DefaultValueHandling)_defaultValueHandling,
                PreserveReferencesHandling = (PreserveReferencesHandling)_preserveReferencesHandling,
                NullValueHandling = (NullValueHandling)_nullValueHandling,
                ObjectCreationHandling = (ObjectCreationHandling)_objectCreationHandling,
                MissingMemberHandling = (MissingMemberHandling)_missingMemberHandling,
                ReferenceLoopHandling = (ReferenceLoopHandling)_referenceLoopHandling,
                ConstructorHandling = (ConstructorHandling)_constructorHandling,
                TypeNameHandling = (TypeNameHandling)_typeNameHandling,
                MetadataPropertyHandling = (MetadataPropertyHandling)_metadataPropertyHandling
            };

            /// <summary>Rebuilds <see cref="Settings"/> after the inspector fields change.</summary>
            public void InvalidateSettings() => settings = NewSettings;
#endif

            public string Serialize(object objectToSerialize)
            {
#if USE_NEWTONSOFT_JSON
                return JsonConvert.SerializeObject(objectToSerialize, Settings);
#else
                throw new Exception("You need to import the Newtonsoft Json package and add the `USE_NEWTONSOFT_JSON` scripting symbol to use the NewtonsoftSerializer.");
#endif
            }

            public byte[] SerializeToBytes(object objectToSerialize)
                => Encoding.UTF8.GetBytes(Serialize(objectToSerialize));

            public object Deserialize(string serializedObject, Type type)
            {
#if USE_NEWTONSOFT_JSON
                return JsonConvert.DeserializeObject(serializedObject, type, Settings);
#else
                throw new Exception("You need to import the Newtonsoft Json package and add the `USE_NEWTONSOFT_JSON` scripting symbol to use the NewtonsoftSerializer.");
#endif
            }

            public T Deserialize<T>(string serializedObject)
            {
#if USE_NEWTONSOFT_JSON
                return JsonConvert.DeserializeObject<T>(serializedObject, Settings);
#else
                throw new Exception("You need to import the Newtonsoft Json package and add the `USE_NEWTONSOFT_JSON` scripting symbol to use the NewtonsoftSerializer.");
#endif
            }

            public object Deserialize(byte[] serializedObject, Type type)
                => Deserialize(Encoding.UTF8.GetString(serializedObject), type);

            public T Deserialize<T>(byte[] serializedObject)
                => Deserialize<T>(Encoding.UTF8.GetString(serializedObject));

            #region Proxies

            public enum FormattingProxy
            {
                None = 0,
                Indented = 1
            }

            public enum DateFormatHandlingProxy
            {
                IsoDateFormat,
                MicrosoftDateFormat
            }

            public enum DateTimeZoneHandlingProxy
            {
                Local = 0,
                Utc = 1,
                Unspecified = 2,
                RoundtripKind = 3
            }

            public enum DateParseHandlingProxy
            {
                None = 0,
                DateTime = 1,
                DateTimeOffset = 2
            }

            public enum FloatFormatHandlingProxy
            {
                String = 0,
                Symbol = 1,
                DefaultValue = 2
            }

            public enum FloatParseHandlingProxy
            {
                Double = 0,
                Decimal = 1
            }

            public enum StringEscapeHandlingProxy
            {
                Default = 0,
                EscapeNonAscii = 1,
                EscapeHtml = 2
            }

            public enum TypeNameAssemblyFormatHandlingProxy
            {
                Simple = 0,
                Full = 1
            }

            [Flags]
            public enum DefaultValueHandlingProxy
            {
                Include = 0,
                Ignore = 1,
                Populate = 2,
                IgnoreAndPopulate = Ignore | Populate
            }

            [Flags]
            public enum PreserveReferencesHandlingProxy
            {
                None = 0,
                Objects = 1,
                Arrays = 2,
                All = Objects | Arrays
            }

            public enum NullValueHandlingProxy
            {
                Include = 0,
                Ignore = 1
            }

            public enum ObjectCreationHandlingProxy
            {
                Auto = 0,
                Reuse = 1,
                Replace = 2
            }

            public enum MissingMemberHandlingProxy
            {
                Ignore = 0,
                Error = 1
            }

            public enum ReferenceLoopHandlingProxy
            {
                Error = 0,
                Ignore = 1,
                Serialize = 2
            }

            public enum ConstructorHandlingProxy
            {
                Default = 0,
                AllowNonPublicDefaultConstructor = 1
            }

            [Flags]
            public enum TypeNameHandlingProxy
            {
                None = 0,
                Objects = 1,
                Arrays = 2,
                All = Objects | Arrays,
                Auto = 4
            }

            public enum MetadataPropertyHandlingProxy
            {
                Default = 0,
                ReadAhead = 1,
                Ignore = 2
            }

            #endregion
        }
    }
}
