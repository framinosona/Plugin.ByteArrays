# ByteArrayCompressionExtensions

This documentation covers compression and decompression extension methods in `ByteArrayCompressionExtensions`. These methods provide easy-to-use compression capabilities using multiple algorithms including GZip, Deflate, and Brotli.

## Overview

ByteArrayCompressionExtensions provides comprehensive compression support for:
- **GZip compression** - Widely supported, good compression ratio
- **Deflate compression** - Raw compression without headers
- **Brotli compression** - Modern algorithm with excellent compression
- **Compression analysis** - Tools for measuring and optimizing compression
- **Automatic algorithm selection** - Find the best compression for your data

## GZip Compression

### CompressGZip
<xref:Plugin.ByteArrays.ByteArrayCompressionExtensions.CompressGZip*>

Compresses byte arrays using the GZip algorithm with optional compression level control.

#### Usage Examples

```csharp
// Basic GZip compression
byte[] textData = Encoding.UTF8.GetBytes("This is a long text that will compress well due to repeated patterns and common words.");
byte[] compressed = textData.CompressGZip();

Console.WriteLine($"Original: {textData.Length} bytes");
Console.WriteLine($"Compressed: {compressed.Length} bytes");
Console.WriteLine($"Compression ratio: {(double)compressed.Length / textData.Length:P2}");

// Compression with specific level
byte[] dataToCompress = GetLargeDataset();
byte[] fastCompression = dataToCompress.CompressGZip(CompressionLevel.Fastest);
byte[] optimalCompression = dataToCompress.CompressGZip(CompressionLevel.Optimal);
byte[] smallestCompression = dataToCompress.CompressGZip(CompressionLevel.SmallestSize);

Console.WriteLine($"Fast: {fastCompression.Length} bytes");
Console.WriteLine($"Optimal: {optimalCompression.Length} bytes");
Console.WriteLine($"Smallest: {smallestCompression.Length} bytes");

// File compression
byte[] fileData = File.ReadAllBytes("large-document.txt");
byte[] compressedFile = fileData.CompressGZip(CompressionLevel.Optimal);
File.WriteAllBytes("large-document.txt.gz", compressedFile);

Console.WriteLine($"File compressed from {fileData.Length} to {compressedFile.Length} bytes");

// Web content compression (HTTP gzip)
public byte[] CompressWebContent(string htmlContent)
{
    byte[] htmlBytes = Encoding.UTF8.GetBytes(htmlContent);
    return htmlBytes.CompressGZip(CompressionLevel.Fastest); // Fast for real-time web serving
}

// Batch compression
string[] logFiles = Directory.GetFiles("logs", "*.log");
foreach (string logFile in logFiles)
{
    byte[] logData = File.ReadAllBytes(logFile);
    byte[] compressed = logData.CompressGZip(CompressionLevel.SmallestSize);

    string compressedPath = logFile + ".gz";
    File.WriteAllBytes(compressedPath, compressed);

    // Calculate space savings
    double savings = 1.0 - ((double)compressed.Length / logData.Length);
    Console.WriteLine($"{Path.GetFileName(logFile)}: {savings:P1} space saved");
}
```

### DecompressGZip
<xref:Plugin.ByteArrays.ByteArrayCompressionExtensions.DecompressGZip*>

Decompresses GZip-compressed byte arrays back to original data.

#### Usage Examples

```csharp
// Basic GZip decompression
byte[] compressedData = GetCompressedData();
try
{
    byte[] decompressed = compressedData.DecompressGZip();
    string text = Encoding.UTF8.GetString(decompressed);
    Console.WriteLine($"Decompressed text: {text}");
}
catch (InvalidDataException ex)
{
    Console.WriteLine($"Invalid GZip data: {ex.Message}");
}

// File decompression
byte[] gzipFile = File.ReadAllBytes("document.txt.gz");
byte[] originalFile = gzipFile.DecompressGZip();
File.WriteAllBytes("document.txt", originalFile);

// Round-trip compression test
byte[] originalData = Encoding.UTF8.GetBytes("Test data for compression");
byte[] compressed = originalData.CompressGZip();
byte[] decompressed = compressed.DecompressGZip();

bool isIdentical = originalData.SequenceEqual(decompressed);
Console.WriteLine($"Round-trip successful: {isIdentical}");

// Batch decompression with error handling
string[] gzipFiles = Directory.GetFiles("compressed", "*.gz");
int successCount = 0;
int errorCount = 0;

foreach (string gzipFile in gzipFiles)
{
    try
    {
        byte[] compressedData = File.ReadAllBytes(gzipFile);
        byte[] decompressed = compressedData.DecompressGZip();

        string outputPath = Path.ChangeExtension(gzipFile, null); // Remove .gz
        File.WriteAllBytes(outputPath, decompressed);

        successCount++;
        Console.WriteLine($"✓ Decompressed: {Path.GetFileName(gzipFile)}");
    }
    catch (Exception ex)
    {
        errorCount++;
        Console.WriteLine($"✗ Failed to decompress {Path.GetFileName(gzipFile)}: {ex.Message}");
    }
}

Console.WriteLine($"Decompression complete: {successCount} success, {errorCount} errors");
```

