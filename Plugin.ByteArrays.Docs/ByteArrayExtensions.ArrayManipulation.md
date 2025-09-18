# ByteArrayExtensions.ArrayManipulation

This documentation covers the array manipulation extension methods available in the `ByteArrayExtensions` class. These methods provide powerful operations for slicing, concatenating, reversing, searching, and manipulating byte arrays.

## Array Manipulation Methods

### TrimEnd
<xref:Plugin.ByteArrays.ByteArrayExtensions.TrimEnd*>

Removes trailing bytes with the specified value from the array. Returns a new trimmed array without modifying the original.

#### Usage Examples

```csharp
// Remove trailing null bytes (default behavior)
byte[] data = { 1, 2, 3, 0, 0, 0 };
byte[] trimmed = data.TrimEnd();
// Result: { 1, 2, 3 }

// Remove trailing specific bytes
byte[] dataWithPadding = { 1, 2, 3, 255, 255 };
byte[] trimmedCustom = dataWithPadding.TrimEnd(255);
// Result: { 1, 2, 3 }

// Handle edge cases
byte[] empty = Array.Empty<byte>();
byte[] emptyResult = empty.TrimEnd(); // Returns empty array

byte[] allZeros = { 0, 0, 0 };
byte[] allZerosResult = allZeros.TrimEnd(); // Returns empty array
```

### TrimEndNonDestructive
<xref:Plugin.ByteArrays.ByteArrayExtensions.TrimEndNonDestructive*>

Explicit non-destructive version of TrimEnd for clarity in code where the non-destructive nature is important.

#### Usage Examples

```csharp
byte[] original = { 1, 2, 3, 0, 0 };
byte[] trimmed = original.TrimEndNonDestructive();
// original remains unchanged: { 1, 2, 3, 0, 0 }
// trimmed: { 1, 2, 3 }
```

### SafeSlice
<xref:Plugin.ByteArrays.ByteArrayExtensions.SafeSlice*>

Safely extracts a portion of a byte array without throwing exceptions on invalid parameters.

#### Usage Examples

```csharp
byte[] data = { 1, 2, 3, 4, 5 };

// Normal slicing
byte[] slice1 = data.SafeSlice(1, 3);
// Result: { 2, 3, 4 }

// Handle out-of-bounds gracefully
byte[] slice2 = data.SafeSlice(3, 10); // Requests more than available
// Result: { 4, 5 } - returns what's available

byte[] slice3 = data.SafeSlice(10, 5); // Start beyond array
// Result: { } - returns empty array

byte[] slice4 = data.SafeSlice(-1, 3); // Invalid start
// Result: { } - returns empty array
```

### Concatenate
<xref:Plugin.ByteArrays.ByteArrayExtensions.Concatenate*>

Efficiently combines multiple byte arrays into a single array.

#### Usage Examples

```csharp
byte[] array1 = { 1, 2 };
byte[] array2 = { 3, 4 };
byte[] array3 = { 5, 6 };

// Concatenate multiple arrays
byte[] combined = ByteArrayExtensions.Concatenate(array1, array2, array3);
// Result: { 1, 2, 3, 4, 5, 6 }

// Handle null arrays gracefully
byte[] withNulls = ByteArrayExtensions.Concatenate(array1, null, array2);
// Result: { 1, 2, 3, 4 } - null arrays are treated as empty

// Single array
byte[] single = ByteArrayExtensions.Concatenate(array1);
// Result: { 1, 2 }

// Empty result
byte[] empty = ByteArrayExtensions.Concatenate();
// Result: { }
```

### Reverse
<xref:Plugin.ByteArrays.ByteArrayExtensions.Reverse*>

Creates a new byte array with elements in reverse order.

#### Usage Examples

```csharp
byte[] data = { 1, 2, 3, 4, 5 };
byte[] reversed = data.Reverse();
// Result: { 5, 4, 3, 2, 1 }

// Original array remains unchanged
Console.WriteLine(string.Join(", ", data)); // Still: 1, 2, 3, 4, 5

// Handle edge cases
byte[] empty = Array.Empty<byte>();
byte[] emptyReversed = empty.Reverse(); // Returns empty array

byte[] single = { 42 };
byte[] singleReversed = single.Reverse(); // Returns { 42 }
```

### Xor
<xref:Plugin.ByteArrays.ByteArrayExtensions.Xor*>

Performs bitwise XOR operation between two byte arrays of equal length.

#### Usage Examples

