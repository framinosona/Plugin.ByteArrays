namespace Plugin.ByteArrays;

/// <summary>
///     Functions converting other types into a byte array.
/// </summary>
public static partial class ByteArrayExtensions
{
    #region Integer Conversions

    /// <summary>
    ///     Converts the byte array into a Signed 16-bit integer
    ///     From -32,768 to 32,767.
    ///     Size = 2 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array. Byref : is auto-incremented by the size of the output type.</param>
    /// <returns>Converted value.</returns>
    public static short ToInt16(this byte[] array, ref int position)
    {
        return ExecuteConversionToType(array, ref position, sizeof(short), BitConverter.ToInt16);
    }

    /// <summary>
    ///     Converts the byte array into a Signed 16-bit integer
    ///     From -32,768 to 32,767.
    ///     Size = 2 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array.</param>
    /// <returns>Converted value.</returns>
    public static short ToInt16(this byte[] array, int position = 0)
    {
        return ToInt16(array, ref position);
    }

    /// <summary>
    ///     Converts the byte array into a Signed 16-bit integer
    ///     From -32,768 to 32,767.
    ///     Size = 2 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array. Byref : is auto-incremented by the size of the output type.</param>
    /// <param name="defaultValue">Value returned if out of bounds</param>
    /// <returns>Converted value.</returns>
    public static short ToInt16OrDefault(this byte[] array, ref int position, short defaultValue = 0)
    {
        return ExecuteConversionToTypeOrDefault(array, ref position, sizeof(short), BitConverter.ToInt16,
                                                defaultValue);
    }

    /// <summary>
    ///     Converts the byte array into a Signed 16-bit integer
    ///     From -32,768 to 32,767.
    ///     Size = 2 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array.</param>
    /// <param name="defaultValue">Default value for that type, can be overriden.</param>
    /// <returns>Converted value.</returns>
    public static short ToInt16OrDefault(this byte[] array, int position = 0, short defaultValue = 0)
    {
        return ToInt16OrDefault(array, ref position, defaultValue);
    }

    /// <summary>
    ///     Converts the byte array into a Unsigned 16-bit integer
    ///     From 0 to 65,535.
    ///     Size = 2 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array. Byref : is auto-incremented by the size of the output type.</param>
    /// <returns>Converted value.</returns>
    public static ushort ToUInt16(this byte[] array, ref int position)
    {
        return ExecuteConversionToType(array, ref position, sizeof(ushort), BitConverter.ToUInt16);
    }

    /// <summary>
    ///     Converts the byte array into a Unsigned 16-bit integer
    ///     From 0 to 65,535.
    ///     Size = 2 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array.</param>
    /// <returns>Converted value.</returns>
    public static ushort ToUInt16(this byte[] array, int position = 0)
    {
        return ToUInt16(array, ref position);
    }

    /// <summary>
    ///     Converts the byte array into a Unsigned 16-bit integer
    ///     From 0 to 65,535.
    ///     Size = 2 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array. Byref : is auto-incremented by the size of the output type.</param>
    /// <param name="defaultValue">Default value for that type, can be overriden.</param>
    /// <returns>Converted value.</returns>
    public static ushort ToUInt16OrDefault(this byte[] array, ref int position, ushort defaultValue = 0)
    {
        return ExecuteConversionToTypeOrDefault(array, ref position, sizeof(ushort), BitConverter.ToUInt16,
                                                defaultValue);
    }

    /// <summary>
    ///     Converts the byte array into a Unsigned 16-bit integer
    ///     From 0 to 65,535.
    ///     Size = 2 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array.</param>
    /// <param name="defaultValue">Default value for that type, can be overriden.</param>
    /// <returns>Converted value.</returns>
    public static ushort ToUInt16OrDefault(this byte[] array, int position = 0, ushort defaultValue = 0)
    {
        return ToUInt16OrDefault(array, ref position, defaultValue);
    }