## Deflate Compression

### CompressDeflate
<xref:Plugin.ByteArrays.ByteArrayCompressionExtensions.CompressDeflate*>

Compresses byte arrays using the Deflate algorithm (raw compression without GZip headers).

#### Usage Examples

```csharp
// Basic Deflate compression (smaller headers than GZip)
byte[] data = GetDataToCompress();
byte[] deflateCompressed = data.CompressDeflate();
byte[] gzipCompressed = data.CompressGZip();

Console.WriteLine($"Deflate size: {deflateCompressed.Length} bytes");
Console.WriteLine($"GZip size: {gzipCompressed.Length} bytes");
Console.WriteLine($"Deflate overhead savings: {gzipCompressed.Length - deflateCompressed.Length} bytes");

// Protocol-level compression (where you control headers)
public byte[] CompressProtocolMessage(byte[] messageData)
{
    // Use Deflate for minimal overhead in custom protocols
    return messageData.CompressDeflate(CompressionLevel.Fastest);
}

// ZIP file content compression
public void CreateCustomZipEntry(string fileName, byte[] fileData)
{
    // ZIP files use Deflate compression internally
    byte[] compressed = fileData.CompressDeflate(CompressionLevel.Optimal);

    // Add to custom ZIP structure
    AddZipEntry(fileName, compressed, fileData.Length);
}

// Memory-efficient compression for large datasets
byte[] sensorData = GetSensorReadings();
byte[] compressed = sensorData.CompressDeflate(CompressionLevel.SmallestSize);

Console.WriteLine($"Sensor data: {sensorData.Length} -> {compressed.Length} bytes");
Console.WriteLine($"Memory saved: {sensorData.Length - compressed.Length} bytes");
```

### DecompressDeflate
<xref:Plugin.ByteArrays.ByteArrayCompressionExtensions.DecompressDeflate*>

Decompresses Deflate-compressed byte arrays.

#### Usage Examples

```csharp
// Protocol message decompression
byte[] compressedMessage = ReceiveProtocolMessage();
try
{
    byte[] messageData = compressedMessage.DecompressDeflate();
    var message = ParseProtocolMessage(messageData);
    ProcessMessage(message);
}
catch (InvalidDataException)
{
    Console.WriteLine("Received corrupted compressed message");
}

// Custom archive extraction
public byte[] ExtractZipEntry(byte[] compressedEntry)
{
    return compressedEntry.DecompressDeflate();
}
```

## Brotli Compression

### CompressBrotli
<xref:Plugin.ByteArrays.ByteArrayCompressionExtensions.CompressBrotli*>

Compresses byte arrays using the modern Brotli algorithm, offering excellent compression ratios.

#### Usage Examples

