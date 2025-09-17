# 🗂️ Plugin.ByteArrays Documentation

Welcome to the **Plugin.ByteArrays** documentation! This library provides a comprehensive set of utilities for working with byte arrays in .NET applications, designed for performance, safety, and ease of use.

---

## 🚀 Quick Start

### Installation

```bash
# Install from NuGet
dotnet add package Plugin.ByteArrays
```

### Basic Usage

```csharp
using Plugin.ByteArrays;

// Build byte arrays fluently
using var builder = new ByteArrayBuilder();
byte[] data = builder
    .Append(0x01)
    .AppendUtf8String("Hello")
    .Append(42)
    .ToByteArray();

// Read primitives from byte arrays
var position = 0;
byte flag = data.ToByte(ref position);
string text = data.ToUtf8String(ref position, 5);
int number = data.ToInt32(ref position);

// Safe array operations
byte[] slice = data.SafeSlice(1, 3);
byte[] trimmed = data.TrimEndNonDestructive();
```

---

## 📚 Core Features

### 🔧 ByteArrayBuilder

Build byte arrays fluently from various data types:

- Primitives (int, float, bool, etc.)
- Strings (UTF-8, ASCII, Hex, Base64)
- Enums and custom objects
- Efficient memory management with IDisposable

### 🔍 Reading Operations

Extract data from byte arrays safely:

- **Primitives**: `ToBoolean`, `ToByte`, `ToInt32`, `ToDouble`, etc.
- **Strings**: `ToUtf8String`, `ToAsciiString`, `ToHexString`
- **Complex Types**: `ToEnum<T>`, `ToVersion`
- **Safe Variants**: `OrDefault` methods that never throw exceptions

### ⚙️ Array Manipulation

Powerful array operations:

- **Slicing**: `SafeSlice` for bounds-safe array segments
- **Concatenation**: `Concatenate` multiple arrays efficiently
- **Trimming**: `TrimEnd` and `TrimEndNonDestructive`
- **Transformation**: `Reverse`, `Xor` operations

### 🔎 Pattern Matching

Search and compare byte arrays:

- **Pattern Search**: `StartsWith`, `EndsWith`, `IndexOf`
- **Equality**: `IsIdenticalTo` with optimized comparisons
- **Format Conversion**: Hex and Base64 utilities

---

## 📖 Documentation Sections

| Section | Description |
|---------|-------------|
| [API Reference](./api/Plugin.ByteArrays.html) | Complete API documentation |

---

## 🎯 Design Principles

- **🛡️ Safety First**: Explicit bounds checking with clear exception messages
- **⚡ Performance**: Zero-allocation friendly with `ReadOnlySpan<byte>` usage
- **🔄 Fail-Safe**: `OrDefault` methods never advance read cursors on failure
- **🎯 Type Safety**: Strict enum conversion with validation
- **🌐 Cross-Platform**: Optimized for .NET 9 and modern C# features

---

## 🚀 Advanced Examples

### Working with Complex Data

```csharp
// Serialize a complex structure
using var builder = new ByteArrayBuilder();
var data = builder
    .Append((ushort)0x1234)           // Header
    .AppendUtf8String("UserData")     // Section name
    .Append(DateTime.Now)             // Timestamp
    .Append(MyEnum.Active)            // Status
    .AppendHexString("DEADBEEF")      // Binary data
    .ToByteArray();

// Read it back safely
var pos = 0;
var header = data.ToUInt16(ref pos);
var section = data.ToUtf8String(ref pos, 8);
var timestamp = data.ToDateTime(ref pos);
var status = data.ToEnum<MyEnum>(ref pos);
```

### Error-Safe Reading

```csharp
byte[] data = GetDataFromSomewhere();
var pos = 0;

// These methods never throw - they return defaults on error
var id = data.ToInt32OrDefault(ref pos, defaultValue: -1);
var name = data.ToUtf8StringOrDefault(ref pos, 20, defaultValue: "Unknown");
var version = data.ToVersionOrDefault(ref pos, defaultValue: new Version(1, 0));

// Position only advances on successful reads
Console.WriteLine($"Processed {pos} bytes successfully");
```

---

## 🔧 Development & Testing

- **Framework**: .NET 9.0
- **Testing**: xUnit + FluentAssertions with comprehensive coverage
- **Build**: `dotnet build -c Release`
- **Test**: `dotnet test -c Release`

---

## 📄 License

This library is licensed under the **MIT License** - see the [LICENSE.md](https://github.com/framinosona/Plugin.ByteArrays/blob/main/LICENSE.md) file for details.

---

## 🤝 Contributing

Contributions are welcome! Please feel free to submit issues, feature requests, or pull requests on [GitHub](https://github.com/framinosona/Plugin.ByteArrays).

---

*Built with ❤️ for modern .NET applications requiring efficient, safe byte array operations.*
