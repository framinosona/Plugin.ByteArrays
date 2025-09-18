# ByteArrayExtensions.PrimitiveTypeConversion

This documentation covers the primitive type conversion extension methods in `ByteArrayExtensions`. These methods handle the most basic data types: boolean, byte, signed byte (sbyte), and character conversions from byte arrays.

## Overview

All primitive type conversion methods follow the established pattern:
- **Main methods** with `ref int position` that advance the position automatically
- **Convenience overloads** with fixed position (default 0)
- **Safe variants** (`OrDefault`) that return default values instead of throwing exceptions
- **1-2 byte operations** for the smallest data types

## Boolean Conversion

### ToBoolean
<xref:Plugin.ByteArrays.ByteArrayExtensions.ToBoolean*>

Converts a single byte to a boolean value. Non-zero bytes are `true`, zero bytes are `false`.

#### Usage Examples

```csharp
// Basic boolean conversion
byte[] data = { 1, 0, 255, 0 };
var position = 0;

bool value1 = data.ToBoolean(ref position); // true (from byte 1)
bool value2 = data.ToBoolean(ref position); // false (from byte 0)
bool value3 = data.ToBoolean(ref position); // true (from byte 255)
bool value4 = data.ToBoolean(ref position); // false (from byte 0)

Console.WriteLine($"Position after conversions: {position}"); // 4

// Fixed position usage
bool first = data.ToBoolean(0);  // true
bool second = data.ToBoolean(1); // false
bool third = data.ToBoolean(2);  // true

// Practical use: Feature flags in binary protocols
byte[] featureFlags = { 0b10101010 }; // Multiple flags in one byte
bool featureA = featureFlags.ToBoolean(0); // true (non-zero)

// Individual bit checking requires bit operations
// Use GetBit() from ArrayManipulation for specific bit positions
```

### ToBooleanOrDefault
<xref:Plugin.ByteArrays.ByteArrayExtensions.ToBooleanOrDefault*>

Safe boolean conversion that returns a default value if the array is too short.

#### Usage Examples

```csharp
byte[] shortData = { 1 };
var position = 0;

bool valid = shortData.ToBooleanOrDefault(ref position);     // true
bool invalid = shortData.ToBooleanOrDefault(ref position);   // false (default)

// Position doesn't advance on failure
Console.WriteLine($"Position: {position}"); // 1 (only advanced once)

// Custom default value
bool customDefault = shortData.ToBooleanOrDefault(5, defaultValue: true); // true
```

## Byte Conversion

### ToByte
<xref:Plugin.ByteArrays.ByteArrayExtensions.ToByte*>

Extracts an unsigned 8-bit integer (0-255) from the byte array.

#### Usage Examples

```csharp
byte[] data = { 0, 128, 255, 42 };
var position = 0;

byte min = data.ToByte(ref position);     // 0
byte mid = data.ToByte(ref position);     // 128
byte max = data.ToByte(ref position);     // 255
byte answer = data.ToByte(ref position);  // 42

// Fixed position access
byte specific = data.ToByte(2); // 255

// Practical use: Reading raw byte values
byte[] binaryData = File.ReadAllBytes("data.bin");
byte header = binaryData.ToByte(0);
byte version = binaryData.ToByte(1);
byte flags = binaryData.ToByte(2);

// Protocol parsing
byte[] packet = ReceivePacket();
var pos = 0;
byte packetType = packet.ToByte(ref pos);
byte packetLength = packet.ToByte(ref pos);
// Continue parsing based on type and length
```

### ToByteOrDefault
<xref:Plugin.ByteArrays.ByteArrayExtensions.ToByteOrDefault*>

Safe byte extraction with default value fallback.

#### Usage Examples

```csharp
byte[] shortData = { 100 };
var position = 0;

byte valid = shortData.ToByteOrDefault(ref position);       // 100
byte invalid = shortData.ToByteOrDefault(ref position);     // 0 (default)

// Custom default for out-of-bounds
byte customDefault = shortData.ToByteOrDefault(5, defaultValue: 255); // 255

// Practical use: Optional protocol fields
byte[] partialPacket = GetPartialPacket();
byte mandatoryField = partialPacket.ToByte(0);
byte optionalField = partialPacket.ToByteOrDefault(1, defaultValue: 0x00);
```

## Signed Byte Conversion

### ToSByte
<xref:Plugin.ByteArrays.ByteArrayExtensions.ToSByte*>

Converts to signed 8-bit integer (-128 to 127).

#### Usage Examples

```csharp
byte[] data = { 0, 127, 128, 255 };
var position = 0;

sbyte zero = data.ToSByte(ref position);     // 0
sbyte positive = data.ToSByte(ref position); // 127
sbyte negative1 = data.ToSByte(ref position); // -128 (128 as signed)
sbyte negative2 = data.ToSByte(ref position); // -1 (255 as signed)

// Understanding signed byte conversion
byte[] examples = { 0, 50, 127, 128, 200, 255 };
for (int i = 0; i < examples.Length; i++)
{
    byte unsigned = examples.ToByte(i);
    sbyte signed = examples.ToSByte(i);
    Console.WriteLine($"Byte {unsigned} as signed: {signed}");
}
// Output:
// Byte 0 as signed: 0
// Byte 50 as signed: 50
// Byte 127 as signed: 127
// Byte 128 as signed: -128
// Byte 200 as signed: -56
// Byte 255 as signed: -1

// Practical use: Temperature readings (can be negative)
byte[] sensorData = ReadSensorData();
var pos = 0;
sbyte temperature = sensorData.ToSByte(ref pos); // -20°C to +127°C range
```

### ToSByteOrDefault
<xref:Plugin.ByteArrays.ByteArrayExtensions.ToSByteOrDefault*>

