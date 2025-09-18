# ByteArrayBuilder

This documentation covers the `ByteArrayBuilder` class, a fluent builder for constructing byte arrays from various data types. The builder provides a convenient, efficient, and type-safe way to create binary data structures.

## Overview

`ByteArrayBuilder` is a powerful utility class that offers:
- **Fluent API** - Method chaining for readable code
- **Type safety** - Compile-time type checking for all operations
- **Performance** - Efficient memory management with underlying MemoryStream
- **Flexibility** - Support for all .NET primitive types and complex data
- **Encoding support** - Multiple string encodings and formats
- **Endianness control** - Big-endian and little-endian byte order support

## Basic Usage

### Constructor and Factory Methods
<xref:Plugin.ByteArrays.ByteArrayBuilder>

Creates a new ByteArrayBuilder instance with optional initial capacity.

#### Usage Examples

```csharp
// Basic constructor
using var builder = new ByteArrayBuilder();

// With initial capacity for performance
using var optimizedBuilder = new ByteArrayBuilder(1024); // 1KB initial capacity

// Factory method for clarity
using var capacityBuilder = ByteArrayBuilder.WithCapacity(2048); // 2KB initial capacity

// Check properties
Console.WriteLine($"Length: {builder.Length}"); // 0
Console.WriteLine($"Capacity: {builder.Capacity}"); // Default capacity
```

### Building and Retrieving Arrays

#### Usage Examples

```csharp
using var builder = new ByteArrayBuilder();

// Build the array
builder.Append((byte)1)
       .Append((short)2)
       .Append(3)
       .Append(4L);

// Get the result
byte[] result = builder.ToByteArray();
Console.WriteLine($"Built array with {result.Length} bytes");

// With size validation
byte[] validated = builder.ToByteArray(maxSize: 1000); // Throws if > 1000 bytes

// Clear and reuse
builder.Clear();
Console.WriteLine($"After clear: {builder.Length} bytes"); // 0
```

## Type Support

### Primitive Types
<xref:Plugin.ByteArrays.ByteArrayBuilder.Append*>

The builder supports all .NET primitive types through the generic `Append<T>()` method.

#### Usage Examples

```csharp
using var builder = new ByteArrayBuilder();

// Numeric types
builder.Append((byte)255)           // 1 byte
       .Append((sbyte)-128)         // 1 byte
       .Append((short)32767)        // 2 bytes
       .Append((ushort)65535)       // 2 bytes
       .Append(-2147483648)         // 4 bytes (int)
       .Append(4294967295u)         // 4 bytes (uint)
       .Append(-9223372036854775808L) // 8 bytes (long)
       .Append(18446744073709551615UL); // 8 bytes (ulong)

// Floating-point types
builder.Append(3.14159f)            // 4 bytes (float)
       .Append(2.718281828459045)   // 8 bytes (double)
       .Append((Half)1.5f)          // 2 bytes (Half)
       .Append(123.456789m);        // 16 bytes (decimal)

// Other primitives
builder.Append(true)                // 1 byte (bool)
       .Append('A')                 // 2 bytes (char)
       .Append(new byte[] { 1, 2, 3, 4 }); // Raw bytes

byte[] result = builder.ToByteArray();
Console.WriteLine($"Total bytes: {result.Length}");
```

### Enum Support

#### Usage Examples

```csharp
public enum FileType : byte { Text = 1, Binary = 2, Image = 3 }
public enum Permissions : int { Read = 1, Write = 2, Execute = 4, Delete = 8 }

using var builder = new ByteArrayBuilder();

// Enums are converted based on their underlying type
builder.Append(FileType.Image)              // 1 byte
       .Append(Permissions.Read | Permissions.Write); // 4 bytes

// Complex enums
[Flags]
public enum Features : long
{
    None = 0,
    Encryption = 1L << 32,
    Compression = 1L << 33,
    All = Encryption | Compression
}

builder.Append(Features.All); // 8 bytes

byte[] result = builder.ToByteArray();
```

### Collections Support

#### Usage Examples

