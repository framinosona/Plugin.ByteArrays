# ByteArrayExtensions.IntegerConversion

This documentation covers integer type conversion extension methods in `ByteArrayExtensions`. These methods handle 16-bit, 32-bit, and 64-bit integers in both signed and unsigned variants, providing comprehensive support for all .NET integer types.

## Overview

Integer conversion methods handle multi-byte values using little-endian byte order (least significant byte first). All methods follow the established pattern:
- **Main methods** with `ref int position` that advance the position automatically
- **Convenience overloads** with fixed position (default 0)
- **Safe variants** (`OrDefault`) that return default values instead of throwing exceptions
- **2, 4, or 8 byte operations** depending on the integer size

## 16-bit Integer Conversion

### ToInt16
<xref:Plugin.ByteArrays.ByteArrayExtensions.ToInt16*>

Converts 2 bytes to a signed 16-bit integer (short) with range -32,768 to 32,767.

#### Usage Examples

```csharp
// Basic 16-bit signed conversion
byte[] data = { 0x00, 0x01, 0xFF, 0xFF, 0x34, 0x12 };
var position = 0;

short value1 = data.ToInt16(ref position); // 256 (0x0100)
short value2 = data.ToInt16(ref position); // -1 (0xFFFF)
short value3 = data.ToInt16(ref position); // 4660 (0x1234)

Console.WriteLine($"Position after conversions: {position}"); // 6

// Fixed position access
short specific = data.ToInt16(4); // 4660 (0x1234)

// Understanding little-endian byte order
byte[] example = { 0x34, 0x12 }; // Little-endian representation
short result = example.ToInt16(0); // 0x1234 = 4660
// Low byte (0x34) comes first, high byte (0x12) comes second

// Practical use: Reading binary file headers
byte[] fileHeader = File.ReadAllBytes("data.bin");
var pos = 0;
short magic = fileHeader.ToInt16(ref pos);     // File format magic number
short version = fileHeader.ToInt16(ref pos);   // Format version
short flags = fileHeader.ToInt16(ref pos);     // Feature flags

// Network protocol parsing
byte[] packet = ReceivePacket();
var packetPos = 0;
short messageType = packet.ToInt16(ref packetPos);
short messageLength = packet.ToInt16(ref packetPos);
```

### ToInt16OrDefault
<xref:Plugin.ByteArrays.ByteArrayExtensions.ToInt16OrDefault*>

Safe signed 16-bit conversion with default value fallback.

#### Usage Examples

```csharp
byte[] shortData = { 0x42 }; // Only 1 byte, need 2
var position = 0;

short invalid = shortData.ToInt16OrDefault(ref position); // 0 (default)
Console.WriteLine($"Position: {position}"); // 0 (not advanced)

// Custom default value
short customDefault = shortData.ToInt16OrDefault(0, defaultValue: -1); // -1
```

### ToUInt16
<xref:Plugin.ByteArrays.ByteArrayExtensions.ToUInt16*>

Converts 2 bytes to an unsigned 16-bit integer (ushort) with range 0 to 65,535.

#### Usage Examples

```csharp
byte[] data = { 0xFF, 0xFF, 0x00, 0x80, 0x34, 0x12 };
var position = 0;

ushort max = data.ToUInt16(ref position);     // 65535 (0xFFFF)
ushort large = data.ToUInt16(ref position);   // 32768 (0x8000)
ushort normal = data.ToUInt16(ref position);  // 4660 (0x1234)

// Practical use: Port numbers and identifiers
byte[] networkData = ReceiveNetworkData();
var pos = 0;
ushort sourcePort = networkData.ToUInt16(ref pos);
ushort destPort = networkData.ToUInt16(ref pos);
ushort sessionId = networkData.ToUInt16(ref pos);

// Reading unsigned values (always positive)
byte[] sensorData = ReadSensorData();
ushort temperature = sensorData.ToUInt16(0); // 0-65535 range
ushort humidity = sensorData.ToUInt16(2);    // Always positive
```

## 32-bit Integer Conversion

### ToInt32
<xref:Plugin.ByteArrays.ByteArrayExtensions.ToInt32*>

Converts 4 bytes to a signed 32-bit integer (int) with range -2,147,483,648 to 2,147,483,647.

#### Usage Examples

