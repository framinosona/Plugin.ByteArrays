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

### Modern High-Performance Usage (NEW! ✨)

```csharp
using Plugin.ByteArrays;

// Zero-allocation operations with ReadOnlySpan<byte>
ReadOnlySpan<byte> span = stackalloc byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
var position = 0;

// Read values without heap allocations
bool flag = span.ToBoolean(ref position);
int number = span.ToInt32(ref position);
float value = span.ToSingle(ref position);

// Pattern matching
ReadOnlySpan<byte> pattern = new byte[] { 3, 4, 5 };
bool contains = span.IndexOf(pattern) >= 0;

// Analysis utilities
double entropy = span.CalculateEntropy();
bool allZeros = span.IsAllZeros();

// Async-compatible with ReadOnlyMemory<byte>
async Task ProcessDataAsync(ReadOnlyMemory<byte> memory)
{
    await Task.Delay(100);
    var value = memory.Span.ToInt32(); // Convert to span for operations
}
```

---

## 📚 Core Features

### 🔧 ByteArrayBuilder

Build byte arrays fluently from various data types:

- Primitives (int, float, bool, etc.)
- Strings (UTF-8, ASCII, Hex, Base64)
- Date/Time types (DateTime, TimeSpan, DateTimeOffset)
- Network types (IP addresses, endpoints)
- Enums and custom objects
- Efficient memory management with IDisposable

### 🔍 Reading Operations

Extract data from byte arrays safely:

- **Primitives**: `ToBoolean`, `ToByte`, `ToInt32`, `ToDouble`, etc.
- **Strings**: `ToUtf8String`, `ToAsciiString`, `ToHexString`
- **Date & Time**: `ToDateTime`, `ToTimeSpan`, `ToDateTimeFromUnixTimestamp`
- **Network Types**: `ToIPAddress`, `ToIPEndPoint`, big-endian conversions
- **Complex Types**: `ToEnum<T>`, `ToVersion`, `ToGuid`
- **Safe Variants**: `OrDefault` methods that never throw exceptions

### ⚙️ Array Manipulation

Powerful array operations:

- **Slicing**: `SafeSlice` for bounds-safe array segments
- **Concatenation**: `Concatenate` multiple arrays efficiently
- **Trimming**: `TrimEnd` and `TrimEndNonDestructive`
- **Transformation**: `Reverse`, `Xor` operations
- **Analysis**: `CalculateEntropy`, `AnalyzeDistribution`

### 🔎 Pattern Matching

Search and compare byte arrays:

- **Pattern Search**: `StartsWith`, `EndsWith`, `IndexOf`
- **Equality**: `IsIdenticalTo` with optimized comparisons
- **Format Conversion**: Hex and Base64 utilities

### 🌐 Network & Protocol Support

Comprehensive networking capabilities:

- **IP Addresses**: IPv4 and IPv6 conversion and validation
- **Endpoints**: IPEndPoint serialization with network byte order
- **Big-Endian**: Network protocol numeric conversions
- **TLV Parsing**: Type-Length-Value protocol structures

### ⏱️ Async Operations

Asynchronous processing with cancellation:

- **File I/O**: `WriteToFileAsync`, `ReadFromFileAsync`
- **Parallel Processing**: Multi-threaded array operations
- **Cryptographic**: Hash computation and random generation

### 🗃️ Compression

Multiple compression algorithms:

- **GZip**: Standard compression with good balance
- **Deflate**: Raw deflate algorithm
- **Brotli**: Modern compression with best ratios

### 🔍 Utilities & Analysis

Advanced analysis and formatting:

- **Binary Representation**: Convert to binary strings
- **Statistical Analysis**: Entropy and distribution calculation
- **Performance Measurement**: Built-in timing utilities
- **Memory Analysis**: Memory usage estimation

---

## 📖 Documentation Sections

### 📋 API Reference

#### Core Extensions

| Class | Description |
|-------|-------------|
| [ByteArrayExtensions](ByteArrayExtensions.md) | Core utilities, pattern matching, and array comparison |
| [ByteArrayExtensions.PrimitiveTypeConversion](ByteArrayExtensions.PrimitiveTypeConversion.md) | Boolean, byte, char, and basic type conversions |
| [ByteArrayExtensions.IntegerConversion](ByteArrayExtensions.IntegerConversion.md) | Integer type conversions (Int16, Int32, Int64, UInt variants) |
| [ByteArrayExtensions.FloatingPointConversion](ByteArrayExtensions.FloatingPointConversion.md) | Float, double, and Half precision conversions |
| [ByteArrayExtensions.StringConversion](ByteArrayExtensions.StringConversion.md) | UTF-8, ASCII, hex, and Base64 string operations |
| [ByteArrayExtensions.ComplexTypeConversion](ByteArrayExtensions.ComplexTypeConversion.md) | Enum, Version, and complex type conversions |
| [ByteArrayExtensions.ArrayManipulation](ByteArrayExtensions.ArrayManipulation.md) | Slicing, concatenation, trimming, and XOR operations |
| [ByteArrayExtensions.DateTimeConversion](ByteArrayExtensions.DateTimeConversion.md) | DateTime, TimeSpan, DateTimeOffset, and Unix timestamps |
| [ByteArrayExtensions.NetworkConversion](ByteArrayExtensions.NetworkConversion.md) | IP addresses, endpoints, and big-endian conversions |
| [ByteArrayExtensions.GuidConversion](ByteArrayExtensions.GuidConversion.md) | GUID conversion utilities |

