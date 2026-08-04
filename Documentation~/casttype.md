# CastTypeSerializer

## Tổng quan

`CastTypeSerializer` **không phải JSON serializer**. Nó chuyển đổi thẳng giữa value và chuỗi
biểu diễn của value đó bằng `Convert.ToString` / `TypeConverter` / `Convert.ChangeType`, luôn
với `CultureInfo.InvariantCulture`.

```csharp
serializer.Serialize(42)                    // "42"
serializer.Serialize(3.14f)                 // "3.14"      (không phụ thuộc locale)
serializer.Deserialize<int>("42")           // 42
serializer.Deserialize<MyEnum>("Active")    // MyEnum.Active  (không phân biệt hoa thường)
```

## Khi nào sử dụng

**Phù hợp khi:**
- Remote config trả về giá trị đơn lẻ dạng string (`"true"`, `"1.5"`, `"Hard"`)
- Đọc/ghi `PlayerPrefs`, query string, CSV cell, env var
- Muốn output là giá trị trần, không có dấu ngoặc kép hay dấu ngoặc nhọn của JSON

**Không phù hợp khi:**
- Data là collection, nested object, hoặc custom class → dùng Built-In/Newtonsoft/Odin

## Supported Types

### ✅ Được hỗ trợ

| Type Category | Examples |
|--------------|----------|
| Primitives | `int`, `long`, `float`, `double`, `decimal`, `bool`, `char` |
| String | `string` (đi thẳng, không escape) |
| Enums | Mọi enum, parse không phân biệt hoa thường |
| Có `TypeConverter` | `DateTime`, `TimeSpan`, `Guid`, `Version`, `Color32`… |

### ❌ Không hỗ trợ

- Collections (`List<T>`, arrays, `Dictionary<,>`)
- Nested objects / custom classes
- Polymorphism

Gọi với các kiểu này sẽ ném `InvalidCastException` hoặc `FormatException` chứ không fail lặng.

## Hành vi biên

| Input | Kết quả |
|-------|---------|
| `Serialize(null)` | `string.Empty` |
| `Deserialize(null/"" , typeof(int))` | `0` (default của value type) |
| `Deserialize(null/"" , typeof(string))` | `null` |
| `Deserialize(text, null)` | `ArgumentNullException` |

## Best Practices

### DO ✅

```csharp
// ✅ Dùng cho giá trị đơn lẻ
int level = _serializer.Deserialize<int>(PlayerPrefs.GetString("level"));

// ✅ Enum từ remote config
Difficulty d = _serializer.Deserialize<Difficulty>(remoteValue);

// ✅ Bọc trong try/catch khi nguồn dữ liệu không tin cậy
try { value = _serializer.Deserialize<float>(raw); }
catch (FormatException) { value = fallback; }
```

### DON'T ❌

```csharp
// ❌ Đừng dùng cho object
_serializer.Serialize(new GameData());   // ra "MyNamespace.GameData"

// ❌ Đừng dùng cho collection
_serializer.Deserialize<List<int>>("1,2,3");   // InvalidCastException

// ❌ Đừng giả định output là JSON
```

---

[← Quay lại Module Overview](../README.md)
