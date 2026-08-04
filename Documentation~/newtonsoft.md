# NewtonsoftSerializer

## Tổng quan

`NewtonsoftSerializer` sử dụng **Newtonsoft.Json (Json.NET)** với nhiều options cấu hình chi tiết.

## Yêu cầu

- **Package:** `com.unity.nuget.newtonsoft-json`
- **Scripting Symbol:** `USE_NEWTONSOFT_JSON` — **tự động**, không cần thêm tay

### Cài đặt

- Window > Package Manager, tìm "Newtonsoft Json"
- Hoặc thêm vào `manifest.json`: `"com.unity.nuget.newtonsoft-json": "x.y.z"`

`Hlight.Serialization.asmdef` khai báo `versionDefines` cho package này, nên `USE_NEWTONSOFT_JSON`
được define ngay khi package có mặt.

## Đặc điểm

### Ưu điểm

- Hỗ trợ properties và fields
- Hỗ trợ `Dictionary<TKey, TValue>`
- Hỗ trợ polymorphism
- Xử lý circular references
- 15+ options cấu hình
- JSON.NET standard

### Nhược điểm

- Cần external package
- Thêm ~200KB vào build
- Chậm hơn JsonUtility (~2-3x)

## Khi nào sử dụng

**Phù hợp khi:**
- Cần serialize properties, dictionaries, polymorphism
- Cần control chi tiết về serialization behavior
- Cross-platform JSON standard
- Complex data structures

**Không phù hợp khi:**
- Data structures đơn giản chỉ dùng fields
- Muốn tránh dependencies
- Cần performance cao nhất
- Cần binary format

## Proxy Enums Pattern

NewtonsoftSerializer sử dụng **Proxy Enums** để tách rời dependency với Newtonsoft.Json:

- Configuration hiển thị trong Inspector ngay cả khi chưa import package
- Tránh compile errors khi thiếu package
- Editor-friendly workflow

```csharp
// Proxy Enum - không phụ thuộc Newtonsoft
public enum FormattingProxy
{
    None = 0,
    Indented = 1
}

[SerializeField] private FormattingProxy _formatting;

#if USE_NEWTONSOFT_JSON
// Cast sang Newtonsoft enum khi sử dụng
Formatting = (Formatting)_formatting
#endif
```

## Common Configurations

### Configuration 1: Production

```csharp
_formatting = FormattingProxy.None;
_nullValueHandling = NullValueHandlingProxy.Ignore;
_defaultValueHandling = DefaultValueHandlingProxy.Ignore;
_dateTimeZoneHandling = DateTimeZoneHandlingProxy.Utc;
_referenceLoopHandling = ReferenceLoopHandlingProxy.Error;
_missingMemberHandling = MissingMemberHandlingProxy.Ignore;
```

Compact JSON, ignore defaults/nulls

---

### Configuration 2: Development/Debugging

```csharp
_formatting = FormattingProxy.Indented;
_nullValueHandling = NullValueHandlingProxy.Include;
_defaultValueHandling = DefaultValueHandlingProxy.Include;
_typeNameHandling = TypeNameHandlingProxy.Auto;
```

Human-readable, full information

---

### Configuration 3: Complex Object Graphs

```csharp
_preserveReferencesHandling = PreserveReferencesHandlingProxy.All;
_referenceLoopHandling = ReferenceLoopHandlingProxy.Ignore;
_typeNameHandling = TypeNameHandlingProxy.Auto;
_maxDepth = 128;
```

Handle circular refs, preserve object identity

---

### Configuration 4: Cross-Platform/API

```csharp
_formatting = FormattingProxy.None;
_dateFormatHandling = DateFormatHandlingProxy.IsoDateFormat;
_dateTimeZoneHandling = DateTimeZoneHandlingProxy.Utc;
_nullValueHandling = NullValueHandlingProxy.Ignore;
_missingMemberHandling = MissingMemberHandlingProxy.Ignore;
```

Standard JSON, UTC dates, backward/forward compatible

## Best Practices

### DO ✅

```csharp
// ✅ Tái sử dụng serializer asset
[SerializeField] private NewtonsoftSerializationAsset _serializer;

// ✅ Dùng properties
public class UserData
{
    public string Username { get; set; }
    public int Level { get; set; }
}

// ✅ Dùng dictionaries
public Dictionary<string, int> Stats { get; set; }

// ✅ Tạo nhiều assets với configurations khác nhau
[SerializeField] private NewtonsoftSerializationAsset _productionSerializer;
[SerializeField] private NewtonsoftSerializationAsset _debugSerializer;

// ✅ Handle errors
try
{
    var data = _serializer.Deserialize<GameData>(json);
}
catch (Newtonsoft.Json.JsonException ex)
{
    Debug.LogError($"Deserialization failed: {ex.Message}");
}
```

### DON'T ❌

```csharp
// ❌ Đừng quên scripting symbol
#if USE_NEWTONSOFT_JSON
// Code
#endif

// ❌ Đừng ignore reference loops
// Configure _referenceLoopHandling hoặc _preserveReferencesHandling

// ❌ Đừng dùng TypeNameHandling.All trong production
// Security risk - dùng Auto hoặc None

// ❌ Đừng tạo instance mới mỗi lần
void Save()
{
    var serializer = ScriptableObject.CreateInstance<NewtonsoftSerializationAsset>(); // Garbage!
}
```

## Performance Tips

### 1. Tái sử dụng Serializer Asset

```csharp
// Tốt: Tái sử dụng asset
[SerializeField] private NewtonsoftSerializationAsset _serializer;

void Awake()
{
    // Asset đã được cached
}
```

### 2. Tránh TypeNameHandling khi không cần

```csharp
// Chậm: TypeNameHandling.All
_typeNameHandling = TypeNameHandlingProxy.All;

// Nhanh hơn: Chỉ khi cần
_typeNameHandling = TypeNameHandlingProxy.Auto;

// Nhanh nhất: Không type handling
_typeNameHandling = TypeNameHandlingProxy.None;
```

### 3. Dùng Formatting.None trong production

```csharp
// Production: compact
_formatting = FormattingProxy.None;

// Development: pretty print
_formatting = FormattingProxy.Indented;
```

---

[← Quay lại Module Overview](../README.md)
