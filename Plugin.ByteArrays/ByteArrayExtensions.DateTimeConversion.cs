namespace Plugin.ByteArrays;

/// <summary>
///     Functions converting DateTime, TimeSpan, and DateTimeOffset types from byte arrays.
/// </summary>
public static partial class ByteArrayExtensions
{
    #region DateTime Conversions

    /// <summary>
    ///     Converts the byte array into a DateTime using binary representation.
    ///     Size = 8 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array. Byref : is auto-incremented by the size of the output type.</param>
    /// <returns>Converted DateTime value.</returns>
    public static DateTime ToDateTime(this byte[] array, ref int position)
    {
        var binaryValue = ExecuteConversionToType(array, ref position, sizeof(long), BitConverter.ToInt64);
        return DateTime.FromBinary(binaryValue);
    }

    /// <summary>
    ///     Converts the byte array into a DateTime using binary representation.
    ///     Size = 8 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array.</param>
    /// <returns>Converted DateTime value.</returns>
    public static DateTime ToDateTime(this byte[] array, int position = 0)
    {
        return ToDateTime(array, ref position);
    }

    /// <summary>
    ///     Converts the byte array into a DateTime using binary representation.
    ///     Returns the default value if conversion fails.
    ///     Size = 8 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array. Byref : is auto-incremented by the size of the output type.</param>
    /// <param name="defaultValue">Default value for that type, can be overridden.</param>
    /// <returns>Converted DateTime value.</returns>
    public static DateTime ToDateTimeOrDefault(this byte[] array, ref int position, DateTime defaultValue = default)
    {
        return ExecuteConversionToTypeOrDefault(array, ref position, sizeof(long),
            (bytes, offset) => DateTime.FromBinary(BitConverter.ToInt64(bytes, offset)), defaultValue);
    }

    /// <summary>
    ///     Converts the byte array into a DateTime using binary representation.
    ///     Returns the default value if conversion fails.
    ///     Size = 8 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array.</param>
    /// <param name="defaultValue">Default value for that type, can be overridden.</param>
    /// <returns>Converted DateTime value.</returns>
    public static DateTime ToDateTimeOrDefault(this byte[] array, int position = 0, DateTime defaultValue = default)
    {
        return ToDateTimeOrDefault(array, ref position, defaultValue);
    }

    #endregion

    #region Unix Timestamp Conversions

    /// <summary>
    ///     Converts the byte array into a DateTime from Unix timestamp (seconds since epoch).
    ///     Size = 4 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array. Byref : is auto-incremented by the size of the output type.</param>
    /// <returns>Converted DateTime value.</returns>
    public static DateTime ToDateTimeFromUnixTimestamp(this byte[] array, ref int position)
    {
        var timestamp = ExecuteConversionToType(array, ref position, sizeof(int), BitConverter.ToInt32);
        return DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime;
    }

    /// <summary>
    ///     Converts the byte array into a DateTime from Unix timestamp (seconds since epoch).
    ///     Size = 4 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array.</param>
    /// <returns>Converted DateTime value.</returns>
    public static DateTime ToDateTimeFromUnixTimestamp(this byte[] array, int position = 0)
    {
        return ToDateTimeFromUnixTimestamp(array, ref position);
    }

    /// <summary>
    ///     Converts the byte array into a DateTime from Unix timestamp (seconds since epoch).
    ///     Returns the default value if conversion fails.
    ///     Size = 4 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array. Byref : is auto-incremented by the size of the output type.</param>
    /// <param name="defaultValue">Default value for that type, can be overridden.</param>
    /// <returns>Converted DateTime value.</returns>
    public static DateTime ToDateTimeFromUnixTimestampOrDefault(this byte[] array, ref int position, DateTime defaultValue = default)
    {
        return ExecuteConversionToTypeOrDefault(array, ref position, sizeof(int),
            (bytes, offset) => DateTimeOffset.FromUnixTimeSeconds(BitConverter.ToInt32(bytes, offset)).DateTime, defaultValue);
    }

    /// <summary>
    ///     Converts the byte array into a DateTime from Unix timestamp (seconds since epoch).
    ///     Returns the default value if conversion fails.
    ///     Size = 4 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array.</param>
    /// <param name="defaultValue">Default value for that type, can be overridden.</param>
    /// <returns>Converted DateTime value.</returns>
    public static DateTime ToDateTimeFromUnixTimestampOrDefault(this byte[] array, int position = 0, DateTime defaultValue = default)
    {
        return ToDateTimeFromUnixTimestampOrDefault(array, ref position, defaultValue);
    }