```csharp
byte[] array1 = { 0b11110000, 0b10101010 };
byte[] array2 = { 0b00001111, 0b01010101 };

byte[] xorResult = array1.Xor(array2);
// Result: { 0b11111111, 0b11111111 } or { 255, 255 }

// Practical use case: Simple encryption/decryption
byte[] message = { 72, 101, 108, 108, 111 }; // "Hello"
byte[] key = { 123, 45, 67, 89, 12 };

byte[] encrypted = message.Xor(key);
byte[] decrypted = encrypted.Xor(key); // XOR again to decrypt
// decrypted equals original message

// Error handling
try
{
    byte[] short1 = { 1, 2 };
    byte[] long1 = { 1, 2, 3, 4 };
    var invalid = short1.Xor(long1); // Throws ArgumentException
}
catch (ArgumentException ex)
{
    Console.WriteLine("Arrays must have the same length");
}
```

## Advanced Search Operations

### IndexOfAll
<xref:Plugin.ByteArrays.ByteArrayExtensions.IndexOfAll*>

Finds all occurrences of a byte pattern within the array and returns their starting indices.

#### Usage Examples

```csharp
byte[] data = { 1, 2, 3, 1, 2, 4, 1, 2, 3, 5 };
byte[] pattern = { 1, 2 };

int[] indices = data.IndexOfAll(pattern);
// Result: { 0, 3, 6 } - pattern found at positions 0, 3, and 6

// More complex pattern
byte[] complexData = { 0xAA, 0xBB, 0xCC, 0xAA, 0xBB, 0xDD, 0xAA, 0xBB, 0xCC };
byte[] complexPattern = { 0xAA, 0xBB, 0xCC };

int[] complexIndices = complexData.IndexOfAll(complexPattern);
// Result: { 0, 6 } - pattern found at positions 0 and 6

// No matches
byte[] noMatch = data.IndexOfAll(new byte[] { 9, 9 });
// Result: { } - empty array

// Edge cases
byte[] emptyPattern = data.IndexOfAll(Array.Empty<byte>());
// Result: { } - empty pattern returns no matches
```

## Bit Operations

### SetBit
<xref:Plugin.ByteArrays.ByteArrayExtensions.SetBit*>

Sets a specific bit position to 1 and returns a new array.

#### Usage Examples

```csharp
byte[] data = { 0b00000000, 0b00000000 }; // { 0, 0 }

// Set bit 3 (4th bit from right in first byte)
byte[] withBit3 = data.SetBit(3);
// Result: { 0b00001000, 0b00000000 } or { 8, 0 }

// Set bit 9 (2nd bit in second byte)
byte[] withBit9 = data.SetBit(9);
// Result: { 0b00000000, 0b00000010 } or { 0, 2 }

// Chain operations (each returns new array)
byte[] multiple = data.SetBit(0).SetBit(7).SetBit(8);
// Result: { 0b10000001, 0b00000001 } or { 129, 1 }
```

### GetBit
<xref:Plugin.ByteArrays.ByteArrayExtensions.GetBit*>

Checks if a specific bit is set (returns true) or unset (returns false).

#### Usage Examples

```csharp
byte[] data = { 0b10101010, 0b01010101 }; // { 170, 85 }

// Check individual bits
bool bit0 = data.GetBit(0); // false (LSB of first byte)
bool bit1 = data.GetBit(1); // true
bool bit2 = data.GetBit(2); // false
bool bit3 = data.GetBit(3); // true

// Check bits in second byte
bool bit8 = data.GetBit(8);  // true (LSB of second byte)
bool bit9 = data.GetBit(9);  // false

// Practical use: Check flags
const int FLAG_ENABLED = 0;
const int FLAG_VISIBLE = 1;
const int FLAG_LOCKED = 2;

bool isEnabled = data.GetBit(FLAG_ENABLED);
bool isVisible = data.GetBit(FLAG_VISIBLE);
bool isLocked = data.GetBit(FLAG_LOCKED);
```

### BitReverse
<xref:Plugin.ByteArrays.ByteArrayExtensions.BitReverse*>

Reverses the bits within each byte (not the byte order).

#### Usage Examples

```csharp
byte[] data = { 0b11110000, 0b10101010 }; // { 240, 170 }

byte[] bitReversed = data.BitReverse();
// Result: { 0b00001111, 0b01010101 } or { 15, 85 }

// Practical use: Endianness conversion for certain protocols
byte[] protocolData = { 0b11000000, 0b00000011 };
byte[] converted = protocolData.BitReverse();
// Result: { 0b00000011, 0b11000000 }

// Note: This reverses bits within each byte, not the byte order
// For byte order reversal, use the Reverse() method
```

