# ReadOnlySpanUtilities

## Class/Module Overview

The `ReadOnlySpanUtilities` class provides utility functions for analyzing and manipulating `ReadOnlySpan<byte>` data. These methods complement the type conversion operations with analytical, statistical, and transformation capabilities designed for data inspection, quality analysis, and manipulation scenarios.

### Purpose
- **Data analysis**: Calculate entropy, analyze byte distributions, and gather statistics
- **Pattern detection**: Find occurrences and analyze byte patterns
- **Data validation**: Check for specific conditions (all zeros, all equal bytes)
- **Data transformation**: Reverse, XOR, and other operations
- **Quality assurance**: Entropy calculation for randomness testing

### Capabilities
- Shannon entropy calculation
- Byte frequency distribution analysis
- Occurrence counting and index finding
- Equality and zero-checking operations
- Byte reversal and XOR operations

## API Documentation

### Analysis Methods

#### CalculateEntropy
```csharp
public static double CalculateEntropy(this ReadOnlySpan<byte> span)
```
Calculates the Shannon entropy of the ReadOnlySpan<byte>. Entropy is a measure of randomness/unpredictability in the data.

**Returns:** A value between 0.0 (perfectly ordered) and 8.0 (maximum entropy for bytes)

**Performance:** O(n) where n is span length

**Example:**
```csharp
// Maximum entropy (all byte values present once)
var bytes = new byte[256];
for (var i = 0; i < 256; i++) bytes[i] = (byte)i;
ReadOnlySpan<byte> span = bytes;
double entropy = span.CalculateEntropy(); // Returns: 8.0

// No entropy (all same value)
ReadOnlySpan<byte> uniform = new byte[] { 5, 5, 5, 5 };
double noEntropy = uniform.CalculateEntropy(); // Returns: 0.0
```

**Use Cases:**
- Testing random number generators
- Validating encryption output
- Detecting compression opportunities
- Quality assurance for randomized data

#### AnalyzeDistribution
```csharp
public static Dictionary<byte, int> AnalyzeDistribution(this ReadOnlySpan<byte> span)
```
Analyzes the distribution of byte values in the ReadOnlySpan<byte>.

**Returns:** A dictionary mapping each byte value to its frequency count

**Performance:** O(n) where n is span length

**Example:**
```csharp
ReadOnlySpan<byte> span = new byte[] { 1, 2, 1, 3, 2, 1 };
var distribution = span.AnalyzeDistribution();
// Result: { 1: 3, 2: 2, 3: 1 }

foreach (var kvp in distribution)
{
    Console.WriteLine($"Byte {kvp.Key}: {kvp.Value} occurrences");
}
```

**Use Cases:**
- Histogram generation
- Data profiling
- Compression analysis
- Pattern detection

### Search and Count Methods

#### CountOccurrences
```csharp
public static int CountOccurrences(this ReadOnlySpan<byte> span, byte value)
```
Counts the occurrences of a specific byte value in the ReadOnlySpan<byte>.

**Performance:** O(n) single-pass

**Example:**
```csharp
ReadOnlySpan<byte> span = new byte[] { 1, 2, 3, 2, 4, 2, 5 };
int count = span.CountOccurrences(2); // Returns: 3
```

#### FindAllIndices
```csharp
public static int[] FindAllIndices(this ReadOnlySpan<byte> span, byte value)
```
Finds all indices where a specific byte value occurs in the ReadOnlySpan<byte>.

**Returns:** An array of indices where the byte value was found

**Performance:** O(n) with potential allocation for results array

**Example:**
```csharp
ReadOnlySpan<byte> span = new byte[] { 1, 2, 3, 2, 4, 2, 5 };
int[] indices = span.FindAllIndices(2); // Returns: [1, 3, 5]

foreach (var index in indices)
{
    Console.WriteLine($"Found at position {index}");
}
```

### Validation Methods

