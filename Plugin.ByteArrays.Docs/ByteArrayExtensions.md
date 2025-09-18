# ByteArrayExtensions Class

The `ByteArrayExtensions` class is the core partial class that provides fundamental byte array operations including debugging utilities, pattern matching, search operations, and array comparison methods. This class serves as the foundation for all other ByteArrayExtensions partial classes.

## Core Debugging Utilities

### ToDebugString
Converts a byte array to a readable string representation for debugging purposes, showing each byte as a decimal value.

#### Syntax
```csharp
public static string ToDebugString(this IEnumerable<byte>? array)
```

#### Example
```csharp
var data = new byte[] { 65, 66, 67, 0, 255 };
var debugString = data.ToDebugString();
Console.WriteLine(debugString);
// Output: [65,66,67,0,255]

// Handle null arrays
byte[]? nullArray = null;
var nullDebug = nullArray.ToDebugString();
Console.WriteLine(nullDebug);
// Output: <null>

// Empty array
var empty = new byte[0];
Console.WriteLine(empty.ToDebugString());
// Output: []
```

### ToHexDebugString
Converts a byte array to a hexadecimal string representation for debugging, showing each byte as a two-digit hex value.

#### Syntax
```csharp
public static string ToHexDebugString(this IEnumerable<byte>? array)
```

#### Example
```csharp
var data = new byte[] { 0xDE, 0xAD, 0xBE, 0xEF, 0x00, 0xFF };
var hexDebug = data.ToHexDebugString();
Console.WriteLine(hexDebug);
// Output: [DE,AD,BE,EF,00,FF]

// Compare with decimal debug
Console.WriteLine(data.ToDebugString());
// Output: [222,173,190,239,0,255]

// Handle edge cases
var singleByte = new byte[] { 0x0A };
Console.WriteLine(singleByte.ToHexDebugString());
// Output: [0A]
```

## Pattern Matching and Search Operations

### StartsWith
Checks if a byte array starts with a specific pattern.

#### Syntax
```csharp
public static bool StartsWith(this byte[] array, byte[] pattern)
```

#### Example
```csharp
var data = new byte[] { 0xFF, 0xFE, 0x01, 0x02, 0x03 };
var pattern = new byte[] { 0xFF, 0xFE };

var startsWithPattern = data.StartsWith(pattern);
Console.WriteLine($"Starts with pattern: {startsWithPattern}"); // True

// Test different patterns
var wrongPattern = new byte[] { 0x01, 0x02 };
Console.WriteLine($"Starts with wrong pattern: {data.StartsWith(wrongPattern)}"); // False

// Empty pattern always matches
var emptyPattern = new byte[0];
Console.WriteLine($"Starts with empty: {data.StartsWith(emptyPattern)}"); // True

// Pattern longer than array
var longPattern = new byte[] { 0xFF, 0xFE, 0x01, 0x02, 0x03, 0x04 };
Console.WriteLine($"Starts with long pattern: {data.StartsWith(longPattern)}"); // False
```

### EndsWith
Determines whether a byte array ends with a specified pattern.

#### Syntax
```csharp
public static bool EndsWith(this byte[] array, byte[] pattern)
```

#### Example
```csharp
var data = new byte[] { 0x01, 0x02, 0x03, 0xFF, 0xFE };
var pattern = new byte[] { 0xFF, 0xFE };

var endsWithPattern = data.EndsWith(pattern);
Console.WriteLine($"Ends with pattern: {endsWithPattern}"); // True

// Protocol footer detection
var httpResponse = "HTTP/1.1 200 OK\r\n\r\n"u8.ToArray();
var crlf = "\r\n\r\n"u8.ToArray();
Console.WriteLine($"HTTP response ends with CRLF: {httpResponse.EndsWith(crlf)}"); // True

// File signature detection
var pngData = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }; // PNG header
var pngEnd = new byte[] { 0x0A, 0x1A, 0x0A };
Console.WriteLine($"PNG ends with signature: {pngData.EndsWith(pngEnd)}"); // True
```

### IndexOf
Finds the first occurrence of a pattern within a byte array.