```csharp
// Brotli compression (best for text and web content)
byte[] htmlContent = Encoding.UTF8.GetBytes(GetLargeHtmlDocument());
byte[] brotliCompressed = htmlContent.CompressBrotli();
byte[] gzipCompressed = htmlContent.CompressGZip();

Console.WriteLine($"Original: {htmlContent.Length} bytes");
Console.WriteLine($"Brotli: {brotliCompressed.Length} bytes");
Console.WriteLine($"GZip: {gzipCompressed.Length} bytes");
Console.WriteLine($"Brotli advantage: {gzipCompressed.Length - brotliCompressed.Length} bytes smaller");

// Web server content compression
public byte[] CompressWebAsset(byte[] assetData, string contentType)
{
    // Brotli is excellent for web content
    if (contentType.StartsWith("text/") || contentType.Contains("javascript") || contentType.Contains("json"))
    {
        return assetData.CompressBrotli(CompressionLevel.Optimal);
    }

    // Use GZip for broader compatibility with binary content
    return assetData.CompressGZip(CompressionLevel.Optimal);
}

// API response compression
public byte[] CompressApiResponse(object responseData)
{
    string json = JsonSerializer.Serialize(responseData);
    byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

    // Brotli excels at JSON compression
    return jsonBytes.CompressBrotli(CompressionLevel.SmallestSize);
}

// Document storage optimization
byte[] documentContent = GetDocumentContent();
byte[] compressed = documentContent.CompressBrotli(CompressionLevel.SmallestSize);

// Store compressed version
StoreDocument(documentId, compressed, isCompressed: true, algorithm: "brotli");

Console.WriteLine($"Document storage: {documentContent.Length} -> {compressed.Length} bytes");
Console.WriteLine($"Storage efficiency: {(1.0 - (double)compressed.Length / documentContent.Length):P1}");
```

### DecompressBrotli
<xref:Plugin.ByteArrays.ByteArrayCompressionExtensions.DecompressBrotli*>

Decompresses Brotli-compressed byte arrays.

#### Usage Examples

```csharp
// Web content decompression
byte[] compressedResponse = DownloadCompressedContent();
try
{
    byte[] htmlContent = compressedResponse.DecompressBrotli();
    string html = Encoding.UTF8.GetString(htmlContent);
    ProcessWebContent(html);
}
catch (InvalidDataException ex)
{
    Console.WriteLine($"Failed to decompress Brotli content: {ex.Message}");
}

// Document retrieval
public byte[] RetrieveDocument(int documentId)
{
    var storedDocument = GetStoredDocument(documentId);

    if (storedDocument.IsCompressed && storedDocument.Algorithm == "brotli")
    {
        return storedDocument.Data.DecompressBrotli();
    }

    return storedDocument.Data;
}
```

## Compression Analysis Tools

### CalculateCompressionRatio
<xref:Plugin.ByteArrays.ByteArrayCompressionExtensions.CalculateCompressionRatio*>

Calculates the compression ratio achieved between original and compressed data.

#### Usage Examples

```csharp
// Analyze compression effectiveness
byte[] testData = GetTestData();
byte[] gzipResult = testData.CompressGZip();
byte[] deflateResult = testData.CompressDeflate();
byte[] brotliResult = testData.CompressBrotli();

double gzipRatio = ByteArrayCompressionExtensions.CalculateCompressionRatio(testData, gzipResult);
double deflateRatio = ByteArrayCompressionExtensions.CalculateCompressionRatio(testData, deflateResult);
double brotliRatio = ByteArrayCompressionExtensions.CalculateCompressionRatio(testData, brotliResult);

Console.WriteLine($"Compression Analysis for {testData.Length} bytes:");
Console.WriteLine($"GZip:    {gzipResult.Length} bytes (ratio: {gzipRatio:P2})");
Console.WriteLine($"Deflate: {deflateResult.Length} bytes (ratio: {deflateRatio:P2})");
Console.WriteLine($"Brotli:  {brotliResult.Length} bytes (ratio: {brotliRatio:P2})");

// Determine space savings
double gzipSavings = 1.0 - gzipRatio;
double deflateSavings = 1.0 - deflateRatio;
double brotliSavings = 1.0 - brotliRatio;

Console.WriteLine($"\nSpace Savings:");
Console.WriteLine($"GZip:    {gzipSavings:P1}");
Console.WriteLine($"Deflate: {deflateSavings:P1}");
Console.WriteLine($"Brotli:  {brotliSavings:P1}");

// Benchmark compression ratios across file types
public void BenchmarkCompressionByFileType()
{
    var fileTypes = new Dictionary<string, string[]>
    {
        { "Text", new[] { "*.txt", "*.log", "*.csv" } },
        { "Code", new[] { "*.cs", "*.js", "*.html", "*.css" } },
        { "Data", new[] { "*.json", "*.xml", "*.sql" } },
        { "Images", new[] { "*.bmp", "*.tiff" } } // Uncompressed formats
    };

    foreach (var category in fileTypes)
    {
        Console.WriteLine($"\n{category.Key} Files:");

        foreach (var pattern in category.Value)
        {
            var files = Directory.GetFiles("test-data", pattern);
            if (files.Length == 0) continue;

            var totalOriginal = 0L;
            var totalGzip = 0L;
            var totalBrotli = 0L;

            foreach (var file in files)
            {
                byte[] data = File.ReadAllBytes(file);
                byte[] gzipCompressed = data.CompressGZip();
                byte[] brotliCompressed = data.CompressBrotli();

                totalOriginal += data.Length;
                totalGzip += gzipCompressed.Length;
                totalBrotli += brotliCompressed.Length;
            }

            double gzipRatio = ByteArrayCompressionExtensions.CalculateCompressionRatio(
                new byte[totalOriginal], new byte[totalGzip]);
            double brotliRatio = ByteArrayCompressionExtensions.CalculateCompressionRatio(
                new byte[totalOriginal], new byte[totalBrotli]);

            Console.WriteLine($"  {pattern}: GZip {gzipRatio:P1}, Brotli {brotliRatio:P1}");
        }
    }
}
```

