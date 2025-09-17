namespace Plugin.ByteArrays;

/// <summary>
///     Functions converting other types into a byte array.
/// </summary>
public static partial class ByteArrayExtensions
{

    #region Floating Point Conversions

    /// <summary>
    ///     Converts the byte array into a Signed 64-bit double-precision floating-point number
    ///     From -1.79769313486232E+308 to 1.79769313486232E+308.
    ///     Size = 8 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array. Byref : is auto-incremented by the size of the output type.</param>
    /// <returns>Converted value.</returns>
    public static double ToDouble(this byte[] array, ref int position)
    {
        return ExecuteConversionToType(array, ref position, sizeof(double), BitConverter.ToDouble);
    }

    /// <summary>
    ///     Converts the byte array into a Signed 64-bit double-precision floating-point number
    ///     From -1.79769313486232E+308 to 1.79769313486232E+308.
    ///     Size = 8 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array.</param>
    /// <returns>Converted value.</returns>
    public static double ToDouble(this byte[] array, int position = 0)
    {
        return ToDouble(array, ref position);
    }

    /// <summary>
    ///     Converts the byte array into a Signed 64-bit double-precision floating-point number
    ///     From -1.79769313486232E+308 to 1.79769313486232E+308.
    ///     Size = 8 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array. Byref : is auto-incremented by the size of the output type.</param>
    /// <param name="defaultValue">Default value for that type, can be overriden.</param>
    /// <returns>Converted value.</returns>
    public static double ToDoubleOrDefault(this byte[] array, ref int position, double defaultValue = 0)
    {
        return ExecuteConversionToTypeOrDefault(array, ref position, sizeof(double), BitConverter.ToDouble,
                                                defaultValue);
    }

    /// <summary>
    ///     Converts the byte array into a Signed 64-bit double-precision floating-point number
    ///     From -1.79769313486232E+308 to 1.79769313486232E+308.
    ///     Size = 8 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array.</param>
    /// <param name="defaultValue">Default value for that type, can be overriden.</param>
    /// <returns>Converted value.</returns>
    public static double ToDoubleOrDefault(this byte[] array, int position = 0, double defaultValue = 0)
    {
        return ToDoubleOrDefault(array, ref position, defaultValue);
    }

    /// <summary>
    ///     Converts the byte array into a Signed 32-bit single-precision floating-point number
    ///     From approximately -3.4 × 10^38 to 3.4 × 10^38.
    ///     Size = 4 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array. Byref : is auto-incremented by the size of the output type.</param>
    /// <returns>Converted value.</returns>
    public static float ToSingle(this byte[] array, ref int position)
    {
        return ExecuteConversionToType(array, ref position, sizeof(float), BitConverter.ToSingle);
    }

    /// <summary>
    ///     Converts the byte array into a Signed 32-bit single-precision floating-point number
    ///     From approximately -3.4 × 10^38 to 3.4 × 10^38.
    ///     Size = 4 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array.</param>
    /// <returns>Converted value.</returns>
    public static float ToSingle(this byte[] array, int position = 0)
    {
        return ToSingle(array, ref position);
    }

    /// <summary>
    ///     Converts the byte array into a Signed 32-bit single-precision floating-point number
    ///     From approximately -3.4 × 10^38 to 3.4 × 10^38.
    ///     Size = 4 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array. Byref : is auto-incremented by the size of the output type.</param>
    /// <param name="defaultValue">Default value for that type, can be overriden.</param>
    /// <returns>Converted value.</returns>
    public static float ToSingleOrDefault(this byte[] array, ref int position, float defaultValue = 0)
    {
        return ExecuteConversionToTypeOrDefault(array, ref position, sizeof(float), BitConverter.ToSingle,
                                                defaultValue);
    }

    /// <summary>
    ///     Converts the byte array into a Signed 32-bit single-precision floating-point number
    ///     From approximately -3.4 × 10^38 to 3.4 × 10^38.
    ///     Size = 4 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array.</param>
    /// <param name="defaultValue">Default value for that type, can be overriden.</param>
    /// <returns>Converted value.</returns>
    public static float ToSingleOrDefault(this byte[] array, int position = 0, float defaultValue = 0)
    {
        return ToSingleOrDefault(array, ref position, defaultValue);
    }

    /// <summary>
    ///     Converts the byte array into a 16-bit half-precision floating-point number
    ///     From approximately -65504 to 65504.
    ///     Size = 2 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array. Byref : is auto-incremented by the size of the output type.</param>
    /// <returns>Converted value.</returns>
    public static Half ToHalf(this byte[] array, ref int position)
    {
        return ExecuteConversionToType(array, ref position, 2, BitConverter.ToHalf);
    }

    /// <summary>
    ///     Converts the byte array into a 16-bit half-precision floating-point number
    ///     From approximately -65504 to 65504.
    ///     Size = 2 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array.</param>
    /// <returns>Converted value.</returns>
    public static Half ToHalf(this byte[] array, int position = 0)
    {
        return ToHalf(array, ref position);
    }

    /// <summary>
    ///     Converts the byte array into a 16-bit half-precision floating-point number
    ///     From approximately -65504 to 65504.
    ///     Size = 2 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array. Byref : is auto-incremented by the size of the output type.</param>
    /// <param name="defaultValue">Default value for that type, can be overriden.</param>
    /// <returns>Converted value.</returns>
    public static Half ToHalfOrDefault(this byte[] array, ref int position, Half defaultValue = default)
    {
        return ExecuteConversionToTypeOrDefault(array, ref position, 2, BitConverter.ToHalf, defaultValue);
    }

    /// <summary>
    ///     Converts the byte array into a 16-bit half-precision floating-point number
    ///     From approximately -65504 to 65504.
    ///     Size = 2 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array.</param>
    /// <param name="defaultValue">Default value for that type, can be overriden.</param>
    /// <returns>Converted value.</returns>
    public static Half ToHalfOrDefault(this byte[] array, int position = 0, Half defaultValue = default)
    {
        return ToHalfOrDefault(array, ref position, defaultValue);
    }

    #endregion

}
