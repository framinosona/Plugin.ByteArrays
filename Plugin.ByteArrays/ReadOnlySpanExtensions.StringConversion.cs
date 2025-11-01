using System.Text;

namespace Plugin.ByteArrays;

/// <summary>
///     Functions for working with ReadOnlySpan&lt;byte&gt; providing zero-allocation operations for reading and manipulating byte data.
/// </summary>
public static partial class ReadOnlySpanExtensions
{
    #region String Conversions

    /// <summary>
    ///     Converts parts of a ReadOnlySpan&lt;byte&gt; into a UTF-8 String.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span. By reference; is auto-incremented by the size of the output type.</param>
    /// <param name="numberOfBytesToRead">Number of bytes to read. If -1, reads from position to end of span.</param>
    /// <returns>Converted value.</returns>
    public static string ToUtf8String(this ReadOnlySpan<byte> span, ref int position, int numberOfBytesToRead = -1)
    {
        if (numberOfBytesToRead == -1)
        {
            numberOfBytesToRead = span.Length - position;
        }
        if (numberOfBytesToRead == 0)
        {
            return string.Empty;
        }

        return ExecuteConversionToType(span, ref position, numberOfBytesToRead, s => Encoding.UTF8.GetString(s));
    }

    /// <summary>
    ///     Converts parts of a ReadOnlySpan&lt;byte&gt; into a UTF-8 String.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span.</param>
    /// <param name="numberOfBytesToRead">Number of bytes to read. If -1, reads from position to end of span.</param>
    /// <returns>Converted value.</returns>
    public static string ToUtf8String(this ReadOnlySpan<byte> span, int position = 0, int numberOfBytesToRead = -1)
    {
        return ToUtf8String(span, ref position, numberOfBytesToRead);
    }

    /// <summary>
    ///     Converts parts of a ReadOnlySpan&lt;byte&gt; into a UTF-8 String.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span. By reference; is auto-incremented by the size of the output type.</param>
    /// <param name="numberOfBytesToRead">Number of bytes to read. If -1, reads from position to end of span.</param>
    /// <param name="defaultValue">Default value for that type, can be overridden.</param>
    /// <returns>Converted value.</returns>
    public static string ToUtf8StringOrDefault(this ReadOnlySpan<byte> span, ref int position, int numberOfBytesToRead = -1, string defaultValue = "")
    {
        if (numberOfBytesToRead == -1)
        {
            numberOfBytesToRead = span.Length - position;
        }

        return ExecuteConversionToTypeOrDefault(span, ref position, numberOfBytesToRead, s => Encoding.UTF8.GetString(s), defaultValue);
    }

    /// <summary>
    ///     Converts parts of a ReadOnlySpan&lt;byte&gt; into a UTF-8 String.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span.</param>
    /// <param name="numberOfBytesToRead">Number of bytes to read. If -1, reads from position to end of span.</param>
    /// <param name="defaultValue">Default value for that type, can be overridden.</param>
    /// <returns>Converted value.</returns>
    public static string ToUtf8StringOrDefault(this ReadOnlySpan<byte> span, int position = 0, int numberOfBytesToRead = -1, string defaultValue = "")
    {
        return ToUtf8StringOrDefault(span, ref position, numberOfBytesToRead, defaultValue);
    }

    /// <summary>
    ///     Converts parts of a ReadOnlySpan&lt;byte&gt; into an ASCII String.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span. By reference; is auto-incremented by the size of the output type.</param>
    /// <param name="numberOfBytesToRead">Number of bytes to read. If -1, reads from position to end of span.</param>
    /// <returns>Converted value.</returns>
    public static string ToAsciiString(this ReadOnlySpan<byte> span, ref int position, int numberOfBytesToRead = -1)
    {
        if (numberOfBytesToRead == -1)
        {
            numberOfBytesToRead = span.Length - position;
        }
        if (numberOfBytesToRead == 0)
        {
            return string.Empty;
        }

        return ExecuteConversionToType(span, ref position, numberOfBytesToRead, s => Encoding.ASCII.GetString(s));
    }

    /// <summary>
    ///     Converts parts of a ReadOnlySpan&lt;byte&gt; into an ASCII String.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span.</param>
    /// <param name="numberOfBytesToRead">Number of bytes to read. If -1, reads from position to end of span.</param>
    /// <returns>Converted value.</returns>
    public static string ToAsciiString(this ReadOnlySpan<byte> span, int position = 0, int numberOfBytesToRead = -1)
    {
        return ToAsciiString(span, ref position, numberOfBytesToRead);
    }

