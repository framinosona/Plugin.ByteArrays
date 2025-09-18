# ByteArrayExtensions.StringConversion

This documentation covers string conversion extension methods in `ByteArrayExtensions`. These methods handle conversion between byte arrays and strings using various encodings including UTF-8, ASCII, UTF-16, UTF-32, hexadecimal, and Base64 formats.

## Overview

String conversion methods provide flexible text encoding support with customizable length handling. All methods follow the established pattern:
- **Main methods** with `ref int position` that advance the position automatically
- **Convenience overloads** with fixed position (default 0)
- **Safe variants** (`OrDefault`) that return default values instead of throwing exceptions
- **Variable-length operations** with optional byte count specification

## UTF-8 String Conversion

### ToUtf8String
<xref:Plugin.ByteArrays.ByteArrayExtensions.ToUtf8String*>

Converts byte arrays to UTF-8 encoded strings, supporting international characters and emojis.

#### Usage Examples

```csharp
// Basic UTF-8 string conversion
byte[] utf8Data = { 0x48, 0x65, 0x6C, 0x6C, 0x6F }; // "Hello" in UTF-8
var position = 0;

string text = utf8Data.ToUtf8String(ref position); // "Hello"
Console.WriteLine($"Position after conversion: {position}"); // 5

// UTF-8 with international characters
byte[] internationalData = { 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x20, 0xF0, 0x9F, 0x8C, 0x8D }; // "Hello 🌍"
string international = internationalData.ToUtf8String(0); // "Hello 🌍"

// Specific length reading
byte[] mixedData = GetMixedData();
position = 0;
string part1 = mixedData.ToUtf8String(ref position, 5); // Read exactly 5 bytes
string part2 = mixedData.ToUtf8String(ref position, 10); // Read next 10 bytes

// Read to end of array
byte[] messageData = GetMessageData();
string fullMessage = messageData.ToUtf8String(0); // numberOfBytesToRead = -1 (default)

// Protocol message parsing
byte[] packet = ReceivePacket();
position = 0;
byte nameLength = packet.ToByte(ref position);
string playerName = packet.ToUtf8String(ref position, nameLength);
byte messageLength = packet.ToByte(ref position);
string chatMessage = packet.ToUtf8String(ref position, messageLength);

// File content reading
byte[] fileContent = File.ReadAllBytes("utf8_document.txt");
string document = fileContent.ToUtf8String(0);
```

### ToUtf8StringOrDefault
<xref:Plugin.ByteArrays.ByteArrayExtensions.ToUtf8StringOrDefault*>

Safe UTF-8 conversion with default value fallback.

#### Usage Examples

```csharp
byte[] shortData = { 0x48, 0x65 }; // Only 2 bytes
var position = 0;

string partial = shortData.ToUtf8StringOrDefault(ref position, 10, defaultValue: "N/A"); // "N/A"
Console.WriteLine($"Position: {position}"); // 0 (not advanced on failure)

// Safe protocol parsing
byte[] unreliablePacket = GetUnreliablePacket();
position = 0;
string safeName = unreliablePacket.ToUtf8StringOrDefault(ref position, 20, defaultValue: "Unknown");
```

## ASCII String Conversion

### ToAsciiString
<xref:Plugin.ByteArrays.ByteArrayExtensions.ToAsciiString*>

Converts byte arrays to ASCII strings (7-bit character set, 0-127).

#### Usage Examples

```csharp
// Basic ASCII conversion
byte[] asciiData = { 0x48, 0x65, 0x6C, 0x6C, 0x6F }; // "Hello"
string text = asciiData.ToAsciiString(0); // "Hello"

// Legacy protocol support
byte[] legacyData = GetLegacyProtocolData();
var position = 0;
string command = legacyData.ToAsciiString(ref position, 8);  // 8-byte command
string parameter = legacyData.ToAsciiString(ref position, 16); // 16-byte parameter

// Fixed-width field parsing
byte[] recordData = GetDatabaseRecord();
position = 0;
string recordId = recordData.ToAsciiString(ref position, 10);     // 10-char ID
string description = recordData.ToAsciiString(ref position, 50);  // 50-char description
string category = recordData.ToAsciiString(ref position, 20);     // 20-char category

// Note: Non-ASCII bytes (128-255) will be converted but may display incorrectly
byte[] extendedData = { 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0xFF };
string extended = extendedData.ToAsciiString(0); // "Hello?" (0xFF becomes ?)
```

