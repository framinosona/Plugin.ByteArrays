namespace Plugin.ByteArrays;

/// <summary>
///     Functions for working with ReadOnlySpan&lt;byte&gt; providing zero-allocation operations for reading and manipulating byte data.
/// </summary>
public static partial class ReadOnlySpanExtensions
{
    #region Floating Point Conversions

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into a Single-precision floating point number (float).
    ///     Size = 4 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span. By reference; is auto-incremented by the size of the output type.</param>
    /// <returns>Converted value.</returns>
    public static float ToSingle(this ReadOnlySpan<byte> span, ref int position)
    {
        return ExecuteConversionToType(span, ref position, sizeof(float), s => BitConverter.ToSingle(s));
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into a Single-precision floating point number (float).
    ///     Size = 4 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span.</param>
    /// <returns>Converted value.</returns>
    public static float ToSingle(this ReadOnlySpan<byte> span, int position = 0)
    {
        return ToSingle(span, ref position);
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into a Single-precision floating point number (float).
    ///     Size = 4 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span. By reference; is auto-incremented by the size of the output type.</param>
    /// <param name="defaultValue">Value returned if out of bounds.</param>
    /// <returns>Converted value.</returns>
    public static float ToSingleOrDefault(this ReadOnlySpan<byte> span, ref int position, float defaultValue = 0.0f)
    {
        return ExecuteConversionToTypeOrDefault(span, ref position, sizeof(float), s => BitConverter.ToSingle(s), defaultValue);
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into a Single-precision floating point number (float).
    ///     Size = 4 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span.</param>
    /// <param name="defaultValue">Default value for that type, can be overridden.</param>
    /// <returns>Converted value.</returns>
    public static float ToSingleOrDefault(this ReadOnlySpan<byte> span, int position = 0, float defaultValue = 0.0f)
    {
        return ToSingleOrDefault(span, ref position, defaultValue);
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into a Double-precision floating point number (double).
    ///     Size = 8 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span. By reference; is auto-incremented by the size of the output type.</param>
    /// <returns>Converted value.</returns>
    public static double ToDouble(this ReadOnlySpan<byte> span, ref int position)
    {
        return ExecuteConversionToType(span, ref position, sizeof(double), s => BitConverter.ToDouble(s));
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into a Double-precision floating point number (double).
    ///     Size = 8 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span.</param>
    /// <returns>Converted value.</returns>
    public static double ToDouble(this ReadOnlySpan<byte> span, int position = 0)
    {
        return ToDouble(span, ref position);
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into a Double-precision floating point number (double).
    ///     Size = 8 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span. By reference; is auto-incremented by the size of the output type.</param>
    /// <param name="defaultValue">Value returned if out of bounds.</param>
    /// <returns>Converted value.</returns>
    public static double ToDoubleOrDefault(this ReadOnlySpan<byte> span, ref int position, double defaultValue = 0.0)
    {
        return ExecuteConversionToTypeOrDefault(span, ref position, sizeof(double), s => BitConverter.ToDouble(s), defaultValue);
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into a Double-precision floating point number (double).
    ///     Size = 8 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span.</param>
    /// <param name="defaultValue">Default value for that type, can be overridden.</param>
    /// <returns>Converted value.</returns>
    public static double ToDoubleOrDefault(this ReadOnlySpan<byte> span, int position = 0, double defaultValue = 0.0)
    {
        return ToDoubleOrDefault(span, ref position, defaultValue);
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into a Half-precision floating point number (Half).
    ///     Size = 2 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span. By reference; is auto-incremented by the size of the output type.</param>
    /// <returns>Converted value.</returns>
    public static Half ToHalf(this ReadOnlySpan<byte> span, ref int position)
    {
        return ExecuteConversionToType(span, ref position, sizeof(ushort), s => BitConverter.ToHalf(s));
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into a Half-precision floating point number (Half).
    ///     Size = 2 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span.</param>
    /// <returns>Converted value.</returns>
    public static Half ToHalf(this ReadOnlySpan<byte> span, int position = 0)
    {
        return ToHalf(span, ref position);
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into a Half-precision floating point number (Half).
    ///     Size = 2 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span. By reference; is auto-incremented by the size of the output type.</param>
    /// <param name="defaultValue">Value returned if out of bounds.</param>
    /// <returns>Converted value.</returns>
    public static Half ToHalfOrDefault(this ReadOnlySpan<byte> span, ref int position, Half defaultValue = default)
    {
        return ExecuteConversionToTypeOrDefault(span, ref position, sizeof(ushort), s => BitConverter.ToHalf(s), defaultValue);
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into a Half-precision floating point number (Half).
    ///     Size = 2 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span.</param>
    /// <param name="defaultValue">Default value for that type, can be overridden.</param>
    /// <returns>Converted value.</returns>
    public static Half ToHalfOrDefault(this ReadOnlySpan<byte> span, int position = 0, Half defaultValue = default)
    {
        return ToHalfOrDefault(span, ref position, defaultValue);
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into a Decimal number.
    ///     Size = 16 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span. By reference; is auto-incremented by the size of the output type.</param>
    /// <returns>Converted value.</returns>
    public static decimal ToDecimal(this ReadOnlySpan<byte> span, ref int position)
    {
        const int decimalSize = 16; // 4 int32s = 16 bytes
        return ExecuteConversionToType(span, ref position, decimalSize, s =>
        {
            var bits = new int[4];
            for (var i = 0; i < 4; i++)
            {
                bits[i] = BitConverter.ToInt32(s.Slice(i * 4, 4));
            }
            return new decimal(bits);
        });
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into a Decimal number.
    ///     Size = 16 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span.</param>
    /// <returns>Converted value.</returns>
    public static decimal ToDecimal(this ReadOnlySpan<byte> span, int position = 0)
    {
        return ToDecimal(span, ref position);
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into a Decimal number.
    ///     Size = 16 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span. By reference; is auto-incremented by the size of the output type.</param>
    /// <param name="defaultValue">Value returned if out of bounds.</param>
    /// <returns>Converted value.</returns>
    public static decimal ToDecimalOrDefault(this ReadOnlySpan<byte> span, ref int position, decimal defaultValue = 0.0m)
    {
        const int decimalSize = 16; // 4 int32s = 16 bytes
        return ExecuteConversionToTypeOrDefault(span, ref position, decimalSize, s =>
        {
            var bits = new int[4];
            for (var i = 0; i < 4; i++)
            {
                bits[i] = BitConverter.ToInt32(s.Slice(i * 4, 4));
            }
            return new decimal(bits);
        }, defaultValue);
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into a Decimal number.
    ///     Size = 16 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span.</param>
    /// <param name="defaultValue">Default value for that type, can be overridden.</param>
    /// <returns>Converted value.</returns>
    public static decimal ToDecimalOrDefault(this ReadOnlySpan<byte> span, int position = 0, decimal defaultValue = 0.0m)
    {
        return ToDecimalOrDefault(span, ref position, defaultValue);
    }

    #endregion
}
