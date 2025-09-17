using System.IO.Compression;

namespace Plugin.ByteArrays;

/// <summary>
///     Compression and decompression extensions for byte arrays.
/// </summary>
public static class ByteArrayCompressionExtensions
{
    #region GZip Compression

    /// <summary>
    ///     Compresses the byte array using GZip compression.
    /// </summary>
    /// <param name="data">The data to compress.</param>
    /// <returns>The compressed data.</returns>
    public static byte[] CompressGZip(this byte[] data)
    {
        ArgumentNullException.ThrowIfNull(data);

        using var output = new MemoryStream();
        using (var gzip = new GZipStream(output, CompressionMode.Compress))
        {
            gzip.Write(data, 0, data.Length);
        }

        return output.ToArray();
    }

    /// <summary>
    ///     Decompresses the byte array using GZip decompression.
    /// </summary>
    /// <param name="compressedData">The compressed data to decompress.</param>
    /// <returns>The decompressed data.</returns>
    /// <exception cref="InvalidDataException">Thrown when the data is not valid GZip format.</exception>
    public static byte[] DecompressGZip(this byte[] compressedData)
    {
        ArgumentNullException.ThrowIfNull(compressedData);

        using var input = new MemoryStream(compressedData);
        using var gzip = new GZipStream(input, CompressionMode.Decompress);
        using var output = new MemoryStream();

        gzip.CopyTo(output);
        return output.ToArray();
    }

    /// <summary>
    ///     Compresses the byte array using GZip compression with specified compression level.
    /// </summary>
    /// <param name="data">The data to compress.</param>
    /// <param name="compressionLevel">The compression level to use.</param>
    /// <returns>The compressed data.</returns>
    public static byte[] CompressGZip(this byte[] data, CompressionLevel compressionLevel)
    {
        ArgumentNullException.ThrowIfNull(data);

        using var output = new MemoryStream();
        using (var gzip = new GZipStream(output, compressionLevel))
        {
            gzip.Write(data, 0, data.Length);
        }

        return output.ToArray();
    }

    #endregion

    #region Deflate Compression

    /// <summary>
    ///     Compresses the byte array using Deflate compression.
    /// </summary>
    /// <param name="data">The data to compress.</param>
    /// <returns>The compressed data.</returns>
    public static byte[] CompressDeflate(this byte[] data)
    {
        ArgumentNullException.ThrowIfNull(data);

        using var output = new MemoryStream();
        using (var deflate = new DeflateStream(output, CompressionMode.Compress))
        {
            deflate.Write(data, 0, data.Length);
        }

        return output.ToArray();
    }

    /// <summary>
    ///     Decompresses the byte array using Deflate decompression.
    /// </summary>
    /// <param name="compressedData">The compressed data to decompress.</param>
    /// <returns>The decompressed data.</returns>
    /// <exception cref="InvalidDataException">Thrown when the data is not valid Deflate format.</exception>
    public static byte[] DecompressDeflate(this byte[] compressedData)
    {
        ArgumentNullException.ThrowIfNull(compressedData);

        using var input = new MemoryStream(compressedData);
        using var deflate = new DeflateStream(input, CompressionMode.Decompress);
        using var output = new MemoryStream();

        deflate.CopyTo(output);
        return output.ToArray();
    }

    /// <summary>
    ///     Compresses the byte array using Deflate compression with specified compression level.
    /// </summary>
    /// <param name="data">The data to compress.</param>
    /// <param name="compressionLevel">The compression level to use.</param>
    /// <returns>The compressed data.</returns>
    public static byte[] CompressDeflate(this byte[] data, CompressionLevel compressionLevel)
    {
        ArgumentNullException.ThrowIfNull(data);

        using var output = new MemoryStream();
        using (var deflate = new DeflateStream(output, compressionLevel))
        {
            deflate.Write(data, 0, data.Length);
        }

        return output.ToArray();
    }

    #endregion

    #region Brotli Compression (available in .NET Core 2.1+)