## Hexadecimal String Conversion

### ToHexString
<xref:Plugin.ByteArrays.ByteArrayExtensions.ToHexString*>

Converts byte arrays to hexadecimal string representation with customizable formatting.

#### Usage Examples

```csharp
byte[] data = { 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0xFF, 0x00 };

// Basic hex conversion (uppercase, no separators)
string hex1 = data.ToHexString(); // "48656C6C6FFF00"

// Lowercase hex
string hex2 = data.ToHexString(upperCase: false); // "48656c6c6fff00"

// With space separators
string hex3 = data.ToHexString(separator: " "); // "48 65 6C 6C 6F FF 00"

// With colon separators (MAC address style)
byte[] macAddress = { 0x00, 0x1B, 0x63, 0x84, 0x45, 0xE6 };
string mac = macAddress.ToHexString(separator: ":"); // "00:1B:63:84:45:E6"

// With 0x prefix
string hex4 = data.ToHexString(separator: " ", prefix: "0x"); // "0x48 0x65 0x6C 0x6C 0x6F 0xFF 0x00"

// Debug output formatting
byte[] debugData = GetDebugData();
string debugOutput = debugData.ToHexString(separator: " ", upperCase: true);
Console.WriteLine($"Data: {debugOutput}");

// Cryptographic hash display
byte[] sha256Hash = GetSHA256Hash();
string hashString = sha256Hash.ToHexString(); // Standard hash format
```

### FromHexString
<xref:Plugin.ByteArrays.ByteArrayExtensions.FromHexString*>

Converts hexadecimal strings back to byte arrays with flexible input parsing.

#### Usage Examples

```csharp
// Basic hex string parsing
byte[] data1 = ByteArrayExtensions.FromHexString("48656C6C6F"); // { 0x48, 0x65, 0x6C, 0x6C, 0x6F }

// With various separators (automatically removed)
byte[] data2 = ByteArrayExtensions.FromHexString("48 65 6C 6C 6F");       // Spaces
byte[] data3 = ByteArrayExtensions.FromHexString("48:65:6C:6C:6F");       // Colons
byte[] data4 = ByteArrayExtensions.FromHexString("48-65-6C-6C-6F");       // Dashes

// With 0x prefixes (automatically removed)
byte[] data5 = ByteArrayExtensions.FromHexString("0x48 0x65 0x6C 0x6C 0x6F");

// Case insensitive
byte[] data6 = ByteArrayExtensions.FromHexString("48656c6c6f"); // Lowercase
byte[] data7 = ByteArrayExtensions.FromHexString("48656C6C6F"); // Uppercase

// Practical use: Configuration parsing
string configHex = GetConfigurationValue("encryption_key");
byte[] encryptionKey = ByteArrayExtensions.FromHexString(configHex);

// Network MAC address parsing
string macString = "00:1B:63:84:45:E6";
byte[] macBytes = ByteArrayExtensions.FromHexString(macString);

// Error handling
try
{
    byte[] invalid = ByteArrayExtensions.FromHexString("48656C6C6"); // Odd length
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Invalid hex string: {ex.Message}");
}
```

## Base64 String Conversion

### ToBase64String
<xref:Plugin.ByteArrays.ByteArrayExtensions.ToBase64String*>

Converts byte arrays to Base64 encoded strings for safe text transmission.

#### Usage Examples