#### Syntax
```csharp
public static int IndexOf(this byte[] array, byte[] pattern)
```

#### Example
```csharp
var data = new byte[] { 0x01, 0x02, 0xFF, 0xFE, 0x03, 0xFF, 0xFE, 0x04 };
var pattern = new byte[] { 0xFF, 0xFE };

var firstIndex = data.IndexOf(pattern);
Console.WriteLine($"First occurrence at index: {firstIndex}"); // 2

// Pattern not found
var notFound = new byte[] { 0xAA, 0xBB };
var notFoundIndex = data.IndexOf(notFound);
Console.WriteLine($"Not found index: {notFoundIndex}"); // -1

// Find all occurrences
var allIndices = new List<int>();
var searchArray = data;
var currentIndex = 0;

while (true)
{
    var index = searchArray.IndexOf(pattern);
    if (index == -1) break;

    allIndices.Add(currentIndex + index);
    currentIndex += index + 1;
    searchArray = data.Skip(currentIndex).ToArray();
}

Console.WriteLine($"All occurrences: [{string.Join(", ", allIndices)}]"); // [2, 5]
```

### Advanced Pattern Matching
```csharp
// Protocol delimiter search
public static List<int> FindAllDelimiters(byte[] data, byte[] delimiter)
{
    var positions = new List<int>();
    var position = 0;

    while (position < data.Length)
    {
        var index = data.Skip(position).ToArray().IndexOf(delimiter);
        if (index == -1) break;

        positions.Add(position + index);
        position += index + delimiter.Length;
    }

    return positions;
}

// Usage example
var message = "Field1|Field2|Field3|Field4"u8.ToArray();
var pipe = "|"u8.ToArray();
var delimiters = FindAllDelimiters(message, pipe);
Console.WriteLine($"Pipe positions: [{string.Join(", ", delimiters)}]");
```

## Array Comparison and Equality

### IsIdenticalTo
Checks if two byte arrays are identical in content and length with optimized performance.

#### Syntax
```csharp
public static bool IsIdenticalTo(this byte[]? a1, byte[]? a2)
```

#### Parameters
- `a1`: First byte array (can be null)
- `a2`: Second byte array (can be null)

#### Returns
`true` if arrays are identical; `false` otherwise

#### Example
```csharp
var array1 = new byte[] { 0x01, 0x02, 0x03 };
var array2 = new byte[] { 0x01, 0x02, 0x03 };
var array3 = new byte[] { 0x01, 0x02, 0x04 };

// Content comparison
Console.WriteLine($"array1 identical to array2: {array1.IsIdenticalTo(array2)}"); // True
Console.WriteLine($"array1 identical to array3: {array1.IsIdenticalTo(array3)}"); // False

// Reference equality (optimization)
var sameReference = array1;
Console.WriteLine($"Same reference: {array1.IsIdenticalTo(sameReference)}"); // True (fast path)

// Null handling
byte[]? nullArray = null;
var emptyArray = new byte[0];
Console.WriteLine($"Null vs empty: {nullArray.IsIdenticalTo(emptyArray)}"); // False
Console.WriteLine($"Null vs null: {nullArray.IsIdenticalTo(null)}"); // True

// Length differences
var shortArray = new byte[] { 0x01 };
var longArray = new byte[] { 0x01, 0x02 };
Console.WriteLine($"Different lengths: {shortArray.IsIdenticalTo(longArray)}"); // False
```

#### Performance Characteristics
The method uses several optimizations:
1. **Reference equality check**: O(1) for same reference
2. **Null and length checks**: O(1) early exit conditions
3. **ReadOnlySpan<byte> comparison**: Optimized SIMD operations when available

```csharp
// Performance comparison example
var large1 = new byte[1_000_000];
var large2 = new byte[1_000_000];
new Random(42).NextBytes(large1);
Array.Copy(large1, large2, large1.Length);

var stopwatch = Stopwatch.StartNew();
var isIdentical = large1.IsIdenticalTo(large2);
stopwatch.Stop();

Console.WriteLine($"Large array comparison: {isIdentical} in {stopwatch.ElapsedMicroseconds}μs");
```