Safe signed byte conversion with default fallback.

#### Usage Examples

```csharp
byte[] limitedData = { 100 };
var position = 0;

sbyte valid = limitedData.ToSByteOrDefault(ref position);     // 100
sbyte invalid = limitedData.ToSByteOrDefault(ref position);   // 0 (default)

// Custom default for signed values
sbyte freezing = limitedData.ToSByteOrDefault(5, defaultValue: -1); // -1
```

## Character Conversion

### ToChar
<xref:Plugin.ByteArrays.ByteArrayExtensions.ToChar*>

Converts 2 bytes to a UTF-16 character using little-endian byte order.

#### Usage Examples

```csharp
// ASCII characters (high byte is 0)
byte[] asciiData = { 65, 0, 66, 0, 67, 0 }; // "ABC" in UTF-16 LE
var position = 0;

char a = asciiData.ToChar(ref position); // 'A' (65 + 0*256)
char b = asciiData.ToChar(ref position); // 'B' (66 + 0*256)
char c = asciiData.ToChar(ref position); // 'C' (67 + 0*256)

// Unicode characters
byte[] unicodeData = { 0x41, 0x00, 0x42, 0x26 }; // 'A' + '♂' symbol
position = 0;
char letter = unicodeData.ToChar(ref position);  // 'A'
char symbol = unicodeData.ToChar(ref position);  // '♂' (0x2642)

// Fixed position access
char specific = asciiData.ToChar(2); // 'B' (starting at position 2)

// Practical use: Reading UTF-16 encoded text
byte[] utf16File = File.ReadAllBytes("text.txt"); // UTF-16 LE file
var pos = 0;
var characters = new List<char>();

while (pos < utf16File.Length - 1) // Ensure we have 2 bytes
{
    char ch = utf16File.ToChar(ref pos);
    if (ch == '\0') break; // Null terminator
    characters.Add(ch);
}

string text = new string(characters.ToArray());

// Protocol with embedded strings
byte[] packet = ReceivePacket();
var packetPos = 0;
byte nameLength = packet.ToByte(ref packetPos);
var name = new char[nameLength];

for (int i = 0; i < nameLength; i++)
{
    name[i] = packet.ToChar(ref packetPos);
}

string playerName = new string(name);
```

### ToCharOrDefault
<xref:Plugin.ByteArrays.ByteArrayExtensions.ToCharOrDefault*>

Safe character conversion with default fallback.

#### Usage Examples

```csharp
byte[] incompleteData = { 65 }; // Only 1 byte, need 2 for char
var position = 0;

char valid = incompleteData.ToCharOrDefault(ref position);   // '\0' (default)

// Custom default character
char customDefault = incompleteData.ToCharOrDefault(0, defaultValue: '?'); // '?'

// Practical use: Reading variable-length strings safely
byte[] packet = ReceivePacket();
var pos = 0;
var result = new StringBuilder();

while (pos < packet.Length)
{
    char ch = packet.ToCharOrDefault(ref pos, defaultValue: '\0');
    if (ch == '\0') break; // End of string or incomplete data
    result.Append(ch);
}

string safeParsedText = result.ToString();
```

## Advanced Usage Patterns

### Position Tracking
```csharp
byte[] mixedData = {
    1,           // boolean (1 byte)
    42,          // byte (1 byte)
    200,         // sbyte (1 byte)
    65, 0        // char (2 bytes)
};

var position = 0;
bool flag = mixedData.ToBoolean(ref position);    // pos = 1
byte value = mixedData.ToByte(ref position);      // pos = 2
sbyte signed = mixedData.ToSByte(ref position);   // pos = 3
char letter = mixedData.ToChar(ref position);     // pos = 5

Console.WriteLine($"Final position: {position}"); // 5
```

### Error Handling Comparison
```csharp
byte[] shortArray = { 1 };

// Throwing versions
try
{
    char invalid = shortArray.ToChar(0); // Throws ArgumentOutOfRangeException
}
catch (ArgumentOutOfRangeException ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}

// Safe versions
char safe = shortArray.ToCharOrDefault(0); // Returns '\0', no exception
```

### Endianness Considerations
```csharp
// Little-endian (Windows/Intel) byte order
byte[] littleEndian = { 0x41, 0x00 }; // 'A' in UTF-16 LE
char charLE = littleEndian.ToChar(0);  // 'A' (correct on little-endian systems)

// If you have big-endian data, reverse the bytes first
byte[] bigEndian = { 0x00, 0x41 };    // 'A' in UTF-16 BE
byte[] converted = { bigEndian[1], bigEndian[0] }; // Swap bytes
char charBE = converted.ToChar(0);     // 'A' (now correct)

// Or use the network conversion methods for proper endian handling
```

## Performance Notes

- **Single-byte operations** (boolean, byte, sbyte) are very fast
- **Character conversion** involves 2-byte reads and UTF-16 interpretation
- **Position tracking** adds minimal overhead
- **OrDefault methods** include bounds checking but avoid exceptions

## Common Use Cases

1. **Binary Protocol Parsing**: Reading headers, flags, and control bytes
2. **File Format Analysis**: Parsing binary file headers and metadata
3. **Network Communication**: Processing packet headers and control information
4. **Embedded Systems Data**: Reading sensor values and device status
5. **Game Development**: Loading binary asset files and save data
6. **Cryptographic Operations**: Processing keys, IVs, and hash values

## See Also

- <xref:Plugin.ByteArrays.ByteArrayExtensions> - For 16/32/64-bit integer conversions, full string handling and bit-level operations
- <xref:Plugin.ByteArrays.ByteArrayBuilder> - For constructing byte arrays from primitives
