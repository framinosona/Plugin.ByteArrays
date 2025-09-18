# ByteArrayUtilities Class

The `ByteArrayUtilities` class provides utility extensions for byte array analysis, formatting, and performance measurement. These tools are essential for debugging, data analysis, and performance optimization of byte array operations.

## Binary String Representation

### ToBinaryString
Converts a byte array to a binary string representation where each byte is shown as 8 bits.

#### Syntax
```csharp
public static string ToBinaryString(this byte[] array, string separator = " ")
```

#### Parameters
- `array`: The byte array to convert
- `separator`: The separator between bytes (default is space)

#### Example
```csharp
var data = new byte[] { 0xAB, 0xCD, 0x12 };
var binary = data.ToBinaryString();
Console.WriteLine(binary);
// Output: "10101011 11001101 00010010"

// Custom separator
var binaryWithDash = data.ToBinaryString("-");
Console.WriteLine(binaryWithDash);
// Output: "10101011-11001101-00010010"
```

### FromBinaryString
Converts a binary string back to a byte array.

#### Syntax
```csharp
public static byte[] FromBinaryString(string binaryString, string separator = " ")
```

#### Example
```csharp
var binaryString = "10101011 11001101 00010010";
var data = ByteArrayUtilities.FromBinaryString(binaryString);

// Result: { 0xAB, 0xCD, 0x12 }
Console.WriteLine($"Reconstructed: {string.Join(" ", data.Select(b => $"0x{b:X2}"))}");

// Round-trip conversion
var original = new byte[] { 0xFF, 0x00, 0x7F };
var roundTrip = ByteArrayUtilities.FromBinaryString(original.ToBinaryString());
Console.WriteLine($"Round-trip successful: {original.SequenceEqual(roundTrip)}");
```

## Hex Dump Functionality

### ToHexDump
Creates a formatted hex dump similar to hex editors, showing offsets, hex bytes, and ASCII representation.

#### Syntax
```csharp
public static string ToHexDump(this byte[] array, int bytesPerLine = 16, bool showAscii = true, bool showOffsets = true)
```

#### Parameters
- `bytesPerLine`: Number of bytes per line (default 16)
- `showAscii`: Whether to show ASCII representation (default true)
- `showOffsets`: Whether to show byte offsets (default true)

#### Example
```csharp
var data = Enumerable.Range(0, 50).Select(i => (byte)i).ToArray();
var hexDump = data.ToHexDump();

Console.WriteLine(hexDump);
/* Output:
00000000: 00 01 02 03 04 05 06 07  08 09 0A 0B 0C 0D 0E 0F |................|
00000010: 10 11 12 13 14 15 16 17  18 19 1A 1B 1C 1D 1E 1F |................|
00000020: 20 21 22 23 24 25 26 27  28 29 2A 2B 2C 2D 2E 2F | !"#$%&'()*+,-./|
00000030: 30 31                                             |01|
*/

// Compact format
var compact = data.ToHexDump(bytesPerLine: 8, showAscii: false);
Console.WriteLine(compact);
/* Output:
00000000: 00 01 02 03 04 05 06 07
00000008: 08 09 0A 0B 0C 0D 0E 0F
...
*/
```

### Custom Formatting Example
```csharp
// Create data with mix of printable and non-printable characters
var mixedData = new byte[]
{
    0x48, 0x65, 0x6C, 0x6C, 0x6F, // "Hello"
    0x00, 0x01, 0x02,             // Non-printable
    0x57, 0x6F, 0x72, 0x6C, 0x64  // "World"
};

var dump = mixedData.ToHexDump(bytesPerLine: 8);
Console.WriteLine(dump);
/* Output:
00000000: 48 65 6C 6C 6F 00 01 02 |Hello...|
00000008: 57 6F 72 6C 64          |World|
*/
```

## Data Analysis

### CalculateEntropy
Calculates the Shannon entropy of the byte array (0-8, where 8 is maximum entropy).

#### Syntax
```csharp
public static double CalculateEntropy(this byte[] array)
```

