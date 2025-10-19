# ReadOnlyMemoryExtensions

## Class/Module Overview

The `ReadOnlyMemoryExtensions` class provides extension methods for working with `ReadOnlyMemory<byte>`, offering memory-safe operations that complement the `ReadOnlySpanExtensions` class. While `ReadOnlySpan<byte>` is optimized for stack-allocated, short-lived operations, `ReadOnlyMemory<byte>` supports heap-allocated, longer-lived scenarios including async operations.

### Purpose

- **Heap-allocated processing**: Support for data that needs to outlive a single method call
- **Async compatibility**: Can be used in async methods (unlike ReadOnlySpan)
- **Storage-friendly**: Can be stored in fields and classes
- **Consistent API**: Delegates to ReadOnlySpan for actual operations

### Capabilities

- Pattern matching (StartsWith, EndsWith, IndexOf)
- Sequence comparison (IsIdenticalTo)
- Debugging utilities (ToDebugString, ToHexDebugString)
- All operations leverage the underlying Span property for zero-copy semantics

## API Documentation

### Debugging Methods

#### ToDebugString

```csharp
public static string ToDebugString(this ReadOnlyMemory<byte> memory)
```

Converts the ReadOnlyMemory\<byte\> into a readable string for debugging purposes.

**Example:**

```csharp
ReadOnlyMemory<byte> memory = new byte[] { 1, 2, 3, 255 };
string debug = memory.ToDebugString(); // Returns: "[1,2,3,255]"
```

#### ToHexDebugString

```csharp
public static string ToHexDebugString(this ReadOnlyMemory<byte> memory)
```

Converts the ReadOnlyMemory\<byte\> to its hexadecimal string representation.

**Example:**

```csharp
ReadOnlyMemory<byte> memory = new byte[] { 0xAB, 0xCD, 0xEF };
string hex = memory.ToHexDebugString(); // Returns: "[AB,CD,EF]"
```

### Pattern Matching

#### StartsWith

```csharp
public static bool StartsWith(this ReadOnlyMemory<byte> memory, ReadOnlySpan<byte> pattern)
```

Checks if a ReadOnlyMemory\<byte\> starts with a specific pattern.

**Example:**

```csharp
ReadOnlyMemory<byte> memory = new byte[] { 1, 2, 3, 4, 5 };
ReadOnlySpan<byte> pattern = new byte[] { 1, 2, 3 };
bool starts = memory.StartsWith(pattern); // Returns: true
```

#### EndsWith

```csharp
public static bool EndsWith(this ReadOnlyMemory<byte> memory, ReadOnlySpan<byte> pattern)
```

Determines whether the ReadOnlyMemory\<byte\> ends with the specified pattern.

**Example:**

```csharp
ReadOnlyMemory<byte> memory = new byte[] { 1, 2, 3, 4, 5 };
ReadOnlySpan<byte> pattern = new byte[] { 4, 5 };
bool ends = memory.EndsWith(pattern); // Returns: true
```

#### IndexOf

```csharp
public static int IndexOf(this ReadOnlyMemory<byte> memory, ReadOnlySpan<byte> pattern)
```

Finds the first occurrence of a pattern in a ReadOnlyMemory\<byte\>.

**Returns:** The index of the first occurrence, or -1 if not found

**Example:**

```csharp
ReadOnlyMemory<byte> memory = new byte[] { 1, 2, 3, 4, 5 };
ReadOnlySpan<byte> pattern = new byte[] { 3, 4 };
int index = memory.IndexOf(pattern); // Returns: 2
```

### Comparison

#### IsIdenticalTo

```csharp
public static bool IsIdenticalTo(this ReadOnlyMemory<byte> memory1, ReadOnlyMemory<byte> memory2)
```

Checks if two ReadOnlyMemory\<byte\> instances are identical in content and length.

**Example:**

```csharp
ReadOnlyMemory<byte> memory1 = new byte[] { 1, 2, 3 };
ReadOnlyMemory<byte> memory2 = new byte[] { 1, 2, 3 };
bool identical = memory1.IsIdenticalTo(memory2); // Returns: true
```

## Practical Examples

### Async File Processing

```csharp
public async Task ProcessFileAsync(string filePath)
{
    // ReadOnlyMemory can be used in async methods
    byte[] buffer = await File.ReadAllBytesAsync(filePath);
    ReadOnlyMemory<byte> memory = buffer;

    // Pattern matching works with async
    ReadOnlySpan<byte> header = new byte[] { 0x50, 0x4B, 0x03, 0x04 }; // ZIP signature
    if (memory.StartsWith(header))
    {
        await ProcessZipFileAsync(memory);
    }
}
```

### Storing Memory References

```csharp
public class DataProcessor
{
    private ReadOnlyMemory<byte> _data;

    public DataProcessor(byte[] data)
    {
        _data = data; // Can store ReadOnlyMemory in fields
    }

    public bool HasPattern(byte[] pattern)
    {
        return _data.IndexOf(pattern) >= 0;
    }
}
```

### Converting Between Memory and Span

```csharp
ReadOnlyMemory<byte> memory = GetDataMemory();

// Access as span for operations
ReadOnlySpan<byte> span = memory.Span;

// Use span extensions
int value = span.ToInt32();
string text = span.ToUtf8String();
```

## Performance Characteristics

### Time Complexity

All operations delegate to ReadOnlySpan, maintaining O(n) characteristics for pattern matching and O(1) for access operations.

### Memory Usage

- **Heap-allocated**: Memory reference itself is stored on heap
- **Zero-copy**: Actual operations use span semantics (no data copying)
- **Reference overhead**: Small overhead for maintaining memory reference

### When to Use ReadOnlyMemory vs ReadOnlySpan

- **Use ReadOnlyMemory when:**
  - Need to store reference in a field or class
  - Working with async methods
  - Data lifetime exceeds single method call
  - Need to pass data across async boundaries

- **Use ReadOnlySpan when:**
  - Data is short-lived (single method)
  - Need absolute maximum performance
  - Working synchronously
  - Can use stack allocation

## Best Practices

### Prefer Span for Operations

```csharp
// DO: Convert to span for intensive operations
ReadOnlyMemory<byte> memory = GetData();
ReadOnlySpan<byte> span = memory.Span;
ProcessWithSpan(span); // More efficient

// AVOID: Using memory methods repeatedly
ProcessWithMemory(memory); // Extra indirection
```

### Async Pattern

```csharp
// DO: Use Memory for async operations
public async Task<int> ReadValueAsync(ReadOnlyMemory<byte> data)
{
    await Task.Delay(100); // Can await
    return data.Span.ToInt32(); // Convert to span for reading
}

// CAN'T: ReadOnlySpan doesn't work with async
// public async Task<int> ReadValueAsync(ReadOnlySpan<byte> data) // Compile error
```

## Cross-References

### Related Components

- [ReadOnlySpanExtensions](ReadOnlySpanExtensions.md) - High-performance span operations
- [ByteArrayAsyncExtensions](ByteArrayAsyncExtensions.md) - Async operations for byte arrays
- [ByteArrayExtensions](ByteArrayExtensions.md) - Traditional byte array operations

### Type Conversion Extensions

For type conversions, convert ReadOnlyMemory to ReadOnlySpan first:

- [ReadOnlySpanExtensions.PrimitiveTypeConversion](ReadOnlySpanExtensions.PrimitiveTypeConversion.md)
- [ReadOnlySpanExtensions.IntegerConversion](ReadOnlySpanExtensions.IntegerConversion.md)
- [ReadOnlySpanExtensions.StringConversion](ReadOnlySpanExtensions.StringConversion.md)
