# ObjectToByteArrayExtensions Class

The `ObjectToByteArrayExtensions` class provides comprehensive extensions for converting objects to byte arrays using various encodings, serialization formats, and specialized operations. This class includes support for JSON serialization, collection handling, memory management, and custom serialization patterns.

## IByteSerializable Interface

### Interface Definition
```csharp
public interface IByteSerializable
{
    byte[] ToBytes();
    void FromBytes(byte[] data);
}
```

### Example Implementation
```csharp
public class CustomData : IByteSerializable
{
    public int Id { get; set; }
    public string Name { get; set; } = "";

    public byte[] ToBytes()
    {
        using var builder = new ByteArrayBuilder();
        return builder
            .Append(Id)
            .AppendUtf8String(Name)
            .ToByteArray();
    }

    public void FromBytes(byte[] data)
    {
        var position = 0;
        Id = data.ToInt32(ref position);
        Name = data.ToUtf8String(ref position, -1);
    }
}

// Usage
var custom = new CustomData { Id = 42, Name = "Test" };
var bytes = custom.ToByteArray();
var restored = new CustomData();
restored.FromBytes(bytes);
```

## String Conversion Methods

### AsciiStringToByteArray
Converts an ASCII string to a byte array.

#### Syntax
```csharp
public static byte[] AsciiStringToByteArray(this string value)
```

#### Example
```csharp
var text = "Hello World";
var asciiBytes = text.AsciiStringToByteArray();

Console.WriteLine($"ASCII bytes: {string.Join(" ", asciiBytes.Select(b => $"0x{b:X2}"))}");
// Output: 0x48 0x65 0x6C 0x6C 0x6F 0x20 0x57 0x6F 0x72 0x6C 0x64

// Handle null/empty strings
string? nullString = null;
var emptyBytes = nullString.AsciiStringToByteArray(); // Returns empty array

var emptyString = "";
var emptyBytes2 = emptyString.AsciiStringToByteArray(); // Returns empty array
```

### Utf8StringToByteArray
Converts a UTF-8 string to a byte array.

#### Syntax
```csharp
public static byte[] Utf8StringToByteArray(this string value)
```

#### Example
```csharp
var text = "Hello 世界"; // Mixed ASCII and Unicode
var utf8Bytes = text.Utf8StringToByteArray();

Console.WriteLine($"UTF-8 bytes: {string.Join(" ", utf8Bytes.Select(b => $"0x{b:X2}"))}");
// ASCII characters are same, Unicode characters use multiple bytes

// Compare with ASCII (will throw for non-ASCII characters)
var asciiSafe = "Hello World";
var asciiBytes = asciiSafe.AsciiStringToByteArray();
var utf8Bytes2 = asciiSafe.Utf8StringToByteArray();
Console.WriteLine($"ASCII and UTF-8 identical: {asciiBytes.SequenceEqual(utf8Bytes2)}"); // True
```

### HexStringToByteArray
Converts a hexadecimal string to a byte array with flexible input format support.

#### Syntax
```csharp
public static byte[] HexStringToByteArray(this string hexString)
```

#### Example
```csharp
// Various hex string formats
var formats = new[]
{
    "DEADBEEF",
    "DE AD BE EF",
    "DE-AD-BE-EF",
    "DE:AD:BE:EF",
    "0xDE 0xAD 0xBE 0xEF"
};

foreach (var format in formats)
{
    var bytes = format.HexStringToByteArray();
    Console.WriteLine($"'{format}' -> {string.Join(" ", bytes.Select(b => $"0x{b:X2}"))}");
    // All produce: 0xDE 0xAD 0xBE 0xEF
}

// Error handling
try
{
    var invalid = "INVALID".HexStringToByteArray(); // Odd number of characters
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
```

### Base64StringToByteArray
Converts a Base64 string to a byte array.

#### Syntax
```csharp
public static byte[] Base64StringToByteArray(this string base64String)
```

#### Example
```csharp
var originalData = "Hello World"u8.ToArray();
var base64 = Convert.ToBase64String(originalData);
Console.WriteLine($"Base64: {base64}");

var decodedBytes = base64.Base64StringToByteArray();
var decodedText = Encoding.UTF8.GetString(decodedBytes);
Console.WriteLine($"Decoded: {decodedText}"); // "Hello World"

// Round-trip verification
Console.WriteLine($"Round-trip successful: {originalData.SequenceEqual(decodedBytes)}");
```