#### Example
```csharp
// Low entropy data (repetitive)
var lowEntropy = Enumerable.Repeat((byte)0xAA, 100).ToArray();
Console.WriteLine($"Low entropy: {lowEntropy.CalculateEntropy():F2}"); // ~0.00

// High entropy data (random)
var random = new Random(42);
var highEntropy = Enumerable.Range(0, 1000).Select(_ => (byte)random.Next(256)).ToArray();
Console.WriteLine($"High entropy: {highEntropy.CalculateEntropy():F2}"); // ~7.8-8.0

// Text data (medium entropy)
var textData = Encoding.UTF8.GetBytes("The quick brown fox jumps over the lazy dog");
Console.WriteLine($"Text entropy: {textData.CalculateEntropy():F2}"); // ~4.5-5.5
```

### GetByteFrequency
Counts the occurrences of each byte value in the array.

#### Syntax
```csharp
public static Dictionary<byte, int> GetByteFrequency(this byte[] array)
```

#### Example
```csharp
var data = new byte[] { 0x01, 0x02, 0x01, 0x03, 0x02, 0x01 };
var frequency = data.GetByteFrequency();

foreach (var kvp in frequency.OrderBy(x => x.Key))
{
    Console.WriteLine($"Byte 0x{kvp.Key:X2}: {kvp.Value} occurrences");
}
/* Output:
Byte 0x01: 3 occurrences
Byte 0x02: 2 occurrences
Byte 0x03: 1 occurrences
*/

// Analyze distribution
var totalBytes = frequency.Values.Sum();
foreach (var kvp in frequency.OrderByDescending(x => x.Value))
{
    var percentage = (kvp.Value * 100.0) / totalBytes;
    Console.WriteLine($"0x{kvp.Key:X2}: {percentage:F1}%");
}
```

### AppearsCompressed
Determines if data appears to be compressed or encrypted based on entropy threshold.

#### Syntax
```csharp
public static bool AppearsCompressed(this byte[] array)
```

#### Example
```csharp
// Test various data types
var plainText = Encoding.UTF8.GetBytes("This is plain text with lots of repetition");
var compressedData = plainText.Compress(); // Using ByteArrayCompressionExtensions
var randomData = new Random().NextBytes(100); // High entropy

Console.WriteLine($"Plain text compressed: {plainText.AppearsCompressed()}"); // False
Console.WriteLine($"Compressed data compressed: {compressedData.AppearsCompressed()}"); // True
Console.WriteLine($"Random data compressed: {randomData.AppearsCompressed()}"); // True

// Manual entropy analysis
Console.WriteLine($"Plain text entropy: {plainText.CalculateEntropy():F2}");
Console.WriteLine($"Compressed entropy: {compressedData.CalculateEntropy():F2}");
```

## Performance Measurement

### MeasurePerformance
Measures execution time of operations on byte arrays with multiple iterations.

#### Syntax
```csharp
public static (T result, TimeSpan totalTime, TimeSpan averageTime) MeasurePerformance<T>(
    this byte[] array,
    Func<byte[], T> operation,
    int iterations = 1)
```

#### Example
```csharp
var data = new byte[1_000_000];
new Random().NextBytes(data);

// Measure checksum calculation performance
var (checksum, totalTime, avgTime) = data.MeasurePerformance(
    arr => arr.CalculateSimpleChecksum(),
    iterations: 1000);

Console.WriteLine($"Checksum: 0x{checksum:X2}");
Console.WriteLine($"Total time: {totalTime.TotalMilliseconds:F2}ms");
Console.WriteLine($"Average time: {avgTime.TotalMicroseconds:F2}μs");

// Measure different operations
var operations = new Dictionary<string, Func<byte[], object>>
{
    ["Entropy"] = arr => arr.CalculateEntropy(),
    ["Frequency"] = arr => arr.GetByteFrequency().Count,
    ["ToHex"] = arr => arr.ToHexString(),
    ["Compress"] = arr => arr.Compress().Length
};

foreach (var op in operations)
{
    var (result, time, avg) = data.MeasurePerformance(op.Value, 10);
    Console.WriteLine($"{op.Key}: {result}, Avg: {avg.TotalMilliseconds:F2}ms");
}
```

