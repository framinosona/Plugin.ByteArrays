# 🚀 Plugin.ByteArrays - Complete Feature Guide

This comprehensive guide covers all features available in Plugin.ByteArrays, including the latest additions for network processing, async operations, compression, and advanced utilities.

---

## 📋 Table of Contents

1. [Core Components](#core-components)
2. [Basic Operations](#basic-operations)
3. [DateTime & Time Operations](#datetime--time-operations)
4. [Network & Protocol Support](#network--protocol-support)
5. [Async Operations](#async-operations)
6. [Compression](#compression)
7. [GUID Operations](#guid-operations)
8. [Utilities & Analysis](#utilities--analysis)
9. [Advanced Examples](#advanced-examples)

---

## 🔧 Core Components

### ByteArrayBuilder
Fluent builder for constructing byte arrays:
```csharp
using var builder = new ByteArrayBuilder();
byte[] data = builder
    .Append(0x01)
    .AppendUtf8String("Hello")
    .Append(42)
    .Append(DateTime.Now)
    .Append(Guid.NewGuid())
    .ToByteArray();
```

### ByteArrayExtensions
Main extension methods for reading and manipulating byte arrays with position tracking and bounds checking.

### Specialized Extensions
- **ByteArrayAsyncExtensions**: Async operations
- **ByteArrayCompressionExtensions**: Compression utilities
- **ByteArrayUtilities**: Analysis and formatting tools
- **ByteArrayProtocolExtensions**: Protocol parsing (TLV, etc.)

---

## 🔍 Basic Operations

### Reading Primitives
```csharp
byte[] data = { 0x01, 0x00, 0x00, 0x00, 0x2A, 0x48, 0x65, 0x6C, 0x6C, 0x6F };
var position = 0;

bool flag = data.ToBoolean(ref position);        // reads 1 byte
int value = data.ToInt32(ref position);          // reads 4 bytes, advances position
string text = data.ToUtf8String(ref position, 5); // reads 5 bytes

// Safe variants that don't throw
var safeValue = data.ToInt16OrDefault(ref position, defaultValue: -1);
```

### Array Manipulation
```csharp
byte[] data = { 0x10, 0x20, 0x30, 0x40, 0x00, 0x00 };

// Safe slicing
byte[] slice = data.SafeSlice(1, 3);  // [0x20, 0x30, 0x40]

// Concatenation
byte[] combined = ByteArrayExtensions.Concatenate(a, b, c);

// Trimming and transformation
byte[] trimmed = data.TrimEndNonDestructive();
byte[] reversed = data.Reverse();
byte[] xorResult = data.Xor(mask);
```

### Pattern Matching
```csharp
byte[] pattern = { 0x65, 0x6C };
bool starts = data.StartsWith(pattern);
bool ends = data.EndsWith(pattern);
int index = data.IndexOf(pattern);
bool identical = data.IsIdenticalTo(other);
```

---

## ⏰ DateTime & Time Operations

### DateTime Conversions
```csharp
// DateTime binary serialization
var now = DateTime.Now;
byte[] dateTimeBytes = BitConverter.GetBytes(now.ToBinary());
var position = 0;
DateTime restored = dateTimeBytes.ToDateTime(ref position);

// Safe operations
DateTime safeDate = invalidData.ToDateTimeOrDefault(ref position, DateTime.MinValue);
```

### Unix Timestamp Support
```csharp
// Unix timestamp conversions
int unixTimestamp = 1672502400; // 2023-01-01 00:00:00 UTC
byte[] timestampBytes = BitConverter.GetBytes(unixTimestamp);
var position = 0;
DateTime fromUnix = timestampBytes.ToDateTimeFromUnixTimestamp(ref position);

// With default fallback
DateTime safeUnix = invalidData.ToDateTimeFromUnixTimestampOrDefault(ref position, DateTime.Now);
```

### TimeSpan Operations
```csharp
// TimeSpan using ticks
var timeSpan = new TimeSpan(1, 2, 3, 4, 5);
byte[] spanBytes = BitConverter.GetBytes(timeSpan.Ticks);
var position = 0;
TimeSpan restored = spanBytes.ToTimeSpan(ref position);

// Safe TimeSpan conversion
TimeSpan safeSpan = invalidData.ToTimeSpanOrDefault(ref position, TimeSpan.Zero);
```

### DateTimeOffset Operations
```csharp
// Complex DateTimeOffset serialization
var offset = new DateTimeOffset(2023, 6, 15, 14, 30, 0, TimeSpan.FromHours(-8));
var dateTimeTicks = BitConverter.GetBytes(offset.DateTime.ToBinary());
var offsetTicks = BitConverter.GetBytes(offset.Offset.Ticks);
var data = dateTimeTicks.Concat(offsetTicks).ToArray();

var position = 0;
DateTimeOffset restored = data.ToDateTimeOffset(ref position);
```

---

## 🌐 Network & Protocol Support

### IP Address Conversions
```csharp
// IPv4 addresses
var ipv4 = IPAddress.Parse("192.168.1.1");
byte[] ipBytes = ipv4.GetAddressBytes();
var position = 0;
IPAddress restored = ipBytes.ToIPAddress(ref position);

// IPv6 addresses
var ipv6 = IPAddress.Parse("2001:db8::1");
byte[] ipv6Bytes = ipv6.GetAddressBytes();
position = 0;
IPAddress restoredV6 = ipv6Bytes.ToIPAddress(ref position, isIPv6: true);

// Safe conversions with defaults
IPAddress safeIP = invalidData.ToIPAddressOrDefault(ref position, IPAddress.Loopback);
```

### Network Endpoints
```csharp
// IPEndPoint serialization with network byte order
var endpoint = new IPEndPoint(IPAddress.Parse("10.0.0.1"), 8080);
var addressBytes = endpoint.Address.GetAddressBytes();
var portBytes = BitConverter.GetBytes((ushort)endpoint.Port);
if (BitConverter.IsLittleEndian)
    Array.Reverse(portBytes); // Convert to big-endian

var endpointData = addressBytes.Concat(portBytes).ToArray();
var position = 0;
IPEndPoint restored = endpointData.ToIPEndPoint(ref position);
```

### Big-Endian Conversions
```csharp
// Network protocol numeric conversions
byte[] networkData = { 0x12, 0x34, 0x56, 0x78 };
var position = 0;

short bigEndianShort = networkData.ToInt16BigEndian(ref position);  // 0x1234
int bigEndianInt = networkData.ToInt32BigEndian(ref position);      // 0x12345678
uint bigEndianUInt = networkData.ToUInt32BigEndian(ref position);   // 0x12345678U
```

### TLV Protocol Parsing
```csharp
// Type-Length-Value structure parsing
byte[] tlvData = { 0x01, 0x00, 0x05, 0x48, 0x65, 0x6C, 0x6C, 0x6F }; // Type=1, Length=5, Value="Hello"
var position = 0;
var tlvRecord = tlvData.ParseTlv(ref position);

Console.WriteLine($"Type: {tlvRecord.Type}");
Console.WriteLine($"Length: {tlvRecord.Length}");
Console.WriteLine($"Value: {Encoding.UTF8.GetString(tlvRecord.Value)}");

// Create TLV records
var newTlv = new TlvRecord(0x02, "World".Utf8StringToByteArray());
byte[] serialized = newTlv.ToByteArray();
```

---

## ⚡ Async Operations

### File I/O Operations
```csharp
// Async file operations with cancellation
byte[] data = "Important data".Utf8StringToByteArray();
var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

// Write to file
await data.WriteToFileAsync("output.bin", cts.Token);

// Read from file
byte[] fileData = await ByteArrayAsyncExtensions.ReadFromFileAsync("input.bin", cts.Token);

// Append to existing file
byte[] additionalData = "More data".Utf8StringToByteArray();
await additionalData.AppendToFileAsync("output.bin", cts.Token);
```

### Parallel Processing
```csharp
// Process multiple byte arrays in parallel
byte[][] dataChunks = SplitLargeData(originalData, chunkSize: 1024);
var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));

var results = await dataChunks.ProcessInParallelAsync(
    async (chunk, cancellationToken) => {
        // Simulate async processing
        await Task.Delay(100, cancellationToken);
        return chunk.CompressGZip();
    },
    maxDegreeOfParallelism: Environment.ProcessorCount,
    cancellationToken: cts.Token
);

// Transform arrays in parallel
var transformedData = await dataChunks.TransformInParallelAsync(
    chunk => chunk.Reverse(),
    maxDegreeOfParallelism: 4
);
```

### Cryptographic Operations
```csharp
// Async hash computation
byte[] data = File.ReadAllBytes("document.pdf");
byte[] sha256Hash = await data.ComputeSha256Async();
byte[] md5Hash = await data.ComputeMd5Async();

// Generate cryptographically secure random data
byte[] randomBytes = await ByteArrayAsyncExtensions.GenerateRandomBytesAsync(32);
byte[] randomSalt = await ByteArrayAsyncExtensions.GenerateRandomBytesAsync(16);
```

---

## 🗜️ Compression

### Multi-Algorithm Support
```csharp
byte[] originalData = File.ReadAllBytes("document.txt");

// GZip compression (good balance of speed and compression)
byte[] gzipCompressed = originalData.CompressGZip();
byte[] gzipDecompressed = gzipCompressed.DecompressGZip();

// Deflate compression (raw deflate algorithm)
byte[] deflateCompressed = originalData.CompressDeflate();
byte[] deflateDecompressed = deflateCompressed.DecompressDeflate();

// Brotli compression (best compression ratio, modern algorithm)
byte[] brotliCompressed = originalData.CompressBrotli();
byte[] brotliDecompressed = brotliCompressed.DecompressBrotli();
```

### Compression Analysis
```csharp
// Compare compression ratios
static double GetCompressionRatio(byte[] original, byte[] compressed) =>
    1.0 - ((double)compressed.Length / original.Length);

Console.WriteLine($"Original size: {originalData.Length:N0} bytes");
Console.WriteLine($"GZip: {gzipCompressed.Length:N0} bytes ({GetCompressionRatio(originalData, gzipCompressed):P1} reduction)");
Console.WriteLine($"Deflate: {deflateCompressed.Length:N0} bytes ({GetCompressionRatio(originalData, deflateCompressed):P1} reduction)");
Console.WriteLine($"Brotli: {brotliCompressed.Length:N0} bytes ({GetCompressionRatio(originalData, brotliCompressed):P1} reduction)");
```

---

## 🆔 GUID Operations

### GUID Conversions
```csharp
// GUID to byte array and back
var originalGuid = Guid.NewGuid();
byte[] guidBytes = originalGuid.ToByteArray();
var position = 0;
Guid restored = guidBytes.ToGuid(ref position);

// Safe GUID operations
Guid safeGuid = invalidData.ToGuidOrDefault(ref position, Guid.Empty);

// Fixed position GUID reading
Guid guidAtPosition = data.ToGuid(16); // Read GUID starting at byte 16
```

---

## 🔍 Utilities & Analysis

### Binary Representation
```csharp
byte[] data = { 0xAA, 0xBB, 0xCC, 0xDD };

// Binary string representation
string binary = data.ToBinaryString(); // "10101010 10111011 11001100 11011101"
string customBinary = data.ToBinaryString("|"); // "10101010|10111011|11001100|11011101"
```

### Statistical Analysis
```csharp
// Calculate data entropy
double entropy = data.CalculateEntropy();
Console.WriteLine($"Data entropy: {entropy:F3} bits per byte");

// Analyze byte distribution
var stats = data.AnalyzeDistribution();
Console.WriteLine($"Most frequent byte: 0x{stats.MostFrequentByte:X2} ({stats.MaxFrequency} occurrences)");
Console.WriteLine($"Least frequent byte: 0x{stats.LeastFrequentByte:X2} ({stats.MinFrequency} occurrences)");
Console.WriteLine($"Unique bytes: {stats.UniqueByteCount}");
```

### Performance Measurement
```csharp
// Built-in performance measurement
var stopwatch = data.StartPerformanceMeasurement();

// Perform operations
byte[] processed = data.CompressGZip().DecompressGZip();

var elapsed = data.StopPerformanceMeasurement(stopwatch);
Console.WriteLine($"Compression round-trip took: {elapsed.TotalMilliseconds:F2}ms");

// Memory usage estimation
long memoryUsage = data.EstimateMemoryUsage();
Console.WriteLine($"Estimated memory usage: {memoryUsage:N0} bytes");
```

---

## 🎯 Advanced Examples

### Complex Protocol Parsing
```csharp
// Parse a network packet with multiple data types
byte[] packet = BuildNetworkPacket();
var position = 0;

// Header
var packetType = packet.ToByte(ref position);
var flags = packet.ToByte(ref position);
var sequenceNumber = packet.ToUInt32BigEndian(ref position);
var timestamp = packet.ToDateTimeFromUnixTimestamp(ref position);

// Address information
var sourceIP = packet.ToIPAddress(ref position);
var destIP = packet.ToIPAddress(ref position);
var port = packet.ToUInt16BigEndian(ref position);

// Variable-length payload
var payloadLength = packet.ToUInt16BigEndian(ref position);
var payload = packet.SafeSlice(position, payloadLength);
position += payloadLength;

// TLV data
var tlvRecords = new List<TlvRecord>();
while (position < packet.Length - 3) // Minimum TLV size is 3 bytes
{
    var tlv = packet.ParseTlv(ref position);
    tlvRecords.Add(tlv);
}
```

### Async Batch Processing with Compression
```csharp
// Process multiple files with compression in parallel
string[] inputFiles = Directory.GetFiles("input", "*.dat");
var semaphore = new SemaphoreSlim(Environment.ProcessorCount);
var cts = new CancellationTokenSource(TimeSpan.FromMinutes(10));

var tasks = inputFiles.Select(async file => {
    await semaphore.WaitAsync(cts.Token);
    try
    {
        var data = await ByteArrayAsyncExtensions.ReadFromFileAsync(file, cts.Token);
        var compressed = data.CompressBrotli();
        var outputFile = Path.ChangeExtension(file.Replace("input", "output"), ".compressed");
        await compressed.WriteToFileAsync(outputFile, cts.Token);

        return new {
            InputFile = file,
            OriginalSize = data.Length,
            CompressedSize = compressed.Length,
            CompressionRatio = GetCompressionRatio(data, compressed)
        };
    }
    finally
    {
        semaphore.Release();
    }
});

var results = await Task.WhenAll(tasks);
foreach (var result in results)
{
    Console.WriteLine($"{Path.GetFileName(result.InputFile)}: {result.OriginalSize:N0} → {result.CompressedSize:N0} bytes ({result.CompressionRatio:P1} reduction)");
}
```

### Error-Resilient Data Processing
```csharp
// Process data with comprehensive error handling
byte[] unreliableData = GetDataFromUnreliableSource();
var position = 0;
var processedItems = 0;

// Use OrDefault methods for safe processing
while (position < unreliableData.Length)
{
    var startPosition = position;

    // Try to read a record
    var recordType = unreliableData.ToByteOrDefault(ref position, 0xFF);
    if (recordType == 0xFF) break; // Invalid data

    var dataLength = unreliableData.ToUInt16OrDefault(ref position, 0);
    if (dataLength == 0 || position + dataLength > unreliableData.Length)
    {
        // Skip invalid record
        position = startPosition + 1;
        continue;
    }

    var recordData = unreliableData.SafeSlice(position, dataLength);
    position += dataLength;

    // Process based on record type
    switch (recordType)
    {
        case 0x01: // DateTime record
            var recordPos = 0;
            var timestamp = recordData.ToDateTimeOrDefault(ref recordPos, DateTime.MinValue);
            if (timestamp != DateTime.MinValue)
            {
                ProcessTimestamp(timestamp);
                processedItems++;
            }
            break;

        case 0x02: // IP address record
            recordPos = 0;
            var ipAddress = recordData.ToIPAddressOrDefault(ref recordPos, IPAddress.None);
            if (!ipAddress.Equals(IPAddress.None))
            {
                ProcessIPAddress(ipAddress);
                processedItems++;
            }
            break;

        case 0x03: // GUID record
            recordPos = 0;
            var guid = recordData.ToGuidOrDefault(ref recordPos, Guid.Empty);
            if (guid != Guid.Empty)
            {
                ProcessGuid(guid);
                processedItems++;
            }
            break;
    }
}

Console.WriteLine($"Successfully processed {processedItems} records from {unreliableData.Length} bytes of data");
```

---

## 🛡️ Best Practices

### Error Handling
- Use `OrDefault` methods when data integrity is uncertain
- Always validate array bounds before processing
- Use position tracking with `ref int position` for sequential reading
- Implement comprehensive logging for data processing pipelines

### Performance
- Use `ReadOnlySpan<byte>` when possible for zero-allocation operations
- Prefer async methods for I/O operations
- Use parallel processing for CPU-intensive operations on large datasets
- Consider compression for storage and network transmission

### Memory Management
- Dispose of `ByteArrayBuilder` instances when done
- Use streaming for large file operations
- Consider memory usage when processing large datasets in parallel
- Monitor entropy and compression ratios for optimal storage

### Testing
- Test with various data sizes and patterns
- Validate boundary conditions and error cases
- Test async operations with cancellation scenarios
- Verify compression/decompression round-trips

---

*This guide covers all major features of Plugin.ByteArrays. For additional examples and API documentation, visit the [GitHub repository](https://github.com/framinosona/Plugin.ByteArrays).*