## Generic Type Conversion

### ToByteArray<T>
Converts any supported type to a byte array using the ByteArrayBuilder.

#### Syntax
```csharp
public static byte[] ToByteArray<T>(this T value)
```

#### Example
```csharp
// Primitive types
var intBytes = 42.ToByteArray();
var floatBytes = 3.14f.ToByteArray();
var boolBytes = true.ToByteArray();

// Complex types
var dateBytes = DateTime.Now.ToByteArray();
var guidBytes = Guid.NewGuid().ToByteArray();

// Strings
var stringBytes = "Hello World".ToByteArray(); // UTF-8 encoding

// Arrays
var arrayBytes = new int[] { 1, 2, 3, 4 }.ToByteArray();

// Null handling
string? nullValue = null;
var nullBytes = nullValue.ToByteArray(); // Returns empty array

Console.WriteLine($"Int32 bytes: {intBytes.Length}"); // 4
Console.WriteLine($"DateTime bytes: {dateBytes.Length}"); // 8
```

## JSON Serialization

### ToJsonByteArray
Serializes an object to JSON as a byte array using System.Text.Json.

#### Syntax
```csharp
public static byte[] ToJsonByteArray<T>(this T value, JsonSerializerOptions? options = null)
```

#### Example
```csharp
var person = new { Name = "John", Age = 30, Active = true };
var jsonBytes = person.ToJsonByteArray();

var jsonString = Encoding.UTF8.GetString(jsonBytes);
Console.WriteLine($"JSON: {jsonString}");
// Output: {"Name":"John","Age":30,"Active":true}

// Custom serialization options
var options = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    WriteIndented = true
};

var formattedJsonBytes = person.ToJsonByteArray(options);
var formattedJson = Encoding.UTF8.GetString(formattedJsonBytes);
Console.WriteLine(formattedJson);
```

### FromJsonByteArray
Deserializes a JSON byte array back to an object.

#### Syntax
```csharp
public static T? FromJsonByteArray<T>(this byte[] jsonBytes, JsonSerializerOptions? options = null)
```

#### Example
```csharp
public record Person(string Name, int Age, bool Active);

var original = new Person("Alice", 25, true);
var jsonBytes = original.ToJsonByteArray();
var restored = jsonBytes.FromJsonByteArray<Person>();

Console.WriteLine($"Original: {original}");
Console.WriteLine($"Restored: {restored}");
Console.WriteLine($"Equal: {original.Equals(restored)}"); // True

// Handle deserialization errors
try
{
    var invalidJson = "{ invalid json }"u8.ToArray();
    var result = invalidJson.FromJsonByteArray<Person>();
}
catch (JsonException ex)
{
    Console.WriteLine($"JSON error: {ex.Message}");
}
```

### ToJsonMemory
Creates a ReadOnlyMemory<byte> containing JSON data for span-based operations.

#### Syntax
```csharp
public static ReadOnlyMemory<byte> ToJsonMemory<T>(this T value, JsonSerializerOptions? options = null)
```

#### Example
```csharp
var data = new { Values = new[] { 1, 2, 3 }, Name = "Test" };
var jsonMemory = data.ToJsonMemory();

// Use with span operations
var jsonSpan = jsonMemory.Span;
Console.WriteLine($"JSON length: {jsonSpan.Length}");

// Convert to string for display
var jsonString = Encoding.UTF8.GetString(jsonSpan);
Console.WriteLine($"JSON: {jsonString}");

// Memory-efficient processing
using var stream = new MemoryStream();
stream.Write(jsonSpan);
```

## Collection Serialization

### Collection ToByteArray
Serializes collections with length-prefixed items.

#### Syntax
```csharp
public static byte[] ToByteArray<T>(this IEnumerable<T> collection, Func<T, byte[]> itemSerializer)
```

#### Example
```csharp
var numbers = new[] { 42, 123, 456 };

// Serialize with custom serializer
var serializedNumbers = numbers.ToByteArray(n => n.ToByteArray());

Console.WriteLine($"Serialized length: {serializedNumbers.Length}");
// Format: [count:4][len:4][data:4][len:4][data:4][len:4][data:4]

// Serialize strings
var strings = new[] { "Hello", "World", "Test" };
var serializedStrings = strings.ToByteArray(s => s.Utf8StringToByteArray());

// Complex objects
var people = new[]
{
    new { Name = "Alice", Age = 30 },
    new { Name = "Bob", Age = 25 }
};
var serializedPeople = people.ToByteArray(p => p.ToJsonByteArray());
```

