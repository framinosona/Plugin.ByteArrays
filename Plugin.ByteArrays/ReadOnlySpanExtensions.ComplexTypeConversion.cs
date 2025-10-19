using System.Globalization;

namespace Plugin.ByteArrays;

/// <summary>
///     Functions for working with ReadOnlySpan&lt;byte&gt; providing zero-allocation operations for reading and manipulating byte data.
/// </summary>
public static partial class ReadOnlySpanExtensions
{
    #region Complex Type Conversions

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into an Enum value of type T.
    ///     The underlying value is stored as an Int32.
    ///     Size = 4 bytes.
    /// </summary>
    /// <typeparam name="T">The enum type.</typeparam>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span. By reference; is auto-incremented by the size of the output type.</param>
    /// <returns>Converted value.</returns>
    /// <exception cref="ArgumentException">Thrown if the value is not defined in the enum.</exception>
    public static T ToEnum<T>(this ReadOnlySpan<byte> span, ref int position) where T : struct, Enum
    {
        var intValue = ExecuteConversionToType(span, ref position, sizeof(int), s => BitConverter.ToInt32(s));
        var enumValue = (T)Enum.ToObject(typeof(T), intValue);

        if (!Enum.IsDefined(enumValue))
        {
            throw new ArgumentException($"Value {intValue} is not defined in enum {typeof(T).Name}");
        }

        return enumValue;
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into an Enum value of type T.
    ///     The underlying value is stored as an Int32.
    ///     Size = 4 bytes.
    /// </summary>
    /// <typeparam name="T">The enum type.</typeparam>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span.</param>
    /// <returns>Converted value.</returns>
    public static T ToEnum<T>(this ReadOnlySpan<byte> span, int position = 0) where T : struct, Enum
    {
        return ToEnum<T>(span, ref position);
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into an Enum value of type T.
    ///     Returns the default value if conversion fails or value is not defined in the enum.
    ///     The underlying value is stored as an Int32.
    ///     Size = 4 bytes.
    /// </summary>
    /// <typeparam name="T">The enum type.</typeparam>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span. By reference; is auto-incremented by the size of the output type.</param>
    /// <param name="defaultValue">Value returned if out of bounds or undefined.</param>
    /// <returns>Converted value.</returns>
    public static T ToEnumOrDefault<T>(this ReadOnlySpan<byte> span, ref int position, T defaultValue = default) where T : struct, Enum
    {
        return ExecuteConversionToTypeOrDefault(span, ref position, sizeof(int), s =>
        {
            var intValue = BitConverter.ToInt32(s);
            var enumValue = (T)Enum.ToObject(typeof(T), intValue);

            if (!Enum.IsDefined(enumValue))
            {
                return defaultValue;
            }

            return enumValue;
        }, defaultValue);
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into an Enum value of type T.
    ///     Returns the default value if conversion fails or value is not defined in the enum.
    ///     The underlying value is stored as an Int32.
    ///     Size = 4 bytes.
    /// </summary>
    /// <typeparam name="T">The enum type.</typeparam>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span.</param>
    /// <param name="defaultValue">Default value for that type, can be overridden.</param>
    /// <returns>Converted value.</returns>
    public static T ToEnumOrDefault<T>(this ReadOnlySpan<byte> span, int position = 0, T defaultValue = default) where T : struct, Enum
    {
        return ToEnumOrDefault(span, ref position, defaultValue);
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into a Version object.
    ///     The Version is stored as: major (int32), minor (int32), build (int32), revision (int32).
    ///     Size = 16 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span. By reference; is auto-incremented by the size of the output type.</param>
    /// <returns>Converted value.</returns>
    public static Version ToVersion(this ReadOnlySpan<byte> span, ref int position)
    {
        const int size = sizeof(int) * 4; // major, minor, build, revision
        return ExecuteConversionToType(span, ref position, size, s =>
        {
            var major = BitConverter.ToInt32(s[..4]);
            var minor = BitConverter.ToInt32(s.Slice(4, 4));
            var build = BitConverter.ToInt32(s.Slice(8, 4));
            var revision = BitConverter.ToInt32(s.Slice(12, 4));

            if (build == -1 && revision == -1)
            {
                return new Version(major, minor);
            }
            if (revision == -1)
            {
                return new Version(major, minor, build);
            }
            return new Version(major, minor, build, revision);
        });
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into a Version object.
    ///     The Version is stored as: major (int32), minor (int32), build (int32), revision (int32).
    ///     Size = 16 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span.</param>
    /// <returns>Converted value.</returns>
    public static Version ToVersion(this ReadOnlySpan<byte> span, int position = 0)
    {
        return ToVersion(span, ref position);
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into a Version object.
    ///     Returns the default value if conversion fails.
    ///     The Version is stored as: major (int32), minor (int32), build (int32), revision (int32).
    ///     Size = 16 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span. By reference; is auto-incremented by the size of the output type.</param>
    /// <param name="defaultValue">Value returned if out of bounds.</param>
    /// <returns>Converted value.</returns>
    public static Version ToVersionOrDefault(this ReadOnlySpan<byte> span, ref int position, Version? defaultValue = null)
    {
        const int size = sizeof(int) * 4; // major, minor, build, revision
        return ExecuteConversionToTypeOrDefault(span, ref position, size, s =>
        {
            var major = BitConverter.ToInt32(s[..4]);
            var minor = BitConverter.ToInt32(s.Slice(4, 4));
            var build = BitConverter.ToInt32(s.Slice(8, 4));
            var revision = BitConverter.ToInt32(s.Slice(12, 4));

            if (build == -1 && revision == -1)
            {
                return new Version(major, minor);
            }
            if (revision == -1)
            {
                return new Version(major, minor, build);
            }
            return new Version(major, minor, build, revision);
        }, defaultValue ?? new Version(0, 0))!;
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into a Version object.
    ///     Returns the default value if conversion fails.
    ///     The Version is stored as: major (int32), minor (int32), build (int32), revision (int32).
    ///     Size = 16 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span.</param>
    /// <param name="defaultValue">Default value for that type, can be overridden.</param>
    /// <returns>Converted value.</returns>
    public static Version ToVersionOrDefault(this ReadOnlySpan<byte> span, int position = 0, Version? defaultValue = null)
    {
        return ToVersionOrDefault(span, ref position, defaultValue);
    }

    #endregion
}
