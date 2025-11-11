namespace Plugin.ByteArrays;

/// <summary>
///     Utility functions for analyzing and working with ReadOnlySpan&lt;byte&gt; data.
/// </summary>
public static class ReadOnlySpanUtilities
{
    /// <summary>
    ///     Calculates the Shannon entropy of the ReadOnlySpan&lt;byte&gt;.
    ///     Entropy is a measure of randomness/unpredictability in the data.
    ///     Returns a value between 0.0 (perfectly ordered) and 8.0 (maximum entropy for bytes).
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt; to analyze.</param>
    /// <returns>The calculated entropy value.</returns>
    public static double CalculateEntropy(this ReadOnlySpan<byte> span)
    {
        if (span.IsEmpty)
        {
            return 0.0;
        }

        var frequency = new int[256];
        foreach (var b in span)
        {
            frequency[b]++;
        }

        var entropy = 0.0;
        var length = span.Length;

        for (var i = 0; i < 256; i++)
        {
            if (frequency[i] == 0)
            {
                continue;
            }

            var probability = (double)frequency[i] / length;
            entropy -= probability * Math.Log2(probability);
        }

        return entropy;
    }

    /// <summary>
    ///     Analyzes the distribution of byte values in the ReadOnlySpan&lt;byte&gt;.
    ///     Returns a dictionary mapping each byte value to its frequency count.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt; to analyze.</param>
    /// <returns>A dictionary with byte values as keys and their frequencies as values.</returns>
    public static Dictionary<byte, int> AnalyzeDistribution(this ReadOnlySpan<byte> span)
    {
        var distribution = new Dictionary<byte, int>(256);

        foreach (var b in span)
        {
            distribution[b] = distribution.TryGetValue(b, out var count) ? count + 1 : 1;
        }

        return distribution;
    }

    /// <summary>
    ///     Counts the occurrences of a specific byte value in the ReadOnlySpan&lt;byte&gt;.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt; to search.</param>
    /// <param name="value">The byte value to count.</param>
    /// <returns>The number of occurrences of the specified byte value.</returns>
    public static int CountOccurrences(this ReadOnlySpan<byte> span, byte value)
    {
        var count = 0;
        foreach (var b in span)
        {
            if (b == value)
            {
                count++;
            }
        }
        return count;
    }

    /// <summary>
    ///     Finds all indices where a specific byte value occurs in the ReadOnlySpan&lt;byte&gt;.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt; to search.</param>
    /// <param name="value">The byte value to find.</param>
    /// <returns>An array of indices where the byte value was found.</returns>
    public static int[] FindAllIndices(this ReadOnlySpan<byte> span, byte value)
    {
        var indices = new List<int>();
        for (var i = 0; i < span.Length; i++)
        {
            if (span[i] == value)
            {
                indices.Add(i);
            }
        }
        return indices.ToArray();
    }

    /// <summary>
    ///     Checks if all bytes in the ReadOnlySpan&lt;byte&gt; have the same value.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt; to check.</param>
    /// <returns>True if all bytes are identical or span is empty; otherwise, false.</returns>
    public static bool AllBytesEqual(this ReadOnlySpan<byte> span)
    {
        if (span.IsEmpty)
        {
            return true;
        }

        var firstByte = span[0];
        for (var i = 1; i < span.Length; i++)
        {
            if (span[i] != firstByte)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    ///     Checks if the ReadOnlySpan&lt;byte&gt; contains only zero bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt; to check.</param>
    /// <returns>True if all bytes are zero or span is empty; otherwise, false.</returns>
    public static bool IsAllZeros(this ReadOnlySpan<byte> span)
    {
        foreach (var b in span)
        {
            if (b != 0)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    ///     Reverses the bytes in a copy of the ReadOnlySpan&lt;byte&gt;.
    ///     Note: This creates a new array since ReadOnlySpan is immutable.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt; to reverse.</param>
    /// <returns>A new byte array with reversed byte order.</returns>
    public static byte[] Reverse(this ReadOnlySpan<byte> span)
    {
        var result = span.ToArray();
        Array.Reverse(result);
        return result;
    }

    /// <summary>
    ///     Performs an XOR operation between two ReadOnlySpan&lt;byte&gt; instances.
    ///     If spans have different lengths, operates on the shorter length.
    /// </summary>
    /// <param name="span1">First ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="span2">Second ReadOnlySpan&lt;byte&gt;.</param>
    /// <returns>A new byte array containing the XOR result.</returns>
    public static byte[] Xor(this ReadOnlySpan<byte> span1, ReadOnlySpan<byte> span2)
    {
        var length = Math.Min(span1.Length, span2.Length);
        var result = new byte[length];

        for (var i = 0; i < length; i++)
        {
            result[i] = (byte)(span1[i] ^ span2[i]);
        }

        return result;
    }
}
