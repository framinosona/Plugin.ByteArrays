namespace Plugin.ByteArrays;

/// <summary>
///     Functions converting GUID types from byte arrays.
/// </summary>
public static partial class ByteArrayExtensions
{
    #region GUID Conversions

    /// <summary>
    ///     Converts the byte array into a GUID.
    ///     Size = 16 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array. Byref : is auto-incremented by the size of the output type.</param>
    /// <returns>Converted GUID value.</returns>
    public static Guid ToGuid(this byte[] array, ref int position)
    {
        ArgumentNullException.ThrowIfNull(array);
        const int guidSize = 16;

        if (position < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(position), $"Position must be greater than or equal to 0. Was {position}.");
        }

        if (array.Length < position + guidSize)
        {
            throw new ArgumentOutOfRangeException(nameof(position),
                $"Array {array.ToDebugString()} is too small. Reading {guidSize} bytes from position {position} is not possible in array of {array.Length}");
        }

        var guidBytes = array.Skip(position).Take(guidSize).ToArray();
        var guid = new Guid(guidBytes);
        position += guidSize;
        return guid;
    }

    /// <summary>
    ///     Converts the byte array into a GUID.
    ///     Size = 16 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array.</param>
    /// <returns>Converted GUID value.</returns>
    public static Guid ToGuid(this byte[] array, int position = 0)
    {
        return ToGuid(array, ref position);
    }

    /// <summary>
    ///     Converts the byte array into a GUID.
    ///     Returns the default value if conversion fails.
    ///     Size = 16 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array. Byref : is auto-incremented by the size of the output type.</param>
    /// <param name="defaultValue">Default value for that type, can be overridden.</param>
    /// <returns>Converted GUID value.</returns>
    public static Guid ToGuidOrDefault(this byte[] array, ref int position, Guid defaultValue = default)
    {
        try
        {
            return ToGuid(array, ref position);
        }
        catch (ArgumentOutOfRangeException)
        {
            return defaultValue;
        }
        catch (ArgumentException)
        {
            return defaultValue;
        }
    }

    /// <summary>
    ///     Converts the byte array into a GUID.
    ///     Returns the default value if conversion fails.
    ///     Size = 16 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array.</param>
    /// <param name="defaultValue">Default value for that type, can be overridden.</param>
    /// <returns>Converted GUID value.</returns>
    public static Guid ToGuidOrDefault(this byte[] array, int position = 0, Guid defaultValue = default)
    {
        return ToGuidOrDefault(array, ref position, defaultValue);
    }

    #endregion
}