### IsLikelyCompressed
<xref:Plugin.ByteArrays.ByteArrayCompressionExtensions.IsLikelyCompressed*>

Analyzes data entropy to determine if data is already compressed or encrypted.

#### Usage Examples

```csharp
// Avoid double compression
public byte[] SmartCompress(byte[] data)
{
    if (data.IsLikelyCompressed())
    {
        Console.WriteLine("Data appears already compressed, skipping compression");
        return data;
    }

    Console.WriteLine("Data appears uncompressed, applying compression");
    return data.CompressBrotli();
}

// File type detection
public string AnalyzeFileContent(byte[] fileData)
{
    if (fileData.IsLikelyCompressed())
    {
        return "Compressed or encrypted file";
    }

    // Check for text content
    try
    {
        string text = Encoding.UTF8.GetString(fileData);
        if (text.All(c => char.IsControl(c) || char.IsWhiteSpace(c) || char.IsLetterOrDigit(c) || char.IsPunctuation(c)))
        {
            return "Text file";
        }
    }
    catch
    {
        // Not valid UTF-8
    }

    return "Binary file";
}

// Compression effectiveness prediction
public void PredictCompressionEffectiveness(byte[] data)
{
    bool isAlreadyCompressed = data.IsLikelyCompressed();

    if (isAlreadyCompressed)
    {
        Console.WriteLine("⚠️  High entropy data - compression will be ineffective");
        Console.WriteLine("   This appears to be compressed, encrypted, or random data");
    }
    else
    {
        Console.WriteLine("✓ Low entropy data - compression should be effective");
        Console.WriteLine("   This appears to be uncompressed structured data");

        // Test compression to validate prediction
        byte[] compressed = data.CompressGZip();
        double ratio = ByteArrayCompressionExtensions.CalculateCompressionRatio(data, compressed);
        Console.WriteLine($"   Actual compression ratio: {ratio:P1}");
    }
}
```

### FindBestCompression
<xref:Plugin.ByteArrays.ByteArrayCompressionExtensions.FindBestCompression*>

Tests multiple compression algorithms and returns the most effective one.

#### Usage Examples