#### Specialized Classes

| Class | Description |
|-------|-------------|
| [ByteArrayBuilder](ByteArrayBuilder.md) | Fluent builder for constructing byte arrays from various types |
| [ByteArrayAsyncExtensions](ByteArrayAsyncExtensions.md) | Asynchronous operations for file I/O and parallel processing |
| [ByteArrayCompressionExtensions](ByteArrayCompressionExtensions.md) | Compression and decompression utilities (GZip, Deflate, Brotli) |
| [ByteArrayProtocolExtensions](ByteArrayProtocolExtensions.md) | Protocol parsing including TLV structures, framing, and checksums |
| [ByteArrayUtilities](ByteArrayUtilities.md) | Analysis, formatting, and performance measurement tools |
| [ObjectToByteArrayExtensions](ObjectToByteArrayExtensions.md) | Type-safe object-to-byte-array conversions and JSON serialization |

#### Modern Memory Types (NEW! ✨)

| Class | Description |
|-------|-------------|
| [ReadOnlySpanExtensions](ReadOnlySpanExtensions.md) | Zero-allocation, high-performance operations for ReadOnlySpan\<byte\> |
| [ReadOnlyMemoryExtensions](ReadOnlyMemoryExtensions.md) | Memory-safe operations for ReadOnlyMemory\<byte\> with async support |
| [ReadOnlySpanUtilities](ReadOnlySpanUtilities.md) | Analysis, entropy calculation, and transformation utilities for spans |

**ReadOnlySpan\<byte\> Partial Classes:**

| Class | Description |
|-------|-------------|
| ReadOnlySpanExtensions.PrimitiveTypeConversion | Boolean, byte, char, and sbyte conversions |
| ReadOnlySpanExtensions.IntegerConversion | All integer type conversions (Int16/32/64, UInt16/32/64) |
| ReadOnlySpanExtensions.FloatingPointConversion | Float, double, Half, and decimal conversions |
| ReadOnlySpanExtensions.StringConversion | UTF-8, ASCII, hex, and Base64 string operations |
| ReadOnlySpanExtensions.DateTimeConversion | DateTime, TimeSpan, and DateTimeOffset conversions |
| ReadOnlySpanExtensions.GuidConversion | GUID conversion utilities |
| ReadOnlySpanExtensions.ComplexTypeConversion | Enum and Version object conversions |
| ReadOnlySpanExtensions.NetworkConversion | IPv4/IPv6 addresses and network byte order |

---

## 🎯 Design Principles

- **🛡️ Safety First**: Explicit bounds checking with clear exception messages
- **⚡ Performance**: Zero-allocation friendly with `ReadOnlySpan<byte>` usage
- **🔄 Fail-Safe**: `OrDefault` methods never advance read cursors on failure
- **🎯 Type Safety**: Strict enum conversion with validation
- **🌐 Cross-Platform**: Optimized for .NET 9 and modern C# features

---

## 🚀 Advanced Examples

### Working with DateTime and Time Types

```csharp
// DateTime operations
var now = DateTime.Now;
using var builder = new ByteArrayBuilder();
builder.Append(now.ToBinary());
byte[] data = builder.ToByteArray();

// Read back with position tracking
var pos = 0;
DateTime restored = data.ToDateTime(ref pos);

// Unix timestamp support
int unixTime = 1672502400;
byte[] timeData = BitConverter.GetBytes(unixTime);
pos = 0;
DateTime fromUnix = timeData.ToDateTimeFromUnixTimestamp(ref pos);

// TimeSpan and DateTimeOffset
var span = TimeSpan.FromHours(2.5);
var offset = new DateTimeOffset(now, TimeSpan.FromHours(-8));
```

### Network Protocol Processing

