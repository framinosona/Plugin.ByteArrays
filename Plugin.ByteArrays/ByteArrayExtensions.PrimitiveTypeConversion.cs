namespace Plugin.ByteArrays;

/// <summary>
///     Functions converting other types into a byte array.
/// </summary>
public static partial class ByteArrayExtensions
{
    #region Primitive Type Conversions

    /// <summary>
    ///     Converts the byte array into a Boolean value
    ///     True[1] or False[0].
    ///     Size = 1 byte.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array. Byref : is auto-incremented by the size of the output type.</param>
    /// <returns>Converted value.</returns>
    public static bool ToBoolean(this byte[] array, ref int position)
    {
        return ExecuteConversionToType(array, ref position, sizeof(bool), BitConverter.ToBoolean);
    }

    /// <summary>
    ///     Converts the byte array into a Boolean value
    ///     True[1] or False[0].
    ///     Size = 1 byte.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array.</param>
    /// <returns>Converted value.</returns>
    public static bool ToBoolean(this byte[] array, int position = 0)
    {
        return ToBoolean(array, ref position);
    }

    /// <summary>
    ///     Converts the byte array into a Boolean value
    ///     True[1] or False[0].
    ///     Returns defaultValue if array too short
    ///     Size = 1 byte.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array.</param>
    /// <param name="defaultValue">Default value for that type, can be overriden.</param>
    /// <returns>Converted value.</returns>
    public static bool ToBooleanOrDefault(this byte[] array, ref int position, bool defaultValue = false)
    {
        return ExecuteConversionToTypeOrDefault(array, ref position, sizeof(bool), BitConverter.ToBoolean,
                                                defaultValue);
    }

    /// <summary>
    ///     Converts the byte array into a Boolean value
    ///     True[1] or False[0].
    ///     Returns defaultValue if array too short
    ///     Size = 1 byte.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array.</param>
    /// <param name="defaultValue">Default value for that type, can be overriden.</param>
    /// <returns>Converted value.</returns>
    public static bool ToBooleanOrDefault(this byte[] array, int position = 0, bool defaultValue = false)
    {
        return ToBooleanOrDefault(array, ref position, defaultValue);
    }

    /// <summary>
    ///     Converts the byte array into an Unsigned 8-bit integer
    ///     From 0 to 255.
    ///     Size = 1 byte.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array. Byref : is auto-incremented by the size of the output type.</param>
    /// <returns>Converted value.</returns>
    public static byte ToByte(this byte[] array, ref int position)
    {
        return ExecuteConversionToType(array, ref position, sizeof(byte), (bytes, i) => bytes[i]);
    }

    /// <summary>
    ///     Converts the byte array into an Unsigned 8-bit integer
    ///     From 0 to 255.
    ///     Size = 1 byte.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array.</param>
    /// <returns>Converted value.</returns>
    public static byte ToByte(this byte[] array, int position = 0)
    {
        return ToByte(array, ref position);
    }

    /// <summary>
    ///     Converts the byte array into an Unsigned 8-bit integer
    ///     From 0 to 255.
    ///     Size = 1 byte.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array. Byref : is auto-incremented by the size of the output type.</param>
    /// <param name="defaultValue">Value returned if out of bounds</param>
    /// <returns>Converted value.</returns>
    public static byte ToByteOrDefault(this byte[] array, ref int position, byte defaultValue = 0)
    {
        return ExecuteConversionToTypeOrDefault(array, ref position, sizeof(byte), (bytes, i) => bytes[i],
                                                defaultValue);
    }

    /// <summary>
    ///     Converts the byte array into an Unsigned 8-bit integer
    ///     From 0 to 255.
    ///     Size = 1 byte.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array.</param>
    /// <param name="defaultValue">Value returned if out of bounds</param>
    /// <returns>Converted value.</returns>
    public static byte ToByteOrDefault(this byte[] array, int position = 0, byte defaultValue = 0)
    {
        return ToByteOrDefault(array, ref position, defaultValue);
    }

    /// <summary>
    ///     Converts the byte array into a Signed 8-bit integer
    ///     From -128 to 127.
    ///     Size = 1 byte.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array. Byref : is auto-incremented by the size of the output type.</param>
    /// <returns>Converted value.</returns>
    public static sbyte ToSByte(this byte[] array, ref int position)
    {
        return ExecuteConversionToType(array, ref position, sizeof(sbyte), (bytes, i) => (sbyte)bytes[i]);
    }

    /// <summary>
    ///     Converts the byte array into a Signed 8-bit integer
    ///     From -128 to 127.
    ///     Size = 1 byte.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array.</param>
    /// <returns>Converted value.</returns>
    public static sbyte ToSByte(this byte[] array, int position = 0)
    {
        return ToSByte(array, ref position);
    }

    /// <summary>
    ///     Converts the byte array into a Signed 8-bit integer
    ///     From -128 to 127.
    ///     Size = 1 byte.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array. Byref : is auto-incremented by the size of the output type.</param>
    /// <param name="defaultValue">Value returned if out of bounds</param>
    /// <returns>Converted value.</returns>
    public static sbyte ToSByteOrDefault(this byte[] array, ref int position, sbyte defaultValue = 0)
    {
        return ExecuteConversionToTypeOrDefault(array, ref position, sizeof(sbyte), (bytes, i) => (sbyte)bytes[i],
                                                defaultValue);
    }

    /// <summary>
    ///     Converts the byte array into a Signed 8-bit integer
    ///     From -128 to 127.
    ///     Size = 1 byte.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array.</param>
    /// <param name="defaultValue">Value returned if out of bounds</param>
    /// <returns>Converted value.</returns>
    public static sbyte ToSByteOrDefault(this byte[] array, int position = 0, sbyte defaultValue = 0)
    {
        return ToSByteOrDefault(array, ref position, defaultValue);
    }

    /// <summary>
    ///     Converts the byte array into a Character in UTF-16 code unit
    ///     Any UTF-16 character.
    ///     Size = 2 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array. Byref : is auto-incremented by the size of the output type.</param>
    /// <returns>Converted value.</returns>
    public static char ToChar(this byte[] array, ref int position)
    {
        return ExecuteConversionToType(array, ref position, sizeof(char), BitConverter.ToChar);
    }

    /// <summary>
    ///     Converts the byte array into a Character in UTF-16 code unit
    ///     Any UTF-16 character.
    ///     Size = 2 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array.</param>
    /// <returns>Converted value.</returns>
    public static char ToChar(this byte[] array, int position = 0)
    {
        return ToChar(array, ref position);
    }

    /// <summary>
    ///     Converts the byte array into a Character in UTF-16 code unit
    ///     Any UTF-16 character.
    ///     Size = 2 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array. Byref : is auto-incremented by the size of the output type.</param>
    /// <param name="defaultValue">Default value for that type, can be overriden.</param>
    /// <returns>Converted value.</returns>
    public static char ToCharOrDefault(this byte[] array, ref int position, char defaultValue = '\0')
    {
        return ExecuteConversionToTypeOrDefault(array, ref position, sizeof(char), BitConverter.ToChar,
                                                defaultValue);
    }

    /// <summary>
    ///     Converts the byte array into a Character in UTF-16 code unit
    ///     Any UTF-16 character.
    ///     Size = 2 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array.</param>
    /// <param name="defaultValue">Default value for that type, can be overriden.</param>
    /// <returns>Converted value.</returns>
    public static char ToCharOrDefault(this byte[] array, int position = 0, char defaultValue = '\0')
    {
        return ToCharOrDefault(array, ref position, defaultValue);
    }

    #endregion

}