## Splitting and Chunking

### Split
<xref:Plugin.ByteArrays.ByteArrayExtensions.Split*>

Divides a byte array into fixed-size chunks.

#### Usage Examples

```csharp
byte[] data = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

// Split into chunks of 3
var chunks = data.Split(3).ToArray();
// Result:
// chunks[0] = { 1, 2, 3 }
// chunks[1] = { 4, 5, 6 }
// chunks[2] = { 7, 8, 9 }
// chunks[3] = { 10 } - last chunk may be smaller

// Process each chunk
foreach (var chunk in data.Split(4))
{
    Console.WriteLine($"Chunk: [{string.Join(", ", chunk)}]");
}

// Practical use: Processing large data in blocks
byte[] largeFile = File.ReadAllBytes("largefile.bin");
foreach (var block in largeFile.Split(1024)) // 1KB blocks
{
    // Process each 1KB block
    ProcessBlock(block);
}
```

### SplitAt
<xref:Plugin.ByteArrays.ByteArrayExtensions.SplitAt*>

Splits a byte array at occurrences of a delimiter byte.

#### Usage Examples

```csharp
// Split text-like data
byte[] csvData = { 65, 44, 66, 44, 67, 44, 68 }; // "A,B,C,D" in ASCII
var fields = csvData.SplitAt(44).ToArray(); // Split at comma (44)
// Result:
// fields[0] = { 65 } // "A"
// fields[1] = { 66 } // "B"
// fields[2] = { 67 } // "C"
// fields[3] = { 68 } // "D"

// Split binary protocol messages
byte[] messages = { 0x01, 0x02, 0x00, 0x03, 0x04, 0x00, 0x05 };
var packets = messages.SplitAt(0x00).ToArray(); // Split at null byte
// Result:
// packets[0] = { 0x01, 0x02 }
// packets[1] = { 0x03, 0x04 }
// packets[2] = { 0x05 }

// Handle multiple consecutive delimiters
byte[] withEmpty = { 1, 0, 0, 2, 0, 3 };
var segments = withEmpty.SplitAt(0).ToArray();
// Result includes empty segments:
// segments[0] = { 1 }
// segments[1] = { } // empty
// segments[2] = { 2 }
// segments[3] = { 3 }
```

## Checksums and Hashing

### CalculateCrc32
<xref:Plugin.ByteArrays.ByteArrayExtensions.CalculateCrc32*>

Computes the CRC32 checksum for data integrity verification.

#### Usage Examples

```csharp
byte[] data = { 1, 2, 3, 4, 5 };
uint crc = data.CalculateCrc32();
// Result: CRC32 checksum as uint

// Verify data integrity
byte[] originalData = File.ReadAllBytes("file.dat");
uint originalCrc = originalData.CalculateCrc32();

// Later, after transmission or storage
byte[] receivedData = ReceiveData();
uint receivedCrc = receivedData.CalculateCrc32();

if (originalCrc == receivedCrc)
{
    Console.WriteLine("Data integrity verified");
}
else
{
    Console.WriteLine("Data corruption detected");
}

// Convert to hex string for display
string crcHex = crc.ToString("X8"); // 8-digit uppercase hex
```

### CalculateMd5
<xref:Plugin.ByteArrays.ByteArrayExtensions.CalculateMd5*>

Computes MD5 hash (legacy support - not recommended for security).

#### Usage Examples

```csharp
byte[] data = "Hello World"u8.ToArray();
byte[] md5Hash = data.CalculateMd5();

// Convert to hex string
string md5Hex = Convert.ToHexString(md5Hash);
// Result: "B10A8DB164E0754105B7A99BE72E3FE5"

// Note: MD5 is cryptographically broken
// Use only for non-security purposes like checksums
byte[] fileContent = File.ReadAllBytes("document.pdf");
byte[] fileHash = fileContent.CalculateMd5();
string fingerprint = Convert.ToHexString(fileHash);
```

### CalculateSha256
<xref:Plugin.ByteArrays.ByteArrayExtensions.CalculateSha256*>

Computes secure SHA-256 hash.

#### Usage Examples