```csharp
// Basic 32-bit signed conversion
byte[] data = { 0x00, 0x00, 0x00, 0x80, 0xFF, 0xFF, 0xFF, 0xFF };
var position = 0;

int largeNegative = data.ToInt32(ref position); // -2147483648 (0x80000000)
int minusOne = data.ToInt32(ref position);      // -1 (0xFFFFFFFF)

// Practical use: File sizes and offsets
byte[] fileInfo = GetFileInfo();
var pos = 0;
int fileSize = fileInfo.ToInt32(ref pos);       // Can be negative if > 2GB
int headerOffset = fileInfo.ToInt32(ref pos);
int dataOffset = fileInfo.ToInt32(ref pos);

// Database record IDs
byte[] recordData = GetDatabaseRecord();
int recordId = recordData.ToInt32(0);
int parentId = recordData.ToInt32(4);
int categoryId = recordData.ToInt32(8);

// Unix timestamps (seconds since epoch)
byte[] timestampData = GetTimestampData();
int unixTimestamp = timestampData.ToInt32(0);
DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(unixTimestamp).DateTime;
```

### ToUInt32
<xref:Plugin.ByteArrays.ByteArrayExtensions.ToUInt32*>

Converts 4 bytes to an unsigned 32-bit integer (uint) with range 0 to 4,294,967,295.

#### Usage Examples

```csharp
byte[] data = { 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x01 };
var position = 0;

uint maxValue = data.ToUInt32(ref position);  // 4294967295 (0xFFFFFFFF)
uint smallValue = data.ToUInt32(ref position); // 16777216 (0x01000000)

// Practical use: Large file sizes (supports up to 4GB)
byte[] fileSizeData = GetFileSizeData();
uint largeFileSize = fileSizeData.ToUInt32(0); // Up to 4GB

// Memory addresses and pointers (32-bit systems)
byte[] memoryDump = GetMemoryDump();
var pos = 0;
uint baseAddress = memoryDump.ToUInt32(ref pos);
uint stackPointer = memoryDump.ToUInt32(ref pos);

// CRC32 checksums and hash values
byte[] checksumData = GetChecksumData();
uint crc32 = checksumData.ToUInt32(0);
uint hash = checksumData.ToUInt32(4);
```

## 64-bit Integer Conversion

### ToInt64
<xref:Plugin.ByteArrays.ByteArrayExtensions.ToInt64*>

Converts 8 bytes to a signed 64-bit integer (long) with range -9,223,372,036,854,775,808 to 9,223,372,036,854,775,807.

#### Usage Examples

```csharp
// Basic 64-bit signed conversion
byte[] data = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80 };
long largeNegative = data.ToInt64(0); // -9223372036854775808

// Practical use: Timestamps with millisecond precision
byte[] timestampData = GetTimestampData();
var pos = 0;
long unixMilliseconds = timestampData.ToInt64(ref pos);
DateTime preciseTime = DateTimeOffset.FromUnixTimeMilliseconds(unixMilliseconds).DateTime;

// Large file sizes and offsets (supports files > 4GB)
byte[] largeFileInfo = GetLargeFileInfo();
pos = 0;
long fileSize = largeFileInfo.ToInt64(ref pos);      // Up to 9 exabytes
long seekPosition = largeFileInfo.ToInt64(ref pos);  // Large file offset

// Database primary keys and counters
byte[] databaseData = GetDatabaseData();
long recordId = databaseData.ToInt64(0);
long sequenceNumber = databaseData.ToInt64(8);

// Performance counters and measurements
byte[] perfData = GetPerformanceData();
pos = 0;
long ticksElapsed = perfData.ToInt64(ref pos);
long memoryUsed = perfData.ToInt64(ref pos);
```

### ToUInt64
<xref:Plugin.ByteArrays.ByteArrayExtensions.ToUInt64*>

Converts 8 bytes to an unsigned 64-bit integer (ulong) with range 0 to 18,446,744,073,709,551,615.

#### Usage Examples

```csharp
byte[] data = { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
ulong maxValue = data.ToUInt64(0); // 18446744073709551615

// Practical use: Very large file sizes and data volumes
byte[] bigDataInfo = GetBigDataInfo();
var pos = 0;
ulong totalBytes = bigDataInfo.ToUInt64(ref pos);      // Up to 18 exabytes
ulong processedBytes = bigDataInfo.ToUInt64(ref pos);

// Cryptographic values and large numbers
byte[] cryptoData = GetCryptoData();
ulong nonce = cryptoData.ToUInt64(0);
ulong keyId = cryptoData.ToUInt64(8);

// High-precision timestamps (nanoseconds since epoch)
byte[] preciseTimestamp = GetPreciseTimestamp();
ulong nanoseconds = preciseTimestamp.ToUInt64(0);

// Memory addresses on 64-bit systems
byte[] memoryAddresses = GetMemoryAddresses();
pos = 0;
ulong baseAddress = memoryAddresses.ToUInt64(ref pos);
ulong stackTop = memoryAddresses.ToUInt64(ref pos);
```

