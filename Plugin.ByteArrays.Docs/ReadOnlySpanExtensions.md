# ReadOnlySpanExtensions

## Class/Module Overview

The `ReadOnlySpanExtensions` class provides comprehensive extension methods for working with `ReadOnlySpan<byte>`, enabling zero-allocation, high-performance operations for reading and manipulating byte data. This class is designed as a modern, performance-optimized alternative to traditional byte array operations, leveraging the power of `Span<T>` APIs introduced in .NET Core.

### Purpose

- **Zero-allocation operations**: All methods work directly on memory without creating intermediate arrays
- **High performance**: Optimized for speed-critical applications using modern Span APIs
- **Type-safe conversions**: Convert byte sequences to various .NET types with compile-time safety
- **Position tracking**: Automatic position advancement for sequential reading patterns
- **Safe variants**: OrDefault methods that never throw exceptions

### Capabilities

This class provides over 100 extension methods organized into the following categories:

- Core utilities (pattern matching, comparison, debugging)
- Primitive type conversions (bool, byte, char, sbyte)
- Integer conversions (all signed/unsigned variants)
- Floating-point conversions (float, double, Half, decimal)
- String conversions (UTF-8, ASCII, hex, Base64)
- DateTime conversions (DateTime, TimeSpan, DateTimeOffset)
- GUID conversions
- Complex type conversions (enums, Version objects)
- Network conversions (IP addresses, network byte order)

### Architectural Role

`ReadOnlySpanExtensions` serves as the primary interface for type-safe, performant byte data reading in modern .NET applications. It complements the existing `ByteArrayExtensions` class by providing:

- Stack-allocated processing for small buffers
- Direct memory access without heap allocations
- Integration with high-performance networking and I/O operations
- Support for modern .NET memory management patterns

## API Documentation

### Core Utilities

#### ToDebugString

```csharp
public static string ToDebugString(this ReadOnlySpan<byte> span)
```

Converts the `ReadOnlySpan<byte>` into a readable string for debugging purposes. Each byte is represented as a decimal value, separated by commas.

**Parameters:**

- `span` - The `ReadOnlySpan<byte>` to process

**Returns:** A string representing the span as comma-separated decimal numbers

**Example:**

```csharp
ReadOnlySpan<byte> span = new byte[] { 1, 2, 3, 255 };
string debug = span.ToDebugString(); // Returns: "[1,2,3,255]"
```

#### ToHexDebugString

```csharp
public static string ToHexDebugString(this ReadOnlySpan<byte> span)
```

Converts the `ReadOnlySpan<byte>` to its hexadecimal string representation for debugging.

**Example:**

```csharp
ReadOnlySpan<byte> span = new byte[] { 1, 2, 15, 255 };
string hex = span.ToHexDebugString(); // Returns: "[01,02,0F,FF]"
```

#### StartsWith

```csharp
public static bool StartsWith(this ReadOnlySpan<byte> span, ReadOnlySpan<byte> pattern)
```

Checks if a `ReadOnlySpan<byte>` starts with a specific pattern.

**Parameters:**

- `span` - The span to check
- `pattern` - The pattern to look for

**Returns:** True if the span starts with the pattern, false otherwise

**Performance:** O(n) where n is the pattern length

**Example:**

```csharp
ReadOnlySpan<byte> span = new byte[] { 1, 2, 3, 4, 5 };
ReadOnlySpan<byte> pattern = new byte[] { 1, 2, 3 };
bool starts = span.StartsWith(pattern); // Returns: true
```

#### EndsWith

```csharp
public static bool EndsWith(this ReadOnlySpan<byte> span, ReadOnlySpan<byte> pattern)
```

Determines whether the `ReadOnlySpan<byte>` ends with the specified pattern.

**Example:**

```csharp
ReadOnlySpan<byte> span = new byte[] { 1, 2, 3, 4, 5 };
ReadOnlySpan<byte> pattern = new byte[] { 3, 4, 5 };
bool ends = span.EndsWith(pattern); // Returns: true
```

#### IndexOf