    /// <summary>
    ///     Converts the byte array into a Signed 32-bit integer
    ///     From -2,147,483,648 to 2,147,483,647.
    ///     Size = 4 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array. Byref : is auto-incremented by the size of the output type.</param>
    /// <returns>Converted value.</returns>
    public static int ToInt32(this byte[] array, ref int position)
    {
        return ExecuteConversionToType(array, ref position, sizeof(int), BitConverter.ToInt32);
    }

    /// <summary>
    ///     Converts the byte array into a Signed 32-bit integer
    ///     From -2,147,483,648 to 2,147,483,647.
    ///     Size = 4 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array.</param>
    /// <returns>Converted value.</returns>
    public static int ToInt32(this byte[] array, int position = 0)
    {
        return ToInt32(array, ref position);
    }

    /// <summary>
    ///     Converts the byte array into a Signed 32-bit integer
    ///     From -2,147,483,648 to 2,147,483,647.
    ///     Size = 4 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array. Byref : is auto-incremented by the size of the output type.</param>
    /// <param name="defaultValue">Default value for that type, can be overriden.</param>
    /// <returns>Converted value.</returns>
    public static int ToInt32OrDefault(this byte[] array, ref int position, int defaultValue = 0)
    {
        return ExecuteConversionToTypeOrDefault(array, ref position, sizeof(int), BitConverter.ToInt32,
                                                defaultValue);
    }

    /// <summary>
    ///     Converts the byte array into a Signed 32-bit integer
    ///     From -2,147,483,648 to 2,147,483,647.
    ///     Size = 4 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array.</param>
    /// <param name="defaultValue">Default value for that type, can be overriden.</param>
    /// <returns>Converted value.</returns>
    public static int ToInt32OrDefault(this byte[] array, int position = 0, int defaultValue = 0)
    {
        return ToInt32OrDefault(array, ref position, defaultValue);
    }

    /// <summary>
    ///     Converts the byte array into a Unsigned 32-bit integer
    ///     From 0 to 4,294,967,295.
    ///     Size = 4 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array. Byref : is auto-incremented by the size of the output type.</param>
    /// <returns>Converted value.</returns>
    public static uint ToUInt32(this byte[] array, ref int position)
    {
        return ExecuteConversionToType(array, ref position, sizeof(uint), BitConverter.ToUInt32);
    }

    /// <summary>
    ///     Converts the byte array into a Unsigned 32-bit integer
    ///     From 0 to 4,294,967,295.
    ///     Size = 4 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array.</param>
    /// <returns>Converted value.</returns>
    public static uint ToUInt32(this byte[] array, int position = 0)
    {
        return ToUInt32(array, ref position);
    }

    /// <summary>
    ///     Converts the byte array into a Unsigned 32-bit integer
    ///     From 0 to 4,294,967,295.
    ///     Size = 4 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array. Byref : is auto-incremented by the size of the output type.</param>
    /// <param name="defaultValue">Default value for that type, can be overriden.</param>
    /// <returns>Converted value.</returns>
    public static uint ToUInt32OrDefault(this byte[] array, ref int position, uint defaultValue = 0)
    {
        return ExecuteConversionToTypeOrDefault(array, ref position, sizeof(uint), BitConverter.ToUInt32,
                                                defaultValue);
    }

    /// <summary>
    ///     Converts the byte array into a Unsigned 32-bit integer
    ///     From 0 to 4,294,967,295.
    ///     Size = 4 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array.</param>
    /// <param name="defaultValue">Default value for that type, can be overriden.</param>
    /// <returns>Converted value.</returns>
    public static uint ToUInt32OrDefault(this byte[] array, int position = 0, uint defaultValue = 0)
    {
        return ToUInt32OrDefault(array, ref position, defaultValue);
    }

