# ByteArrayExtensions.ComplexTypeConversion

This documentation covers complex type conversion extension methods in `ByteArrayExtensions`. These methods handle more sophisticated data types including Version objects and strongly-typed enums with validation.

## Overview

Complex type conversion methods provide safe, validated conversions for:
- **Version objects** from UTF-8 string representations
- **Enum types** with compile-time type safety and runtime validation
- **Flags enums** with bitwise validation
- **Type safety** ensuring invalid values are rejected

## Version Conversion

### ToVersion
<xref:Plugin.ByteArrays.ByteArrayExtensions.ToVersion*>

Converts UTF-8 string data to System.Version objects with proper parsing.

#### Usage Examples

```csharp
// Basic version parsing
byte[] versionData = "1.2.3.4"u8.ToArray();
var position = 0;

Version version = versionData.ToVersion(ref position);
Console.WriteLine($"Version: {version}"); // 1.2.3.4
Console.WriteLine($"Position: {position}"); // 7

// Fixed position parsing
byte[] mixedData = GetMixedData();
Version appVersion = mixedData.ToVersion(0, 10); // Read exactly 10 bytes

// File header version parsing
byte[] fileHeader = File.ReadAllBytes("data.bin");
position = 8; // Skip file signature
Version fileFormat = fileHeader.ToVersion(ref position, 6); // "2.1.0\0"

// Protocol version negotiation
byte[] handshakeData = ReceiveHandshake();
position = 0;
byte protocolType = handshakeData.ToByte(ref position);
Version protocolVersion = handshakeData.ToVersion(ref position, 8);

if (protocolVersion < new Version(2, 0))
{
    throw new NotSupportedException($"Protocol version {protocolVersion} not supported");
}

// Assembly version reading
byte[] assemblyMetadata = GetAssemblyMetadata();
position = 0;
Version assemblyVersion = assemblyMetadata.ToVersion(ref position, 12);
Version fileVersion = assemblyMetadata.ToVersion(ref position, 12);

Console.WriteLine($"Assembly: {assemblyVersion}, File: {fileVersion}");

// Common version formats
byte[] semverData = "2.1.0-beta"u8.ToArray();
Version semver = semverData.ToVersion(0); // Parses as 2.1.0.0

byte[] buildData = "1.0.0.12345"u8.ToArray();
Version buildVersion = buildData.ToVersion(0); // 1.0.0.12345
```

### ToVersionOrDefault
<xref:Plugin.ByteArrays.ByteArrayExtensions.ToVersionOrDefault*>

Safe version parsing with default value fallback for invalid or missing data.

#### Usage Examples

```csharp
byte[] corruptData = { 0x01, 0x02, 0x03 }; // Invalid version string
var position = 0;

// Use default version for invalid data
Version defaultVersion = new Version(1, 0, 0);
Version parsed = corruptData.ToVersionOrDefault(ref position, defaultValue: defaultVersion);
Console.WriteLine($"Parsed: {parsed}"); // 1.0.0.0
Console.WriteLine($"Position: {position}"); // 0 (not advanced on failure)

// Safe configuration parsing
byte[] configData = GetConfigurationData();
position = 0;
Version minVersion = configData.ToVersionOrDefault(ref position, 8, new Version(1, 0));
Version maxVersion = configData.ToVersionOrDefault(ref position, 8, new Version(99, 0));

// Backward compatibility handling
byte[] legacyFile = GetLegacyFile();
Version fileVersion = legacyFile.ToVersionOrDefault(4, 6, new Version(0, 9));

if (fileVersion < new Version(1, 0))
{
    Console.WriteLine("Using legacy compatibility mode");
    ProcessLegacyFormat(legacyFile);
}
else
{
    ProcessModernFormat(legacyFile);
}
```

## Enum Conversion

### ToEnum<T>
<xref:Plugin.ByteArrays.ByteArrayExtensions.ToEnum*>

Converts byte arrays to strongly-typed enum values with comprehensive validation.

#### Usage Examples