```csharp
public static int IndexOf(this ReadOnlySpan<byte> span, ReadOnlySpan<byte> pattern)
```

Finds the first occurrence of a pattern in a `ReadOnlySpan<byte>`.

**Returns:** The index of the first occurrence, or -1 if not found

**Performance:** O(n*m) where n is span length and m is pattern length

**Example:**

```csharp
ReadOnlySpan<byte> span = new byte[] { 1, 2, 3, 4, 5 };
ReadOnlySpan<byte> pattern = new byte[] { 3, 4 };
int index = span.IndexOf(pattern); // Returns: 2
```

#### IsIdenticalTo

```csharp
public static bool IsIdenticalTo(this ReadOnlySpan<byte> span1, ReadOnlySpan<byte> span2)
```

Checks if two `ReadOnlySpan<byte>` instances are identical in content and length.

**Performance:** O(n) using optimized SequenceEqual

**Example:**

```csharp
ReadOnlySpan<byte> span1 = new byte[] { 1, 2, 3 };
ReadOnlySpan<byte> span2 = new byte[] { 1, 2, 3 };
bool identical = span1.IsIdenticalTo(span2); // Returns: true
```

### Type Conversion Methods

All type conversion methods follow a consistent 4-method pattern:

1. **ToType(ref position)** - Advances position automatically
2. **ToType(position = 0)** - Non-ref convenience overload
3. **ToTypeOrDefault(ref position, defaultValue)** - Safe variant, never throws
4. **ToTypeOrDefault(position = 0, defaultValue)** - Safe non-ref overload

This pattern is maintained across all type conversions for consistency and ease of use.

## Practical Examples

### Sequential Reading Pattern

```csharp
var data = new byte[] {
    1,                        // bool
    42,                       // byte
    0x12, 0x34,              // int16
    0x12, 0x34, 0x56, 0x78   // int32
};
ReadOnlySpan<byte> span = data;
var position = 0;

bool flag = span.ToBoolean(ref position);     // position advances to 1
byte value = span.ToByte(ref position);       // position advances to 2
short count = span.ToInt16(ref position);     // position advances to 4
int total = span.ToInt32(ref position);       // position advances to 8
```

### Error-Safe Reading

```csharp
ReadOnlySpan<byte> span = new byte[] { 1, 2 }; // Only 2 bytes
var position = 0;

// This would throw ArgumentOutOfRangeException (needs 4 bytes)
// int value = span.ToInt32(ref position);

// Safe variant returns default value
int value = span.ToInt32OrDefault(ref position, -1); // Returns: -1
// position unchanged (0) on failure
```

### String Operations

```csharp
ReadOnlySpan<byte> span = Encoding.UTF8.GetBytes("Hello, World! 🌍");

// Read entire span as UTF-8 string
string text = span.ToUtf8String(); // Returns: "Hello, World! 🌍"

// Read partial string
var position = 0;
string hello = span.ToUtf8String(ref position, 5); // Returns: "Hello"

// Convert to hex
ReadOnlySpan<byte> data = new byte[] { 0xAB, 0xCD, 0xEF };
string hex = data.ToHexString(); // Returns: "ABCDEF"

// Convert to Base64
string base64 = data.ToBase64String();
```

## Advanced Usage Patterns

### Network Protocol Parsing

```csharp
// Parse network packet with mixed data types
byte[] packet = GetNetworkPacket();
ReadOnlySpan<byte> span = packet;
var pos = 0;

// Read header
var version = span.ToByte(ref pos);
var flags = span.ToByte(ref pos);
var messageLength = span.ToUInt16NetworkOrder(ref pos);

// Read payload
var messageBytes = span.Slice(pos, messageLength);
var message = messageBytes.ToUtf8String();
```

### Configuration File Parsing

```csharp
ReadOnlySpan<byte> configData = File.ReadAllBytes("config.bin");
var position = 0;

// Read version information
var majorVersion = configData.ToInt32(ref position);
var minorVersion = configData.ToInt32(ref position);
var buildNumber = configData.ToInt32(ref position);
var revision = configData.ToInt32(ref position);
var version = configData.ToVersion(position - 16); // Reconstruct Version object

// Read timestamp
var ticks = configData.ToInt64(ref position);
var timestamp = new DateTime(ticks);

// Read feature flags enum
var features = configData.ToEnum<FeatureFlags>(ref position);
```