### FromByteArrayToList
Deserializes a byte array back to a list of objects.

#### Syntax
```csharp
public static IList<T> FromByteArrayToList<T>(this byte[] data, Func<byte[], T> itemDeserializer)
```

#### Example
```csharp
// Deserialize numbers
var originalNumbers = new[] { 42, 123, 456 };
var serialized = originalNumbers.ToByteArray(n => n.ToByteArray());
var restored = serialized.FromByteArrayToList(bytes => bytes.ToInt32());

Console.WriteLine($"Original: [{string.Join(", ", originalNumbers)}]");
Console.WriteLine($"Restored: [{string.Join(", ", restored)}]");
Console.WriteLine($"Equal: {originalNumbers.SequenceEqual(restored)}");

// Round-trip with strings
var originalStrings = new[] { "Hello", "World", "Test" };
var stringsSerialized = originalStrings.ToByteArray(s => s.Utf8StringToByteArray());
var stringsRestored = stringsSerialized.FromByteArrayToList(bytes => bytes.ToUtf8String());

Console.WriteLine($"Strings equal: {originalStrings.SequenceEqual(stringsRestored)}");
```

### Dictionary Serialization
Serializes dictionaries with key-value pairs.

#### Syntax
```csharp
public static byte[] ToByteArray<TKey, TValue>(
    this IDictionary<TKey, TValue> dictionary,
    Func<TKey, byte[]> keySerializer,
    Func<TValue, byte[]> valueSerializer)
```

#### Example
```csharp
var settings = new Dictionary<string, object>
{
    ["MaxRetries"] = 5,
    ["Timeout"] = 30.0,
    ["EnableLogging"] = true,
    ["ServiceUrl"] = "https://api.example.com"
};

var serialized = settings.ToByteArray(
    key => key.Utf8StringToByteArray(),
    value => value.ToJsonByteArray()
);

Console.WriteLine($"Dictionary serialized to {serialized.Length} bytes");

// Custom strongly-typed dictionary
var scores = new Dictionary<string, int>
{
    ["Alice"] = 95,
    ["Bob"] = 87,
    ["Charlie"] = 92
};

var scoresSerialized = scores.ToByteArray(
    name => name.Utf8StringToByteArray(),
    score => score.ToByteArray()
);
```

## Span and Memory Operations

### ReadOnlySpan ToByteArray
Converts a span to a byte array.

#### Syntax
```csharp
public static byte[] ToByteArray(this ReadOnlySpan<byte> span)
public static byte[] ToByteArray(this ReadOnlyMemory<byte> memory)
```

#### Example
```csharp
var originalArray = new byte[] { 1, 2, 3, 4, 5 };

// Create spans
var span = originalArray.AsSpan(1, 3); // [2, 3, 4]
var spanArray = span.ToByteArray();

var memory = originalArray.AsMemory(2, 2); // [3, 4]
var memoryArray = memory.ToByteArray();

Console.WriteLine($"Original: [{string.Join(", ", originalArray)}]");
Console.WriteLine($"Span: [{string.Join(", ", spanArray)}]");
Console.WriteLine($"Memory: [{string.Join(", ", memoryArray)}]");
```

### TryWriteToSpan
Writes an object to a span using a custom serializer.

#### Syntax
```csharp
public static int TryWriteToSpan<T>(this T value, Span<byte> destination, Func<T, Span<byte>, int> serializer)
```

#### Example
```csharp
// Custom serializer for a point structure
static int SerializePoint((int X, int Y) point, Span<byte> dest)
{
    if (dest.Length < 8) return 0;

    BitConverter.TryWriteBytes(dest[0..4], point.X);
    BitConverter.TryWriteBytes(dest[4..8], point.Y);
    return 8;
}

var point = (X: 100, Y: 200);
var buffer = new byte[16];
var bytesWritten = point.TryWriteToSpan(buffer, SerializePoint);

Console.WriteLine($"Bytes written: {bytesWritten}");
Console.WriteLine($"Buffer: {string.Join(" ", buffer.Take(bytesWritten).Select(b => $"0x{b:X2}"))}");
```