    /// <summary>
    ///     Converts the byte array into a Signed 64-bit integer
    ///     From -9,223,372,036,854,775,808 to 9,223,372,036,854,775,807.
    ///     Size = 8 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array. Byref : is auto-incremented by the size of the output type.</param>
    /// <returns>Converted value.</returns>
    public static long ToInt64(this byte[] array, ref int position)
    {
        return ExecuteConversionToType(array, ref position, sizeof(long), BitConverter.ToInt64);
    }

    /// <summary>
    ///     Converts the byte array into a Signed 64-bit integer
    ///     From -9,223,372,036,854,775,808 to 9,223,372,036,854,775,807.
    ///     Size = 8 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array.</param>
    /// <returns>Converted value.</returns>
    public static long ToInt64(this byte[] array, int position = 0)
    {
        return ToInt64(array, ref position);
    }

    /// <summary>
    ///     Converts the byte array into a Signed 64-bit integer
    ///     From -9,223,372,036,854,775,808 to 9,223,372,036,854,775,807.
    ///     Size = 8 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array. Byref : is auto-incremented by the size of the output type.</param>
    /// <param name="defaultValue">Default value for that type, can be overriden.</param>
    /// <returns>Converted value.</returns>
    public static long ToInt64OrDefault(this byte[] array, ref int position, long defaultValue = 0)
    {
        return ExecuteConversionToTypeOrDefault(array, ref position, sizeof(long), BitConverter.ToInt64,
                                                defaultValue);
    }

    /// <summary>
    ///     Converts the byte array into a Signed 64-bit integer
    ///     From -9,223,372,036,854,775,808 to 9,223,372,036,854,775,807.
    ///     Size = 8 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array.</param>
    /// <param name="defaultValue">Default value for that type, can be overriden.</param>
    /// <returns>Converted value.</returns>
    public static long ToInt64OrDefault(this byte[] array, int position = 0, long defaultValue = 0)
    {
        return ToInt64OrDefault(array, ref position, defaultValue);
    }

    /// <summary>
    ///     Converts the byte array into a Unsigned 64-bit integer
    ///     From 0 to 18,446,744,073,709,551,615.
    ///     Size = 8 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array. Byref : is auto-incremented by the size of the output type.</param>
    /// <returns>Converted value.</returns>
    public static ulong ToUInt64(this byte[] array, ref int position)
    {
        return ExecuteConversionToType(array, ref position, sizeof(ulong), BitConverter.ToUInt64);
    }

    /// <summary>
    ///     Converts the byte array into a Unsigned 64-bit integer
    ///     From 0 to 18,446,744,073,709,551,615.
    ///     Size = 8 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array.</param>
    /// <returns>Converted value.</returns>
    public static ulong ToUInt64(this byte[] array, int position = 0)
    {
        return ToUInt64(array, ref position);
    }

    /// <summary>
    ///     Converts the byte array into a Unsigned 64-bit integer
    ///     From 0 to 18,446,744,073,709,551,615.
    ///     Size = 8 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array. Byref : is auto-incremented by the size of the output type.</param>
    /// <param name="defaultValue">Default value for that type, can be overriden.</param>
    /// <returns>Converted value.</returns>
    public static ulong ToUInt64OrDefault(this byte[] array, ref int position, ulong defaultValue = 0)
    {
        return ExecuteConversionToTypeOrDefault(array, ref position, sizeof(ulong), BitConverter.ToUInt64,
                                                defaultValue);
    }

    /// <summary>
    ///     Converts the byte array into a Unsigned 64-bit integer
    ///     From 0 to 18,446,744,073,709,551,615.
    ///     Size = 8 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array.</param>
    /// <param name="defaultValue">Default value for that type, can be overriden.</param>
    /// <returns>Converted value.</returns>
    public static ulong ToUInt64OrDefault(this byte[] array, int position = 0, ulong defaultValue = 0)
    {
        return ToUInt64OrDefault(array, ref position, defaultValue);
    }

    #endregion

}