```csharp
using var builder = new ByteArrayBuilder();

// Byte collections
var byteList = new List<byte> { 10, 20, 30, 40 };
var byteArray = new byte[] { 50, 60, 70, 80 };

builder.Append(byteList)    // IEnumerable<byte>
       .Append(byteArray);  // byte[]

// Custom byte enumerable
IEnumerable<byte> GetBytes()
{
    for (byte i = 100; i < 110; i++)
        yield return i;
}

builder.Append(GetBytes());

byte[] result = builder.ToByteArray();
```

## String Encodings

### Standard Encodings
<xref:Plugin.ByteArrays.ByteArrayBuilder.AppendUtf8String*>
<xref:Plugin.ByteArrays.ByteArrayBuilder.AppendAsciiString*>
<xref:Plugin.ByteArrays.ByteArrayBuilder.AppendUtf16String*>
<xref:Plugin.ByteArrays.ByteArrayBuilder.AppendUtf32String*>

#### Usage Examples

```csharp
using var builder = new ByteArrayBuilder();

// Standard encodings
builder.AppendUtf8String(\"Hello, 世界!\")     // UTF-8 (variable length)
       .AppendAsciiString(\"ASCII Text\")      // ASCII (1 byte per char)
       .AppendUtf16String(\"UTF-16 Text\")     // UTF-16 (2 bytes per char)
       .AppendUtf32String(\"UTF-32 Text\");    // UTF-32 (4 bytes per char)

// Custom encoding
builder.AppendString(\"Custom Text\", Encoding.Latin1);

byte[] result = builder.ToByteArray();
Console.WriteLine($\"String data: {result.Length} bytes\");
```

### Protocol String Formats
<xref:Plugin.ByteArrays.ByteArrayBuilder.AppendLengthPrefixedString*>
<xref:Plugin.ByteArrays.ByteArrayBuilder.AppendNullTerminatedString*>

#### Usage Examples

```csharp
using var builder = new ByteArrayBuilder();

// Length-prefixed strings (common in binary protocols)
builder.AppendLengthPrefixedString(\"Username\", Encoding.UTF8);
// Result: [8, 0, 'U', 's', 'e', 'r', 'n', 'a', 'm', 'e']
//         |length|      string content

builder.AppendLengthPrefixedString(\"Password123\", Encoding.UTF8);

// Null-terminated strings (C-style strings)
builder.AppendNullTerminatedString(\"Config\", Encoding.ASCII);
// Result: ['C', 'o', 'n', 'f', 'i', 'g', 0]
//          string content              |null|

// Network protocol example
builder.AppendByte(1)  // Protocol version
       .AppendLengthPrefixedString(\"GET /api/data\", Encoding.ASCII)
       .AppendNullTerminatedString(\"Host: example.com\", Encoding.ASCII);

byte[] protocolMessage = builder.ToByteArray();
```

### Encoded String Formats
<xref:Plugin.ByteArrays.ByteArrayBuilder.AppendHexString*>
<xref:Plugin.ByteArrays.ByteArrayBuilder.AppendBase64String*>

#### Usage Examples

```csharp
using var builder = new ByteArrayBuilder();

// Hexadecimal strings
builder.AppendHexString(\"DEADBEEF\");        // 4 bytes: [0xDE, 0xAD, 0xBE, 0xEF]
builder.AppendHexString(\"48656C6C6F\");      // \"Hello\" as hex

// Base64 strings
builder.AppendBase64String(\"SGVsbG8=\");      // \"Hello\" in Base64

// Configuration data parsing
string configHex = GetConfigurationHex();     // \"1A2B3C4D\"
string tokenBase64 = GetSecurityToken();      // Base64 token

builder.AppendHexString(configHex)
       .AppendBase64String(tokenBase64);

byte[] configData = builder.ToByteArray();
```

## Endianness Control

### Big-Endian (Network Byte Order)
<xref:Plugin.ByteArrays.ByteArrayBuilder.AppendBigEndian*>

#### Usage Examples

```csharp
using var builder = new ByteArrayBuilder();

// Network protocols typically use big-endian
builder.AppendBigEndian((short)8080)     // Port number
       .AppendBigEndian(0x12345678)      // Message ID
       .AppendBigEndian(1234567890L);    // Timestamp

// TCP header example
builder.AppendBigEndian((short)12345)    // Source port
       .AppendBigEndian((short)80)       // Destination port (HTTP)
       .AppendBigEndian(0x12345678)      // Sequence number
       .AppendBigEndian(0x87654321);     // Acknowledgment number

byte[] networkPacket = builder.ToByteArray();
```