#### AllBytesEqual
```csharp
public static bool AllBytesEqual(this ReadOnlySpan<byte> span)
```
Checks if all bytes in the ReadOnlySpan<byte> have the same value.

**Performance:** O(n) best case O(1), worst case O(n)

**Example:**
```csharp
ReadOnlySpan<byte> uniform = new byte[] { 5, 5, 5, 5 };
bool allSame = uniform.AllBytesEqual(); // Returns: true

ReadOnlySpan<byte> mixed = new byte[] { 5, 5, 6, 5 };
bool notAllSame = mixed.AllBytesEqual(); // Returns: false
```

**Use Cases:**
- Validating initialization
- Detecting padding
- Quality checks

#### IsAllZeros
```csharp
public static bool IsAllZeros(this ReadOnlySpan<byte> span)
```
Checks if the ReadOnlySpan<byte> contains only zero bytes.

**Performance:** O(n) best case O(1), worst case O(n)

**Example:**
```csharp
ReadOnlySpan<byte> zeros = new byte[100]; // Default-initialized to zeros
bool isZero = zeros.IsAllZeros(); // Returns: true

ReadOnlySpan<byte> hasData = new byte[] { 0, 0, 1, 0 };
bool notZero = hasData.IsAllZeros(); // Returns: false
```

**Use Cases:**
- Validating cleared buffers
- Detecting empty/initialized regions
- Security checks for sensitive data clearing

### Transformation Methods

#### Reverse
```csharp
public static byte[] Reverse(this ReadOnlySpan<byte> span)
```
Reverses the bytes in a copy of the ReadOnlySpan<byte>. Note: This creates a new array since ReadOnlySpan is immutable.

**Returns:** A new byte array with reversed byte order

**Performance:** O(n) with allocation

**Example:**
```csharp
ReadOnlySpan<byte> span = new byte[] { 1, 2, 3, 4, 5 };
byte[] reversed = span.Reverse(); // Returns: [5, 4, 3, 2, 1]
```

**Use Cases:**
- Endianness conversion
- String reversal for byte-encoded text
- Protocol data manipulation

#### Xor
```csharp
public static byte[] Xor(this ReadOnlySpan<byte> span1, ReadOnlySpan<byte> span2)
```
Performs an XOR operation between two ReadOnlySpan<byte> instances. If spans have different lengths, operates on the shorter length.

**Returns:** A new byte array containing the XOR result

**Performance:** O(min(n,m)) where n and m are span lengths

**Example:**
```csharp
ReadOnlySpan<byte> key = new byte[] { 0xFF, 0x00, 0xAA };
ReadOnlySpan<byte> data = new byte[] { 0x0F, 0xFF, 0x55 };
byte[] xored = key.Xor(data); // Returns: [0xF0, 0xFF, 0xFF]

// XOR again to decrypt
byte[] original = xored.AsSpan().Xor(key); // Returns: [0x0F, 0xFF, 0x55]
```

**Use Cases:**
- Simple encryption/decryption
- Checksum calculation
- Data masking
- Bitwise operations

## Practical Examples

### Data Quality Analysis
```csharp
public class DataQualityReport
{
    public static void AnalyzeData(byte[] data)
    {
        ReadOnlySpan<byte> span = data;
        
        // Calculate entropy
        double entropy = span.CalculateEntropy();
        Console.WriteLine($"Entropy: {entropy:F2} bits");
        
        // Check for patterns
        if (span.IsAllZeros())
        {
            Console.WriteLine("WARNING: Data is all zeros!");
        }
        else if (span.AllBytesEqual())
        {
            Console.WriteLine($"WARNING: All bytes are {span[0]}");
        }
        
        // Analyze distribution
        var distribution = span.AnalyzeDistribution();
        Console.WriteLine($"Unique bytes: {distribution.Count}");
        
        // Find most common byte
        var mostCommon = distribution.OrderByDescending(kvp => kvp.Value).First();
        Console.WriteLine($"Most common: {mostCommon.Key} ({mostCommon.Value} times)");
    }
}
```

