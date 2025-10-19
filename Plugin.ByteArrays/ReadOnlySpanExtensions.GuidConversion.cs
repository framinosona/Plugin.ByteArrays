namespace Plugin.ByteArrays;

/// <summary>
///     Functions for working with ReadOnlySpan&lt;byte&gt; providing zero-allocation operations for reading and manipulating byte data.
/// </summary>
public static partial class ReadOnlySpanExtensions
{
    #region GUID Conversions

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into a Guid value.
    ///     Size = 16 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span. By reference; is auto-incremented by the size of the output type.</param>
    /// <returns>Converted value.</returns>
    public static Guid ToGuid(this ReadOnlySpan<byte> span, ref int position)
    {
        const int guidSize = 16;
        return ExecuteConversionToType(span, ref position, guidSize, s => new Guid(s));
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into a Guid value.
    ///     Size = 16 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span.</param>
    /// <returns>Converted value.</returns>
    public static Guid ToGuid(this ReadOnlySpan<byte> span, int position = 0)
    {
        return ToGuid(span, ref position);
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into a Guid value.
    ///     Size = 16 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span. By reference; is auto-incremented by the size of the output type.</param>
    /// <param name="defaultValue">Value returned if out of bounds.</param>
    /// <returns>Converted value.</returns>
    public static Guid ToGuidOrDefault(this ReadOnlySpan<byte> span, ref int position, Guid defaultValue = default)
    {
        const int guidSize = 16;
        return ExecuteConversionToTypeOrDefault(span, ref position, guidSize, s => new Guid(s), defaultValue);
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into a Guid value.
    ///     Size = 16 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span.</param>
    /// <param name="defaultValue">Default value for that type, can be overridden.</param>
    /// <returns>Converted value.</returns>
    public static Guid ToGuidOrDefault(this ReadOnlySpan<byte> span, int position = 0, Guid defaultValue = default)
    {
        return ToGuidOrDefault(span, ref position, defaultValue);
    }

    #endregion
}