## Advanced Usage Patterns

### Mixed Integer Types in Protocols
```csharp
byte[] protocolMessage = ReceiveMessage();
var position = 0;

// Protocol header with mixed integer types
short messageType = protocolMessage.ToInt16(ref position);     // 2 bytes
int messageLength = protocolMessage.ToInt32(ref position);     // 4 bytes
long timestamp = protocolMessage.ToInt64(ref position);        // 8 bytes
ushort flags = protocolMessage.ToUInt16(ref position);         // 2 bytes

Console.WriteLine($"Header parsed, data starts at position: {position}"); // 16
```

### Endianness Handling
```csharp
// Data from big-endian source (network byte order)
byte[] bigEndianData = { 0x12, 0x34, 0x56, 0x78 };

// Convert to little-endian for ToInt32
byte[] littleEndianData = {
    bigEndianData[3], bigEndianData[2],
    bigEndianData[1], bigEndianData[0]
};

int value = littleEndianData.ToInt32(0); // 0x78563412

// Or use network conversion methods for proper handling
// See ByteArrayExtensions.NetworkConversion documentation
```

### Range Validation
```csharp
byte[] userData = GetUserInput();

// Validate ranges for specific use cases
int userAge = userData.ToInt32(0);
if (userAge < 0 || userAge > 150)
{
    throw new ArgumentOutOfRangeException("Invalid age");
}

ushort port = userData.ToUInt16(4);
if (port < 1024) // Reserved ports
{
    throw new ArgumentException("Port must be >= 1024");
}
```

### Performance-Optimized Reading
```csharp
byte[] largeDataset = File.ReadAllBytes("large_file.dat");
var results = new List<DataRecord>();
var position = 0;

// Efficient sequential reading
while (position < largeDataset.Length - 16) // Ensure we have enough bytes
{
    var record = new DataRecord
    {
        Id = largeDataset.ToInt32(ref position),
        Timestamp = largeDataset.ToInt64(ref position),
        Value = largeDataset.ToUInt32(ref position)
    };
    results.Add(record);
}
```

### Safe Parsing with Error Recovery
```csharp
byte[] unreliableData = GetUnreliableData();
var position = 0;
var parsedValues = new List<int>();

while (position < unreliableData.Length)
{
    int value = unreliableData.ToInt32OrDefault(ref position, defaultValue: -1);
    if (value == -1 && position < unreliableData.Length)
    {
        // Skip corrupt data and try next position
        position++;
        continue;
    }

    if (value != -1)
    {
        parsedValues.Add(value);
    }
}
```

## Performance Characteristics

- **16-bit operations**: ~2-3 CPU cycles, minimal memory allocation
- **32-bit operations**: ~3-4 CPU cycles, optimal for most scenarios
- **64-bit operations**: ~4-5 CPU cycles, best for large values
- **Position tracking**: Adds ~1 CPU cycle overhead
- **OrDefault methods**: Include bounds checking but avoid exception overhead

## Common Use Cases

1. **Binary File Formats**: Reading headers, metadata, and structured data
2. **Network Protocols**: Parsing packet headers and payload lengths
3. **Database Records**: Reading fixed-width integer fields
4. **Cryptographic Data**: Processing keys, signatures, and hash values
5. **Performance Monitoring**: Handling counters and measurement data
6. **Game Development**: Loading binary assets and save files
7. **Embedded Systems**: Processing sensor data and control messages

## Type Selection Guidelines

| Type | Size | Range | Best For |
|------|------|-------|----------|
| `short` | 2 bytes | ±32K | Small signed values, offsets |
| `ushort` | 2 bytes | 0-65K | Port numbers, small IDs |
| `int` | 4 bytes | ±2B | General purpose, file sizes |
| `uint` | 4 bytes | 0-4B | Large positive values, checksums |
| `long` | 8 bytes | ±9E | Timestamps, large file offsets |
| `ulong` | 8 bytes | 0-18E | Very large sizes, crypto values |

## See Also

- <xref:Plugin.ByteArrays.ByteArrayExtensions> - For single-byte types, decimal numbers and endianness handling
- <xref:Plugin.ByteArrays.ByteArrayBuilder> - For constructing byte arrays from integers