```csharp
// Automatic algorithm selection
byte[] dataToCompress = GetDataForCompression();
var (bestCompressed, algorithm) = dataToCompress.FindBestCompression();

Console.WriteLine($"Best compression: {algorithm}");
Console.WriteLine($"Original size: {dataToCompress.Length} bytes");
Console.WriteLine($"Compressed size: {bestCompressed.Length} bytes");
Console.WriteLine($"Space saved: {dataToCompress.Length - bestCompressed.Length} bytes");

// Smart storage system
public void StoreDataWithOptimalCompression(string key, byte[] data)
{
    if (data.IsLikelyCompressed())
    {
        // Store without compression
        dataStore.Store(key, data, compressionAlgorithm: "none");
    }
    else
    {
        var (compressed, algorithm) = data.FindBestCompression();

        // Only compress if we achieve significant savings
        double ratio = ByteArrayCompressionExtensions.CalculateCompressionRatio(data, compressed);
        if (ratio < 0.9) // More than 10% savings
        {
            dataStore.Store(key, compressed, compressionAlgorithm: algorithm);
        }
        else
        {
            // Compression not worthwhile
            dataStore.Store(key, data, compressionAlgorithm: "none");
        }
    }
}

// Batch optimization
public void OptimizeFileStorage(string directory)
{
    var files = Directory.GetFiles(directory, "*", SearchOption.AllDirectories);
    long totalOriginalSize = 0;
    long totalCompressedSize = 0;
    var algorithmStats = new Dictionary<string, int>();

    foreach (string file in files)
    {
        byte[] data = File.ReadAllBytes(file);
        totalOriginalSize += data.Length;

        if (data.IsLikelyCompressed())
        {
            totalCompressedSize += data.Length;
            algorithmStats["none"] = algorithmStats.GetValueOrDefault("none") + 1;
            continue;
        }

        var (compressed, algorithm) = data.FindBestCompression();
        double ratio = ByteArrayCompressionExtensions.CalculateCompressionRatio(data, compressed);

        if (ratio < 0.8) // Significant compression achieved
        {
            string compressedPath = file + "." + algorithm.ToLower();
            File.WriteAllBytes(compressedPath, compressed);
            totalCompressedSize += compressed.Length;
            algorithmStats[algorithm] = algorithmStats.GetValueOrDefault(algorithm) + 1;

            Console.WriteLine($"{Path.GetFileName(file)}: {algorithm} ({ratio:P1})");
        }
        else
        {
            totalCompressedSize += data.Length;
            algorithmStats["none"] = algorithmStats.GetValueOrDefault("none") + 1;
        }
    }

    Console.WriteLine($"\nOptimization Results:");
    Console.WriteLine($"Total original size: {totalOriginalSize:N0} bytes");
    Console.WriteLine($"Total optimized size: {totalCompressedSize:N0} bytes");
    Console.WriteLine($"Total space saved: {totalOriginalSize - totalCompressedSize:N0} bytes ({(1.0 - (double)totalCompressedSize / totalOriginalSize):P1})");

    Console.WriteLine($"\nAlgorithm usage:");
    foreach (var stat in algorithmStats.OrderByDescending(kvp => kvp.Value))
    {
        Console.WriteLine($"  {stat.Key}: {stat.Value} files");
    }
}
```

## Performance Characteristics

| Algorithm | Compression Speed | Decompression Speed | Compression Ratio | Best For |
|-----------|------------------|-------------------|------------------|----------|
| **GZip** | Good | Excellent | Good | General purpose, web content |
| **Deflate** | Good | Excellent | Good | Custom protocols, minimal overhead |
| **Brotli** | Slower | Good | Excellent | Web assets, text content |

## Compression Level Guidelines

- **Fastest**: Real-time applications, live streaming
- **Optimal**: Balanced speed/size for most applications
- **SmallestSize**: Archival storage, bandwidth-critical applications

## Best Practices

- **Check if data is already compressed** using `IsLikelyCompressed()` before compressing
- **Use appropriate algorithms** based on your use case and compatibility requirements
- **Test compression ratios** with your actual data to choose the best algorithm
- **Consider decompression speed** for frequently accessed data
- **Handle compression errors** gracefully with proper exception handling
- **Measure compression effectiveness** to validate your compression strategy
- **Use streaming compression** for very large datasets to manage memory usage

## Common Use Cases

1. **Web Content**: Compress HTML, CSS, JavaScript, and JSON responses
2. **File Storage**: Reduce storage space for documents and logs
3. **Network Protocols**: Minimize bandwidth usage in custom protocols
4. **Database Storage**: Compress large text fields and binary data
5. **Backup Systems**: Reduce backup size and transfer times
6. **API Responses**: Compress large JSON or XML payloads
7. **Cache Systems**: Store more data in limited cache space
8. **Mobile Applications**: Reduce app size and data transfer

## Algorithm Selection Guide

- **Choose GZip** for: Web servers, HTTP compression, broad compatibility
- **Choose Deflate** for: Custom protocols, ZIP files, minimal overhead
- **Choose Brotli** for: Modern web applications, static assets, best compression
- **Use `FindBestCompression()`** when: Optimal compression is critical and you can test multiple algorithms

## See Also

- <xref:Plugin.ByteArrays.ByteArrayAsyncExtensions> - For async compression operations
- <xref:Plugin.ByteArrays.ByteArrayUtilities> - For entropy and data analysis
- <xref:Plugin.ByteArrays.ByteArrayBuilder> - For building compressed data structures
- <xref:Plugin.ByteArrays.ByteArrayExtensions> - For working with compressed data
