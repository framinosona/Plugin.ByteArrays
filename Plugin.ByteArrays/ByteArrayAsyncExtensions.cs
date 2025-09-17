using System.Diagnostics.CodeAnalysis;

namespace Plugin.ByteArrays;

/// <summary>
///     Asynchronous operations for byte arrays.
/// </summary>
public static class ByteArrayAsyncExtensions
{
    #region Async File Operations

    /// <summary>
    ///     Asynchronously writes the byte array to a file.
    /// </summary>
    /// <param name="data">The data to write to the file.</param>
    /// <param name="filePath">The path to the file to write.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task WriteToFileAsync(this byte[] data, string filePath, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(data);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        await File.WriteAllBytesAsync(filePath, data, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    ///     Asynchronously reads a byte array from a file.
    /// </summary>
    /// <param name="filePath">The path to the file to read.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with the file content as byte array.</returns>
    public static async Task<byte[]> ReadFromFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        return await File.ReadAllBytesAsync(filePath, cancellationToken).ConfigureAwait(false);
    }

    #endregion

    #region Async Stream Operations

    /// <summary>
    ///     Asynchronously writes the byte array to a stream.
    /// </summary>
    /// <param name="data">The data to write to the stream.</param>
    /// <param name="stream">The stream to write to.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task WriteToStreamAsync(this byte[] data, Stream stream, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(data);
        ArgumentNullException.ThrowIfNull(stream);

        await stream.WriteAsync(data.AsMemory(), cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    ///     Asynchronously reads a byte array from a stream.
    /// </summary>
    /// <param name="stream">The stream to read from.</param>
    /// <param name="bufferSize">The buffer size for reading (default 4096).</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with the stream content as byte array.</returns>
    public static async Task<byte[]> ReadFromStreamAsync(Stream stream, int bufferSize = 4096, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);

        if (bufferSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(bufferSize), "Buffer size must be positive");
        }

        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream, bufferSize, cancellationToken).ConfigureAwait(false);
        return memoryStream.ToArray();
    }

    /// <summary>
    ///     Asynchronously copies a byte array to a stream in chunks.
    /// </summary>
    /// <param name="data">The data to copy to the stream.</param>
    /// <param name="stream">The destination stream.</param>
    /// <param name="chunkSize">The size of each chunk (default 4096).</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static async Task CopyToStreamAsync(this byte[] data, Stream stream, int chunkSize = 4096, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(data);
        ArgumentNullException.ThrowIfNull(stream);

        if (chunkSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(chunkSize), "Chunk size must be positive");
        }

        for (var offset = 0; offset < data.Length; offset += chunkSize)
        {
            var length = Math.Min(chunkSize, data.Length - offset);
            await stream.WriteAsync(data.AsMemory(offset, length), cancellationToken).ConfigureAwait(false);
        }
    }

    #endregion

    #region Async Processing Operations

    /// <summary>
    ///     Asynchronously processes byte array data in parallel chunks.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="data">The data to process.</param>
    /// <param name="processor">The function to process each chunk.</param>
    /// <param name="chunkSize">The size of each chunk (default 4096).</param>
    /// <param name="maxConcurrency">The maximum number of concurrent operations (default Environment.ProcessorCount).</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with the processed results.</returns>
    public static async Task<TResult[]> ProcessInParallelAsync<TResult>(
        this byte[] data,
        Func<byte[], Task<TResult>> processor,
        int chunkSize = 4096,
        int maxConcurrency = 0,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(data);
        ArgumentNullException.ThrowIfNull(processor);

        if (chunkSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(chunkSize), "Chunk size must be positive");
        }

        if (maxConcurrency <= 0)
        {
            maxConcurrency = Environment.ProcessorCount;
        }

        var chunks = new List<byte[]>();
        for (var offset = 0; offset < data.Length; offset += chunkSize)
        {
            var length = Math.Min(chunkSize, data.Length - offset);
            var chunk = new byte[length];
            Array.Copy(data, offset, chunk, 0, length);
            chunks.Add(chunk);
        }

        using var semaphore = new SemaphoreSlim(maxConcurrency, maxConcurrency);
        var tasks = chunks.Select(async chunk =>
        {
            await semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                return await processor(chunk).ConfigureAwait(false);
            }
            finally
            {
                semaphore.Release();
            }
        });

        return await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    /// <summary>
    ///     Asynchronously transforms byte array data using a transformation function.
    /// </summary>
    /// <param name="data">The data to transform.</param>
    /// <param name="transformer">The transformation function.</param>
    /// <param name="chunkSize">The size of each chunk for processing (default 4096).</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with the transformed data.</returns>
    public static async Task<byte[]> TransformAsync(
        this byte[] data,
        Func<byte[], Task<byte[]>> transformer,
        int chunkSize = 4096,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(data);
        ArgumentNullException.ThrowIfNull(transformer);

        if (chunkSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(chunkSize), "Chunk size must be positive");
        }

        var results = new List<byte[]>();

        for (var offset = 0; offset < data.Length; offset += chunkSize)
        {
            var length = Math.Min(chunkSize, data.Length - offset);
            var chunk = new byte[length];
            Array.Copy(data, offset, chunk, 0, length);

            var transformedChunk = await transformer(chunk).ConfigureAwait(false);
            results.Add(transformedChunk);
        }

        // Concatenate all results
        var totalLength = results.Sum(r => r.Length);
        var result = new byte[totalLength];
        var position = 0;

        foreach (var chunk in results)
        {
            Array.Copy(chunk, 0, result, position, chunk.Length);
            position += chunk.Length;
        }

        return result;
    }

    #endregion

    #region Async Cryptographic Operations

    /// <summary>
    ///     Asynchronously computes a hash of the byte array using the specified algorithm.
    /// </summary>
    /// <param name="data">The data to hash.</param>
    /// <param name="algorithmName">The hash algorithm name (e.g., "SHA256", "MD5").</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with the computed hash.</returns>
    [SuppressMessage("Security", "CA5350:Do Not Use Weak Cryptographic Algorithms", Justification = "Legacy support with clear documentation")]
    [SuppressMessage("Security", "CA5351:Do Not Use Broken Cryptographic Algorithms", Justification = "Legacy support with clear documentation")]
    public static async Task<byte[]> ComputeHashAsync(this byte[] data, string algorithmName, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(data);
        ArgumentException.ThrowIfNullOrWhiteSpace(algorithmName);

        return await Task.Run(() =>
        {
            return algorithmName.ToUpperInvariant() switch
            {
                "SHA256" => System.Security.Cryptography.SHA256.HashData(data),
                "SHA1" => System.Security.Cryptography.SHA1.HashData(data),
                "MD5" => System.Security.Cryptography.MD5.HashData(data),
                _ => throw new ArgumentException($"Unsupported hash algorithm: {algorithmName}", nameof(algorithmName))
            };
        }, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    ///     Asynchronously verifies the integrity of data against a provided hash.
    /// </summary>
    /// <param name="data">The data to verify.</param>
    /// <param name="expectedHash">The expected hash value.</param>
    /// <param name="algorithmName">The hash algorithm name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with the verification result.</returns>
    public static async Task<bool> VerifyIntegrityAsync(this byte[] data, byte[] expectedHash, string algorithmName, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(data);
        ArgumentNullException.ThrowIfNull(expectedHash);
        ArgumentException.ThrowIfNullOrWhiteSpace(algorithmName);

        var actualHash = await data.ComputeHashAsync(algorithmName, cancellationToken).ConfigureAwait(false);
        return actualHash.SequenceEqual(expectedHash);
    }

    #endregion

    #region Async Utility Operations

    /// <summary>
    ///     Asynchronously searches for a pattern in the byte array.
    /// </summary>
    /// <param name="data">The data to search in.</param>
    /// <param name="pattern">The pattern to search for.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with the index of the first occurrence, or -1 if not found.</returns>
    public static async Task<int> IndexOfAsync(this byte[] data, byte[] pattern, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(data);
        ArgumentNullException.ThrowIfNull(pattern);

        return await Task.Run(() => data.IndexOf(pattern), cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    ///     Asynchronously compares two byte arrays for equality.
    /// </summary>
    /// <param name="data1">The first byte array.</param>
    /// <param name="data2">The second byte array.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with the comparison result.</returns>
    public static async Task<bool> SequenceEqualAsync(this byte[] data1, byte[] data2, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(data1);
        ArgumentNullException.ThrowIfNull(data2);

        return await Task.Run(() => data1.SequenceEqual(data2), cancellationToken).ConfigureAwait(false);
    }

    #endregion
}