```csharp
// IP address handling
var ipv4 = IPAddress.Parse("192.168.1.1");
var ipv6 = IPAddress.Parse("2001:db8::1");

byte[] ipData = ipv4.GetAddressBytes();
pos = 0;
IPAddress restored = ipData.ToIPAddress(ref pos);

// Network endpoints with big-endian ports
var endpoint = new IPEndPoint(ipv4, 8080);
byte[] endpointData = SerializeEndpoint(endpoint); // Custom serialization
pos = 0;
var restoredEndpoint = endpointData.ToIPEndPoint(ref pos);

// Big-endian network data
byte[] networkPacket = { 0x12, 0x34, 0x56, 0x78 };
pos = 0;
int sequenceNumber = networkPacket.ToInt32BigEndian(ref pos); // 0x12345678

// TLV protocol parsing
byte[] tlvData = { 0x01, 0x00, 0x05, 0x48, 0x65, 0x6C, 0x6C, 0x6F };
pos = 0;
var tlv = tlvData.ParseTlv(ref pos);
string value = Encoding.UTF8.GetString(tlv.Value); // "Hello"
```

### Async Operations with Cancellation

```csharp
// File operations
byte[] data = "Important data".Utf8StringToByteArray();
await data.WriteToFileAsync("output.bin");

byte[] fileData = await ByteArrayAsyncExtensions.ReadFromFileAsync("input.bin");

// Parallel processing with timeout
var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
byte[][] chunks = SplitDataIntoChunks(largeData, chunkSize: 1024);

var results = await chunks.ProcessInParallelAsync(
    async (chunk, ct) => {
        // Process each chunk
        await SimulateAsyncWork(chunk, ct);
        return chunk.CompressGZip();
    },
    maxDegreeOfParallelism: Environment.ProcessorCount,
    cancellationToken: cts.Token
);

// Cryptographic operations
byte[] hash = await data.ComputeSha256Async();
byte[] randomData = await ByteArrayAsyncExtensions.GenerateRandomBytesAsync(32);
```

### Compression and Analysis

```csharp
// Multi-algorithm compression comparison
byte[] originalData = File.ReadAllBytes("document.pdf");

byte[] gzipData = originalData.CompressGZip();
byte[] deflateData = originalData.CompressDeflate();
byte[] brotliData = originalData.CompressBrotli();

Console.WriteLine($"Original: {originalData.Length:N0} bytes");
Console.WriteLine($"GZip: {gzipData.Length:N0} bytes ({GetCompressionRatio(originalData, gzipData):P1})");
Console.WriteLine($"Brotli: {brotliData.Length:N0} bytes ({GetCompressionRatio(originalData, brotliData):P1})");

// Statistical analysis
double entropy = originalData.CalculateEntropy();
var distribution = originalData.AnalyzeDistribution();

Console.WriteLine($"Data entropy: {entropy:F3}");
Console.WriteLine($"Most frequent byte: 0x{distribution.MostFrequentByte:X2} ({distribution.MaxFrequency} occurrences)");

// Performance measurement
var stopwatch = originalData.StartPerformanceMeasurement();
// Perform operations...
var elapsed = originalData.StopPerformanceMeasurement(stopwatch);
Console.WriteLine($"Processing took: {elapsed.TotalMilliseconds:F2}ms");
```

### Working with Complex Data

```csharp
// Serialize a complex structure
using var builder = new ByteArrayBuilder();
var data = builder
    .Append((ushort)0x1234)           // Header
    .AppendUtf8String("UserData")     // Section name
    .Append(DateTime.Now)             // Timestamp
    .Append(MyEnum.Active)            // Status
    .Append(Guid.NewGuid())           // Unique ID
    .AppendHexString("DEADBEEF")      // Binary data
    .ToByteArray();

// Read it back safely
var pos = 0;
var header = data.ToUInt16(ref pos);
var section = data.ToUtf8String(ref pos, 8);
var timestamp = data.ToDateTime(ref pos);
var status = data.ToEnum<MyEnum>(ref pos);
var id = data.ToGuid(ref pos);
var binaryData = data.SafeSlice(pos, 4);

// TLV record creation and parsing
var tlvRecord = new TlvRecord(0x01, "Hello World".Utf8StringToByteArray());
byte[] tlvBytes = tlvRecord.ToByteArray();
pos = 0;
var parsedTlv = tlvBytes.ParseTlv(ref pos);
```

---

## 🔧 Development & Testing

- **Framework**: .NET 9.0
- **Testing**: xUnit + FluentAssertions with comprehensive coverage
- **Build**: `dotnet build -c Release`
- **Test**: `dotnet test -c Release`

---

## 📄 License

This library is licensed under the **MIT License** - see the [LICENSE.md](https://github.com/laerdal/Plugin.ByteArrays/blob/main/LICENSE.md) file for details.

---

## 🤝 Contributing

Contributions are welcome! Please feel free to submit issues, feature requests, or pull requests on [GitHub](https://github.com/laerdal/Plugin.ByteArrays).

---

*Built with ❤️ for modern .NET applications requiring efficient, safe byte array operations.*