```csharp
byte[] binaryData = { 0x48, 0x65, 0x6C, 0x6C, 0x6F };
string base64 = binaryData.ToBase64String(); // "SGVsbG8="

// File content encoding
byte[] fileContent = File.ReadAllBytes("image.jpg");
string base64File = fileContent.ToBase64String();

// Network transmission
byte[] binaryMessage = GetBinaryMessage();
string encodedMessage = binaryMessage.ToBase64String();
SendOverNetwork(encodedMessage); // Safe for text protocols

// Email attachment encoding
byte[] attachmentData = GetAttachmentData();
string emailAttachment = attachmentData.ToBase64String();

// Empty array handling
byte[] empty = Array.Empty<byte>();
string emptyBase64 = empty.ToBase64String(); // "" (empty string)
```

### FromBase64String
<xref:Plugin.ByteArrays.ByteArrayExtensions.FromBase64String*>

Converts Base64 strings back to byte arrays.

#### Usage Examples

```csharp
// Basic Base64 decoding
string base64 = "SGVsbG8=";
byte[] decoded = ByteArrayExtensions.FromBase64String(base64); // { 0x48, 0x65, 0x6C, 0x6C, 0x6F }

// File content decoding
string base64FileContent = GetBase64FileContent();
byte[] fileBytes = ByteArrayExtensions.FromBase64String(base64FileContent);
File.WriteAllBytes("decoded_file.jpg", fileBytes);

// Network message decoding
string receivedMessage = ReceiveFromNetwork();
byte[] messageBytes = ByteArrayExtensions.FromBase64String(receivedMessage);

// Email attachment decoding
string attachmentBase64 = GetEmailAttachment();
byte[] attachmentBytes = ByteArrayExtensions.FromBase64String(attachmentBase64);

// Error handling
try
{
    byte[] invalid = ByteArrayExtensions.FromBase64String("Invalid!!!");
}
catch (FormatException ex)
{
    Console.WriteLine($"Invalid Base64: {ex.Message}");
}

// Empty/null handling
byte[] empty1 = ByteArrayExtensions.FromBase64String(""); // Empty array
byte[] empty2 = ByteArrayExtensions.FromBase64String(null); // Empty array
```

## Unicode String Conversions

### ToUtf16String
<xref:Plugin.ByteArrays.ByteArrayExtensions.ToUtf16String*>

Converts byte arrays to UTF-16 encoded strings (Windows Unicode format).

#### Usage Examples

```csharp
// UTF-16 Little Endian (Windows format)
byte[] utf16Data = { 0x48, 0x00, 0x65, 0x00, 0x6C, 0x00, 0x6C, 0x00, 0x6F, 0x00 }; // "Hello"
string text = utf16Data.ToUtf16String(0); // "Hello"

// Windows file system strings
byte[] windowsPath = GetWindowsPathBytes();
string path = windowsPath.ToUtf16String(0);

// Registry value parsing
byte[] registryValue = GetRegistryValue();
var position = 0;
string keyName = registryValue.ToUtf16String(ref position, 32); // 32-byte key name
```

### ToUtf32String
<xref:Plugin.ByteArrays.ByteArrayExtensions.ToUtf32String*>

Converts byte arrays to UTF-32 encoded strings (4 bytes per character).

#### Usage Examples

```csharp
// UTF-32 encoding (4 bytes per character)
byte[] utf32Data = { 0x48, 0x00, 0x00, 0x00, 0x65, 0x00, 0x00, 0x00 }; // "He"
string text = utf32Data.ToUtf32String(0); // "He"

// Unicode code point processing
byte[] unicodeData = GetUnicodeCodePoints();
string unicodeText = unicodeData.ToUtf32String(0);
```

### ToString (Custom Encoding)
<xref:Plugin.ByteArrays.ByteArrayExtensions.ToString*>

Converts byte arrays to strings using custom encoding.

#### Usage Examples

```csharp
// Custom encoding support
byte[] data = GetEncodedData();
var position = 0;

// Latin1 encoding
string latin1 = data.ToString(ref position, Encoding.Latin1, 20);

// Windows-1252 encoding
string windows1252 = data.ToString(ref position, Encoding.GetEncoding(1252), 30);

// Big Endian UTF-16
string bigEndianUtf16 = data.ToString(ref position, Encoding.BigEndianUnicode, 40);
```

## Advanced Usage Patterns

