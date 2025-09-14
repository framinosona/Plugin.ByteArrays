namespace Plugin.ByteArrays;

/// <summary>
///     Functions converting other types into a byte array.
/// </summary>
public static partial class ByteArrayExtensions
{
    #region Core Utilities

    /// <summary>
    ///     Converts the byte array into a readable string for debugging purposes.
    ///     Each byte is represented as a decimal value, separated by commas.
    ///     Returns "&lt;null&gt;" if the input is null.
    /// </summary>
    /// <param name="array">The byte-enumerable to process.</param>
    /// <returns>A string representing the byte-enumerable as comma-separated decimal numbers.</returns>
    public static string ToDebugString(this IEnumerable<byte>? array)
    {
        if (array == null)
        {
            return "<null>";
        }

        return $"[{string.Join(",", array)}]";
    }

    /// <summary>
    ///     Converts the given byte-array to its hexadecimal string representation for debugging.
    ///     Each byte is represented as a two-digit hex value, separated by commas.
    ///     Returns "&lt;null&gt;" if the input is null.
    /// </summary>
    /// <param name="array">The byte-enumerable to process.</param>
    /// <returns>A string representing the byte-enumerable as comma-separated hex numbers.</returns>
    public static string ToHexDebugString(this IEnumerable<byte>? array)
    {
        if (array == null)
        {
            return "<null>";
        }

        return $"[{string.Join(",", array.Select(x => $"{x:X2}"))}]";
    }

    /// <summary>
    ///     Executes the conversion of a segment of the byte array to a value of type T using the provided converter function.
    ///     Advances the position by the size of the type.
    /// </summary>
    /// <param name="array">The byte array to process. Cannot be null.</param>
    /// <param name="position">The position within the array. By reference; is auto-incremented by the size of the output type.</param>
    /// <param name="size">The size in bytes of the output type.</param>
    /// <param name="bitConverterFunction">The function to convert the byte array to the output type.</param>
    /// <typeparam name="T">Type of the output.</typeparam>
    /// <returns>Converted value of type T.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the array is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the position or size is invalid, or if the array is too small.</exception>
    private static T ExecuteConversionToType<T>(this byte[] array, ref int position, int size, Func<byte[], int, T> bitConverterFunction)
    {
        ArgumentNullException.ThrowIfNull(array);
        if (size <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(size), $"Size must be greater than 0. Was {size}.");
        }
        if (position < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(position), $"Position must be greater than or equal to  0. Was {position}.");
        }
        if (array.Length < position + size)
        {
            throw new ArgumentOutOfRangeException(nameof(position), $"Array {array.ToDebugString()} is too small. Reading {size} bytes from position {position} is not possible in array of {array.Length}");
        }
        var usefulArray = array.Skip(position).Take(size).ToArray();
        var output = bitConverterFunction.Invoke(usefulArray, 0);
        position += size;
        return output;
    }

    /// <summary>
    ///     Executes the conversion of a segment of the byte array to a value of type T using the provided converter function.
    ///     Returns the default value if conversion fails. Advances the position by the size of the type if successful.
    /// </summary>
    /// <param name="array">The byte array to process. Cannot be null.</param>
    /// <param name="position">The position within the array. By reference; is auto-incremented by the size of the output type.</param>
    /// <param name="size">The size in bytes of the output type.</param>
    /// <param name="bitConverterFunction">The function to convert the byte array to the output type.</param>
    /// <param name="defaultValue">Default value to return if conversion fails.</param>
    /// <typeparam name="T">Type of the output.</typeparam>
    /// <param name="errorHandler">Optional error handler invoked on conversion failure.</param>
    /// <returns>Converted value of type T, or defaultValue if conversion fails.</returns>
    private static T ExecuteConversionToTypeOrDefault<T>(
        this byte[] array,
        ref int position,
        int size,
        Func<byte[], int, T> bitConverterFunction,
        T defaultValue,
        Action<byte[], int, int, Exception>? errorHandler = null
    )
    {
        try
        {
            return ExecuteConversionToType(array, ref position, size, bitConverterFunction);
        }
        catch (Exception ex)
        {
            errorHandler?.Invoke(array, position, size, ex);
            return defaultValue;
        }
    }

    #endregion

    #region Pattern Matching and Search

    /// <summary>
    ///     Checks if a byte array starts with a specific pattern.
    /// </summary>
    /// <param name="array">The array to check.</param>
    /// <param name="pattern">The pattern to look for.</param>
    /// <returns>True if the array starts with the pattern, false otherwise.</returns>
    public static bool StartsWith(this byte[] array, byte[] pattern)
    {
        if (pattern.Length == 0)
        {
            return true;
        }
        if (array.Length < pattern.Length)
        {
            return false;
        }

        return ((ReadOnlySpan<byte>)array[..pattern.Length]).SequenceEqual(pattern);
    }

    /// <summary>
    ///     Determines whether the byte array ends with the specified pattern.
    ///     This method is case-sensitive and culture-insensitive.
    /// </summary>
    /// <param name="array">The byte array to check.</param>
    /// <param name="pattern">The pattern to check for at the end of the array.</param>
    /// <returns>True if the array ends with the specified pattern; otherwise, false.</returns>
    public static bool EndsWith(this byte[] array, byte[] pattern)
    {
        if (pattern.Length == 0)
        {
            return true;
        }
        if (array.Length < pattern.Length)
        {
            return false;
        }

        var startIndex = array.Length - pattern.Length;
        return ((ReadOnlySpan<byte>)array[startIndex..]).SequenceEqual(pattern);
    }


    /// <summary>
    ///     Finds the first occurrence of a pattern in a byte array.
    /// </summary>
    /// <param name="array">The array to search in.</param>
    /// <param name="pattern">The pattern to search for.</param>
    /// <returns>The index of the first occurrence, or -1 if not found.</returns>
    public static int IndexOf(this byte[] array, byte[] pattern)
    {
        if (pattern.Length == 0)
        {
            return 0;
        }
        if (array.Length < pattern.Length)
        {
            return -1;
        }

        for (var i = 0; i <= array.Length - pattern.Length; i++)
        {
            if (((ReadOnlySpan<byte>)array.AsSpan(i, pattern.Length)).SequenceEqual(pattern))
            {
                return i;
            }
        }

        return -1;
    }

    #endregion

    #region Comparison and Equality

    /// <summary>
    ///     Checks if two byte arrays are identical in content and length.
    ///     Uses reference equality and null checks for performance and correctness.
    /// </summary>
    /// <param name="a1">First byte array.</param>
    /// <param name="a2">Second byte array.</param>
    /// <returns>True if arrays are identical; otherwise, false.</returns>
    public static bool IsIdenticalTo(this byte[]? a1, byte[]? a2)
    {
        if (ReferenceEquals(a1, a2)) //00 order
        {
            return true;
        }

        if (a1 == null || a2 == null || a1.Length != a2.Length) //00 order
        {
            return false;
        }

        return ((ReadOnlySpan<byte>)a1).SequenceEqual(a2); //10

        //00  we need the reference-equals checks and the null checks because if we invoke sequence-equals directly and one of the arrays
        //    is null and the other one a zero length array then sequence-equals will return that they are equal even though they are not!
        //
        //10  vital to cast to ReadOnlySpan<byte> for the sake of extra performance per https://stackoverflow.com/a/79421673/863651
    }

    #endregion
}