```csharp
// Define example enums
public enum FileType : byte
{
    Unknown = 0,
    Text = 1,
    Binary = 2,
    Image = 3,
    Archive = 4
}

[Flags]
public enum Permissions : uint
{
    None = 0,
    Read = 1,
    Write = 2,
    Execute = 4,
    Delete = 8,
    ReadWrite = Read | Write,
    FullControl = Read | Write | Execute | Delete
}

public enum Priority : short
{
    Low = -1,
    Normal = 0,
    High = 1,
    Critical = 2
}

// Basic enum conversion (1 byte enum)
byte[] fileData = { 3, 1, 2, 0 };
var position = 0;

FileType type1 = fileData.ToEnum<FileType>(ref position); // Image
FileType type2 = fileData.ToEnum<FileType>(ref position); // Text
FileType type3 = fileData.ToEnum<FileType>(ref position); // Binary
FileType type4 = fileData.ToEnum<FileType>(ref position); // Unknown

Console.WriteLine($"Position: {position}"); // 4

// Fixed position access
FileType specificType = fileData.ToEnum<FileType>(2); // Binary

// Flags enum conversion (4 bytes)
byte[] permData = { 0x0F, 0x00, 0x00, 0x00 }; // 15 = Read|Write|Execute|Delete
Permissions perms = permData.ToEnum<Permissions>(0); // FullControl

Console.WriteLine($"Has Read: {perms.HasFlag(Permissions.Read)}"); // True
Console.WriteLine($"Has Write: {perms.HasFlag(Permissions.Write)}"); // True

// Different underlying types
byte[] priorityData = { 0xFF, 0xFF, 0x01, 0x00 }; // -1 as short, then 1
position = 0;
Priority lowPriority = priorityData.ToEnum<Priority>(ref position);  // Low (-1)
Priority highPriority = priorityData.ToEnum<Priority>(ref position); // High (1)

// Protocol message parsing
byte[] messageData = ReceiveMessage();
position = 0;
byte messageLength = messageData.ToByte(ref position);
MessageType msgType = messageData.ToEnum<MessageType>(ref position);
Priority msgPriority = messageData.ToEnum<Priority>(ref position);

// File format enum parsing
byte[] formatData = GetFileFormatData();
position = 0;
CompressionType compression = formatData.ToEnum<CompressionType>(ref position);
EncryptionType encryption = formatData.ToEnum<EncryptionType>(ref position);

// Validation ensures only valid enum values
try
{
    byte[] invalidData = { 99 }; // Not a valid FileType
    FileType invalid = invalidData.ToEnum<FileType>(0); // Throws ArgumentException
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Invalid enum value: {ex.Message}");
}
```

### ToEnumOrDefault<T>
<xref:Plugin.ByteArrays.ByteArrayExtensions.ToEnumOrDefault*>

Safe enum conversion with default value fallback for invalid data.

#### Usage Examples

```csharp
byte[] invalidData = { 99 }; // Invalid FileType value
var position = 0;

// Safe conversion with default
FileType safeType = invalidData.ToEnumOrDefault<FileType>(ref position, FileType.Unknown);
Console.WriteLine($"Type: {safeType}"); // Unknown
Console.WriteLine($"Position: {position}"); // 0 (not advanced on failure)

// Safe protocol parsing
byte[] protocolData = GetProtocolData();
position = 0;
MessageType msgType = protocolData.ToEnumOrDefault<MessageType>(ref position, MessageType.Heartbeat);
Priority priority = protocolData.ToEnumOrDefault<Priority>(ref position, Priority.Normal);

// Graceful degradation for unknown values
byte[] featureFlags = GetFeatureFlags();
position = 0;

while (position < featureFlags.Length - 3) // Ensure 4 bytes for uint enum
{
    FeatureFlag flag = featureFlags.ToEnumOrDefault<FeatureFlag>(ref position, FeatureFlag.None);
    if (flag != FeatureFlag.None)
    {
        EnableFeature(flag);
    }
}

// Backward compatibility with new enum values
byte[] futureData = GetDataFromNewerVersion();
position = 0;
NewFeature feature = futureData.ToEnumOrDefault<NewFeature>(ref position, NewFeature.LegacyMode);

if (feature == NewFeature.LegacyMode)
{
    Console.WriteLine("Unknown feature, using legacy mode");
}
```

## Advanced Usage Patterns

### Mixed Complex Types in Protocols
```csharp
byte[] protocolHeader = ReceiveHeader();
var position = 0;

// Parse complex protocol header
Version protocolVersion = protocolHeader.ToVersion(ref position, 8);
MessageType messageType = protocolHeader.ToEnum<MessageType>(ref position);
Permissions requiredPerms = protocolHeader.ToEnum<Permissions>(ref position);
Priority messagePriority = protocolHeader.ToEnum<Priority>(ref position);

Console.WriteLine($"Protocol {protocolVersion}, Type: {messageType}, Perms: {requiredPerms}, Priority: {messagePriority}");
```

### Flags Enum Validation
```csharp
[Flags]
public enum AccessRights : byte
{
    None = 0,
    Read = 1,
    Write = 2,
    Execute = 4,
    Admin = 8
}

byte[] accessData = { 0x0F }; // All flags set
AccessRights rights = accessData.ToEnum<AccessRights>(0);

// Check individual flags
if (rights.HasFlag(AccessRights.Read))
    Console.WriteLine("Has read access");

if (rights.HasFlag(AccessRights.Write))
    Console.WriteLine("Has write access");

// Invalid flags combination (bit 5 set, not defined)
try
{
    byte[] invalidFlags = { 0x20 }; // Bit 5 set (32), not defined in enum
    AccessRights invalid = invalidFlags.ToEnum<AccessRights>(0); // Throws
}
catch (ArgumentException)
{
    Console.WriteLine("Invalid flags combination detected");
}
```

