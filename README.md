# 🗂️ Plugin.ByteArray

[![CI](https://github.com/framinosona/Plugin.ByteArray/actions/workflows/ci.yml/badge.svg)](https://github.com/framinosona/Plugin.ByteArray/actions/workflows/ci.yml)
[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![NuGet](https://img.shields.io/nuget/v/Plugin.ByteArrays?logo=nuget&color=004880)](https://www.nuget.org/packages/Plugin.ByteArrays)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Plugin.ByteArrays?logo=nuget&color=004880)](https://www.nuget.org/packages/Plugin.ByteArrays)
[![GitHub Release](https://img.shields.io/github/v/release/framinosona/Plugin.ByteArray?logo=github)](https://github.com/framinosona/Plugin.ByteArray/releases)
[![License](https://img.shields.io/github/license/framinosona/Plugin.ByteArray?color=blue)](LICENSE.md)
[![GitHub Pages](https://img.shields.io/badge/docs-GitHub%20Pages-blue?logo=github)](https://framinosona.github.io/Plugin.ByteArray/)

A comprehensive set of utilities for working with byte arrays in .NET applications. These helpers are designed for performance, safety, and ease of use across all supported platforms.

---

## 📦 Features

- **ByteArrayBuilder**: Fluently build byte arrays from various types and encodings
- **ByteArrayExtensions**: Extension methods for reading primitives, strings, and complex types from byte arrays
- **Array Manipulation**: Safe operations like slicing, concatenation, trimming, reversing, and XOR
- **Pattern Matching**: Search and compare byte arrays with StartsWith, EndsWith, IndexOf, and equality checks
- **Format Conversion**: Convert to/from hex strings, Base64, ASCII, and UTF-8 encodings
- **Object Serialization**: Convert objects and primitives to byte arrays with type safety
- **Safe Operations**: OrDefault methods that never throw exceptions and bounds-checked operations

---

## 🛠️ Usage Examples

### ByteArrayBuilder

```csharp
using Plugin.ByteArrays;

using var builder = new ByteArrayBuilder();
builder.Append(0x01)
       .AppendUtf8String("Hello")
       .Append(new byte[] { 0x02, 0x03 })
       .Append(42)
       .AppendHexString("DEADBEEF");

byte[] result = builder.ToByteArray();
```

### Reading Primitives from Byte Arrays

```csharp
using Plugin.ByteArrays;

byte[] data = { 0x01, 0x00, 0x48, 0x65, 0x6C, 0x6C, 0x6F };
var position = 0;

bool flag = data.ToBoolean(ref position);        // reads 1 byte
int value = data.ToInt32(ref position);          // reads 4 bytes, advances position
string text = data.ToUtf8String(ref position, 5); // reads 5 bytes

// Safe variants that don't throw
var safeValue = data.ToInt16OrDefault(ref position, defaultValue: -1);
```

### Array Manipulation

```csharp
using Plugin.ByteArrays;

byte[] data = { 0x10, 0x20, 0x30, 0x40, 0x00, 0x00 };

// Safe slicing
byte[] slice = data.SafeSlice(1, 3);  // [0x20, 0x30, 0x40]

// Concatenation
byte[] a = { 0x01, 0x02 };
byte[] b = { 0x03, 0x04 };
byte[] combined = ByteArrayExtensions.Concatenate(a, b);  // [0x01, 0x02, 0x03, 0x04]

// Trimming trailing zeros
byte[] trimmed = data.TrimEndNonDestructive();  // [0x10, 0x20, 0x30, 0x40]

// Reverse and XOR operations
byte[] reversed = data.Reverse();
byte[] xorResult = a.Xor(new byte[] { 0xFF, 0xFF });
```

### Pattern Matching and Search

```csharp
using Plugin.ByteArrays;

byte[] data = { 0x48, 0x65, 0x6C, 0x6C, 0x6F };
byte[] pattern = { 0x65, 0x6C };

bool starts = data.StartsWith(new byte[] { 0x48, 0x65 });  // true
bool ends = data.EndsWith(new byte[] { 0x6C, 0x6F });      // true
int index = data.IndexOf(pattern);                         // 1

// Array comparison
byte[] other = { 0x48, 0x65, 0x6C, 0x6C, 0x6F };
bool identical = data.IsIdenticalTo(other);                // true
```

### String and Format Conversions

```csharp
using Plugin.ByteArrays;

// String to byte array
byte[] utf8Bytes = "Hello World".Utf8StringToByteArray();
byte[] asciiBytes = "Hello".AsciiStringToByteArray();
byte[] hexBytes = "48656C6C6F".HexStringToByteArray();     // "Hello" in hex
byte[] base64Bytes = "SGVsbG8=".Base64StringToByteArray(); // "Hello" in base64

// Byte array to string
string hex = data.ToHexString("-", "0x");  // "0x48-0x65-0x6C-0x6C-0x6F"
string base64 = data.ToBase64String();
string utf8 = data.ToUtf8String(ref position, length: 5);
```

### Object Serialization

```csharp
using Plugin.ByteArrays;

// Convert any supported type to byte array
int number = 42;
byte[] numberBytes = number.ToByteArray();

DateTime now = DateTime.Now;
byte[] dateBytes = now.ToByteArray();

// Enums are supported
MyEnum enumValue = MyEnum.SomeValue;
byte[] enumBytes = enumValue.ToByteArray();
```

---

## 📋 API Overview

### Core Classes

- **`ByteArrayBuilder`** - Fluent builder for constructing byte arrays
- **`ByteArrayExtensions`** - Extension methods for reading and manipulating byte arrays
- **`ObjectToByteArrayExtensions`** - Object-to-byte-array conversion helpers

### Reading Operations

- **Primitives**: `ToBoolean`, `ToByte`, `ToSByte`, `ToChar`, `ToInt16/32/64`, `ToUInt16/32/64`
- **Floating Point**: `ToSingle`, `ToDouble`, `ToHalf`
- **Strings**: `ToUtf8String`, `ToAsciiString`, `ToHexString`, `ToBase64String`
- **Complex Types**: `ToEnum<T>`, `ToVersion`
- **Safe Variants**: All methods have `OrDefault` versions that return defaults instead of throwing

### Writing Operations

- **Fluent Building**: `Append<T>`, `AppendUtf8String`, `AppendAsciiString`, `AppendHexString`, `AppendBase64String`
- **Direct Conversion**: `Utf8StringToByteArray`, `HexStringToByteArray`, `ToByteArray<T>`

### Array Operations

- **Manipulation**: `SafeSlice`, `Concatenate`, `TrimEnd`, `TrimEndNonDestructive`, `Reverse`, `Xor`
- **Pattern Matching**: `StartsWith`, `EndsWith`, `IndexOf`, `IsIdenticalTo`
- **Debugging**: `ToDebugString`, `ToHexDebugString`

---

## 🔧 Design Principles

- **Safety First**: Explicit bounds checking with clear exception messages
- **Zero-Allocation Friendly**: Efficient operations using `ReadOnlySpan<byte>` and `SequenceEqual`
- **Fail-Safe Defaults**: `OrDefault` methods never advance read cursors on failure
- **Type Safety**: Strict enum conversion with validation for undefined values
- **Cross-Platform**: Optimized for .NET 9 and modern C# features

---

## 🧪 Development

- **Testing**: xUnit + FluentAssertions with comprehensive coverage
- **Build**: `dotnet build -c Release`
- **Test**: `dotnet test -c Release`
- **Framework**: .NET 9.0

---

## 📄 License

MIT — see `LICENSE.md` for details.

---

*Designed for modern .NET applications requiring efficient, safe byte array operations.*
