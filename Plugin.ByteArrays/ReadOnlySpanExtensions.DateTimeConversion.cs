namespace Plugin.ByteArrays;

/// <summary>
///     Functions for working with ReadOnlySpan&lt;byte&gt; providing zero-allocation operations for reading and manipulating byte data.
/// </summary>
public static partial class ReadOnlySpanExtensions
{
    #region DateTime Conversions

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into a DateTime value.
    ///     The DateTime is stored as a 64-bit integer representing ticks.
    ///     Size = 8 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span. By reference; is auto-incremented by the size of the output type.</param>
    /// <returns>Converted value.</returns>
    public static DateTime ToDateTime(this ReadOnlySpan<byte> span, ref int position)
    {
        return ExecuteConversionToType(span, ref position, sizeof(long), s => new DateTime(BitConverter.ToInt64(s)));
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into a DateTime value.
    ///     The DateTime is stored as a 64-bit integer representing ticks.
    ///     Size = 8 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span.</param>
    /// <returns>Converted value.</returns>
    public static DateTime ToDateTime(this ReadOnlySpan<byte> span, int position = 0)
    {
        return ToDateTime(span, ref position);
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into a DateTime value.
    ///     The DateTime is stored as a 64-bit integer representing ticks.
    ///     Size = 8 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span. By reference; is auto-incremented by the size of the output type.</param>
    /// <param name="defaultValue">Value returned if out of bounds.</param>
    /// <returns>Converted value.</returns>
    public static DateTime ToDateTimeOrDefault(this ReadOnlySpan<byte> span, ref int position, DateTime defaultValue = default)
    {
        return ExecuteConversionToTypeOrDefault(span, ref position, sizeof(long), s => new DateTime(BitConverter.ToInt64(s)), defaultValue);
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into a DateTime value.
    ///     The DateTime is stored as a 64-bit integer representing ticks.
    ///     Size = 8 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span.</param>
    /// <param name="defaultValue">Default value for that type, can be overridden.</param>
    /// <returns>Converted value.</returns>
    public static DateTime ToDateTimeOrDefault(this ReadOnlySpan<byte> span, int position = 0, DateTime defaultValue = default)
    {
        return ToDateTimeOrDefault(span, ref position, defaultValue);
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into a TimeSpan value.
    ///     The TimeSpan is stored as a 64-bit integer representing ticks.
    ///     Size = 8 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span. By reference; is auto-incremented by the size of the output type.</param>
    /// <returns>Converted value.</returns>
    public static TimeSpan ToTimeSpan(this ReadOnlySpan<byte> span, ref int position)
    {
        return ExecuteConversionToType(span, ref position, sizeof(long), s => new TimeSpan(BitConverter.ToInt64(s)));
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into a TimeSpan value.
    ///     The TimeSpan is stored as a 64-bit integer representing ticks.
    ///     Size = 8 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span.</param>
    /// <returns>Converted value.</returns>
    public static TimeSpan ToTimeSpan(this ReadOnlySpan<byte> span, int position = 0)
    {
        return ToTimeSpan(span, ref position);
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into a TimeSpan value.
    ///     The TimeSpan is stored as a 64-bit integer representing ticks.
    ///     Size = 8 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span. By reference; is auto-incremented by the size of the output type.</param>
    /// <param name="defaultValue">Value returned if out of bounds.</param>
    /// <returns>Converted value.</returns>
    public static TimeSpan ToTimeSpanOrDefault(this ReadOnlySpan<byte> span, ref int position, TimeSpan defaultValue = default)
    {
        return ExecuteConversionToTypeOrDefault(span, ref position, sizeof(long), s => new TimeSpan(BitConverter.ToInt64(s)), defaultValue);
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into a TimeSpan value.
    ///     The TimeSpan is stored as a 64-bit integer representing ticks.
    ///     Size = 8 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span.</param>
    /// <param name="defaultValue">Default value for that type, can be overridden.</param>
    /// <returns>Converted value.</returns>
    public static TimeSpan ToTimeSpanOrDefault(this ReadOnlySpan<byte> span, int position = 0, TimeSpan defaultValue = default)
    {
        return ToTimeSpanOrDefault(span, ref position, defaultValue);
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into a DateTimeOffset value.
    ///     The DateTimeOffset is stored as 10 bytes: 8 bytes for DateTime ticks and 2 bytes for offset minutes.
    ///     Size = 10 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span. By reference; is auto-incremented by the size of the output type.</param>
    /// <returns>Converted value.</returns>
    public static DateTimeOffset ToDateTimeOffset(this ReadOnlySpan<byte> span, ref int position)
    {
        const int size = sizeof(long) + sizeof(short); // DateTime ticks + offset minutes
        return ExecuteConversionToType(span, ref position, size, s =>
        {
            var ticks = BitConverter.ToInt64(s[..8]);
            var offsetMinutes = BitConverter.ToInt16(s[8..10]);
            return new DateTimeOffset(ticks, TimeSpan.FromMinutes(offsetMinutes));
        });
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into a DateTimeOffset value.
    ///     The DateTimeOffset is stored as 10 bytes: 8 bytes for DateTime ticks and 2 bytes for offset minutes.
    ///     Size = 10 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span.</param>
    /// <returns>Converted value.</returns>
    public static DateTimeOffset ToDateTimeOffset(this ReadOnlySpan<byte> span, int position = 0)
    {
        return ToDateTimeOffset(span, ref position);
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into a DateTimeOffset value.
    ///     The DateTimeOffset is stored as 10 bytes: 8 bytes for DateTime ticks and 2 bytes for offset minutes.
    ///     Size = 10 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span. By reference; is auto-incremented by the size of the output type.</param>
    /// <param name="defaultValue">Value returned if out of bounds.</param>
    /// <returns>Converted value.</returns>
    public static DateTimeOffset ToDateTimeOffsetOrDefault(this ReadOnlySpan<byte> span, ref int position, DateTimeOffset defaultValue = default)
    {
        const int size = sizeof(long) + sizeof(short); // DateTime ticks + offset minutes
        return ExecuteConversionToTypeOrDefault(span, ref position, size, s =>
        {
            var ticks = BitConverter.ToInt64(s[..8]);
            var offsetMinutes = BitConverter.ToInt16(s[8..10]);
            return new DateTimeOffset(ticks, TimeSpan.FromMinutes(offsetMinutes));
        }, defaultValue);
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into a DateTimeOffset value.
    ///     The DateTimeOffset is stored as 10 bytes: 8 bytes for DateTime ticks and 2 bytes for offset minutes.
    ///     Size = 10 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span.</param>
    /// <param name="defaultValue">Default value for that type, can be overridden.</param>
    /// <returns>Converted value.</returns>
    public static DateTimeOffset ToDateTimeOffsetOrDefault(this ReadOnlySpan<byte> span, int position = 0, DateTimeOffset defaultValue = default)
    {
        return ToDateTimeOffsetOrDefault(span, ref position, defaultValue);
    }

    #endregion
}