```csharp
byte[] password = "mySecretPassword"u8.ToArray();
byte[] sha256Hash = password.CalculateSha256();

// Store hash for verification
string storedHash = Convert.ToBase64String(sha256Hash);

// Later verification
byte[] inputPassword = GetUserInput();
byte[] inputHash = inputPassword.CalculateSha256();
bool isValid = storedHash == Convert.ToBase64String(inputHash);

// File integrity verification
byte[] fileData = File.ReadAllBytes("important.zip");
byte[] fileHash = fileData.CalculateSha256();
string hashString = Convert.ToHexString(fileHash);
Console.WriteLine($"SHA-256: {hashString}");
```

### CalculateSha1
<xref:Plugin.ByteArrays.ByteArrayExtensions.CalculateSha1*>

Computes SHA-1 hash (weak algorithm - legacy support only).

#### Usage Examples

```csharp
byte[] data = "test data"u8.ToArray();
byte[] sha1Hash = data.CalculateSha1();

// Convert to hex (common format for SHA-1)
string sha1Hex = Convert.ToHexString(sha1Hash);
// Result: 40-character hex string

// Note: SHA-1 is considered weak
// Use SHA-256 or higher for security-critical applications
```

## Padding Operations

### Pad
<xref:Plugin.ByteArrays.ByteArrayExtensions.Pad*>

Extends an array to a specified length by adding padding bytes.

#### Usage Examples

```csharp
byte[] data = { 1, 2, 3 };

// Pad to the right (default)
byte[] paddedRight = data.Pad(6);
// Result: { 1, 2, 3, 0, 0, 0 }

// Pad to the left
byte[] paddedLeft = data.Pad(6, padLeft: true);
// Result: { 0, 0, 0, 1, 2, 3 }

// Pad with custom byte
byte[] paddedCustom = data.Pad(6, paddingByte: 0xFF);
// Result: { 1, 2, 3, 255, 255, 255 }

// Pad left with custom byte
byte[] paddedLeftCustom = data.Pad(6, paddingByte: 0xAA, padLeft: true);
// Result: { 170, 170, 170, 1, 2, 3 }

// No padding needed (array already long enough)
byte[] alreadyLong = { 1, 2, 3, 4, 5, 6, 7 };
byte[] noPadding = alreadyLong.Pad(5);
// Result: { 1, 2, 3, 4, 5, 6, 7 } - unchanged

// Practical use: Align data to block boundaries
byte[] message = "Hello"u8.ToArray();
byte[] aligned = message.Pad(16); // Pad to 16-byte boundary
```

### RemovePadding
<xref:Plugin.ByteArrays.ByteArrayExtensions.RemovePadding*>

Removes padding bytes from an array.

#### Usage Examples

```csharp
// Remove padding from the right (default)
byte[] paddedData = { 1, 2, 3, 0, 0, 0 };
byte[] unpadded = paddedData.RemovePadding();
// Result: { 1, 2, 3 }

// Remove padding from the left
byte[] leftPadded = { 0, 0, 0, 1, 2, 3 };
byte[] unpaddedLeft = leftPadded.RemovePadding(fromLeft: true);
// Result: { 1, 2, 3 }

// Remove custom padding byte
byte[] customPadded = { 1, 2, 3, 255, 255 };
byte[] unpaddedCustom = customPadded.RemovePadding(paddingByte: 255);
// Result: { 1, 2, 3 }

// Remove custom padding from left
byte[] leftCustomPadded = { 170, 170, 1, 2, 3 };
byte[] unpaddedLeftCustom = leftCustomPadded.RemovePadding(
    paddingByte: 170,
    fromLeft: true);
// Result: { 1, 2, 3 }

// Handle all-padding array
byte[] allPadding = { 0, 0, 0, 0 };
byte[] allRemoved = allPadding.RemovePadding();
// Result: { } - empty array

// Practical use: Clean up protocol messages
byte[] receivedBlock = ReceiveAlignedBlock(); // May have padding
byte[] actualMessage = receivedBlock.RemovePadding();
```

## Performance Notes

- All methods create new arrays and are non-destructive to the original data
- For frequent operations on large arrays, consider using `Span<byte>` for better performance
- The `Concatenate` method pre-calculates total length for optimal memory allocation
- Bit operations work on individual bits within the byte array structure
- Hash calculations use the optimized .NET implementations

## Thread Safety

All extension methods are thread-safe as they:
- Do not modify input arrays
- Create new arrays for results
- Use only read operations on input data
- Rely on thread-safe .NET framework methods

## See Also

- <xref:Plugin.ByteArrays.ByteArrayExtensions> - Main extension class
- <xref:Plugin.ByteArrays.ByteArrayBuilder> - For efficient byte array construction
- <xref:Plugin.ByteArrays.ByteArrayUtilities> - Analysis and performance measurement tools