### CalculateThroughput
Calculates throughput in bytes per second for byte array operations.

#### Syntax
```csharp
public static double CalculateThroughput(this byte[] array, TimeSpan elapsedTime)
```

#### Example
```csharp
var data = new byte[10_000_000]; // 10MB
new Random().NextBytes(data);

var stopwatch = Stopwatch.StartNew();
var compressed = data.Compress();
stopwatch.Stop();

var throughput = data.CalculateThroughput(stopwatch.Elapsed);
Console.WriteLine($"Compression throughput: {throughput / 1_000_000:F2} MB/s");

// Compare different operations
var operations = new[]
{
    ("CRC32", new Func<byte[], object>(arr => arr.CalculateCrc32())),
    ("MD5", new Func<byte[], object>(arr => arr.ToMd5())),
    ("Entropy", new Func<byte[], object>(arr => arr.CalculateEntropy()))
};

foreach (var (name, operation) in operations)
{
    var sw = Stopwatch.StartNew();
    operation(data);
    sw.Stop();

    var mbps = data.CalculateThroughput(sw.Elapsed) / 1_000_000;
    Console.WriteLine($"{name}: {mbps:F2} MB/s");
}
```

## Comprehensive Analysis Example

### Data Analysis Pipeline
```csharp
public class ByteArrayAnalyzer
{
    public void AnalyzeData(byte[] data, string description = "")
    {
        Console.WriteLine($"=== Analysis: {description} ===");
        Console.WriteLine($"Size: {data.Length:N0} bytes");

        // Basic statistics
        var entropy = data.CalculateEntropy();
        var compressed = data.AppearsCompressed();

        Console.WriteLine($"Entropy: {entropy:F3} bits/byte");
        Console.WriteLine($"Appears compressed: {compressed}");

        // Frequency analysis
        var frequency = data.GetByteFrequency();
        var mostCommon = frequency.OrderByDescending(x => x.Value).Take(5);

        Console.WriteLine("Most common bytes:");
        foreach (var (value, count) in mostCommon)
        {
            var percentage = (count * 100.0) / data.Length;
            Console.WriteLine($"  0x{value:X2}: {count:N0} ({percentage:F1}%)");
        }

        // Performance measurement
        var (_, time, _) = data.MeasurePerformance(arr => arr.CalculateEntropy(), 100);
        var throughput = data.CalculateThroughput(time);

        Console.WriteLine($"Entropy calculation: {throughput / 1_000_000:F2} MB/s");

        // Visual representation (first 256 bytes)
        if (data.Length > 0)
        {
            var sample = data.Take(Math.Min(256, data.Length)).ToArray();
            Console.WriteLine("\nHex dump (first 256 bytes):");
            Console.WriteLine(sample.ToHexDump());
        }

        Console.WriteLine();
    }
}

// Usage
var analyzer = new ByteArrayAnalyzer();

// Analyze different data types
analyzer.AnalyzeData(Encoding.UTF8.GetBytes("Hello World!"), "Text");
analyzer.AnalyzeData(File.ReadAllBytes("image.jpg"), "JPEG Image");
analyzer.AnalyzeData(File.ReadAllBytes("document.pdf"), "PDF Document");

var random = new Random();
var randomData = new byte[10000];
random.NextBytes(randomData);
analyzer.AnalyzeData(randomData, "Random Data");
```

## Debugging and Troubleshooting