### ArrayPool Integration
Efficient memory management using ArrayPool.

#### Syntax
```csharp
public static (byte[] buffer, int length) ToRentedBuffer<T>(
    this T value,
    Func<T, byte[]> serializer,
    int minimumLength = 1024)

public static void ReturnRentedBuffer(byte[] buffer, bool clearArray = false)
```

#### Example
```csharp
var largeObject = new { Data = Enumerable.Range(0, 1000).ToArray() };

// Rent buffer for serialization
var (buffer, length) = largeObject.ToRentedBuffer(obj => obj.ToJsonByteArray(), 4096);

try
{
    // Use buffer
    var actualData = buffer.AsSpan(0, length);
    Console.WriteLine($"Serialized {length} bytes into rented buffer of {buffer.Length}");

    // Process the data
    ProcessSerializedData(actualData);
}
finally
{
    // Always return the buffer
    ObjectToByteArrayExtensions.ReturnRentedBuffer(buffer, clearArray: true);
}

static void ProcessSerializedData(ReadOnlySpan<byte> data)
{
    // Process without additional allocations
    Console.WriteLine($"Processing {data.Length} bytes");
}
```

## Custom Serialization Support

### IByteSerializable ToByteArray
Serializes objects implementing IByteSerializable.

#### Example
```csharp
public class NetworkPacket : IByteSerializable
{
    public byte PacketType { get; set; }
    public ushort PacketId { get; set; }
    public byte[] Payload { get; set; } = Array.Empty<byte>();

    public byte[] ToBytes()
    {
        using var builder = new ByteArrayBuilder();
        return builder
            .Append(PacketType)
            .Append(PacketId)
            .Append((ushort)Payload.Length)
            .Append(Payload)
            .ToByteArray();
    }

    public void FromBytes(byte[] data)
    {
        var position = 0;
        PacketType = data.ToByte(ref position);
        PacketId = data.ToUInt16(ref position);
        var payloadLength = data.ToUInt16(ref position);
        Payload = data.Skip(position).Take(payloadLength).ToArray();
    }
}

// Usage
var packet = new NetworkPacket
{
    PacketType = 0x01,
    PacketId = 1234,
    Payload = "Hello"u8.ToArray()
};

var bytes = packet.ToByteArray(); // Uses IByteSerializable implementation
```

### Custom Serializer Function
Use custom serialization logic.

#### Syntax
```csharp
public static byte[] ToByteArray<T>(this T value, Func<T, byte[]> customSerializer)
```

#### Example
```csharp
// Custom serializer for DateTime as Unix timestamp
static byte[] SerializeDateTimeAsUnix(DateTime dt)
{
    var unixTimestamp = ((DateTimeOffset)dt).ToUnixTimeSeconds();
    return BitConverter.GetBytes(unixTimestamp);
}

var now = DateTime.Now;
var customBytes = now.ToByteArray(SerializeDateTimeAsUnix);
Console.WriteLine($"Custom DateTime serialization: {customBytes.Length} bytes");

// Compare with default serialization
var defaultBytes = now.ToByteArray();
Console.WriteLine($"Default DateTime serialization: {defaultBytes.Length} bytes");
```

### Cached Serializer
Creates a caching serializer for expensive operations.

#### Syntax
```csharp
public static Func<T, byte[]> CreateCachedSerializer<T>(Func<T, byte[]> baseSerializer, int maxCacheSize = 100)
    where T : notnull
```

#### Example
```csharp
// Expensive serialization function (simulated)
static byte[] ExpensiveSerializer(ComplexObject obj)
{
    Thread.Sleep(10); // Simulate expensive operation
    return obj.ToJsonByteArray();
}

// Create cached version
var cachedSerializer = ObjectToByteArrayExtensions.CreateCachedSerializer<ComplexObject>(
    ExpensiveSerializer, maxCacheSize: 50);

var objects = Enumerable.Range(0, 100).Select(i => new ComplexObject { Id = i % 20 }).ToList();

// Measure performance
var stopwatch = Stopwatch.StartNew();
foreach (var obj in objects)
{
    var bytes = cachedSerializer(obj); // Will cache results for repeated objects
}
stopwatch.Stop();

Console.WriteLine($"Cached serialization took: {stopwatch.ElapsedMilliseconds}ms");
// Significant speedup due to caching for repeated objects
```