    #endregion

    #region TimeSpan Conversions

    /// <summary>
    ///     Converts the byte array into a TimeSpan using ticks representation.
    ///     Size = 8 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array. Byref : is auto-incremented by the size of the output type.</param>
    /// <returns>Converted TimeSpan value.</returns>
    public static TimeSpan ToTimeSpan(this byte[] array, ref int position)
    {
        var ticks = ExecuteConversionToType(array, ref position, sizeof(long), BitConverter.ToInt64);
        return new TimeSpan(ticks);
    }

    /// <summary>
    ///     Converts the byte array into a TimeSpan using ticks representation.
    ///     Size = 8 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array.</param>
    /// <returns>Converted TimeSpan value.</returns>
    public static TimeSpan ToTimeSpan(this byte[] array, int position = 0)
    {
        return ToTimeSpan(array, ref position);
    }

    /// <summary>
    ///     Converts the byte array into a TimeSpan using ticks representation.
    ///     Returns the default value if conversion fails.
    ///     Size = 8 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array. Byref : is auto-incremented by the size of the output type.</param>
    /// <param name="defaultValue">Default value for that type, can be overridden.</param>
    /// <returns>Converted TimeSpan value.</returns>
    public static TimeSpan ToTimeSpanOrDefault(this byte[] array, ref int position, TimeSpan defaultValue = default)
    {
        return ExecuteConversionToTypeOrDefault(array, ref position, sizeof(long),
            (bytes, offset) => new TimeSpan(BitConverter.ToInt64(bytes, offset)), defaultValue);
    }

    /// <summary>
    ///     Converts the byte array into a TimeSpan using ticks representation.
    ///     Returns the default value if conversion fails.
    ///     Size = 8 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array.</param>
    /// <param name="defaultValue">Default value for that type, can be overridden.</param>
    /// <returns>Converted TimeSpan value.</returns>
    public static TimeSpan ToTimeSpanOrDefault(this byte[] array, int position = 0, TimeSpan defaultValue = default)
    {
        return ToTimeSpanOrDefault(array, ref position, defaultValue);
    }

    #endregion

    #region DateTimeOffset Conversions

    /// <summary>
    ///     Converts the byte array into a DateTimeOffset.
    ///     Size = 16 bytes (8 for DateTime, 8 for TimeSpan offset).
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array. Byref : is auto-incremented by the size of the output type.</param>
    /// <returns>Converted DateTimeOffset value.</returns>
    public static DateTimeOffset ToDateTimeOffset(this byte[] array, ref int position)
    {
        var dateTime = ToDateTime(array, ref position);
        var offsetTicks = ExecuteConversionToType(array, ref position, sizeof(long), BitConverter.ToInt64);
        var offset = new TimeSpan(offsetTicks);
        return new DateTimeOffset(dateTime, offset);
    }

    /// <summary>
    ///     Converts the byte array into a DateTimeOffset.
    ///     Size = 16 bytes (8 for DateTime, 8 for TimeSpan offset).
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array.</param>
    /// <returns>Converted DateTimeOffset value.</returns>
    public static DateTimeOffset ToDateTimeOffset(this byte[] array, int position = 0)
    {
        return ToDateTimeOffset(array, ref position);
    }

    /// <summary>
    ///     Converts the byte array into a DateTimeOffset.
    ///     Returns the default value if conversion fails.
    ///     Size = 16 bytes (8 for DateTime, 8 for TimeSpan offset).
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array. Byref : is auto-incremented by the size of the output type.</param>
    /// <param name="defaultValue">Default value for that type, can be overridden.</param>
    /// <returns>Converted DateTimeOffset value.</returns>
    public static DateTimeOffset ToDateTimeOffsetOrDefault(this byte[] array, ref int position, DateTimeOffset defaultValue = default)
    {
        try
        {
            return ToDateTimeOffset(array, ref position);
        }
        catch (ArgumentOutOfRangeException)
        {
            return defaultValue;
        }
    }

    /// <summary>
    ///     Converts the byte array into a DateTimeOffset.
    ///     Returns the default value if conversion fails.
    ///     Size = 16 bytes (8 for DateTime, 8 for TimeSpan offset).
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array.</param>
    /// <param name="defaultValue">Default value for that type, can be overridden.</param>
    /// <returns>Converted DateTimeOffset value.</returns>
    public static DateTimeOffset ToDateTimeOffsetOrDefault(this byte[] array, int position = 0, DateTimeOffset defaultValue = default)
    {
        return ToDateTimeOffsetOrDefault(array, ref position, defaultValue);
    }

    #endregion
}
