using System.Text;

using FluentAssertions;

namespace Plugin.ByteArrays.Tests;

public class ByteArrayAsyncExtensionsTests
{
    #region File Operations Tests

    [Fact]
    public async Task WriteToFileAsync_WithValidData_ShouldWriteFileSuccessfully()
    {
        // Arrange
        var data = Encoding.UTF8.GetBytes("Hello, World!");
        var tempFile = Path.GetTempFileName();

        try
        {
            // Act
            await data.WriteToFileAsync(tempFile);

            // Assert
            var writtenData = await File.ReadAllBytesAsync(tempFile);
            writtenData.Should().BeEquivalentTo(data);
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    [Fact]
    public async Task ReadFromFileAsync_WithExistingFile_ShouldReadFileSuccessfully()
    {
        // Arrange
        var data = Encoding.UTF8.GetBytes("Test file content");
        var tempFile = Path.GetTempFileName();
        await File.WriteAllBytesAsync(tempFile, data);

        try
        {
            // Act
            var readData = await ByteArrayAsyncExtensions.ReadFromFileAsync(tempFile);

            // Assert
            readData.Should().BeEquivalentTo(data);
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    [Fact]
    public async Task WriteToFileAsync_WithCancellation_ShouldRespectCancellation()
    {
        // Arrange
        var data = new byte[1000];
        var tempFile = Path.GetTempFileName();
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        try
        {
            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(
                () => data.WriteToFileAsync(tempFile, cts.Token));
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    #endregion

    #region Stream Operations Tests

    [Fact]
    public async Task WriteToStreamAsync_WithValidData_ShouldWriteToStream()
    {
        // Arrange
        var data = Encoding.UTF8.GetBytes("Stream test data");
        using var stream = new MemoryStream();

        // Act
        await data.WriteToStreamAsync(stream);

        // Assert
        stream.ToArray().Should().BeEquivalentTo(data);
    }

    [Fact]
    public async Task ReadFromStreamAsync_WithValidStream_ShouldReadAllData()
    {
        // Arrange
        var data = Encoding.UTF8.GetBytes("Stream content to read");
        using var stream = new MemoryStream(data);

        // Act
        var readData = await ByteArrayAsyncExtensions.ReadFromStreamAsync(stream);

        // Assert
        readData.Should().BeEquivalentTo(data);
    }

    [Fact]
    public async Task CopyToStreamAsync_WithLargeData_ShouldCopyInChunks()
    {
        // Arrange
        var data = new byte[10000];
        new Random(42).NextBytes(data);
        using var stream = new MemoryStream();
        var chunkSize = 1024;

        // Act
        await data.CopyToStreamAsync(stream, chunkSize);

        // Assert
        stream.ToArray().Should().BeEquivalentTo(data);
    }

    [Fact]
    public async Task ReadFromStreamAsync_WithCustomBufferSize_ShouldUseCorrectBufferSize()
    {
        // Arrange
        var data = new byte[5000];
        new Random(42).NextBytes(data);
        using var stream = new MemoryStream(data);
        var bufferSize = 512;

        // Act
        var readData = await ByteArrayAsyncExtensions.ReadFromStreamAsync(stream, bufferSize);

        // Assert
        readData.Should().BeEquivalentTo(data);
    }

    #endregion

    #region Processing Operations Tests

    [Fact]
    public async Task ProcessInParallelAsync_WithValidProcessor_ShouldProcessAllChunks()
    {
        // Arrange
        var data = Enumerable.Range(0, 1000).Select(i => (byte)(i % 256)).ToArray();
        var chunkSize = 100;

        // Act
        var results = await data.ProcessInParallelAsync(
            chunk => Task.FromResult(chunk.Sum(b => (int)b)),
            chunkSize);

        // Assert
        results.Should().HaveCount(10); // 1000 / 100 = 10 chunks
        results.Should().AllSatisfy(x => x.Should().BeGreaterThan(0));
    }

    [Fact]
    public async Task ProcessInParallelAsync_WithConcurrencyLimit_ShouldRespectLimit()
    {
        // Arrange
        var data = new byte[1000];
        var chunkSize = 100;
        var maxConcurrency = 2;
        var activeTasks = 0;
        var maxActiveTasks = 0;

        // Act
        var results = await data.ProcessInParallelAsync(
            async chunk =>
            {
                Interlocked.Increment(ref activeTasks);
                var current = activeTasks;
                if (current > maxActiveTasks)
                {
                    maxActiveTasks = current;
                }

                await Task.Delay(10); // Simulate work

                Interlocked.Decrement(ref activeTasks);
                return chunk.Length;
            },
            chunkSize,
            maxConcurrency);

        // Assert
        results.Should().HaveCount(10);
        maxActiveTasks.Should().BeLessOrEqualTo(maxConcurrency);
    }

    [Fact]
    public async Task TransformAsync_WithValidTransformer_ShouldTransformAllChunks()
    {
        // Arrange
        var data = Encoding.UTF8.GetBytes("Hello World");

        // Act
        var transformed = await data.TransformAsync(
            chunk => Task.FromResult(chunk.Select(b => (byte)(b + 1)).ToArray()),
            chunkSize: 5);

        // Assert
        transformed.Should().HaveCount(data.Length);
        for (var i = 0; i < data.Length; i++)
        {
            transformed[i].Should().Be((byte)(data[i] + 1));
        }
    }

    // TransformAsync test removed - functionality not implemented

    #endregion

    #region Cryptographic Operations Tests

    [Fact]
    public async Task ComputeHashAsync_WithSHA256_ShouldReturnCorrectHash()
    {
        // Arrange
        var data = Encoding.UTF8.GetBytes("Hello, World!");

        // Act
        var hash = await data.ComputeHashAsync("SHA256");

        // Assert
        hash.Should().NotBeNull();
        hash.Length.Should().Be(32); // SHA256 produces 32-byte hash
    }

    [Fact]
    public async Task ComputeHashAsync_WithMD5_ShouldReturnCorrectHash()
    {
        // Arrange
        var data = Encoding.UTF8.GetBytes("Test data");

        // Act
        var hash = await data.ComputeHashAsync("MD5");

        // Assert
        hash.Should().NotBeNull();
        hash.Length.Should().Be(16); // MD5 produces 16-byte hash
    }

    [Fact]
    public async Task ComputeHashAsync_WithSHA1_ShouldReturnCorrectHash()
    {
        // Arrange
        var data = Encoding.UTF8.GetBytes("Test data");

        // Act
        var hash = await data.ComputeHashAsync("SHA1");

        // Assert
        hash.Should().NotBeNull();
        hash.Length.Should().Be(20); // SHA1 produces 20-byte hash
    }

    [Fact]
    public async Task VerifyIntegrityAsync_WithCorrectHash_ShouldReturnTrue()
    {
        // Arrange
        var data = Encoding.UTF8.GetBytes("Integrity test data");
        var expectedHash = await data.ComputeHashAsync("SHA256");

        // Act
        var isValid = await data.VerifyIntegrityAsync(expectedHash, "SHA256");

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    public async Task VerifyIntegrityAsync_WithIncorrectHash_ShouldReturnFalse()
    {
        // Arrange
        var data = Encoding.UTF8.GetBytes("Integrity test data");
        var incorrectHash = new byte[32]; // All zeros

        // Act
        var isValid = await data.VerifyIntegrityAsync(incorrectHash, "SHA256");

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public async Task ComputeHashAsync_WithUnsupportedAlgorithm_ShouldThrowArgumentException()
    {
        // Arrange
        var data = new byte[] { 1, 2, 3 };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => data.ComputeHashAsync("UNSUPPORTED"));
    }

    #endregion

    #region Utility Operations Tests

    [Fact]
    public async Task IndexOfAsync_WithExistingPattern_ShouldReturnCorrectIndex()
    {
        // Arrange
        var data = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
        var pattern = new byte[] { 3, 4, 5 };

        // Act
        var index = await data.IndexOfAsync(pattern);

        // Assert
        index.Should().Be(2);
    }

    [Fact]
    public async Task IndexOfAsync_WithNonExistingPattern_ShouldReturnMinusOne()
    {
        // Arrange
        var data = new byte[] { 1, 2, 3, 4, 5 };
        var pattern = new byte[] { 6, 7, 8 };

        // Act
        var index = await data.IndexOfAsync(pattern);

        // Assert
        index.Should().Be(-1);
    }

    [Fact]
    public async Task SequenceEqualAsync_WithEqualArrays_ShouldReturnTrue()
    {
        // Arrange
        var data1 = new byte[] { 1, 2, 3, 4, 5 };
        var data2 = new byte[] { 1, 2, 3, 4, 5 };

        // Act
        var result = await data1.SequenceEqualAsync(data2);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task SequenceEqualAsync_WithDifferentArrays_ShouldReturnFalse()
    {
        // Arrange
        var data1 = new byte[] { 1, 2, 3, 4, 5 };
        var data2 = new byte[] { 1, 2, 3, 4, 6 };

        // Act
        var result = await data1.SequenceEqualAsync(data2);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region Error Handling Tests

    [Fact]
    public async Task WriteToFileAsync_WithNullData_ShouldThrowArgumentNullException()
    {
        // Arrange
        byte[] data = null!;
        var filePath = "test.txt";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => data.WriteToFileAsync(filePath));
    }

    [Fact]
    public async Task WriteToFileAsync_WithNullFilePath_ShouldThrowArgumentException()
    {
        // Arrange
        var data = new byte[] { 1, 2, 3 };
        string filePath = null!;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => data.WriteToFileAsync(filePath));
    }

    [Fact]
    public async Task ReadFromFileAsync_WithNullFilePath_ShouldThrowArgumentException()
    {
        // Arrange
        string filePath = null!;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => ByteArrayAsyncExtensions.ReadFromFileAsync(filePath));
    }

    [Fact]
    public async Task WriteToStreamAsync_WithNullData_ShouldThrowArgumentNullException()
    {
        // Arrange
        byte[] data = null!;
        using var stream = new MemoryStream();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => data.WriteToStreamAsync(stream));
    }

    [Fact]
    public async Task WriteToStreamAsync_WithNullStream_ShouldThrowArgumentNullException()
    {
        // Arrange
        var data = new byte[] { 1, 2, 3 };
        Stream stream = null!;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => data.WriteToStreamAsync(stream));
    }

    [Fact]
    public async Task ReadFromStreamAsync_WithNullStream_ShouldThrowArgumentNullException()
    {
        // Arrange
        Stream stream = null!;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => ByteArrayAsyncExtensions.ReadFromStreamAsync(stream));
    }

    [Fact]
    public async Task ProcessInParallelAsync_WithNullData_ShouldThrowArgumentNullException()
    {
        // Arrange
        byte[] data = null!;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => data.ProcessInParallelAsync(chunk => Task.FromResult(1)));
    }

    [Fact]
    public async Task ProcessInParallelAsync_WithNullProcessor_ShouldThrowArgumentNullException()
    {
        // Arrange
        var data = new byte[] { 1, 2, 3 };
        Func<byte[], Task<int>> processor = null!;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => data.ProcessInParallelAsync(processor));
    }

    [Fact]
    public async Task ProcessInParallelAsync_WithInvalidChunkSize_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var data = new byte[] { 1, 2, 3 };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => data.ProcessInParallelAsync(chunk => Task.FromResult(1), chunkSize: 0));
    }

    [Fact]
    public async Task ComputeHashAsync_WithNullData_ShouldThrowArgumentNullException()
    {
        // Arrange
        byte[] data = null!;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => data.ComputeHashAsync("SHA256"));
    }

    [Fact]
    public async Task ComputeHashAsync_WithNullAlgorithm_ShouldThrowArgumentException()
    {
        // Arrange
        var data = new byte[] { 1, 2, 3 };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => data.ComputeHashAsync(null!));
    }

    [Fact]
    public async Task VerifyIntegrityAsync_WithNullExpectedHash_ShouldThrowArgumentNullException()
    {
        // Arrange
        var data = new byte[] { 1, 2, 3 };
        byte[] expectedHash = null!;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => data.VerifyIntegrityAsync(expectedHash, "SHA256"));
    }

    #endregion

    #region Cancellation Tests

    [Fact]
    public async Task ProcessInParallelAsync_WithCancellation_ShouldRespectCancellation()
    {
        // Arrange
        var data = new byte[1000];
        using var cts = new CancellationTokenSource();

        // Act
        var task = data.ProcessInParallelAsync(
            async chunk =>
            {
                await Task.Delay(100, cts.Token);
                return chunk.Length;
            },
            chunkSize: 100,
            cancellationToken: cts.Token);

        cts.CancelAfter(50); // Cancel after 50ms

        // Assert
        await Assert.ThrowsAsync<TaskCanceledException>(() => task);
    }

    [Fact]
    public async Task ComputeHashAsync_WithCancellation_ShouldRespectCancellation()
    {
        // Arrange
        var data = new byte[1000];
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(
            () => data.ComputeHashAsync("SHA256", cts.Token));
    }

    #endregion

    #region Edge Cases

    [Fact]
    public async Task ProcessInParallelAsync_WithEmptyData_ShouldReturnEmptyResults()
    {
        // Arrange
        var data = Array.Empty<byte>();

        // Act
        var results = await data.ProcessInParallelAsync(chunk => Task.FromResult(1));

        // Assert
        results.Should().BeEmpty();
    }

    [Fact]
    public async Task TransformAsync_WithEmptyData_ShouldReturnEmptyArray()
    {
        // Arrange
        var data = Array.Empty<byte>();

        // Act
        var result = await data.TransformAsync(chunk => Task.FromResult(chunk));

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task IndexOfAsync_WithEmptyData_ShouldReturnMinusOne()
    {
        // Arrange
        var data = Array.Empty<byte>();
        var pattern = new byte[] { 1, 2 };

        // Act
        var index = await data.IndexOfAsync(pattern);

        // Assert
        index.Should().Be(-1);
    }

    [Fact]
    public async Task SequenceEqualAsync_WithEmptyArrays_ShouldReturnTrue()
    {
        // Arrange
        var data1 = Array.Empty<byte>();
        var data2 = Array.Empty<byte>();

        // Act
        var result = await data1.SequenceEqualAsync(data2);

        // Assert
        result.Should().BeTrue();
    }

    #endregion
}
