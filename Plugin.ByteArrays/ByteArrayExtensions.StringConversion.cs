using System.Text;

namespace Plugin.ByteArrays;

/// <summary>
///     Functions converting other types into a byte array.
/// </summary>
public static partial class ByteArrayExtensions
{

    #region String Conversions

    /// <summary>
    ///     Converts parts of a byte array into a UTF8 String.
    /// </summary>
    /// <param name="array">The byte array. Cannot be null.</param>
    /// <param name="position">The position within the array. Byref : is auto-incremented by the size of the output type.</param>
    /// <param name="numberOfBytesToRead">Number of characters of the expected string</param>
    /// <returns>Converted value.</returns>
    public static string ToUtf8String(this byte[] array, ref int position, int numberOfBytesToRead = -1)
    {
        ArgumentNullException.ThrowIfNull(array);
        if (numberOfBytesToRead == -1)
        {
            numberOfBytesToRead = array.Length - position;
        }
        if (numberOfBytesToRead == 0)
        {
            return string.Empty;
        }

        return ExecuteConversionToType(array, ref position, numberOfBytesToRead, (bytes, _) => Encoding.UTF8.GetString(bytes));
    }

    /// <summary>
    ///     Converts parts of a byte array into a UTF8 String.
    /// </summary>
    /// <param name="array">The byte array. Cannot be null.</param>
    /// <param name="position">The position within the array.</param>
    /// <param name="numberOfBytesToRead">Number of characters of the expected string</param>
    /// <returns>Converted value.</returns>
    public static string ToUtf8String(this byte[] array, int position = 0, int numberOfBytesToRead = -1)
    {
        return ToUtf8String(array, ref position, numberOfBytesToRead);
    }

    /// <summary>
    ///     Converts parts of a byte array into a UTF8 String.
    /// </summary>
    /// <param name="array">The byte array. Cannot be null.</param>
    /// <param name="position">The position within the array. Byref : is auto-incremented by the size of the output type.</param>
    /// <param name="numberOfBytesToRead">Number of characters of the expected string</param>
    /// <param name="defaultValue">Default value for that type, can be overriden.</param>
    /// <returns>Converted value.</returns>
    public static string ToUtf8StringOrDefault(this byte[] array, ref int position, int numberOfBytesToRead = -1, string defaultValue = "")
    {
        ArgumentNullException.ThrowIfNull(array);
        if (numberOfBytesToRead == -1)
        {
            numberOfBytesToRead = array.Length - position;
        }

        return ExecuteConversionToTypeOrDefault(array, ref position, numberOfBytesToRead, (bytes, _) => Encoding.UTF8.GetString(bytes),
                                                defaultValue);
    }

    /// <summary>
    ///     Converts parts of a byte array into a UTF8 String.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array.</param>
    /// <param name="numberOfBytesToRead">Number of characters of the expected string</param>
    /// <param name="defaultValue">Default value for that type, can be overriden.</param>
    /// <returns>Converted value.</returns>
    public static string ToUtf8StringOrDefault(this byte[] array, int position = 0, int numberOfBytesToRead = -1, string defaultValue = "")
    {
        return ToUtf8StringOrDefault(array, ref position, numberOfBytesToRead, defaultValue);
    }

    /// <summary>
    ///     Converts parts of a byte array into an ASCII String.
    /// </summary>
    /// <param name="array">The byte array. Cannot be null.</param>
    /// <param name="position">The position within the array. Byref : is auto-incremented by the size of the output type.</param>
    /// <param name="numberOfBytesToRead">Number of characters of the expected string</param>
    /// <returns>Converted value.</returns>
    public static string ToAsciiString(this byte[] array, ref int position, int numberOfBytesToRead = -1)
    {
        ArgumentNullException.ThrowIfNull(array);
        if (numberOfBytesToRead == -1)
        {
            numberOfBytesToRead = array.Length - position;
        }
        if (numberOfBytesToRead == 0)
        {
            return string.Empty;
        }

        return ExecuteConversionToType(array, ref position, numberOfBytesToRead, (bytes, _) => Encoding.ASCII.GetString(bytes));
    }

    /// <summary>
    ///     Converts parts of a byte array into an ASCII String.
    /// </summary>
    /// <param name="array">The byte array. Cannot be null.</param>
    /// <param name="position">The position within the array.</param>
    /// <param name="numberOfBytesToRead">Number of characters of the expected string</param>
    /// <returns>Converted value.</returns>
    public static string ToAsciiString(this byte[] array, int position = 0, int numberOfBytesToRead = -1)
    {
        return ToAsciiString(array, ref position, numberOfBytesToRead);
    }