### Little-Endian (Intel Byte Order)
<xref:Plugin.ByteArrays.ByteArrayBuilder.AppendLittleEndian*>

#### Usage Examples

```csharp
using var builder = new ByteArrayBuilder();

// File formats often use little-endian
builder.AppendLittleEndian((short)0x4D42)  // BMP file signature \"BM\"
       .AppendLittleEndian(1024)           // File size
       .AppendLittleEndian(54);            // Header size

// Database record format
builder.AppendLittleEndian(12345L)         // Record ID
       .AppendLittleEndian(DateTime.Now.Ticks) // Timestamp
       .AppendLittleEndian(100);           // Data length

byte[] fileHeader = builder.ToByteArray();
```

## Advanced Data Types

### DateTime and TimeSpan
<xref:Plugin.ByteArrays.ByteArrayBuilder.Append*>

#### Usage Examples

```csharp
using var builder = new ByteArrayBuilder();

// Temporal data
var now = DateTime.Now;
var duration = TimeSpan.FromHours(2.5);
var offset = new DateTimeOffset(now, TimeSpan.FromHours(-5));

builder.Append(now)           // 8 bytes (DateTime binary format)
       .Append(duration)      // 8 bytes (TimeSpan ticks)
       .Append(offset);       // 16 bytes (DateTime + offset)

// Unix timestamp format
var unixTime = ((DateTimeOffset)now).ToUnixTimeSeconds();
builder.Append((int)unixTime); // 4 bytes Unix timestamp

// Log entry example
builder.Append(DateTime.UtcNow)
       .AppendUtf8String(\"INFO\")
       .AppendNullTerminatedString(\"Application started\", Encoding.UTF8);

byte[] logEntry = builder.ToByteArray();
```

### GUID Support
<xref:Plugin.ByteArrays.ByteArrayBuilder.Append*>

#### Usage Examples

```csharp
using var builder = new ByteArrayBuilder();

var sessionId = Guid.NewGuid();
var requestId = Guid.NewGuid();

builder.Append(sessionId)     // 16 bytes
       .Append(requestId);    // 16 bytes

// Protocol message with GUIDs
builder.AppendByte(1)         // Message version
       .Append(sessionId)     // Session identifier
       .Append(requestId)     // Request identifier
       .AppendLengthPrefixedString(\"API_CALL\", Encoding.ASCII);

byte[] message = builder.ToByteArray();
```

### Network Types
<xref:Plugin.ByteArrays.ByteArrayBuilder.Append*>

#### Usage Examples

```csharp
using var builder = new ByteArrayBuilder();

var serverIP = IPAddress.Parse(\"192.168.1.100\");
var endpoint = new IPEndPoint(serverIP, 8080);

// Network configuration data
builder.Append(serverIP.GetAddressBytes())  // 4 bytes for IPv4
       .AppendBigEndian((short)8080);       // 2 bytes port (network order)

// Complete endpoint
builder.Append(endpoint.Address.GetAddressBytes())
       .AppendBigEndian((short)endpoint.Port);

byte[] networkConfig = builder.ToByteArray();
```

## Performance Optimization

### Capacity Management

#### Usage Examples

```csharp
// Pre-allocate capacity for known data size
var estimatedSize = 1000; // Estimate based on data structure
using var builder = ByteArrayBuilder.WithCapacity(estimatedSize);

// Build data without reallocations
for (int i = 0; i < 100; i++)
{
    builder.Append(i)                    // 4 bytes
           .AppendUtf8String($\"Item{i}\") // Variable length
           .Append(DateTime.Now);        // 8 bytes
}

Console.WriteLine($\"Capacity used: {builder.Length}/{builder.Capacity}\");
byte[] result = builder.ToByteArray();
```

### Bulk Operations

#### Usage Examples

