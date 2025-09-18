# ByteArrayProtocolExtensions Class

The `ByteArrayProtocolExtensions` class provides specialized extensions for protocol-specific operations on byte arrays, including TLV (Type-Length-Value) parsing, frame operations, and checksum calculations. This class is essential for network protocol implementation and binary data structure manipulation.

## TlvRecord Structure

### Overview
The `TlvRecord` is a readonly struct representing a Type-Length-Value structure commonly used in network protocols and binary data formats.

### Properties
- **Type**: `byte` - The type field of the TLV record
- **Length**: `ushort` - The length field of the TLV record
- **Value**: `IReadOnlyList<byte>` - The value field of the TLV record

### Example Usage
```csharp
// Create a TLV record
var tlvRecord = new TlvRecord(0x01, 4, new byte[] { 0x12, 0x34, 0x56, 0x78 });

// Access properties
Console.WriteLine($"Type: 0x{tlvRecord.Type:X2}");
Console.WriteLine($"Length: {tlvRecord.Length}");
Console.WriteLine($"Value: {string.Join(", ", tlvRecord.Value)}");
```

## TLV Operations

### ParseTlvRecord
Parses a single TLV record from a byte array at the specified position.

#### Syntax
```csharp
public static TlvRecord ParseTlvRecord(this byte[] array, ref int position)
```

#### Parameters
- `array`: The byte array containing TLV data
- `position`: The position to start parsing from (advanced by record size)

#### Example
```csharp
var data = new byte[] { 0x01, 0x04, 0x00, 0x12, 0x34, 0x56, 0x78 };
var position = 0;
var tlv = data.ParseTlvRecord(ref position);

Console.WriteLine($"Parsed TLV: Type=0x{tlv.Type:X2}, Length={tlv.Length}");
// position is now 7 (1 + 2 + 4 bytes)
```

### ParseAllTlvRecords
Parses all TLV records from a byte array.

#### Syntax
```csharp
public static IEnumerable<TlvRecord> ParseAllTlvRecords(this byte[] array)
```

#### Example
```csharp
var data = new byte[] {
    0x01, 0x02, 0x00, 0x12, 0x34,           // First TLV
    0x02, 0x03, 0x00, 0x56, 0x78, 0x9A      // Second TLV
};

var allTlvs = data.ParseAllTlvRecords().ToList();
Console.WriteLine($"Found {allTlvs.Count} TLV records");

foreach (var tlv in allTlvs)
{
    Console.WriteLine($"Type: 0x{tlv.Type:X2}, Data: {string.Join(" ", tlv.Value.Select(b => $"0x{b:X2}"))}");
}
```

### CreateTlvRecord
Creates a TLV record as a byte array from type and value.

#### Syntax
```csharp
public static byte[] CreateTlvRecord(byte type, byte[] value)
```

#### Example
```csharp
var value = new byte[] { 0x12, 0x34, 0x56, 0x78 };
var tlvBytes = ByteArrayProtocolExtensions.CreateTlvRecord(0x01, value);

// Result: { 0x01, 0x04, 0x00, 0x12, 0x34, 0x56, 0x78 }
Console.WriteLine($"TLV bytes: {string.Join(" ", tlvBytes.Select(b => $"0x{b:X2}"))}");
```

## Frame Operations

### Simple Framing

#### AddSimpleFrame
Adds start and end marker bytes around data.

```csharp
public static byte[] AddSimpleFrame(this byte[] data, byte startMarker = 0x7E, byte endMarker = 0x7E)
```

#### Example
```csharp
var data = new byte[] { 0x01, 0x02, 0x03 };
var framed = data.AddSimpleFrame();

// Result: { 0x7E, 0x01, 0x02, 0x03, 0x7E }
Console.WriteLine($"Framed: {string.Join(" ", framed.Select(b => $"0x{b:X2}"))}");

// Custom markers
var customFramed = data.AddSimpleFrame(0xAA, 0xBB);
// Result: { 0xAA, 0x01, 0x02, 0x03, 0xBB }
```

