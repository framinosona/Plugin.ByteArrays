namespace Plugin.ByteArrays;

/// <summary>
///     Functions for working with ReadOnlySpan&lt;byte&gt; providing zero-allocation operations for reading and manipulating byte data.
/// </summary>
public static partial class ReadOnlySpanExtensions
{
    #region Integer Conversions

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into a Signed 16-bit integer
    ///     From -32,768 to 32,767.
    ///     Size = 2 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span. By reference; is auto-incremented by the size of the output type.</param>
    /// <returns>Converted value.</returns>
    public static short ToInt16(this ReadOnlySpan<byte> span, ref int position)
    {
        return ExecuteConversionToType(span, ref position, sizeof(short), s => BitConverter.ToInt16(s));
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into a Signed 16-bit integer
    ///     From -32,768 to 32,767.
    ///     Size = 2 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span.</param>
    /// <returns>Converted value.</returns>
    public static short ToInt16(this ReadOnlySpan<byte> span, int position = 0)
    {
        return ToInt16(span, ref position);
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into a Signed 16-bit integer
    ///     From -32,768 to 32,767.
    ///     Size = 2 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span. By reference; is auto-incremented by the size of the output type.</param>
    /// <param name="defaultValue">Value returned if out of bounds.</param>
    /// <returns>Converted value.</returns>
    public static short ToInt16OrDefault(this ReadOnlySpan<byte> span, ref int position, short defaultValue = 0)
    {
        return ExecuteConversionToTypeOrDefault(span, ref position, sizeof(short), s => BitConverter.ToInt16(s), defaultValue);
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into a Signed 16-bit integer
    ///     From -32,768 to 32,767.
    ///     Size = 2 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span.</param>
    /// <param name="defaultValue">Default value for that type, can be overridden.</param>
    /// <returns>Converted value.</returns>
    public static short ToInt16OrDefault(this ReadOnlySpan<byte> span, int position = 0, short defaultValue = 0)
    {
        return ToInt16OrDefault(span, ref position, defaultValue);
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into an Unsigned 16-bit integer
    ///     From 0 to 65,535.
    ///     Size = 2 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span. By reference; is auto-incremented by the size of the output type.</param>
    /// <returns>Converted value.</returns>
    public static ushort ToUInt16(this ReadOnlySpan<byte> span, ref int position)
    {
        return ExecuteConversionToType(span, ref position, sizeof(ushort), s => BitConverter.ToUInt16(s));
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into an Unsigned 16-bit integer
    ///     From 0 to 65,535.
    ///     Size = 2 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span.</param>
    /// <returns>Converted value.</returns>
    public static ushort ToUInt16(this ReadOnlySpan<byte> span, int position = 0)
    {
        return ToUInt16(span, ref position);
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into an Unsigned 16-bit integer
    ///     From 0 to 65,535.
    ///     Size = 2 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span. By reference; is auto-incremented by the size of the output type.</param>
    /// <param name="defaultValue">Default value for that type, can be overridden.</param>
    /// <returns>Converted value.</returns>
    public static ushort ToUInt16OrDefault(this ReadOnlySpan<byte> span, ref int position, ushort defaultValue = 0)
    {
        return ExecuteConversionToTypeOrDefault(span, ref position, sizeof(ushort), s => BitConverter.ToUInt16(s), defaultValue);
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into an Unsigned 16-bit integer
    ///     From 0 to 65,535.
    ///     Size = 2 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span.</param>
    /// <param name="defaultValue">Default value for that type, can be overridden.</param>
    /// <returns>Converted value.</returns>
    public static ushort ToUInt16OrDefault(this ReadOnlySpan<byte> span, int position = 0, ushort defaultValue = 0)
    {
        return ToUInt16OrDefault(span, ref position, defaultValue);
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into a Signed 32-bit integer
    ///     From -2,147,483,648 to 2,147,483,647.
    ///     Size = 4 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span. By reference; is auto-incremented by the size of the output type.</param>
    /// <returns>Converted value.</returns>
    public static int ToInt32(this ReadOnlySpan<byte> span, ref int position)
    {
        return ExecuteConversionToType(span, ref position, sizeof(int), s => BitConverter.ToInt32(s));
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into a Signed 32-bit integer
    ///     From -2,147,483,648 to 2,147,483,647.
    ///     Size = 4 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span.</param>
    /// <returns>Converted value.</returns>
    public static int ToInt32(this ReadOnlySpan<byte> span, int position = 0)
    {
        return ToInt32(span, ref position);
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into a Signed 32-bit integer
    ///     From -2,147,483,648 to 2,147,483,647.
    ///     Size = 4 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span. By reference; is auto-incremented by the size of the output type.</param>
    /// <param name="defaultValue">Value returned if out of bounds.</param>
    /// <returns>Converted value.</returns>
    public static int ToInt32OrDefault(this ReadOnlySpan<byte> span, ref int position, int defaultValue = 0)
    {
        return ExecuteConversionToTypeOrDefault(span, ref position, sizeof(int), s => BitConverter.ToInt32(s), defaultValue);
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into a Signed 32-bit integer
    ///     From -2,147,483,648 to 2,147,483,647.
    ///     Size = 4 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span.</param>
    /// <param name="defaultValue">Default value for that type, can be overridden.</param>
    /// <returns>Converted value.</returns>
    public static int ToInt32OrDefault(this ReadOnlySpan<byte> span, int position = 0, int defaultValue = 0)
    {
        return ToInt32OrDefault(span, ref position, defaultValue);
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into an Unsigned 32-bit integer
    ///     From 0 to 4,294,967,295.
    ///     Size = 4 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span. By reference; is auto-incremented by the size of the output type.</param>
    /// <returns>Converted value.</returns>
    public static uint ToUInt32(this ReadOnlySpan<byte> span, ref int position)
    {
        return ExecuteConversionToType(span, ref position, sizeof(uint), s => BitConverter.ToUInt32(s));
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into an Unsigned 32-bit integer
    ///     From 0 to 4,294,967,295.
    ///     Size = 4 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span.</param>
    /// <returns>Converted value.</returns>
    public static uint ToUInt32(this ReadOnlySpan<byte> span, int position = 0)
    {
        return ToUInt32(span, ref position);
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into an Unsigned 32-bit integer
    ///     From 0 to 4,294,967,295.
    ///     Size = 4 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span. By reference; is auto-incremented by the size of the output type.</param>
    /// <param name="defaultValue">Default value for that type, can be overridden.</param>
    /// <returns>Converted value.</returns>
    public static uint ToUInt32OrDefault(this ReadOnlySpan<byte> span, ref int position, uint defaultValue = 0)
    {
        return ExecuteConversionToTypeOrDefault(span, ref position, sizeof(uint), s => BitConverter.ToUInt32(s), defaultValue);
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into an Unsigned 32-bit integer
    ///     From 0 to 4,294,967,295.
    ///     Size = 4 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span.</param>
    /// <param name="defaultValue">Default value for that type, can be overridden.</param>
    /// <returns>Converted value.</returns>
    public static uint ToUInt32OrDefault(this ReadOnlySpan<byte> span, int position = 0, uint defaultValue = 0)
    {
        return ToUInt32OrDefault(span, ref position, defaultValue);
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into a Signed 64-bit integer
    ///     From -9,223,372,036,854,775,808 to 9,223,372,036,854,775,807.
    ///     Size = 8 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span. By reference; is auto-incremented by the size of the output type.</param>
    /// <returns>Converted value.</returns>
    public static long ToInt64(this ReadOnlySpan<byte> span, ref int position)
    {
        return ExecuteConversionToType(span, ref position, sizeof(long), s => BitConverter.ToInt64(s));
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into a Signed 64-bit integer
    ///     From -9,223,372,036,854,775,808 to 9,223,372,036,854,775,807.
    ///     Size = 8 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span.</param>
    /// <returns>Converted value.</returns>
    public static long ToInt64(this ReadOnlySpan<byte> span, int position = 0)
    {
        return ToInt64(span, ref position);
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into a Signed 64-bit integer
    ///     From -9,223,372,036,854,775,808 to 9,223,372,036,854,775,807.
    ///     Size = 8 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span. By reference; is auto-incremented by the size of the output type.</param>
    /// <param name="defaultValue">Value returned if out of bounds.</param>
    /// <returns>Converted value.</returns>
    public static long ToInt64OrDefault(this ReadOnlySpan<byte> span, ref int position, long defaultValue = 0)
    {
        return ExecuteConversionToTypeOrDefault(span, ref position, sizeof(long), s => BitConverter.ToInt64(s), defaultValue);
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into a Signed 64-bit integer
    ///     From -9,223,372,036,854,775,808 to 9,223,372,036,854,775,807.
    ///     Size = 8 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span.</param>
    /// <param name="defaultValue">Default value for that type, can be overridden.</param>
    /// <returns>Converted value.</returns>
    public static long ToInt64OrDefault(this ReadOnlySpan<byte> span, int position = 0, long defaultValue = 0)
    {
        return ToInt64OrDefault(span, ref position, defaultValue);
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into an Unsigned 64-bit integer
    ///     From 0 to 18,446,744,073,709,551,615.
    ///     Size = 8 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span. By reference; is auto-incremented by the size of the output type.</param>
    /// <returns>Converted value.</returns>
    public static ulong ToUInt64(this ReadOnlySpan<byte> span, ref int position)
    {
        return ExecuteConversionToType(span, ref position, sizeof(ulong), s => BitConverter.ToUInt64(s));
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into an Unsigned 64-bit integer
    ///     From 0 to 18,446,744,073,709,551,615.
    ///     Size = 8 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span.</param>
    /// <returns>Converted value.</returns>
    public static ulong ToUInt64(this ReadOnlySpan<byte> span, int position = 0)
    {
        return ToUInt64(span, ref position);
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into an Unsigned 64-bit integer
    ///     From 0 to 18,446,744,073,709,551,615.
    ///     Size = 8 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span. By reference; is auto-incremented by the size of the output type.</param>
    /// <param name="defaultValue">Default value for that type, can be overridden.</param>
    /// <returns>Converted value.</returns>
    public static ulong ToUInt64OrDefault(this ReadOnlySpan<byte> span, ref int position, ulong defaultValue = 0)
    {
        return ExecuteConversionToTypeOrDefault(span, ref position, sizeof(ulong), s => BitConverter.ToUInt64(s), defaultValue);
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into an Unsigned 64-bit integer
    ///     From 0 to 18,446,744,073,709,551,615.
    ///     Size = 8 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span.</param>
    /// <param name="defaultValue">Default value for that type, can be overridden.</param>
    /// <returns>Converted value.</returns>
    public static ulong ToUInt64OrDefault(this ReadOnlySpan<byte> span, int position = 0, ulong defaultValue = 0)
    {
        return ToUInt64OrDefault(span, ref position, defaultValue);
    }

    #endregion
}