### Version Comparison and Compatibility
```csharp
byte[] versionInfo = GetVersionInfo();
var position = 0;

Version currentVersion = versionInfo.ToVersion(ref position, 6);
Version minimumVersion = versionInfo.ToVersion(ref position, 6);
Version maximumVersion = versionInfo.ToVersion(ref position, 6);

// Version compatibility checks
Version myVersion = new Version(2, 1, 0);

if (myVersion < minimumVersion)
{
    throw new InvalidOperationException($"Version {myVersion} is too old. Minimum: {minimumVersion}");
}

if (myVersion > maximumVersion)
{
    Console.WriteLine($"Warning: Version {myVersion} is newer than tested version {maximumVersion}");
}

// Feature availability based on version
if (currentVersion >= new Version(2, 0))
{
    EnableAdvancedFeatures();
}

if (currentVersion.Major >= 3)
{
    EnableNextGenFeatures();
}
```

### Safe Complex Data Parsing
```csharp
byte[] complexData = GetComplexData();
var results = new List<DataRecord>();
var position = 0;

while (position < complexData.Length)
{
    // Try to parse each record safely
    var record = new DataRecord();

    record.Version = complexData.ToVersionOrDefault(ref position, 8, new Version(1, 0));
    record.Type = complexData.ToEnumOrDefault<RecordType>(ref position, RecordType.Unknown);
    record.Status = complexData.ToEnumOrDefault<Status>(ref position, Status.Pending);

    // Skip record if parsing failed
    if (record.Type == RecordType.Unknown && record.Status == Status.Pending)
    {
        position++; // Skip one byte and try again
        continue;
    }

    results.Add(record);
}

Console.WriteLine($"Successfully parsed {results.Count} records");
```

## Type Safety Features

### Compile-Time Type Safety
```csharp
// Generic constraint ensures only enums can be used
public static void ProcessEnum<T>(byte[] data) where T : Enum
{
    T value = data.ToEnum<T>(0); // Compile-time safe
    Console.WriteLine($"Processed {typeof(T).Name}: {value}");
}

// Usage
ProcessEnum<FileType>(fileData);
ProcessEnum<Priority>(priorityData);
// ProcessEnum<string>(stringData); // Compile error - string is not an enum
```

### Runtime Validation
```csharp
// Enum validation catches:
// 1. Out-of-range values for regular enums
// 2. Invalid flag combinations for [Flags] enums
// 3. Insufficient data for the underlying type

public enum Color : int { Red = 1, Green = 2, Blue = 3 }

byte[] colorData = { 0x04, 0x00, 0x00, 0x00 }; // 4 (invalid)

try
{
    Color color = colorData.ToEnum<Color>(0); // Throws ArgumentException
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Invalid color value: {ex.Message}");
    // Message includes valid values: "Valid values are: Red, Green, Blue"
}
```

## Performance Characteristics

| Operation | Performance | Notes |
|-----------|-------------|-------|
| `ToVersion` | Moderate | UTF-8 parsing + Version.Parse |
| `ToEnum<T>` (byte) | Fast | Direct cast + validation |
| `ToEnum<T>` (larger) | Fast | BitConverter + validation |
| `OrDefault` variants | Fast | Exception-free validation |

## Best Practices

- **Use specific enum types** rather than generic integer conversions for type safety
- **Validate enum values** using the built-in validation to catch data corruption
- **Handle version compatibility** with proper comparison logic
- **Use OrDefault variants** for robust parsing of potentially corrupted data
- **Define meaningful default values** for OrDefault methods
- **Document enum underlying types** to ensure correct byte array sizing

## Common Use Cases

1. **Protocol Messages**: Type-safe parsing of message types and flags
2. **File Formats**: Reading format versions and type indicators
3. **Configuration Files**: Safe parsing of settings and feature flags
4. **Network Communication**: Protocol version negotiation
5. **Data Serialization**: Type-safe enum storage and retrieval
6. **Legacy System Integration**: Handling unknown enum values gracefully
7. **Version Management**: Application and data format versioning

## See Also

- <xref:Plugin.ByteArrays.ByteArrayExtensions> - For underlying type conversions and Version string parsing
- <xref:Plugin.ByteArrays.ByteArrayBuilder> - For constructing byte arrays from complex types
- <xref:Plugin.ByteArrays.ObjectToByteArrayExtensions> - For object serialization