```csharp
using var builder = new ByteArrayBuilder(10000); // Large initial capacity

// Efficient bulk data appending
var dataPoints = Enumerable.Range(1, 1000)
    .Select(i => new DataPoint { Id = i, Value = i * 1.5, Timestamp = DateTime.Now })
    .ToList();

foreach (var point in dataPoints)
{
    builder.Append(point.Id)
           .Append(point.Value)
           .Append(point.Timestamp);
}

byte[] bulkData = builder.ToByteArray();

public struct DataPoint
{
    public int Id { get; set; }
    public double Value { get; set; }
    public DateTime Timestamp { get; set; }
}
```

## Real-World Usage Patterns

### Binary File Format Creation

#### Usage Examples

```csharp
// Create a custom binary file format
using var builder = new ByteArrayBuilder();

// File header
builder.AppendAsciiString(\"MYFORMAT\")      // 8-byte signature
       .Append((short)1)                    // Version
       .Append((short)0)                    // Flags
       .Append(DateTime.Now)                // Creation time
       .Append(Guid.NewGuid());             // File ID

// Data section
var records = GetDataRecords();
builder.Append(records.Count);              // Record count

foreach (var record in records)
{
    builder.Append(record.Id)
           .AppendLengthPrefixedString(record.Name, Encoding.UTF8)
           .Append(record.Value)
           .Append(record.Flags);
}

// Write to file
byte[] fileData = builder.ToByteArray();
File.WriteAllBytes(\"data.myformat\", fileData);
```

### Network Protocol Implementation

#### Usage Examples

```csharp
// HTTP/2-style protocol frame
using var builder = new ByteArrayBuilder();

string payload = GetPayloadData();
var payloadBytes = Encoding.UTF8.GetBytes(payload);

// Frame header (network byte order)
builder.AppendBigEndian(payloadBytes.Length)  // 4 bytes: payload length
       .AppendByte(0x01)                      // 1 byte: frame type (DATA)
       .AppendByte(0x00)                      // 1 byte: flags
       .AppendBigEndian(12345);               // 4 bytes: stream ID

// Frame payload
builder.Append(payloadBytes);

byte[] frame = builder.ToByteArray();
SendOverNetwork(frame);
```

### Database Record Serialization

#### Usage Examples

```csharp
// Serialize database records to binary format
using var builder = new ByteArrayBuilder();

var users = GetUsers();

// Header
builder.AppendAsciiString(\"USERDB\")         // 6-byte signature
       .Append((short)2)                     // Schema version
       .Append(users.Count);                 // Record count

// Records
foreach (var user in users)
{
    builder.Append(user.Id)                              // 8 bytes
           .AppendLengthPrefixedString(user.Username, Encoding.UTF8)
           .AppendLengthPrefixedString(user.Email, Encoding.UTF8)
           .Append(user.CreatedAt)                       // 8 bytes
           .Append(user.IsActive)                        // 1 byte
           .Append(user.LastLoginAt ?? DateTime.MinValue); // 8 bytes
}

byte[] databaseDump = builder.ToByteArray();
```

### Configuration File Generation

#### Usage Examples

```csharp
// Generate binary configuration file
using var builder = new ByteArrayBuilder();

var config = GetApplicationConfig();

// Configuration header
builder.AppendAsciiString(\"CONFIG\")         // Signature
       .Append((byte)1)                      // Version
       .Append(config.Settings.Count);      // Setting count

// Settings
foreach (var setting in config.Settings)
{
    builder.AppendLengthPrefixedString(setting.Key, Encoding.UTF8);

    switch (setting.Value)
    {
        case string str:
            builder.AppendByte(1) // Type: string
                   .AppendLengthPrefixedString(str, Encoding.UTF8);
            break;
        case int integer:
            builder.AppendByte(2) // Type: int
                   .Append(integer);
            break;
        case bool boolean:
            builder.AppendByte(3) // Type: bool
                   .Append(boolean);
            break;
        case double dbl:
            builder.AppendByte(4) // Type: double
                   .Append(dbl);
            break;
    }
}

byte[] configData = builder.ToByteArray();
File.WriteAllBytes(\"app.config\", configData);
```

## Error Handling and Validation

### Size Validation

#### Usage Examples

