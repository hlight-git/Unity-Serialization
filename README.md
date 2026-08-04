# Hlight Serialization

Serializer dạng `ScriptableObject`: cấu hình một lần trong Inspector, kéo thả vào bất kỳ field
nào có kiểu `ASerializationAsset`, đổi backend mà không sửa code.

## Cài đặt

Thêm vào `Packages/manifest.json`:

```json
"com.hlight.serialization": "1.0.0"
```

Các backend ngoài (Newtonsoft, Odin) là **tùy chọn** — package vẫn compile khi không có chúng,
chỉ ném exception nếu bạn thực sự gọi backend chưa cài.

| Backend | Yêu cầu | Symbol |
|---------|---------|--------|
| Built-In | không | — |
| Cast Type | không | — |
| Newtonsoft | `com.unity.nuget.newtonsoft-json` | `USE_NEWTONSOFT_JSON` (tự động qua `versionDefines`) |
| Odin | Odin Inspector hoặc OdinSerializer standalone | `ODIN_SERIALIZER` (tự động khi có `ODIN_INSPECTOR`) |

## Chọn backend

| | Built-In | Cast Type | Newtonsoft | Odin |
|---|---|---|---|---|
| Dependencies | không | không | ~200KB | Odin |
| Fields | ✅ | — | ✅ | ✅ |
| Properties | ❌ | — | ✅ | ✅ |
| `Dictionary<,>` | ❌ | ❌ | ✅ | ✅ |
| Polymorphism | ❌ | ❌ | ✅ | ✅ |
| Circular refs | ❌ | ❌ | ✅ | ✅ |
| Unity types | ✅ | ❌ | ⚠️ | ✅ |
| Binary format | ❌ | ❌ | ❌ | ✅ |
| Tốc độ | nhanh nhất | nhanh nhất | chậm hơn ~2-3x | nhanh |

- **Built-In** (`JsonUtility`) — data đơn giản chỉ dùng fields. [Chi tiết](Documentation~/builtin.md)
- **Cast Type** — chỉ cho primitive/simple types (`int`, `enum`, `DateTime`, `Guid`…), không phải JSON. [Chi tiết](Documentation~/casttype.md)
- **Newtonsoft** — JSON đầy đủ tính năng, 20 options cấu hình. [Chi tiết](Documentation~/newtonsoft.md)
- **Odin** — Unity types + binary format. [Chi tiết](Documentation~/odin.md)

## Sử dụng

### 1. Tạo asset

`Assets > Create > Hlight > Serialization > ...` rồi cấu hình trong Inspector.

### 2. Kéo thả vào script

```csharp
using Hlight.Serialization.Serializer;

public class DataManager : MonoBehaviour
{
    [SerializeField] private ASerializationAsset _serializer;

    public void Save(GameData data)
        => PlayerPrefs.SetString("GameData", _serializer.Serialize(data));

    public GameData Load()
        => _serializer.Deserialize<GameData>(PlayerPrefs.GetString("GameData"));
}
```

Asset được cache sẵn nên không tạo garbage, và tái sử dụng được ở nhiều nơi. Cần nhiều cấu hình
khác nhau (production compact vs debug pretty-print) thì tạo nhiều asset.

### 3. Binary

`ISerializer.SerializeToBytes` / `IDeserializer.Deserialize(byte[], …)` có sẵn trên mọi backend.
Built-In/Cast Type/Newtonsoft chỉ là UTF-8 của chuỗi; Odin cho binary format thật.

## Inspector Serialization Tool

Gắn `[ShowInspectorSerializationTool]` lên field/property để có nút Serialize/Deserialize ngay
trong Inspector — tiện để copy state ra clipboard hoặc paste data test vào.

```csharp
using Hlight.Serialization.InspectorSerializationTool;

[ShowInspectorSerializationTool]
[SerializeField] private GameData _defaultValue;
```

Attribute có `[Conditional("UNITY_EDITOR")]` nên không bị emit vào build. Có drawer riêng cho
Odin Inspector và một IMGUI fallback khi không có Odin.

## Viết backend mới

Kế thừa `ASerializationAsset` và delegate sang một class `[Serializable]` lồng bên trong —
class đó implement `ISerializer` + `IDeserializer` và giữ toàn bộ config, nhờ vậy dùng lại được
ngoài ScriptableObject.

```csharp
[CreateAssetMenu(menuName = "Hlight/Serialization/My Serializer")]
public class MySerializationAsset : ASerializationAsset
{
    [SerializeField] private MySerializer serializer = new();

    public override string Serialize(object o) => serializer.Serialize(o);
    // … forward phần còn lại

    [Serializable]
    public class MySerializer : ISerializer, IDeserializer { /* … */ }
}
```

Với dependency ngoài, dùng **proxy enum** để config vẫn hiện trong Inspector khi package chưa
được cài, và `#if` quanh chỗ thực sự gọi library:

```csharp
public enum FormatProxy { Compact = 0, Pretty = 1 }   // giá trị khớp enum thật

[SerializeField] private FormatProxy _format;

#if MY_LIB
    ExternalFormat format = (ExternalFormat)_format;
#else
    throw new NotSupportedException("Import MyLib và thêm symbol `MY_LIB`.");
#endif
```

## Cấu trúc

```
Runtime/
  ISerializer.cs, IDeserializer.cs, ASerializationAsset.cs
  ShowInspectorSerializationToolAttribute.cs
  BuiltIn/ CastType/ Newtonsoft/ Odin/
Editor/
  ShowInspectorSerializationToolAttributeDrawer.cs        (IMGUI fallback)
  ShowInspectorSerializationToolAttributeOdinDrawer.cs    (Odin)
Documentation~/
```

Assembly: `Hlight.Serialization` (runtime), `Hlight.Serialization.Editor` (editor).