    /// <summary>
    ///     Converts parts of a ReadOnlySpan&lt;byte&gt; into an ASCII String.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span. By reference; is auto-incremented by the size of the output type.</param>
    /// <param name="numberOfBytesToRead">Number of bytes to read. If -1, reads from position to end of span.</param>
    /// <param name="defaultValue">Default value for that type, can be overridden.</param>
    /// <returns>Converted value.</returns>
    public static string ToAsciiStringOrDefault(this ReadOnlySpan<byte> span, ref int position, int numberOfBytesToRead = -1, string defaultValue = "")
    {
        if (numberOfBytesToRead == -1)
        {
            numberOfBytesToRead = span.Length - position;
        }

        return ExecuteConversionToTypeOrDefault(span, ref position, numberOfBytesToRead, s => Encoding.ASCII.GetString(s), defaultValue);
    }

    /// <summary>
    ///     Converts parts of a ReadOnlySpan&lt;byte&gt; into an ASCII String.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span.</param>
    /// <param name="numberOfBytesToRead">Number of bytes to read. If -1, reads from position to end of span.</param>
    /// <param name="defaultValue">Default value for that type, can be overridden.</param>
    /// <returns>Converted value.</returns>
    public static string ToAsciiStringOrDefault(this ReadOnlySpan<byte> span, int position = 0, int numberOfBytesToRead = -1, string defaultValue = "")
    {
        return ToAsciiStringOrDefault(span, ref position, numberOfBytesToRead, defaultValue);
    }

    /// <summary>
    ///     Converts parts of a ReadOnlySpan&lt;byte&gt; into a UTF-16 String.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span. By reference; is auto-incremented by the size of the output type.</param>
    /// <param name="numberOfBytesToRead">Number of bytes to read. If -1, reads from position to end of span.</param>
    /// <returns>Converted value.</returns>
    public static string ToUtf16String(this ReadOnlySpan<byte> span, ref int position, int numberOfBytesToRead = -1)
    {
        if (numberOfBytesToRead == -1)
        {
            numberOfBytesToRead = span.Length - position;
        }
        if (numberOfBytesToRead == 0)
        {
            return string.Empty;
        }

        return ExecuteConversionToType(span, ref position, numberOfBytesToRead, s => Encoding.Unicode.GetString(s));
    }

    /// <summary>
    ///     Converts parts of a ReadOnlySpan&lt;byte&gt; into a UTF-16 String.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span.</param>
    /// <param name="numberOfBytesToRead">Number of bytes to read. If -1, reads from position to end of span.</param>
    /// <returns>Converted value.</returns>
    public static string ToUtf16String(this ReadOnlySpan<byte> span, int position = 0, int numberOfBytesToRead = -1)
    {
        return ToUtf16String(span, ref position, numberOfBytesToRead);
    }

    /// <summary>
    ///     Converts parts of a ReadOnlySpan&lt;byte&gt; into a UTF-16 String.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span. By reference; is auto-incremented by the size of the output type.</param>
    /// <param name="numberOfBytesToRead">Number of bytes to read. If -1, reads from position to end of span.</param>
    /// <param name="defaultValue">Default value for that type, can be overridden.</param>
    /// <returns>Converted value.</returns>
    public static string ToUtf16StringOrDefault(this ReadOnlySpan<byte> span, ref int position, int numberOfBytesToRead = -1, string defaultValue = "")
    {
        if (numberOfBytesToRead == -1)
        {
            numberOfBytesToRead = span.Length - position;
        }

        return ExecuteConversionToTypeOrDefault(span, ref position, numberOfBytesToRead, s => Encoding.Unicode.GetString(s), defaultValue);
    }

    /// <summary>
    ///     Converts parts of a ReadOnlySpan&lt;byte&gt; into a UTF-16 String.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span.</param>
    /// <param name="numberOfBytesToRead">Number of bytes to read. If -1, reads from position to end of span.</param>
    /// <param name="defaultValue">Default value for that type, can be overridden.</param>
    /// <returns>Converted value.</returns>
    public static string ToUtf16StringOrDefault(this ReadOnlySpan<byte> span, int position = 0, int numberOfBytesToRead = -1, string defaultValue = "")
    {
        return ToUtf16StringOrDefault(span, ref position, numberOfBytesToRead, defaultValue);
    }

    /// <summary>
    ///     Converts the entire ReadOnlySpan&lt;byte&gt; into a hexadecimal string representation.
    ///     Each byte is represented as a two-character hexadecimal value.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt; to convert.</param>
    /// <returns>A hexadecimal string representation.</returns>
    public static string ToHexString(this ReadOnlySpan<byte> span)
    {
        return Convert.ToHexString(span);
    }

    /// <summary>
    ///     Converts the entire ReadOnlySpan&lt;byte&gt; into a Base64 string representation.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt; to convert.</param>
    /// <returns>A Base64 string representation.</returns>
    public static string ToBase64String(this ReadOnlySpan<byte> span)
    {
        return Convert.ToBase64String(span);
    }

    #endregion
}
