namespace Plugin.ByteArrays;

/// <summary>
///     Provides extension methods for converting objects and strings to byte arrays using various encodings and formats.
/// </summary>
public static class ObjectToByteArrayExtensions
{
    /// <summary>
    ///     Creates a byte array from an ASCII-encoded string.
    /// </summary>
    /// <param name="value">The input string to convert to a byte array.</param>
    /// <returns>A new byte array containing the ASCII-encoded bytes of the input string, or an empty array if the input is null or empty.</returns>
    public static byte[] AsciiStringToByteArray(this string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return [];
        }

        using var byteArrayBuilder = new ByteArrayBuilder();
        return byteArrayBuilder.AppendAsciiString(value).ToByteArray();
    }

    /// <summary>
    ///     Creates a byte array from a UTF-8 encoded string.
    /// </summary>
    /// <param name="value">The input string to convert to a byte array.</param>
    /// <returns>A new byte array containing the UTF-8 encoded bytes of the input string, or an empty array if the input is null or empty.</returns>
    public static byte[] Utf8StringToByteArray(this string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return [];
        }

        using var byteArrayBuilder = new ByteArrayBuilder();
        return byteArrayBuilder.AppendUtf8String(value).ToByteArray();
    }

    /// <summary>
    ///     Creates a byte array from a hexadecimal string.
    /// </summary>
    /// <param name="hexString">The input hexadecimal string to convert to a byte array.</param>
    /// <returns>A new byte array containing the bytes represented by the hexadecimal string, or an empty array if the input is null or empty.</returns>
    /// <exception cref="ArgumentException">Thrown if the input string is not a valid hexadecimal string.</exception>
    /// <remarks>
    /// The input string must have an even number of characters, as each pair of characters represents a byte.
    /// If the string is empty or null, an empty byte array is returned.
    /// </remarks>
    public static byte[] HexStringToByteArray(this string hexString)
    {
        if (string.IsNullOrEmpty(hexString))
        {
            return [];
        }

        using var byteArrayBuilder = new ByteArrayBuilder();
        return byteArrayBuilder.AppendHexString(hexString).ToByteArray();
    }

    /// <summary>
    ///     Creates a byte array from a Base64-encoded string.
    /// </summary>
    /// <param name="base64String">The input Base64 string to convert to a byte array.</param>
    /// <returns>A new byte array containing the decoded bytes of the Base64 string, or an empty array if the input is null or empty.</returns>
    public static byte[] Base64StringToByteArray(this string base64String)
    {
        if (string.IsNullOrEmpty(base64String))
        {
            return [];
        }

        using var byteArrayBuilder = new ByteArrayBuilder();
        return byteArrayBuilder.AppendBase64String(base64String).ToByteArray();
    }

    /// <summary>
    ///     Creates a byte array from an object of any supported type.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <param name="value">The input value to convert to a byte array.</param>
    /// <returns>A new byte array representing the input value, or an empty array if the input is null.</returns>
    public static byte[] ToByteArray<T>(this T value)
    {
        if (value is null)
        {
            return [];
        }

        using var byteArrayBuilder = new ByteArrayBuilder();
        return byteArrayBuilder.Append(value).ToByteArray();
    }
}
