# BuiltInSerializer

## Tổng quan

`BuiltInSerializer` sử dụng `JsonUtility` của Unity. Không có dependencies bên ngoài.

## Đặc điểm

### Ưu điểm

- Không có dependencies bên ngoài
- Nhanh
- Không thêm size cho build
- Không cần configuration
- Tương thích với Unity serialization system

### Nhược điểm

- Chỉ serialize fields (không serialize properties)
- Không hỗ trợ `Dictionary<,>`
- Không hỗ trợ polymorphism
- Không xử lý circular references
- Cần `[Serializable]` attribute

## Khi nào sử dụng

**Phù hợp khi:**
- Data structures đơn giản (primitives, lists, nested classes)
- Không cần dependencies
- Chỉ dùng fields (không có properties)
- Cần performance cao
- Prototype và testing nhanh

**Không phù hợp khi:**
- Cần serialize properties
- Cần Dictionary
- Cần polymorphism
- Cần binary format
- Data structures phức tạp

## Supported Types

### ✅ Được hỗ trợ

| Type Category | Examples |
|--------------|----------|
| Primitives | `int`, `float`, `bool`, `string` |
| Unity Types | `Vector2/3/4`, `Quaternion`, `Color`, `Rect` |
| Arrays | `int[]`, `string[]`, `CustomClass[]` |
| Lists | `List<int>`, `List<string>`, `List<CustomClass>` |
| Nested Classes | Classes có `[Serializable]` |
| Enums | Tất cả enum types |

### ❌ Không hỗ trợ

| Type Category | Lý do |
|--------------|-------|
| Properties | JsonUtility chỉ serialize fields |
| Dictionaries | Không hỗ trợ bởi Unity serialization |
| Interfaces | Không thể instantiate |
| Abstract Classes | Không thể instantiate |
| Polymorphic Collections | `List<BaseClass>` với derived types |
| Circular References | Gây infinite loop |
| Static Fields | Không phải instance data |

## Best Practices

### DO ✅

```csharp
// ✅ Dùng [Serializable] attribute
[System.Serializable]
public class MyData
{
    public int value;
}

// ✅ Dùng fields, không properties
public class MyData
{
    public int score; // OK
    [SerializeField] private int _level; // OK
}

// ✅ Dùng Lists thay vì Dictionaries
public class MyData
{
    public List<string> keys;
    public List<int> values;
}

// ✅ Initialize collections
public class MyData
{
    public List<int> items = new List<int>();
}

// ✅ Tái sử dụng serializer asset
[SerializeField] private BuiltInSerializationAsset _serializer; // Kéo thả asset
```

### DON'T ❌

```csharp
// ❌ Đừng dùng properties
public class MyData
{
    public int Score { get; set; } // Không serialize
}

// ❌ Đừng dùng Dictionary
public class MyData
{
    public Dictionary<string, int> items; // Không hỗ trợ
}

// ❌ Đừng quên [Serializable]
public class MyData // Thiếu attribute
{
    public int value;
}

// ❌ Đừng dùng polymorphism
public class MyData
{
    public BaseClass item; // Không serialize derived type đúng
}

// ❌ Đừng tạo instance mới mỗi lần
void Save()
{
    var serializer = ScriptableObject.CreateInstance<BuiltInSerializationAsset>(); // Tạo garbage!
}
```

---

[← Quay lại Module Overview](../README.md)
