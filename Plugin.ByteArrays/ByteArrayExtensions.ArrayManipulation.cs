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

}
