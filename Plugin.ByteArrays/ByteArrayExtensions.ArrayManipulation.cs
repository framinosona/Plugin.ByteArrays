using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;

namespace Plugin.ByteArrays;

/// <summary>
///     Functions converting other types into a byte array.
/// </summary>
public static partial class ByteArrayExtensions
{
    #region Array Manipulation

    /// <summary>
    ///     Removes trailing bytes with the specified value from the array.
    ///     This method returns a new trimmed array and does not modify the original.
    ///     Use TrimEndNonDestructive for clarity or this method for backward compatibility.
    /// </summary>
    /// <param name="array">The byte array to trim. Cannot be null.</param>
    /// <param name="byteValueToTrim">The byte value to trim from the end. Default is 0.</param>
    /// <returns>A new array with trailing bytes removed.</returns>
    public static byte[] TrimEnd(this byte[] array, byte byteValueToTrim = 0)
    {
        return TrimEndNonDestructive(array, byteValueToTrim);
    }

    /// <summary>
    ///     Creates a new byte array with trailing bytes removed (non-destructive version of TrimEnd).
    /// </summary>
    /// <param name="array">The byte array to process.</param>
    /// <param name="byteValueToTrim">The byte value to trim from the end. Default is 0.</param>
    /// <returns>A new byte array with trailing bytes removed.</returns>
    public static byte[] TrimEndNonDestructive(this byte[] array, byte byteValueToTrim = 0)
    {
        ArgumentNullException.ThrowIfNull(array);
        if (array.Length == 0)
        {
            return array;
        }

        var lastIndex = Array.FindLastIndex(array, b => b != byteValueToTrim);

        if (lastIndex < 0)
        {
            return []; // All bytes are the trim value
        }

        if (lastIndex == array.Length - 1)
        {
            return array; // No trimming needed
        }

        var result = new byte[lastIndex + 1];
        Array.Copy(array, result, lastIndex + 1);
        return result;
    }

    /// <summary>
    ///     Safely slices a byte array without throwing exceptions.
    /// </summary>
    /// <param name="array">The source array.</param>
    /// <param name="start">The starting index.</param>
    /// <param name="length">The number of bytes to take.</param>
    /// <returns>A new byte array containing the sliced data, or empty array if parameters are invalid.</returns>
    public static byte[] SafeSlice(this byte[] array, int start, int length)
    {
        ArgumentNullException.ThrowIfNull(array);
        if (start < 0 || length <= 0 || start >= array.Length)
        {
            return [];
        }

        var actualLength = Math.Min(length, array.Length - start);
        var result = new byte[actualLength];
        Array.Copy(array, start, result, 0, actualLength);
        return result;
    }

    /// <summary>
    ///     Concatenates multiple byte arrays efficiently.
    /// </summary>
    /// <param name="arrays">The byte arrays to concatenate.</param>
    /// <returns>A new byte array containing all input arrays concatenated.</returns>
    public static byte[] Concatenate(params byte[][] arrays)
    {
        ArgumentNullException.ThrowIfNull(arrays);
        if (arrays.Length == 0)
        {
            return [];
        }

        // Treat null arrays as empty arrays
        var totalLength = arrays.Where(a => a != null).Sum(a => a.Length);
        if (totalLength == 0)
        {
            return [];
        }

        var result = new byte[totalLength];
        var offset = 0;

        foreach (var array in arrays)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (array == null || array.Length == 0)
            {
                continue;
            }
            Array.Copy(array, 0, result, offset, array.Length);
            offset += array.Length;
        }

