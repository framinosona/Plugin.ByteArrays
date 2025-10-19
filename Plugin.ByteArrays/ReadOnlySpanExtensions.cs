using System.Globalization;

namespace Plugin.ByteArrays;

/// <summary>
///     Functions for working with ReadOnlySpan&lt;byte&gt; providing zero-allocation operations for reading and manipulating byte data.
/// </summary>
public static partial class ReadOnlySpanExtensions
{
    #region Core Utilities

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into a readable string for debugging purposes.
    ///     Each byte is represented as a decimal value, separated by commas.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt; to process.</param>
    /// <returns>A string representing the span as comma-separated decimal numbers.</returns>
    public static string ToDebugString(this ReadOnlySpan<byte> span)
    {
        if (span.IsEmpty)
        {
            return "[]";
        }

        return $"[{string.Join(",", span.ToArray())}]";
    }

    /// <summary>
    ///     Converts the given ReadOnlySpan&lt;byte&gt; to its hexadecimal string representation for debugging.
    ///     Each byte is represented as a two-digit hex value, separated by commas.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt; to process.</param>
    /// <returns>A string representing the span as comma-separated hex numbers.</returns>
    public static string ToHexDebugString(this ReadOnlySpan<byte> span)
    {
        if (span.IsEmpty)
        {
            return "[]";
        }

        return $"[{string.Join(",", span.ToArray().Select(x => $"{x:X2}"))}]";
    }

    /// <summary>
    ///     Executes the conversion of a segment of the ReadOnlySpan&lt;byte&gt; to a value of type T using the provided converter function.
    ///     Advances the position by the size of the type.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt; to process.</param>
    /// <param name="position">The position within the span. By reference; is auto-incremented by the size of the output type.</param>
    /// <param name="size">The size in bytes of the output type.</param>
    /// <param name="converter">The function to convert the ReadOnlySpan&lt;byte&gt; to the output type.</param>
    /// <typeparam name="T">Type of the output.</typeparam>
    /// <returns>Converted value of type T.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the position or size is invalid, or if the span is too small.</exception>
    private static T ExecuteConversionToType<T>(this ReadOnlySpan<byte> span, ref int position, int size, Func<ReadOnlySpan<byte>, T> converter)
    {
        if (size <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(size), $"Size must be greater than 0. Was {size}.");
        }
        if (position < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(position), $"Position must be greater than or equal to 0. Was {position}.");
        }
        if (span.Length < position + size)
        {
            throw new ArgumentOutOfRangeException(nameof(position), $"Span {span.ToDebugString()} is too small. Reading {size} bytes from position {position} is not possible in span of {span.Length}");
        }

        var slice = span.Slice(position, size);
        var output = converter(slice);
        position += size;
        return output;
    }

    /// <summary>
    ///     Executes the conversion of a segment of the ReadOnlySpan&lt;byte&gt; to a value of type T using the provided converter function.
    ///     Returns the default value if conversion fails. Advances the position by the size of the type if successful.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt; to process.</param>
    /// <param name="position">The position within the span. By reference; is auto-incremented by the size of the output type.</param>
    /// <param name="size">The size in bytes of the output type.</param>
    /// <param name="converter">The function to convert the ReadOnlySpan&lt;byte&gt; to the output type.</param>
    /// <param name="defaultValue">Default value to return if conversion fails.</param>
    /// <typeparam name="T">Type of the output.</typeparam>
    /// <param name="errorHandler">Optional error handler invoked on conversion failure.</param>
    /// <returns>Converted value of type T, or defaultValue if conversion fails.</returns>
    private static T ExecuteConversionToTypeOrDefault<T>(
        this ReadOnlySpan<byte> span,
        ref int position,
        int size,
        Func<ReadOnlySpan<byte>, T> converter,
        T defaultValue,
        Action<ReadOnlySpan<byte>, int, int, Exception>? errorHandler = null
    )
    {
#pragma warning disable CA1031 // Do not catch general exception types
        try
        {
            return ExecuteConversionToType(span, ref position, size, converter);
        }
        catch (Exception ex)
        {
            errorHandler?.Invoke(span, position, size, ex);
            return defaultValue;
        }
#pragma warning restore CA1031 // Do not catch general exception types
    }

    #endregion

    #region Pattern Matching and Search

    /// <summary>
    ///     Checks if a ReadOnlySpan&lt;byte&gt; starts with a specific pattern.
    /// </summary>
    /// <param name="span">The span to check.</param>
    /// <param name="pattern">The pattern to look for.</param>
    /// <returns>True if the span starts with the pattern, false otherwise.</returns>
    public static bool StartsWith(this ReadOnlySpan<byte> span, ReadOnlySpan<byte> pattern)
    {
        if (pattern.IsEmpty)
        {
            return true;
        }
        if (span.Length < pattern.Length)
        {
            return false;
        }

        return span[..pattern.Length].SequenceEqual(pattern);
    }

    /// <summary>
    ///     Determines whether the ReadOnlySpan&lt;byte&gt; ends with the specified pattern.
    ///     This method is case-sensitive and culture-insensitive.
    /// </summary>
    /// <param name="span">The span to check.</param>
    /// <param name="pattern">The pattern to check for at the end of the span.</param>
    /// <returns>True if the span ends with the specified pattern; otherwise, false.</returns>
    public static bool EndsWith(this ReadOnlySpan<byte> span, ReadOnlySpan<byte> pattern)
    {
        if (pattern.IsEmpty)
        {
            return true;
        }
        if (span.Length < pattern.Length)
        {
            return false;
        }

        var startIndex = span.Length - pattern.Length;
        return span[startIndex..].SequenceEqual(pattern);
    }

    /// <summary>
    ///     Finds the first occurrence of a pattern in a ReadOnlySpan&lt;byte&gt;.
    /// </summary>
    /// <param name="span">The span to search in.</param>
    /// <param name="pattern">The pattern to search for.</param>
    /// <returns>The index of the first occurrence, or -1 if not found.</returns>
    public static int IndexOf(this ReadOnlySpan<byte> span, ReadOnlySpan<byte> pattern)
    {
        if (pattern.IsEmpty)
        {
            return 0;
        }
        if (span.Length < pattern.Length)
        {
            return -1;
        }

        for (var i = 0; i <= span.Length - pattern.Length; i++)
        {
            if (span.Slice(i, pattern.Length).SequenceEqual(pattern))
            {
                return i;
            }
        }

        return -1;
    }

    #endregion

    #region Comparison and Equality

    /// <summary>
    ///     Checks if two ReadOnlySpan&lt;byte&gt; instances are identical in content and length.
    /// </summary>
    /// <param name="span1">First ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="span2">Second ReadOnlySpan&lt;byte&gt;.</param>
    /// <returns>True if spans are identical; otherwise, false.</returns>
    public static bool IsIdenticalTo(this ReadOnlySpan<byte> span1, ReadOnlySpan<byte> span2)
    {
        return span1.SequenceEqual(span2);
    }

    #endregion
}