### Simple XOR Cipher
```csharp
public class XorCipher
{
    public static byte[] Encrypt(ReadOnlySpan<byte> data, ReadOnlySpan<byte> key)
    {
        // Expand key to match data length
        var expandedKey = new byte[data.Length];
        for (int i = 0; i < data.Length; i++)
        {
            expandedKey[i] = key[i % key.Length];
        }
        
        return data.Xor(expandedKey);
    }
    
    public static byte[] Decrypt(ReadOnlySpan<byte> encrypted, ReadOnlySpan<byte> key)
    {
        // XOR is symmetric
        return Encrypt(encrypted, key);
    }
}

// Usage
ReadOnlySpan<byte> message = Encoding.UTF8.GetBytes("Secret");
ReadOnlySpan<byte> key = new byte[] { 0xAB, 0xCD, 0xEF };
byte[] encrypted = XorCipher.Encrypt(message, key);
byte[] decrypted = XorCipher.Decrypt(encrypted, key);
```

### Finding Delimiter Positions
```csharp
public static List<(int start, int length)> FindRecords(
    ReadOnlySpan<byte> data, 
    byte delimiter)
{
    var records = new List<(int, int)>();
    var indices = data.FindAllIndices(delimiter);
    
    int lastIndex = 0;
    foreach (var index in indices)
    {
        var length = index - lastIndex;
        if (length > 0)
        {
            records.Add((lastIndex, length));
        }
        lastIndex = index + 1;
    }
    
    // Add final record if exists
    if (lastIndex < data.Length)
    {
        records.Add((lastIndex, data.Length - lastIndex));
    }
    
    return records;
}
```

## Performance Characteristics

### Time Complexity
- **CalculateEntropy**: O(n) - single pass with 256-element frequency array
- **AnalyzeDistribution**: O(n) - single pass with dictionary operations
- **CountOccurrences**: O(n) - single pass
- **FindAllIndices**: O(n) - single pass with list growth
- **AllBytesEqual**: O(n) - best case O(1) if first bytes differ
- **IsAllZeros**: O(n) - best case O(1) if first byte non-zero
- **Reverse**: O(n) - single pass with allocation
- **Xor**: O(min(n,m)) - single pass, operates on shorter span

### Memory Usage
- **Analysis methods**: O(1) for CountOccurrences, AllBytesEqual, IsAllZeros
- **AnalyzeDistribution**: O(unique bytes) - typically O(256) maximum
- **FindAllIndices**: O(matches) - allocates array for match positions
- **Reverse/Xor**: O(n) - creates new array with results

### Optimization Notes
- Entropy and distribution calculations are single-pass
- Early-exit optimizations for equality checks
- Dictionary uses TryGetValue for optimal performance
- Array.Reverse is highly optimized in .NET

## Best Practices

### Use Appropriate Methods
```csharp
// DO: Use specific check for zeros
if (span.IsAllZeros()) { }

// DON'T: Use distribution for simple checks
if (span.AnalyzeDistribution().Count == 1 && 
    span.AnalyzeDistribution().ContainsKey(0)) { }
```

### Reuse Distribution Results
```csharp
// DO: Calculate once, use multiple times
var distribution = span.AnalyzeDistribution();
var uniqueCount = distribution.Count;
var mostCommon = distribution.MaxBy(kvp => kvp.Value);

// DON'T: Recalculate repeatedly
var uniqueCount = span.AnalyzeDistribution().Count;
var mostCommon = span.AnalyzeDistribution().MaxBy(kvp => kvp.Value);
```

## Cross-References

### Related Components
- [ReadOnlySpanExtensions](ReadOnlySpanExtensions.md) - Core span operations
- [ByteArrayUtilities](ByteArrayUtilities.md) - Similar utilities for byte arrays
- [ByteArrayExtensions.ArrayManipulation](ByteArrayExtensions.ArrayManipulation.md) - Array manipulation operations