        return result;
    }

    /// <summary>
    ///     Reverses the byte array and returns a new array.
    /// </summary>
    /// <param name="array">The byte array to reverse.</param>
    /// <returns>A new byte array with elements in reverse order.</returns>
    public static byte[] Reverse(this byte[] array)
    {
        ArgumentNullException.ThrowIfNull(array);
        var result = new byte[array.Length];
        for (var i = 0; i < array.Length; i++)
        {
            result[i] = array[array.Length - 1 - i];
        }
        return result;
    }

    /// <summary>
    ///     Performs an XOR operation between two byte arrays.
    /// </summary>
    /// <param name="array1">The first array.</param>
    /// <param name="array2">The second array.</param>
    /// <returns>A new byte array containing the XOR result.</returns>
    /// <exception cref="ArgumentException">Thrown when arrays have different lengths.</exception>
    public static byte[] Xor(this byte[] array1, byte[] array2)
    {
        if (array1 == null || array2 == null)
        {
            throw new ArgumentNullException(array1 == null ? nameof(array1) : nameof(array2));
        }

        if (array1.Length != array2.Length)
        {
            throw new ArgumentException("Arrays must have the same length for XOR operation.");
        }

        var result = new byte[array1.Length];
        for (var i = 0; i < array1.Length; i++)
        {
            result[i] = (byte)(array1[i] ^ array2[i]);
        }
        return result;
    }

    #endregion

    #region Advanced Search Operations

    /// <summary>
    ///     Searches for all occurrences of a byte pattern in the array.
    ///     Returns an array of indices where the pattern is found.
    /// </summary>
    /// <param name="array">The source array to search in.</param>
    /// <param name="pattern">The pattern to search for.</param>
    /// <returns>An array of indices where the pattern occurs.</returns>
    public static int[] IndexOfAll(this byte[] array, byte[] pattern)
    {
        ArgumentNullException.ThrowIfNull(array);
        ArgumentNullException.ThrowIfNull(pattern);

        var indices = new List<int>();

        if (pattern.Length == 0 || pattern.Length > array.Length)
        {
            return indices.ToArray();
        }

        for (var i = 0; i <= array.Length - pattern.Length; i++)
        {
            var found = true;
            for (var j = 0; j < pattern.Length; j++)
            {
                if (array[i + j] != pattern[j])
                {
                    found = false;
                    break;
                }
            }
            if (found)
            {
                indices.Add(i);
            }
        }

        return indices.ToArray();
    }

    #endregion

    #region Bit Operations

    /// <summary>
    ///     Sets a specific bit at the given position to 1.
    /// </summary>
    /// <param name="array">The byte array to modify.</param>
    /// <param name="bitPosition">The bit position (0-based, where 0 is LSB of first byte).</param>
    /// <returns>A new byte array with the bit set.</returns>
    public static byte[] SetBit(this byte[] array, int bitPosition)
    {
        ArgumentNullException.ThrowIfNull(array);

        var byteIndex = bitPosition / 8;
        var bitIndex = bitPosition % 8;

        if (byteIndex >= array.Length)
            throw new ArgumentOutOfRangeException(nameof(bitPosition), "Bit position exceeds array length");

        var result = new byte[array.Length];
        Array.Copy(array, result, array.Length);

        result[byteIndex] |= (byte)(1 << bitIndex);
        return result;
    }

    /// <summary>
    ///     Gets the value of a specific bit at the given position.
    /// </summary>
    /// <param name="array">The byte array to read from.</param>
    /// <param name="bitPosition">The bit position (0-based, where 0 is LSB of first byte).</param>
    /// <returns>True if the bit is set, false otherwise.</returns>
    public static bool GetBit(this byte[] array, int bitPosition)
    {
        ArgumentNullException.ThrowIfNull(array);

        var byteIndex = bitPosition / 8;
        var bitIndex = bitPosition % 8;

        if (byteIndex >= array.Length)
            throw new ArgumentOutOfRangeException(nameof(bitPosition), "Bit position exceeds array length");

        return (array[byteIndex] & (1 << bitIndex)) != 0;
    }

    /// <summary>
    ///     Reverses the bits in each byte of the array.
    /// </summary>
    /// <param name="array">The byte array to process.</param>
    /// <returns>A new byte array with bits reversed in each byte.</returns>
    public static byte[] BitReverse(this byte[] array)
    {
        ArgumentNullException.ThrowIfNull(array);

        var result = new byte[array.Length];
        for (var i = 0; i < array.Length; i++)
        {
            var b = array[i];
            var reversed = (byte)0;
            for (var j = 0; j < 8; j++)
            {
                reversed = (byte)((reversed << 1) | (b & 1));
                b >>= 1;
            }
            result[i] = reversed;
        }
        return result;
    }

    #endregion

    #region Splitting and Chunking

    /// <summary>
    ///     Splits the byte array into chunks of the specified size.
    /// </summary>
    /// <param name="array">The source array to split.</param>
    /// <param name="chunkSize">The size of each chunk.</param>
    /// <returns>An enumerable of byte arrays representing the chunks.</returns>
    public static IEnumerable<byte[]> Split(this byte[] array, int chunkSize)
    {
        ArgumentNullException.ThrowIfNull(array);
        if (chunkSize <= 0)
        {
            throw new ArgumentException("Chunk size must be positive", nameof(chunkSize));
        }

        for (var i = 0; i < array.Length; i += chunkSize)
        {
            var actualChunkSize = Math.Min(chunkSize, array.Length - i);
            var chunk = new byte[actualChunkSize];
            Array.Copy(array, i, chunk, 0, actualChunkSize);
            yield return chunk;
        }
    }

    /// <summary>
    ///     Splits the byte array at the specified delimiter.
    /// </summary>
    /// <param name="array">The source array to split.</param>
    /// <param name="delimiter">The delimiter byte.</param>
    /// <returns>An enumerable of byte arrays split at the delimiter.</returns>
    public static IEnumerable<byte[]> SplitAt(this byte[] array, byte delimiter)
    {
        ArgumentNullException.ThrowIfNull(array);

        var start = 0;
        for (var i = 0; i < array.Length; i++)
        {
            if (array[i] == delimiter)
            {
                var segment = new byte[i - start];
                Array.Copy(array, start, segment, 0, i - start);
                yield return segment;
                start = i + 1;
            }
        }

        // Return the last segment
        if (start < array.Length)
        {
            var lastSegment = new byte[array.Length - start];
            Array.Copy(array, start, lastSegment, 0, array.Length - start);
            yield return lastSegment;
        }
    }

    #endregion

    #region Checksums and Hashing

    /// <summary>
    ///     Calculates the CRC32 checksum of the byte array.
    /// </summary>
    /// <param name="array">The byte array to calculate checksum for.</param>
    /// <returns>The CRC32 checksum as a 32-bit unsigned integer.</returns>
    public static uint CalculateCrc32(this byte[] array)
    {
        ArgumentNullException.ThrowIfNull(array);

        const uint polynomial = 0xEDB88320;
        var crc = 0xFFFFFFFFu;

        foreach (var b in array)
        {
            crc ^= b;
            for (var i = 0; i < 8; i++)
            {
                crc = (crc & 1) != 0 ? (crc >> 1) ^ polynomial : crc >> 1;
            }
        }

        return ~crc;
    }

    /// <summary>
    ///     Calculates the MD5 hash of the byte array.
    ///     Note: MD5 is considered cryptographically broken and should not be used for security purposes.
    /// </summary>
    /// <param name="array">The byte array to hash.</param>
    /// <returns>The MD5 hash as a byte array.</returns>
    [SuppressMessage("Security", "CA5351:Do Not Use Broken Cryptographic Algorithms", Justification = "Legacy support with clear documentation warning")]
    public static byte[] CalculateMd5(this byte[] array)
    {
        ArgumentNullException.ThrowIfNull(array);
        return MD5.HashData(array);
    }

    /// <summary>
    ///     Calculates the SHA-256 hash of the byte array.
    /// </summary>
    /// <param name="array">The byte array to hash.</param>
    /// <returns>The SHA-256 hash as a byte array.</returns>
    public static byte[] CalculateSha256(this byte[] array)
    {
        ArgumentNullException.ThrowIfNull(array);
        return SHA256.HashData(array);
    }

    /// <summary>
    ///     Calculates the SHA-1 hash of the byte array.
    ///     Note: SHA-1 is considered weak and should not be used for security-critical applications.
    /// </summary>
    /// <param name="array">The byte array to hash.</param>
    /// <returns>The SHA-1 hash as a byte array.</returns>
    [SuppressMessage("Security", "CA5350:Do Not Use Weak Cryptographic Algorithms", Justification = "Legacy support with clear documentation warning")]
    public static byte[] CalculateSha1(this byte[] array)
    {
        ArgumentNullException.ThrowIfNull(array);
        return SHA1.HashData(array);
    }

    #endregion

    #region Padding Operations

    /// <summary>
    ///     Pads the byte array to the specified length with the given padding byte.
    /// </summary>
    /// <param name="array">The source array to pad.</param>
    /// <param name="totalLength">The desired total length.</param>
    /// <param name="paddingByte">The byte to use for padding (default is 0).</param>
    /// <param name="padLeft">True to pad on the left, false to pad on the right.</param>
    /// <returns>A new padded byte array.</returns>
    public static byte[] Pad(this byte[] array, int totalLength, byte paddingByte = 0, bool padLeft = false)
    {
        ArgumentNullException.ThrowIfNull(array);

        if (totalLength <= array.Length)
        {
            return array;
        }

        var result = new byte[totalLength];
        var paddingLength = totalLength - array.Length;

        if (padLeft)
        {
            // Pad on the left
            for (var i = 0; i < paddingLength; i++)
            {
                result[i] = paddingByte;
            }
            Array.Copy(array, 0, result, paddingLength, array.Length);
        }
        else
        {
            // Pad on the right
            Array.Copy(array, 0, result, 0, array.Length);
            for (var i = array.Length; i < totalLength; i++)
            {
                result[i] = paddingByte;
            }
        }

        return result;
    }

    /// <summary>
    ///     Removes padding from the byte array.
    /// </summary>
    /// <param name="array">The padded array.</param>
    /// <param name="paddingByte">The padding byte to remove (default is 0).</param>
    /// <param name="fromLeft">True to remove padding from the left, false from the right.</param>
    /// <returns>A new byte array with padding removed.</returns>
    public static byte[] RemovePadding(this byte[] array, byte paddingByte = 0, bool fromLeft = false)
    {
        ArgumentNullException.ThrowIfNull(array);

        if (array.Length == 0)
        {
            return array;
        }

        var start = 0;
        var end = array.Length;

        if (fromLeft)
        {
            // Remove padding from the left
            while (start < array.Length && array[start] == paddingByte)
            {
                start++;
            }
        }
        else
        {
            // Remove padding from the right
            while (end > 0 && array[end - 1] == paddingByte)
            {
                end--;
            }
        }

        if (start >= end)
        {
            return Array.Empty<byte>();
        }

        var result = new byte[end - start];
        Array.Copy(array, start, result, 0, end - start);
        return result;
    }

    #endregion

}
