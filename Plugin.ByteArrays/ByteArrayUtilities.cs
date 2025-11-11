using System.Globalization;

namespace Plugin.ByteArrays;

/// <summary>
///     Utility extensions for byte array analysis, formatting, and performance measurement.
/// </summary>
public static class ByteArrayUtilities
{
    #region Binary String Representation

    /// <summary>
    ///     Converts the byte array to a binary string representation.
    ///     Each byte is represented as 8 bits separated by spaces.
    /// </summary>
    /// <param name="array">The byte array to convert.</param>
    /// <param name="separator">The separator between bytes (default is space).</param>
    /// <returns>A binary string representation of the byte array.</returns>
    public static string ToBinaryString(this byte[] array, string separator = " ")
    {
        ArgumentNullException.ThrowIfNull(array);
        ArgumentNullException.ThrowIfNull(separator);

        if (array.Length == 0)
        {
            return string.Empty;
        }

        return string.Join(separator, array.Select(b => Convert.ToString(b, 2).PadLeft(8, '0')));
    }

    /// <summary>
    ///     Converts a binary string back to a byte array.
    /// </summary>
    /// <param name="binaryString">The binary string to convert (e.g., "10101010 11110000").</param>
    /// <param name="separator">The separator used between bytes (default is space).</param>
    /// <returns>The byte array represented by the binary string.</returns>
    public static byte[] FromBinaryString(string binaryString, string separator = " ")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(binaryString);
        ArgumentNullException.ThrowIfNull(separator);

        var binaryParts = binaryString.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);
        var result = new byte[binaryParts.Length];

        for (var i = 0; i < binaryParts.Length; i++)
        {
            var binaryPart = binaryParts[i].Trim();
            if (binaryPart.Length != 8)
            {
                throw new ArgumentException($"Invalid binary representation at position {i}: '{binaryPart}'. Each byte must be exactly 8 bits.");
            }

            result[i] = Convert.ToByte(binaryPart, 2);
        }

        return result;
    }

    #endregion

    #region Hex Dump Functionality

    /// <summary>
    ///     Creates a formatted hex dump of the byte array similar to hex editors.
    /// </summary>
    /// <param name="array">The byte array to dump.</param>
    /// <param name="bytesPerLine">Number of bytes per line (default 16).</param>
    /// <param name="showAscii">Whether to show ASCII representation (default true).</param>
    /// <param name="showOffsets">Whether to show byte offsets (default true).</param>
    /// <returns>A formatted hex dump string.</returns>
    public static string ToHexDump(this byte[] array, int bytesPerLine = 16, bool showAscii = true, bool showOffsets = true)
    {
        ArgumentNullException.ThrowIfNull(array);

        if (bytesPerLine <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(bytesPerLine), "Bytes per line must be positive");
        }

        if (array.Length == 0)
        {
            return string.Empty;
        }

        var result = new System.Text.StringBuilder();
        var lineBuilder = new System.Text.StringBuilder();

        for (var offset = 0; offset < array.Length; offset += bytesPerLine)
        {
            lineBuilder.Clear();

            // Add offset
            if (showOffsets)
            {
                lineBuilder.Append(CultureInfo.InvariantCulture, $"{offset:X8}: ");
            }

            // Add hex bytes
            var bytesInLine = Math.Min(bytesPerLine, array.Length - offset);
            for (var i = 0; i < bytesPerLine; i++)
            {
                if (i < bytesInLine)
                {
                    lineBuilder.Append(CultureInfo.InvariantCulture, $"{array[offset + i]:X2} ");
                }
                else
                {
                    lineBuilder.Append("   "); // Space for missing bytes
                }

                // Add extra space in the middle for readability
                if (i == bytesPerLine / 2 - 1)
                {
                    lineBuilder.Append(' ');
                }
            }

            // Add ASCII representation
            if (showAscii)
            {
                lineBuilder.Append(" |");
                for (var i = 0; i < bytesInLine; i++)
                {
                    var b = array[offset + i];
                    lineBuilder.Append(b >= 32 && b <= 126 ? (char)b : '.');
                }
                lineBuilder.Append('|');
            }

            result.AppendLine(lineBuilder.ToString());
        }

        return result.ToString().TrimEnd();
    }

    #endregion

    #region Basic Analysis

    /// <summary>
    ///     Calculates the entropy of the byte array (0-8, where 8 is maximum entropy).
    /// </summary>
    /// <param name="array">The byte array to analyze.</param>
    /// <returns>The entropy value.</returns>
    public static double CalculateEntropy(this byte[] array)
    {
        ArgumentNullException.ThrowIfNull(array);

        if (array.Length == 0)
        {
            return 0.0;
        }

        var frequency = new int[256];
        foreach (var b in array)
        {
            frequency[b]++;
        }

        var entropy = 0.0;
        var length = array.Length;

        for (var i = 0; i < 256; i++)
        {
            if (frequency[i] > 0)
            {
                var probability = (double)frequency[i] / length;
                entropy -= probability * Math.Log2(probability);
            }
        }

        return entropy;
    }

    /// <summary>
    ///     Counts the occurrences of each byte value in the array.
    /// </summary>
    /// <param name="array">The byte array to analyze.</param>
    /// <returns>A dictionary with byte frequencies.</returns>
    public static Dictionary<byte, int> GetByteFrequency(this byte[] array)
    {
        ArgumentNullException.ThrowIfNull(array);

        var frequency = new Dictionary<byte, int>();
        foreach (var b in array)
        {
            frequency[b] = frequency.GetValueOrDefault(b, 0) + 1;
        }

        return frequency;
    }

    /// <summary>
    ///     Determines if the data appears to be compressed or encrypted based on entropy.
    /// </summary>
    /// <param name="array">The byte array to analyze.</param>
    /// <returns>True if the data appears compressed/encrypted, false otherwise.</returns>
    public static bool AppearsCompressed(this byte[] array)
    {
        return array.CalculateEntropy() > 7.5;
    }

    #endregion

    #region Performance Measurement

    /// <summary>
    ///     Measures the execution time of an operation on the byte array.
    /// </summary>
    /// <typeparam name="T">The return type of the operation.</typeparam>
    /// <param name="array">The byte array to operate on.</param>
    /// <param name="operation">The operation to measure.</param>
    /// <param name="iterations">Number of iterations to run (default 1).</param>
    /// <returns>A tuple with the result, total time, and average time per iteration.</returns>
    public static (T result, TimeSpan totalTime, TimeSpan averageTime) MeasurePerformance<T>(
        this byte[] array,
        Func<byte[], T> operation,
        int iterations = 1)
    {
        ArgumentNullException.ThrowIfNull(array);
        ArgumentNullException.ThrowIfNull(operation);

        if (iterations <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(iterations), "Iterations must be positive");
        }

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        T result = default!;

        for (var i = 0; i < iterations; i++)
        {
            result = operation(array);
        }

        stopwatch.Stop();
        var totalTime = stopwatch.Elapsed;
        var averageTime = TimeSpan.FromTicks(totalTime.Ticks / iterations);

        return (result, totalTime, averageTime);
    }

    /// <summary>
    ///     Calculates the throughput rate for a byte array operation.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="elapsedTime">The time taken to process the array.</param>
    /// <returns>The throughput in bytes per second.</returns>
    public static double CalculateThroughput(this byte[] array, TimeSpan elapsedTime)
    {
        ArgumentNullException.ThrowIfNull(array);

        if (elapsedTime.TotalSeconds <= 0)
        {
            return 0;
        }

        return array.Length / elapsedTime.TotalSeconds;
    }

    #endregion
}
