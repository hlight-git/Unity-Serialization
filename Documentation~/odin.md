# OdinSerializer

## Tổng quan

`OdinSerializer` sử dụng **Sirenix OdinSerializer**. Tối ưu cho Unity types và hỗ trợ 3 formats: Binary, JSON, Nodes.

## Yêu cầu

- **Package:** Odin Inspector hoặc Odin Serializer (standalone, free)
- **Scripting Symbol:** `ODIN_SERIALIZER`

### Cài đặt

#### Option 1: Odin Inspector (Paid)

1. Mua từ Unity Asset Store
2. Import vào project

Xong. Odin Inspector đã define `ODIN_INSPECTOR` và ship sẵn `Sirenix.Serialization`, nên
`OdinSerializationAsset.cs` tự bật `ODIN_SERIALIZER` — không cần thêm symbol tay.

#### Option 2: Odin Serializer (Free, Standalone)

1. Download từ: https://odininspector.com/download
2. Import package
3. Edit > Project Settings > Player > Scripting Define Symbols, thêm `ODIN_SERIALIZER`

## Đặc điểm

### Ưu điểm

- Tối ưu cho Unity types (Vector3, Quaternion, Color, etc.)
- 3 formats: Binary, JSON, Nodes
- Hỗ trợ dictionaries
- Hỗ trợ polymorphism
- Nhanh, đặc biệt với Binary format

### Nhược điểm

- Cần external package
- Binary format không human-readable
- JSON format lớn hơn JsonUtility (includes metadata)

## Khi nào sử dụng

**Phù hợp khi:**
- Cần serialize Unity types (Vector3, Quaternion, Color, etc.)
- Cần binary format (compact, fast)
- Cần dictionaries với Unity types as keys
- Complex Unity objects

**Không phù hợp khi:**
- Data đơn giản không chứa Unity types
- Muốn tránh dependencies
- Cần JSON nhỏ nhất có thể

## Data Formats

### DataFormatProxy Enum

```csharp
public enum DataFormatProxy
{
    Binary,  // Compact binary format, Base64 encoded
    JSON,    // Human-readable JSON format
    Nodes,   // Odin's proprietary node format
}
```

### 1. Binary Format

**Đặc điểm:**
- Compact (~30-50% nhỏ hơn JSON)
- Nhanh
- Output: Base64 string
- Không human-readable

**Output example:**
```
H4sIAAAAAAAAA+2QMQ6DMBBE...base64...
```

**Khi nào dùng:**
- Network transmission
- File storage (save games)
- Performance critical
- Size matters

---

### 2. JSON Format

**Đặc điểm:**
- Human-readable
- Debug-friendly
- Cross-platform compatible
- Lớn hơn Binary
- Includes type metadata

**Output example:**
```json
{
  "$type": "UnityEngine.Vector3, UnityEngine",
  "x": 1.5,
  "y": 2.0,
  "z": 3.5
}
```

**Khi nào dùng:**
- Development/debugging
- Cần edit manually
- Cross-platform/API integration

---

### 3. Nodes Format

**Đặc điểm:**
- Odin's proprietary format
- Balance giữa size và features
- Ít dùng

**Khi nào dùng:**
- Debug
- Special Odin features
- Internal Unity Editor serialization

---

## Supported Types

### ✅ Fully Supported

| Category | Types |
|----------|-------|
| Primitives | `int`, `float`, `bool`, `string`, etc. |
| Unity Math | `Vector2/3/4`, `Quaternion`, `Matrix4x4` |
| Unity Types | `Color`, `Rect`, `Bounds`, `AnimationCurve` |
| Collections | `List<T>`, `Array`, `Dictionary<TKey, TValue>` |
| Generic Types | `MyClass<T>`, `Container<T1, T2>` |
| Properties | Properties và fields |
| Enums | All enum types |
| Nullable Types | `int?`, `float?`, etc. |
| Multi-dimensional Arrays | `int[,]`, `int[,,]` |

### ⚠️ Partially Supported

| Type | Note |
|------|------|
| Unity Object References | `GameObject`, `Component` - serialized as references |
| ScriptableObjects | Cần proper handling |

### ❌ Not Supported

- Static fields
- Delegates and events
- Pointers/IntPtr

## Binary Format Details

### Size Comparison

Ví dụ với cùng một object:

```
Binary (Odin):  ~150 bytes (Base64)
JSON (Odin):    ~250 bytes
```

## Best Practices

### DO ✅

```csharp
// ✅ Tái sử dụng serializer assets
[SerializeField] private OdinSerializationAsset _productionSerializer; // Binary
[SerializeField] private OdinSerializationAsset _debugSerializer; // JSON

// ✅ Serialize Unity types trực tiếp
public Vector3 Position { get; set; }

// ✅ Dùng dictionaries
public Dictionary<string, ItemData> Items { get; set; }

// ✅ Properties và fields
public int Level { get; set; }
private int _experience;

// ✅ Polymorphic collections
public List<Enemy> Enemies { get; set; }

// ✅ Complex nested structures
public Dictionary<Vector3, List<ItemData>> WorldItems { get; set; }
```

### DON'T ❌

```csharp
// ❌ Đừng quên scripting symbol
#if ODIN_SERIALIZER
// Code
#endif

// ❌ Đừng serialize Unity Object references trong saved files
public GameObject MyObject { get; set; } // Không work across scenes

// ❌ Đừng mix Binary và JSON
string binary = _binarySerializer.Serialize(data);
var loaded = _jsonSerializer.Deserialize<T>(binary); // ERROR!

// ❌ Đừng tạo instance mới mỗi lần
void Save()
{
    var serializer = ScriptableObject.CreateInstance<OdinSerializationAsset>(); // Garbage!
}
```

---

[← Quay lại Module Overview](../README.md)