    /// <summary>
    ///     Converts parts of a byte array into an ASCII String.
    /// </summary>
    /// <param name="array">The byte array. Cannot be null.</param>
    /// <param name="position">The position within the array. Byref : is auto-incremented by the size of the output type.</param>
    /// <param name="numberOfBytesToRead">Number of characters of the expected string</param>
    /// <param name="defaultValue">Default value for that type, can be overriden.</param>
    /// <returns>Converted value.</returns>
    public static string ToAsciiStringOrDefault(this byte[] array, ref int position, int numberOfBytesToRead = -1, string defaultValue = "")
    {
        ArgumentNullException.ThrowIfNull(array);
        if (numberOfBytesToRead == -1)
        {
            numberOfBytesToRead = array.Length - position;
        }
        if (numberOfBytesToRead == 0)
        {
            return string.Empty;
        }

        return ExecuteConversionToTypeOrDefault(array, ref position, numberOfBytesToRead, (bytes, _) => Encoding.ASCII.GetString(bytes), defaultValue);
    }

    /// <summary>
    ///     Converts parts of a byte array into an ASCII String.
    /// </summary>
    /// <param name="array">The byte array. Cannot be null.</param>
    /// <param name="position">The position within the array.</param>
    /// <param name="numberOfBytesToRead">Number of characters of the expected string</param>
    /// <param name="defaultValue">Default value for that type, can be overriden.</param>
    /// <returns>Converted value.</returns>
    public static string ToAsciiStringOrDefault(this byte[] array, int position = 0, int numberOfBytesToRead = -1, string defaultValue = "")
    {
        return ToAsciiStringOrDefault(array, ref position, numberOfBytesToRead, defaultValue);
    }

    /// <summary>
    ///     Converts a byte array to a hex string with customizable formatting.
    /// </summary>
    /// <param name="array">The byte array to convert.</param>
    /// <param name="separator">The separator between hex values. Default is empty string.</param>
    /// <param name="prefix">The prefix for each hex value. Default is empty string.</param>
    /// <param name="upperCase">Whether to use uppercase hex digits. Default is true.</param>
    /// <returns>A hex string representation of the byte array.</returns>
    public static string ToHexString(this byte[] array, string separator = "", string prefix = "", bool upperCase = true)
    {
        ArgumentNullException.ThrowIfNull(array);
        if (array.Length == 0)
        {
            return string.Empty;
        }

        var format = upperCase ? "X2" : "x2";
        var hexStrings = array.Select(b => prefix + b.ToString(format, System.Globalization.CultureInfo.InvariantCulture));
        return string.Join(separator, hexStrings);
    }

    /// <summary>
    ///     Converts a hex string back to a byte array.
    /// </summary>
    /// <param name="hexString">The hex string to convert.</param>
    /// <returns>A byte array representation of the hex string.</returns>
    /// <exception cref="ArgumentException">Thrown when the hex string is invalid.</exception>
    public static byte[] FromHexString(string hexString)
    {
        if (string.IsNullOrWhiteSpace(hexString))
        {
            return [];
        }

        // Remove common separators and prefixes
        hexString = hexString.Replace(" ", "", StringComparison.Ordinal)
                             .Replace("-", "", StringComparison.Ordinal)
                             .Replace(":", "", StringComparison.Ordinal)
                             .Replace("0x", "", StringComparison.OrdinalIgnoreCase)
                             .Replace("0X", "", StringComparison.OrdinalIgnoreCase);

        if (hexString.Length % 2 != 0)
        {
            throw new ArgumentException("Hex string length must be even.", nameof(hexString));
        }

        var result = new byte[hexString.Length / 2];
        for (var i = 0; i < result.Length; i++)
        {
            var hexPair = hexString.Substring(i * 2, 2);
            if (!byte.TryParse(hexPair, System.Globalization.NumberStyles.HexNumber, null, out result[i]))
            {
                throw new ArgumentException($"Invalid hex string: {hexPair}", nameof(hexString));
            }
        }

        return result;
    }

    /// <summary>
    ///     Converts a byte array to a Base64 string.
    /// </summary>
    /// <param name="array">The byte array to convert.</param>
    /// <returns>A Base64 string representation.</returns>
    public static string ToBase64String(this byte[] array)
    {
        ArgumentNullException.ThrowIfNull(array);
        return array.Length == 0 ? string.Empty : Convert.ToBase64String(array);
    }

    /// <summary>
    ///     Converts a Base64 string to a byte array.
    /// </summary>
    /// <param name="base64String">The Base64 string to convert.</param>
    /// <returns>A byte array representation.</returns>
    /// <exception cref="FormatException">Thrown when the Base64 string is invalid.</exception>
    public static byte[] FromBase64String(string base64String)
    {
        return string.IsNullOrWhiteSpace(base64String) ? [] : Convert.FromBase64String(base64String);
    }

    #endregion

}