## Core Conversion Infrastructure

### ExecuteConversionToType (Internal)
The internal method that powers all type conversion operations in the ByteArrayExtensions family.

#### Key Features
- **Bounds checking**: Validates array size and position
- **Position advancement**: Automatically increments position by type size
- **Error handling**: Provides detailed error messages with array contents
- **Generic support**: Works with any type via BitConverter functions

#### Error Handling Example
```csharp
// This internal method is used by all conversion methods
try
{
    var data = new byte[] { 0x01, 0x02 }; // Too small for int32
    var position = 0;
    var value = data.ToInt32(ref position); // Will throw
}
catch (ArgumentOutOfRangeException ex)
{
    Console.WriteLine(ex.Message);
    // "Array [1,2] is too small. Reading 4 bytes from position 0 is not possible in array of 2"
}
```

## Practical Usage Patterns

### Protocol Header Validation
```csharp
public static bool IsValidProtocolMessage(byte[] data)
{
    // Check minimum length
    if (data.Length < 8) return false;

    // Check magic number
    var magicNumber = new byte[] { 0x12, 0x34, 0x56, 0x78 };
    if (!data.StartsWith(magicNumber)) return false;

    // Check footer
    var footer = new byte[] { 0xFF, 0xFF };
    if (!data.EndsWith(footer)) return false;

    return true;
}

// Usage
var validMessage = new byte[] { 0x12, 0x34, 0x56, 0x78, 0xAA, 0xBB, 0xFF, 0xFF };
var invalidMessage = new byte[] { 0x11, 0x34, 0x56, 0x78, 0xAA, 0xBB, 0xFF, 0xFF };

Console.WriteLine($"Valid: {IsValidProtocolMessage(validMessage)}"); // True
Console.WriteLine($"Invalid: {IsValidProtocolMessage(invalidMessage)}"); // False
```

### Data Integrity Verification
```csharp
public static bool VerifyDataIntegrity(byte[] received, byte[] expected)
{
    // Quick length check
    if (received.Length != expected.Length)
    {
        Console.WriteLine($"Length mismatch: received {received.Length}, expected {expected.Length}");
        return false;
    }

    // Content comparison
    if (!received.IsIdenticalTo(expected))
    {
        Console.WriteLine("Content mismatch detected");
        Console.WriteLine($"Received: {received.ToHexDebugString()}");
        Console.WriteLine($"Expected: {expected.ToHexDebugString()}");
        return false;
    }

    return true;
}
```

### Message Parsing
```csharp
public static List<byte[]> SplitByDelimiter(byte[] data, byte[] delimiter)
{
    var parts = new List<byte[]>();
    var start = 0;

    while (start < data.Length)
    {
        var remaining = data.Skip(start).ToArray();
        var delimiterIndex = remaining.IndexOf(delimiter);

        if (delimiterIndex == -1)
        {
            // Last part (no more delimiters)
            parts.Add(remaining);
            break;
        }

        // Extract part before delimiter
        var part = remaining.Take(delimiterIndex).ToArray();
        parts.Add(part);

        // Move past delimiter
        start += delimiterIndex + delimiter.Length;
    }

    return parts;
}

// Usage example
var csvData = "Name,Age,City\nJohn,30,NYC\nJane,25,LA"u8.ToArray();
var newline = "\n"u8.ToArray();
var lines = SplitByDelimiter(csvData, newline);

foreach (var line in lines)
{
    var lineText = Encoding.UTF8.GetString(line);
    Console.WriteLine($"Line: {lineText}");
}
```

## Debugging and Troubleshooting