## Advanced Usage Patterns

### Polymorphic Serialization
```csharp
public abstract class Shape : IByteSerializable
{
    public abstract byte ShapeType { get; }
    public abstract byte[] ToBytes();
    public abstract void FromBytes(byte[] data);

    public static Shape FromTypeAndBytes(byte shapeType, byte[] data)
    {
        Shape shape = shapeType switch
        {
            1 => new Circle(),
            2 => new Rectangle(),
            _ => throw new NotSupportedException($"Shape type {shapeType} not supported")
        };

        shape.FromBytes(data);
        return shape;
    }
}

public class Circle : Shape
{
    public override byte ShapeType => 1;
    public double Radius { get; set; }

    public override byte[] ToBytes()
    {
        using var builder = new ByteArrayBuilder();
        return builder.Append(ShapeType).Append(Radius).ToByteArray();
    }

    public override void FromBytes(byte[] data)
    {
        var position = 1; // Skip type byte
        Radius = data.ToDouble(ref position);
    }
}
```

### Versioned Serialization
```csharp
public class VersionedData : IByteSerializable
{
    public const byte CurrentVersion = 2;

    public string Name { get; set; } = "";
    public int Value { get; set; }
    public DateTime? CreatedAt { get; set; } // Added in version 2

    public byte[] ToBytes()
    {
        using var builder = new ByteArrayBuilder();
        return builder
            .Append(CurrentVersion)
            .AppendUtf8String(Name)
            .Append(Value)
            .Append(CreatedAt?.ToBinary() ?? 0)
            .ToByteArray();
    }

    public void FromBytes(byte[] data)
    {
        var position = 0;
        var version = data.ToByte(ref position);

        Name = data.ToUtf8String(ref position);
        Value = data.ToInt32(ref position);

        if (version >= 2)
        {
            var ticks = data.ToInt64(ref position);
            CreatedAt = ticks != 0 ? DateTime.FromBinary(ticks) : null;
        }
    }
}
```

## Performance Characteristics

### Time Complexity
- **String conversions**: O(n) where n is string length
- **JSON serialization**: O(n) where n is object complexity
- **Collection serialization**: O(k×m) where k is item count and m is average item size
- **Cached serialization**: O(1) for cache hits, O(n) for cache misses

### Memory Efficiency
- **Span operations**: Zero-copy when possible
- **ArrayPool integration**: Reduces garbage collection pressure
- **Streaming serialization**: Constant memory usage for large data

## Best Practices

### Choose Appropriate Serialization
```csharp
// For interoperability: JSON
var jsonBytes = obj.ToJsonByteArray();

// For performance: Custom binary
var binaryBytes = obj.ToByteArray(CustomBinarySerializer);

// For .NET specific: Built-in types
var nativeBytes = obj.ToByteArray(); // Uses ByteArrayBuilder
```

### Error Handling
```csharp
try
{
    var bytes = complexObject.ToJsonByteArray();
    var restored = bytes.FromJsonByteArray<ComplexObject>();
}
catch (JsonException ex)
{
    // Handle JSON serialization errors
    Console.WriteLine($"JSON error: {ex.Message}");
}
catch (ArgumentException ex)
{
    // Handle conversion errors
    Console.WriteLine($"Conversion error: {ex.Message}");
}
```

### Memory Management
```csharp
// Use ArrayPool for large operations
var (buffer, length) = largeData.ToRentedBuffer(serialize, 8192);
try
{
    // Use buffer
}
finally
{
    ObjectToByteArrayExtensions.ReturnRentedBuffer(buffer, true);
}

// Use spans for zero-copy operations
ReadOnlySpan<byte> span = existingArray;
var newArray = span.ToByteArray(); // Only when necessary
```

## Cross-References

- <xref:Plugin.ByteArrays.ByteArrayBuilder> - Fluent byte array construction
- <xref:Plugin.ByteArrays.ByteArrayExtensions> - Core conversion operations
- <xref:Plugin.ByteArrays.ByteArrayAsyncExtensions> - Asynchronous serialization
- <xref:Plugin.ByteArrays.ByteArrayCompressionExtensions> - Compression support
- <xref:Plugin.ByteArrays.ByteArrayUtilities> - Analysis and debugging tools