#### RemoveSimpleFrame
Removes frame markers from data.

```csharp
public static byte[] RemoveSimpleFrame(this byte[] framedData, byte startMarker = 0x7E, byte endMarker = 0x7E)
```

#### Example
```csharp
var framedData = new byte[] { 0x7E, 0x01, 0x02, 0x03, 0x7E };
var unframed = framedData.RemoveSimpleFrame();

// Result: { 0x01, 0x02, 0x03 }
Console.WriteLine($"Unframed: {string.Join(" ", unframed.Select(b => $"0x{b:X2}"))}");
```

### Length-Prefixed Framing

#### AddLengthPrefixedFrame
Prefixes data with a 2-byte length field.

```csharp
public static byte[] AddLengthPrefixedFrame(this byte[] data)
```

#### Example
```csharp
var data = new byte[] { 0x01, 0x02, 0x03, 0x04 };
var framed = data.AddLengthPrefixedFrame();

// Result: { 0x04, 0x00, 0x01, 0x02, 0x03, 0x04 } (little-endian length)
Console.WriteLine($"Length-prefixed: {string.Join(" ", framed.Select(b => $"0x{b:X2}"))}");
```

#### RemoveLengthPrefixedFrame
Removes the length prefix and validates frame integrity.

```csharp
public static byte[] RemoveLengthPrefixedFrame(this byte[] framedData)
```

#### Example
```csharp
var framedData = new byte[] { 0x04, 0x00, 0x01, 0x02, 0x03, 0x04 };
var unframed = framedData.RemoveLengthPrefixedFrame();

// Result: { 0x01, 0x02, 0x03, 0x04 }
Console.WriteLine($"Unframed: {string.Join(" ", unframed.Select(b => $"0x{b:X2}"))}");
```

## Checksum Operations

### Simple Checksum
Calculates an 8-bit checksum by summing all bytes.

```csharp
public static byte CalculateSimpleChecksum(this byte[] data)
```

#### Example
```csharp
var data = new byte[] { 0x01, 0x02, 0x03, 0x04 };
var checksum = data.CalculateSimpleChecksum();

// Sum: 1 + 2 + 3 + 4 = 10 = 0x0A
Console.WriteLine($"Simple checksum: 0x{checksum:X2}");
```

### XOR Checksum
Calculates a checksum by XORing all bytes.

```csharp
public static byte CalculateXorChecksum(this byte[] data)
```

#### Example
```csharp
var data = new byte[] { 0x01, 0x02, 0x03, 0x04 };
var checksum = data.CalculateXorChecksum();

// XOR: 1 ^ 2 ^ 3 ^ 4 = 4 = 0x04
Console.WriteLine($"XOR checksum: 0x{checksum:X2}");
```

### Checksum Validation and Appending

#### AppendChecksum
Appends a checksum to data using a specified checksum function.

```csharp
public static byte[] AppendChecksum(this byte[] data, Func<byte[], byte> checksumFunction)
```

#### ValidateChecksum
Validates data against its trailing checksum.

```csharp
public static bool ValidateChecksum(this byte[] dataWithChecksum, Func<byte[], byte> checksumFunction)
```

#### Example
```csharp
var data = new byte[] { 0x01, 0x02, 0x03, 0x04 };

// Append simple checksum
var dataWithChecksum = data.AppendChecksum(d => d.CalculateSimpleChecksum());
Console.WriteLine($"Data with checksum: {string.Join(" ", dataWithChecksum.Select(b => $"0x{b:X2}"))}");

// Validate checksum
var isValid = dataWithChecksum.ValidateChecksum(d => d.CalculateSimpleChecksum());
Console.WriteLine($"Checksum valid: {isValid}"); // True

// Test with corrupted data
dataWithChecksum[0] = 0xFF; // Corrupt first byte
isValid = dataWithChecksum.ValidateChecksum(d => d.CalculateSimpleChecksum());
Console.WriteLine($"Corrupted checksum valid: {isValid}"); // False
```