    /// <summary>
    ///     Compresses the byte array using Brotli compression.
    /// </summary>
    /// <param name="data">The data to compress.</param>
    /// <returns>The compressed data.</returns>
    public static byte[] CompressBrotli(this byte[] data)
    {
        ArgumentNullException.ThrowIfNull(data);

        using var output = new MemoryStream();
        using (var brotli = new BrotliStream(output, CompressionMode.Compress))
        {
            brotli.Write(data, 0, data.Length);
        }

        return output.ToArray();
    }

    /// <summary>
    ///     Decompresses the byte array using Brotli decompression.
    /// </summary>
    /// <param name="compressedData">The compressed data to decompress.</param>
    /// <returns>The decompressed data.</returns>
    /// <exception cref="InvalidDataException">Thrown when the data is not valid Brotli format.</exception>
    public static byte[] DecompressBrotli(this byte[] compressedData)
    {
        ArgumentNullException.ThrowIfNull(compressedData);

        using var input = new MemoryStream(compressedData);
        using var brotli = new BrotliStream(input, CompressionMode.Decompress);
        using var output = new MemoryStream();

        brotli.CopyTo(output);
        return output.ToArray();
    }

    /// <summary>
    ///     Compresses the byte array using Brotli compression with specified compression level.
    /// </summary>
    /// <param name="data">The data to compress.</param>
    /// <param name="compressionLevel">The compression level to use.</param>
    /// <returns>The compressed data.</returns>
    public static byte[] CompressBrotli(this byte[] data, CompressionLevel compressionLevel)
    {
        ArgumentNullException.ThrowIfNull(data);

        using var output = new MemoryStream();
        using (var brotli = new BrotliStream(output, compressionLevel))
        {
            brotli.Write(data, 0, data.Length);
        }

        return output.ToArray();
    }

    #endregion

    #region Compression Utilities

    /// <summary>
    ///     Calculates the compression ratio achieved by compressing the data.
    /// </summary>
    /// <param name="originalData">The original uncompressed data.</param>
    /// <param name="compressedData">The compressed data.</param>
    /// <returns>The compression ratio (0.0 to 1.0, where 0.5 means 50% compression).</returns>
    public static double CalculateCompressionRatio(byte[] originalData, byte[] compressedData)
    {
        ArgumentNullException.ThrowIfNull(originalData);
        ArgumentNullException.ThrowIfNull(compressedData);

        if (originalData.Length == 0)
        {
            return 0.0;
        }

        return (double)compressedData.Length / originalData.Length;
    }

    /// <summary>
    ///     Determines if data is likely already compressed by analyzing entropy.
    /// </summary>
    /// <param name="data">The data to analyze.</param>
    /// <returns>True if the data appears to be compressed or encrypted, false otherwise.</returns>
    public static bool IsLikelyCompressed(this byte[] data)
    {
        ArgumentNullException.ThrowIfNull(data);

        if (data.Length < 100) // Small files can't be reliably analyzed
        {
            return false;
        }

        // Calculate byte frequency distribution
        var frequency = new int[256];
        foreach (var b in data)
        {
            frequency[b]++;
        }

        // Calculate entropy
        var entropy = 0.0;
        var length = data.Length;

        for (var i = 0; i < 256; i++)
        {
            if (frequency[i] > 0)
            {
                var probability = (double)frequency[i] / length;
                entropy -= probability * Math.Log2(probability);
            }
        }

        // High entropy (close to 8.0) suggests compressed/encrypted data
        return entropy > 7.5;
    }

    /// <summary>
    ///     Tests multiple compression algorithms and returns the best result.
    /// </summary>
    /// <param name="data">The data to compress.</param>
    /// <returns>A tuple containing the compressed data and the algorithm name used.</returns>
    public static (byte[] compressedData, string algorithm) FindBestCompression(this byte[] data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var results = new List<(byte[] compressed, string algorithm)>
        {
            (data.CompressGZip(), "GZip"),
            (data.CompressDeflate(), "Deflate"),
            (data.CompressBrotli(), "Brotli")
        };

        // Return the compression method that produces the smallest result
        var best = results.OrderBy(r => r.compressed.Length).First();
        return best;
    }

    #endregion
}