### Zero-Copy Buffer Processing

```csharp
// Process large buffer without allocations
Memory<byte> largeBuffer = GetLargeDataBuffer();
ReadOnlySpan<byte> span = largeBuffer.Span;

// Scan for patterns without creating substrings
ReadOnlySpan<byte> delimiter = new byte[] { 0x0D, 0x0A }; // CRLF
int index = span.IndexOf(delimiter);

while (index >= 0)
{
    // Process line without allocation
    var line = span[..index];
    ProcessLine(line);

    span = span[(index + delimiter.Length)..];
    index = span.IndexOf(delimiter);
}
```

## Performance Characteristics

### Time Complexity

- **Pattern matching** (StartsWith, EndsWith): O(n) where n is pattern length
- **IndexOf**: O(n*m) where n is span length, m is pattern length
- **Type conversions**: O(1) - constant time for all fixed-size types
- **String conversions**: O(n) where n is the number of bytes to convert

### Memory Usage

- **Zero heap allocations** for all read operations
- **Stack-only** processing for small spans (<= 512 bytes typical)
- **No intermediate arrays** created during conversions
- **Shared memory** - operates directly on existing buffers

### Scalability

- **Constant performance** regardless of source buffer size (when using Slice)
- **Thread-safe** for read operations (ReadOnlySpan is immutable)
- **Cache-friendly** sequential access patterns

### Optimization Notes

- Use `ref position` parameters for sequential reading to minimize overhead
- Prefer `OrDefault` methods in hot paths to avoid exception overhead
- Slice large spans before passing to methods to improve locality
- Consider using stackalloc for temporary spans up to 512 bytes

## Best Practices

### Position Management

```csharp
// DO: Use ref position for sequential reading
var position = 0;
var a = span.ToInt32(ref position);
var b = span.ToInt32(ref position);

// DON'T: Manual position tracking (error-prone)
var a = span.ToInt32(position);
position += 4;
var b = span.ToInt32(position);
position += 4;
```

### Error Handling

```csharp
// DO: Use OrDefault for potentially invalid data
var config = untrustedData.ToInt32OrDefault(ref pos, DEFAULT_CONFIG);

// DON'T: Use throwing methods for untrusted data
try {
    var config = untrustedData.ToInt32(ref pos);
} catch (ArgumentOutOfRangeException) {
    // Exception handling is expensive
}
```

### Memory Efficiency

```csharp
// DO: Slice before processing
var relevantData = largeSpan.Slice(offset, length);
ProcessData(relevantData);

// DON'T: Pass entire span when only part is needed
ProcessData(largeSpan, offset, length);
```

### Common Pitfalls

1. **Capturing spans in lambdas**: ReadOnlySpan cannot be used in async methods or captured by lambdas
2. **Storing spans**: Spans are stack-only types; use ReadOnlyMemory for storage
3. **Position validation**: Always verify sufficient data exists before reading
4. **Endianness**: Be aware of system endianness for multi-byte types

## Cross-References

### Related Components

- [ReadOnlyMemoryExtensions](ReadOnlyMemoryExtensions.md) - Memory-based operations
- [ReadOnlySpanUtilities](ReadOnlySpanUtilities.md) - Analysis and utility operations
- [ByteArrayExtensions](ByteArrayExtensions.md) - Traditional byte array operations
- [ByteArrayBuilder](ByteArrayBuilder.md) - Building byte arrays

### See Also

- [Primitive Type Conversions](ReadOnlySpanExtensions.PrimitiveTypeConversion.md)
- [Integer Conversions](ReadOnlySpanExtensions.IntegerConversion.md)
- [String Conversions](ReadOnlySpanExtensions.StringConversion.md)
- [Network Conversions](ReadOnlySpanExtensions.NetworkConversion.md)