### Protocol Message Parsing
```csharp
byte[] protocolMessage = ReceiveMessage();
var position = 0;

// Mixed string types in one message
byte messageType = protocolMessage.ToByte(ref position);
short nameLength = protocolMessage.ToInt16(ref position);
string playerName = protocolMessage.ToUtf8String(ref position, nameLength);
short messageLength = protocolMessage.ToInt16(ref position);
string chatMessage = protocolMessage.ToUtf8String(ref position, messageLength);

Console.WriteLine($"Player {playerName}: {chatMessage}");
```

### File Format Processing
```csharp
byte[] fileHeader = File.ReadAllBytes("document.dat");
var position = 0;

// Fixed-width string fields
string fileSignature = fileHeader.ToAsciiString(ref position, 4);    // "DOC1"
string version = fileHeader.ToAsciiString(ref position, 8);          // "v2.1.0  "
string author = fileHeader.ToUtf8String(ref position, 32);          // UTF-8 name
string title = fileHeader.ToUtf16String(ref position, 64);          // UTF-16 title

if (fileSignature != "DOC1")
{
    throw new InvalidDataException("Invalid file format");
}
```

### Safe String Extraction
```csharp
byte[] corruptedData = GetPotentiallyCorruptedData();
var extractedStrings = new List<string>();
var position = 0;

while (position < corruptedData.Length)
{
    // Try to extract 16-byte strings safely
    string extracted = corruptedData.ToUtf8StringOrDefault(ref position, 16, defaultValue: null);

    if (extracted != null && !string.IsNullOrWhiteSpace(extracted))
    {
        extractedStrings.Add(extracted.Trim());
    }
    else
    {
        position++; // Skip one byte and try again
    }
}
```

### Encoding Detection and Conversion
```csharp
byte[] unknownTextData = GetUnknownTextData();

// Try different encodings
string[] encodingAttempts = {
    unknownTextData.ToUtf8StringOrDefault(0, defaultValue: null),
    unknownTextData.ToAsciiStringOrDefault(0, defaultValue: null),
    unknownTextData.ToUtf16StringOrDefault(0, defaultValue: null)
};

foreach (var attempt in encodingAttempts.Where(s => s != null))
{
    if (IsValidText(attempt))
    {
        Console.WriteLine($"Detected text: {attempt}");
        break;
    }
}
```

## Performance Considerations

| Operation | Performance | Best For |
|-----------|-------------|----------|
| `ToAsciiString` | Fastest | English text, protocols |
| `ToUtf8String` | Fast | International text, web |
| `ToUtf16String` | Moderate | Windows systems |
| `ToUtf32String` | Slower | Unicode analysis |
| `ToHexString` | Fast | Debug, hashes |
| `ToBase64String` | Fast | Network transmission |

## Encoding Guidelines

- **Use UTF-8** for: Web content, JSON, modern protocols, international text
- **Use ASCII** for: Legacy protocols, command strings, simple identifiers
- **Use UTF-16** for: Windows APIs, .NET string interop, registry values
- **Use Hex** for: Debug output, cryptographic values, binary data display
- **Use Base64** for: Email attachments, JSON binary fields, safe text transmission

## Common Use Cases

1. **Network Protocols**: Parsing text fields in binary messages
2. **File Formats**: Reading string metadata from binary files
3. **Database Records**: Extracting fixed-width text fields
4. **Configuration Files**: Converting hex/Base64 values to binary
5. **Debug Output**: Displaying binary data in readable format
6. **Cryptography**: Converting keys and hashes to/from text
7. **Web Development**: Handling encoded data in APIs
8. **Game Development**: Loading localized text from binary assets

## See Also

- <xref:Plugin.ByteArrays.ByteArrayExtensions> - For character conversions
- <xref:Plugin.ByteArrays.ByteArrayBuilder> - For constructing byte arrays from strings
- <xref:Plugin.ByteArrays.ByteArrayUtilities> - For string analysis and validation
- <xref:Plugin.ByteArrays.ObjectToByteArrayExtensions> - For object serialization