### Binary Data Debugging
```csharp
public static void DebugByteArray(byte[] data, string context = "")
{
    Console.WriteLine($"=== Debug: {context} ===");

    // Quick overview
    Console.WriteLine($"Length: {data.Length}");
    Console.WriteLine($"First 16 bytes: {string.Join(" ", data.Take(16).Select(b => $"{b:X2}"))}");

    // Binary representation for small arrays
    if (data.Length <= 8)
    {
        Console.WriteLine($"Binary: {data.ToBinaryString()}");
    }

    // Pattern detection
    var unique = data.Distinct().Count();
    Console.WriteLine($"Unique values: {unique}/256 ({unique * 100.0 / 256:F1}%)");

    // Structural analysis
    if (data.Length >= 16)
    {
        Console.WriteLine("Hex dump:");
        Console.WriteLine(data.Take(64).ToArray().ToHexDump());
    }
}

// Usage in debugging scenarios
var protocolData = new byte[] { 0x7E, 0x01, 0x04, 0x00, 0x12, 0x34, 0x56, 0x78, 0x7E };
DebugByteArray(protocolData, "Protocol Frame");
```

## Performance Characteristics

### Time Complexity
- **ToBinaryString**: O(n) where n is array length
- **ToHexDump**: O(n) where n is array length
- **CalculateEntropy**: O(n) where n is array length
- **GetByteFrequency**: O(n) where n is array length
- **MeasurePerformance**: O(k×f) where k is iterations and f is operation complexity

### Memory Usage
- **Binary string operations**: Creates new strings, memory usage ~8n bytes
- **Hex dump**: Creates formatted string, memory usage varies with formatting options
- **Frequency analysis**: Uses Dictionary<byte, int>, memory usage ~1KB maximum
- **Performance measurement**: Minimal overhead, reuses same array

## Best Practices

### Entropy Analysis
```csharp
// Use entropy to classify data types
public static string ClassifyDataType(byte[] data)
{
    var entropy = data.CalculateEntropy();

    return entropy switch
    {
        < 1.0 => "Highly repetitive (padding, zeros)",
        < 3.0 => "Low entropy (text, structured data)",
        < 6.0 => "Medium entropy (executable, mixed data)",
        < 7.5 => "High entropy (natural language text)",
        _ => "Very high entropy (compressed/encrypted)"
    };
}
```

### Performance Optimization
```csharp
// Measure and compare different implementations
public static void OptimizeOperation(byte[] testData)
{
    var implementations = new Dictionary<string, Func<byte[], byte>>
    {
        ["Simple"] = arr => arr.CalculateSimpleChecksum(),
        ["XOR"] = arr => arr.CalculateXorChecksum(),
        ["CRC8"] = arr => arr.CalculateCrc8()
    };

    foreach (var impl in implementations)
    {
        var (result, time, avg) = testData.MeasurePerformance(impl.Value, 1000);
        var throughput = testData.CalculateThroughput(avg);

        Console.WriteLine($"{impl.Key}: {throughput / 1_000_000:F2} MB/s");
    }
}
```

### Debugging Complex Data
```csharp
// Use hex dump for protocol analysis
public static void AnalyzeProtocolFrame(byte[] frame)
{
    Console.WriteLine("Raw frame:");
    Console.WriteLine(frame.ToHexDump(bytesPerLine: 8));

    // Check for common patterns
    if (frame.StartsWith(new byte[] { 0x7E }))
    {
        Console.WriteLine("Detected frame start marker");
    }

    // Entropy analysis for payload detection
    if (frame.Length > 10)
    {
        var payload = frame.Skip(5).Take(frame.Length - 10).ToArray();
        var entropy = payload.CalculateEntropy();
        Console.WriteLine($"Payload entropy: {entropy:F2} (compressed: {entropy > 7.5})");
    }
}
```

## Cross-References

- <xref:Plugin.ByteArrays.ByteArrayExtensions> - Core byte array conversion operations
- <xref:Plugin.ByteArrays.ByteArrayProtocolExtensions> - Protocol-specific operations
- <xref:Plugin.ByteArrays.ByteArrayCompressionExtensions> - Compression and decompression
- <xref:Plugin.ByteArrays.ByteArrayBuilder> - Efficient byte array construction
- <xref:Plugin.ByteArrays.ByteArrayAsyncExtensions> - Asynchronous operations