### Visual Array Comparison
```csharp
public static void CompareArraysVisually(byte[] array1, byte[] array2, string label1 = "Array1", string label2 = "Array2")
{
    Console.WriteLine($"=== Array Comparison ===");
    Console.WriteLine($"{label1}: {array1.ToHexDebugString()}");
    Console.WriteLine($"{label2}: {array2.ToHexDebugString()}");
    Console.WriteLine($"Identical: {array1.IsIdenticalTo(array2)}");

    if (!array1.IsIdenticalTo(array2))
    {
        var minLength = Math.Min(array1.Length, array2.Length);
        for (int i = 0; i < minLength; i++)
        {
            if (array1[i] != array2[i])
            {
                Console.WriteLine($"First difference at index {i}: 0x{array1[i]:X2} vs 0x{array2[i]:X2}");
                break;
            }
        }

        if (array1.Length != array2.Length)
        {
            Console.WriteLine($"Length difference: {array1.Length} vs {array2.Length}");
        }
    }
}
```

### Pattern Analysis
```csharp
public static void AnalyzePatterns(byte[] data, byte[] pattern)
{
    Console.WriteLine($"=== Pattern Analysis ===");
    Console.WriteLine($"Data: {data.ToHexDebugString()}");
    Console.WriteLine($"Pattern: {pattern.ToHexDebugString()}");

    var firstIndex = data.IndexOf(pattern);
    Console.WriteLine($"First occurrence: {(firstIndex >= 0 ? $"index {firstIndex}" : "not found")}");

    var startsWithPattern = data.StartsWith(pattern);
    var endsWithPattern = data.EndsWith(pattern);

    Console.WriteLine($"Starts with pattern: {startsWithPattern}");
    Console.WriteLine($"Ends with pattern: {endsWithPattern}");

    // Count occurrences
    var count = 0;
    var searchIndex = 0;
    while (searchIndex < data.Length)
    {
        var remaining = data.Skip(searchIndex).ToArray();
        var index = remaining.IndexOf(pattern);
        if (index == -1) break;
        count++;
        searchIndex += index + 1;
    }

    Console.WriteLine($"Total occurrences: {count}");
}
```

## Performance Characteristics

### Time Complexity
- **ToDebugString**: O(n) where n is array length
- **ToHexDebugString**: O(n) where n is array length
- **StartsWith/EndsWith**: O(m) where m is pattern length
- **IndexOf**: O(n×m) where n is array length and m is pattern length
- **IsIdenticalTo**: O(1) for reference equality, O(n) for content comparison

### Memory Usage
- **Debug methods**: Create new strings, memory usage proportional to array size
- **Pattern matching**: Uses ReadOnlySpan for zero-copy operations when possible
- **Comparison**: Minimal allocations due to span-based implementation

### Optimization Tips
```csharp
// Use spans for repeated pattern matching
ReadOnlySpan<byte> dataSpan = largeArray;
ReadOnlySpan<byte> patternSpan = searchPattern;

// More efficient than creating new arrays
for (int i = 0; i <= dataSpan.Length - patternSpan.Length; i++)
{
    if (dataSpan.Slice(i, patternSpan.Length).SequenceEqual(patternSpan))
    {
        // Found match at position i
    }
}
```

## Best Practices

### Error Handling
```csharp
// Always validate inputs for public methods
public static bool SafeStartsWith(byte[]? array, byte[]? pattern)
{
    if (array == null || pattern == null) return false;
    return array.StartsWith(pattern);
}
```

### Performance Optimization
```csharp
// Cache patterns for repeated searches
private static readonly byte[] HttpHeader = "HTTP/"u8.ToArray();
private static readonly byte[] ContentLength = "Content-Length:"u8.ToArray();

public static bool IsHttpResponse(byte[] data)
{
    return data.StartsWith(HttpHeader);
}
```

### Debugging Integration
```csharp
// Use debug methods in conditional compilation
[Conditional("DEBUG")]
public static void LogArrayContents(byte[] data, string context)
{
    Console.WriteLine($"[{context}] {data.ToHexDebugString()}");
}
```

## Cross-References

- <xref:Plugin.ByteArrays.ByteArrayExtensions> - Basic type conversions, String encoding operations and Array slicing and manipulation
- <xref:Plugin.ByteArrays.ByteArrayUtilities> - Advanced analysis and formatting tools
- <xref:Plugin.ByteArrays.ByteArrayBuilder> - Efficient byte array construction