## Protocol Implementation Example

### Complete Protocol Handler
```csharp
public class SimpleProtocolHandler
{
    public byte[] CreateMessage(byte messageType, byte[] payload)
    {
        // Create TLV record
        var tlv = ByteArrayProtocolExtensions.CreateTlvRecord(messageType, payload);

        // Add frame
        var framed = tlv.AddLengthPrefixedFrame();

        // Add checksum
        var final = framed.AppendChecksum(data => data.CalculateXorChecksum());

        return final;
    }

    public (byte messageType, byte[] payload) ParseMessage(byte[] rawData)
    {
        // Validate checksum
        if (!rawData.ValidateChecksum(data => data.CalculateXorChecksum()))
        {
            throw new InvalidDataException("Checksum validation failed");
        }

        // Remove checksum
        var dataWithoutChecksum = new byte[rawData.Length - 1];
        Array.Copy(rawData, 0, dataWithoutChecksum, 0, dataWithoutChecksum.Length);

        // Remove frame
        var tlvData = dataWithoutChecksum.RemoveLengthPrefixedFrame();

        // Parse TLV
        var position = 0;
        var tlv = tlvData.ParseTlvRecord(ref position);

        return (tlv.Type, tlv.Value.ToArray());
    }
}

// Usage
var handler = new SimpleProtocolHandler();
var message = handler.CreateMessage(0x01, new byte[] { 0x12, 0x34, 0x56 });
var (type, payload) = handler.ParseMessage(message);
```

## Performance Characteristics

### TLV Operations
- **ParseTlvRecord**: O(1) for individual record parsing
- **ParseAllTlvRecords**: O(n) where n is the total data size
- **CreateTlvRecord**: O(m) where m is the value size

### Frame Operations
- **Simple Framing**: O(n) where n is data size (requires array copy)
- **Length-Prefixed**: O(n) where n is data size (requires array copy)

### Checksum Operations
- **Simple Checksum**: O(n) where n is data size
- **XOR Checksum**: O(n) where n is data size

## Best Practices

### TLV Usage
```csharp
// Use structured approach for multiple TLVs
var tlvs = new List<TlvRecord>
{
    new TlvRecord(0x01, 4, BitConverter.GetBytes(42)),
    new TlvRecord(0x02, 8, BitConverter.GetBytes(DateTime.Now.Ticks))
};

// Serialize multiple TLVs
using var builder = new ByteArrayBuilder();
foreach (var tlv in tlvs)
{
    var tlvBytes = ByteArrayProtocolExtensions.CreateTlvRecord(tlv.Type, tlv.Value.ToArray());
    builder.Append(tlvBytes);
}
var serialized = builder.ToByteArray();
```

### Frame Selection
```csharp
// Use simple framing for protocols with unique delimiters
var simpleFramed = data.AddSimpleFrame(0x7E, 0x7E);

// Use length-prefixed framing for variable-length messages
var lengthFramed = data.AddLengthPrefixedFrame();

// Combine both for robust protocols
var robust = data.AddLengthPrefixedFrame().AddSimpleFrame(0xAA, 0xBB);
```

### Error Handling
```csharp
try
{
    var position = 0;
    while (position < data.Length)
    {
        var tlv = data.ParseTlvRecord(ref position);
        // Process TLV
    }
}
catch (ArgumentException ex)
{
    Console.WriteLine($"TLV parsing failed: {ex.Message}");
    // Handle malformed data
}
```

## Cross-References

- <xref:Plugin.ByteArrays.ByteArrayExtensions> - Core byte array operations
- <xref:Plugin.ByteArrays.ByteArrayBuilder> - Building byte arrays fluently
- <xref:Plugin.ByteArrays.ByteArrayUtilities> - Analysis and formatting utilities
- <xref:Plugin.ByteArrays.ByteArrayAsyncExtensions> - Asynchronous operations