```csharp
using var builder = new ByteArrayBuilder();

try
{
    // Build data with size constraints
    BuildLargeDataStructure(builder);

    // Validate size before using
    byte[] result = builder.ToByteArray(maxSize: 1024 * 1024); // 1MB limit
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($\"Data too large: {ex.Message}\");
    // Handle oversized data
}

void BuildLargeDataStructure(ByteArrayBuilder builder)
{
    // Build potentially large data structure
    var largeString = new string('A', 2000000); // 2MB string
    builder.AppendUtf8String(largeString);
}
```

### Safe Building Patterns

#### Usage Examples

```csharp
// Safe building with validation
using var builder = new ByteArrayBuilder();

try
{
    BuildProtocolMessage(builder, GetMessageData());
    byte[] message = builder.ToByteArray();
    ValidateMessage(message);
}
catch (ArgumentException ex)
{
    Console.WriteLine($\"Invalid message data: {ex.Message}\");
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($\"Message building failed: {ex.Message}\");
}

void BuildProtocolMessage(ByteArrayBuilder builder, MessageData data)
{
    if (string.IsNullOrEmpty(data.Content))
        throw new ArgumentException(\"Message content cannot be empty\");

    if (data.Content.Length > 1000)
        throw new ArgumentException(\"Message content too long\");

    builder.AppendByte(data.Type)
           .AppendLengthPrefixedString(data.Content, Encoding.UTF8)
           .Append(data.Timestamp);
}
```

## Memory Management

### Disposal Patterns

#### Usage Examples

```csharp
// Automatic disposal with using statement
using var builder = new ByteArrayBuilder();
builder.Append(\"data\");
byte[] result = builder.ToByteArray();
// Automatically disposed at end of scope

// Async disposal
await using var asyncBuilder = new ByteArrayBuilder();
asyncBuilder.Append(\"async data\");
byte[] asyncResult = asyncBuilder.ToByteArray();
// Automatically disposed asynchronously

// Manual disposal
var manualBuilder = new ByteArrayBuilder();
try
{
    manualBuilder.Append(\"manual data\");
    byte[] manualResult = manualBuilder.ToByteArray();
}
finally
{
    manualBuilder.Dispose(); // Manual cleanup
}
```

### Reuse Patterns

#### Usage Examples

```csharp
using var builder = new ByteArrayBuilder(1024);

// Build multiple messages with same builder
var messages = new List<byte[]>();

for (int i = 0; i < 10; i++)
{
    builder.Clear(); // Reset for reuse

    builder.AppendByte((byte)i)
           .AppendUtf8String($\"Message {i}\")
           .Append(DateTime.Now);

    messages.Add(builder.ToByteArray());
}

Console.WriteLine($\"Built {messages.Count} messages\");
```

## Performance Characteristics

| Operation | Complexity | Notes |
|-----------|------------|-------|
| `Append<T>()` | O(1) amortized | May trigger array resize |
| `AppendString()` | O(n) | n = string length |
| `ToByteArray()` | O(n) | n = total length, creates copy |
| `Clear()` | O(1) | Resets position, keeps capacity |

## Best Practices

- **Pre-allocate capacity** when final size is known or estimable
- **Use specific append methods** for better type safety and performance
- **Prefer using statements** for automatic resource cleanup
- **Validate input data** before building to catch errors early
- **Reuse builders** for multiple similar operations
- **Consider endianness** for cross-platform or network data
- **Use length-prefixed strings** for reliable protocol parsing

## Common Use Cases

1. **Binary File Formats**: Creating custom file headers and data structures
2. **Network Protocols**: Building protocol messages and packets
3. **Database Serialization**: Converting objects to binary storage format
4. **Configuration Files**: Generating binary configuration data
5. **Performance Logging**: Efficient binary log format creation
6. **Cryptographic Operations**: Building key material and certificate data
7. **Game Development**: Creating binary asset and save file formats
8. **Embedded Systems**: Generating firmware data and communication protocols

## See Also

- <xref:Plugin.ByteArrays.ByteArrayExtensions> - For reading the built byte arrays
- <xref:Plugin.ByteArrays.ObjectToByteArrayExtensions> - For object serialization
- <xref:Plugin.ByteArrays.ByteArrayUtilities> - For analysis and validation
- <xref:Plugin.ByteArrays.ByteArrayCompressionExtensions> - For compressing built data
