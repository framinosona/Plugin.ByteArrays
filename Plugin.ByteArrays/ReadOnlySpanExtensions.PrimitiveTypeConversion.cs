namespace Plugin.ByteArrays;

/// <summary>
///     Functions for working with ReadOnlySpan&lt;byte&gt; providing zero-allocation operations for reading and manipulating byte data.
/// </summary>
public static partial class ReadOnlySpanExtensions
{
    #region Primitive Type Conversions

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into a Boolean value.
    ///     Size = 1 byte.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span. By reference; is auto-incremented by the size of the output type.</param>
    /// <returns>Converted value.</returns>
    public static bool ToBoolean(this ReadOnlySpan<byte> span, ref int position)
    {
        return ExecuteConversionToType(span, ref position, sizeof(bool), s => BitConverter.ToBoolean(s));
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into a Boolean value.
    ///     Size = 1 byte.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span.</param>
    /// <returns>Converted value.</returns>
    public static bool ToBoolean(this ReadOnlySpan<byte> span, int position = 0)
    {
        return ToBoolean(span, ref position);
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into a Boolean value.
    ///     Size = 1 byte.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span. By reference; is auto-incremented by the size of the output type.</param>
    /// <param name="defaultValue">Value returned if out of bounds.</param>
    /// <returns>Converted value.</returns>
    public static bool ToBooleanOrDefault(this ReadOnlySpan<byte> span, ref int position, bool defaultValue = false)
    {
        return ExecuteConversionToTypeOrDefault(span, ref position, sizeof(bool), s => BitConverter.ToBoolean(s), defaultValue);
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into a Boolean value.
    ///     Size = 1 byte.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span.</param>
    /// <param name="defaultValue">Default value for that type, can be overridden.</param>
    /// <returns>Converted value.</returns>
    public static bool ToBooleanOrDefault(this ReadOnlySpan<byte> span, int position = 0, bool defaultValue = false)
    {
        return ToBooleanOrDefault(span, ref position, defaultValue);
    }

    /// <summary>
    ///     Extracts a single byte from the ReadOnlySpan&lt;byte&gt;.
    ///     Size = 1 byte.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span. By reference; is auto-incremented by the size of the output type.</param>
    /// <returns>The byte value.</returns>
    public static byte ToByte(this ReadOnlySpan<byte> span, ref int position)
    {
        return ExecuteConversionToType(span, ref position, sizeof(byte), s => s[0]);
    }

    /// <summary>
    ///     Extracts a single byte from the ReadOnlySpan&lt;byte&gt;.
    ///     Size = 1 byte.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span.</param>
    /// <returns>The byte value.</returns>
    public static byte ToByte(this ReadOnlySpan<byte> span, int position = 0)
    {
        return ToByte(span, ref position);
    }

    /// <summary>
    ///     Extracts a single byte from the ReadOnlySpan&lt;byte&gt;.
    ///     Size = 1 byte.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span. By reference; is auto-incremented by the size of the output type.</param>
    /// <param name="defaultValue">Value returned if out of bounds.</param>
    /// <returns>The byte value.</returns>
    public static byte ToByteOrDefault(this ReadOnlySpan<byte> span, ref int position, byte defaultValue = 0)
    {
        return ExecuteConversionToTypeOrDefault(span, ref position, sizeof(byte), s => s[0], defaultValue);
    }

    /// <summary>
    ///     Extracts a single byte from the ReadOnlySpan&lt;byte&gt;.
    ///     Size = 1 byte.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span.</param>
    /// <param name="defaultValue">Default value for that type, can be overridden.</param>
    /// <returns>The byte value.</returns>
    public static byte ToByteOrDefault(this ReadOnlySpan<byte> span, int position = 0, byte defaultValue = 0)
    {
        return ToByteOrDefault(span, ref position, defaultValue);
    }

    /// <summary>
    ///     Extracts a signed byte from the ReadOnlySpan&lt;byte&gt;.
    ///     Range: -128 to 127.
    ///     Size = 1 byte.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span. By reference; is auto-incremented by the size of the output type.</param>
    /// <returns>The sbyte value.</returns>
    public static sbyte ToSByte(this ReadOnlySpan<byte> span, ref int position)
    {
        return ExecuteConversionToType(span, ref position, sizeof(sbyte), s => (sbyte)s[0]);
    }

    /// <summary>
    ///     Extracts a signed byte from the ReadOnlySpan&lt;byte&gt;.
    ///     Range: -128 to 127.
    ///     Size = 1 byte.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span.</param>
    /// <returns>The sbyte value.</returns>
    public static sbyte ToSByte(this ReadOnlySpan<byte> span, int position = 0)
    {
        return ToSByte(span, ref position);
    }

    /// <summary>
    ///     Extracts a signed byte from the ReadOnlySpan&lt;byte&gt;.
    ///     Range: -128 to 127.
    ///     Size = 1 byte.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span. By reference; is auto-incremented by the size of the output type.</param>
    /// <param name="defaultValue">Value returned if out of bounds.</param>
    /// <returns>The sbyte value.</returns>
    public static sbyte ToSByteOrDefault(this ReadOnlySpan<byte> span, ref int position, sbyte defaultValue = 0)
    {
        return ExecuteConversionToTypeOrDefault(span, ref position, sizeof(sbyte), s => (sbyte)s[0], defaultValue);
    }

    /// <summary>
    ///     Extracts a signed byte from the ReadOnlySpan&lt;byte&gt;.
    ///     Range: -128 to 127.
    ///     Size = 1 byte.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span.</param>
    /// <param name="defaultValue">Default value for that type, can be overridden.</param>
    /// <returns>The sbyte value.</returns>
    public static sbyte ToSByteOrDefault(this ReadOnlySpan<byte> span, int position = 0, sbyte defaultValue = 0)
    {
        return ToSByteOrDefault(span, ref position, defaultValue);
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into a Unicode character.
    ///     Size = 2 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span. By reference; is auto-incremented by the size of the output type.</param>
    /// <returns>Converted value.</returns>
    public static char ToChar(this ReadOnlySpan<byte> span, ref int position)
    {
        return ExecuteConversionToType(span, ref position, sizeof(char), s => BitConverter.ToChar(s));
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into a Unicode character.
    ///     Size = 2 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span.</param>
    /// <returns>Converted value.</returns>
    public static char ToChar(this ReadOnlySpan<byte> span, int position = 0)
    {
        return ToChar(span, ref position);
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into a Unicode character.
    ///     Size = 2 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span. By reference; is auto-incremented by the size of the output type.</param>
    /// <param name="defaultValue">Value returned if out of bounds.</param>
    /// <returns>Converted value.</returns>
    public static char ToCharOrDefault(this ReadOnlySpan<byte> span, ref int position, char defaultValue = '\0')
    {
        return ExecuteConversionToTypeOrDefault(span, ref position, sizeof(char), s => BitConverter.ToChar(s), defaultValue);
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into a Unicode character.
    ///     Size = 2 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span.</param>
    /// <param name="defaultValue">Default value for that type, can be overridden.</param>
    /// <returns>Converted value.</returns>
    public static char ToCharOrDefault(this ReadOnlySpan<byte> span, int position = 0, char defaultValue = '\0')
    {
        return ToCharOrDefault(span, ref position, defaultValue);
    }

    #endregion
}
