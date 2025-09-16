using System.Runtime.InteropServices;

namespace Plugin.ByteArrays;

/// <summary>
///     Functions converting other types into a byte array.
/// </summary>
public static partial class ByteArrayExtensions
{
    #region Complex Type Conversions

    /// <summary>
    ///     Converts the full byte array into a Version.
    /// </summary>
    /// <param name="array">The byte array. Cannot be null.</param>
    /// <param name="position">The position within the array. Byref : is auto-incremented by the size of the output type.</param>
    /// <param name="numberOfBytesToRead">Number of characters of the expected string</param>
    /// <returns>Converted value.</returns>
    public static Version ToVersion(this byte[] array, ref int position, int numberOfBytesToRead = -1)
    {
        return Version.Parse(array.ToUtf8String(ref position, numberOfBytesToRead));
    }

    /// <summary>
    ///     Converts the full byte array into a Version.
    /// </summary>
    /// <param name="array">The byte array. Cannot be null.</param>
    /// <param name="position">The position within the array.</param>
    /// <param name="numberOfBytesToRead">Number of characters of the expected string</param>
    /// <returns>Converted value.</returns>
    public static Version ToVersion(this byte[] array, int position = 0, int numberOfBytesToRead = -1)
    {
        return ToVersion(array, ref position, numberOfBytesToRead);
    }

    /// <summary>
    ///     Converts the full byte array into a Version.
    /// </summary>
    /// <param name="array">The byte array. Cannot be null.</param>
    /// <param name="position">The position within the array. Byref : is auto-incremented by the size of the output type.</param>
    /// <param name="defaultValue">Default value for that type, can be overriden.</param>
    /// <param name="numberOfBytesToRead">Number of characters of the expected string</param>
    /// <returns>Converted value.</returns>
    public static Version ToVersionOrDefault(this byte[] array, ref int position, int numberOfBytesToRead = -1, Version? defaultValue = null)
    {
        defaultValue ??= new Version(0, 0, 0);
        var utf8String = array.ToUtf8StringOrDefault(ref position, numberOfBytesToRead, defaultValue.ToString());
        try
        {
            return Version.Parse(utf8String);
        }
        catch (FormatException)
        {
            // If parsing fails, return the default value
            return defaultValue;
        }
    }

    /// <summary>
    ///     Converts the full byte array into a Version.
    /// </summary>
    /// <param name="array">The byte array. Cannot be null.</param>
    /// <param name="position">The position within the array.</param>
    /// <param name="defaultValue">Default value for that type, can be overriden.</param>
    /// <param name="numberOfBytesToRead">Number of characters of the expected string</param>
    /// <returns>Converted value.</returns>
    public static Version ToVersionOrDefault(this byte[] array, int position = 0, int numberOfBytesToRead = -1, Version? defaultValue = null)
    {
        return ToVersionOrDefault(array, ref position, numberOfBytesToRead, defaultValue);
    }

    /// <summary>
    ///     Converts the byte array into a given Enum.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array. Byref : is auto-incremented by the size of the output type.</param>
    /// <typeparam name="T">Type of Enum.</typeparam>
    /// <returns>Converted value.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the array is null.</exception>
    public static T ToEnum<T>(this byte[] array, ref int position) where T : Enum
    {
        ArgumentNullException.ThrowIfNull(array);
        if (position < 0)
        {
            throw new ArgumentException($"Position must be greater than or equal to 0. Was {position}", nameof(position));
        }
        var outputType = typeof(T).IsEnum ? Enum.GetUnderlyingType(typeof(T)) : typeof(T);
        var size = Marshal.SizeOf(outputType);
        if (array.Length < position + size)
        {
            throw new ArgumentException($"Array {array.ToDebugString()} is too small. Reading {size} bytes ({typeof(T).Name}) from position {position} is not possible in array of {array.Length}", nameof(position));
        }

        ulong enumArrayValue = size switch
        {
            1 => array.ToByte(position),
            2 => array.ToUInt16(position),
            4 => array.ToUInt32(position),
            8 => array.ToUInt64(position),
            var _ => 0,
        };

        T output;
        if (typeof(T).IsDefined(typeof(FlagsAttribute), false))
        {
            // For [Flags] enums, ensure all bits in enumArrayValue are valid
            var allDefinedBits = Convert.ToUInt64(Enum.GetValues(typeof(T)).Cast<object>().Aggregate(0UL, (acc, v) => acc | Convert.ToUInt64(v, System.Globalization.CultureInfo.InvariantCulture)));
            if ((enumArrayValue & ~allDefinedBits) != 0)
            {
                throw new ArgumentException($"Value {enumArrayValue} contains bits not defined in [Flags] enum {typeof(T).Name}. Valid bits: 0x{allDefinedBits:X}");
            }
            output = (T)Enum.ToObject(typeof(T), enumArrayValue);
        }
        else
        {
            var validValues = Enum.GetValues(typeof(T));
            output = (T)Enum.ToObject(typeof(T), enumArrayValue);
            if (!validValues.Cast<T>().Contains(output))
            {
                throw new ArgumentException($"Value {enumArrayValue} is not a valid value for enum {typeof(T).Name}. Valid values are: {string.Join(", ", validValues.Cast<T>())}");
            }
        }
        position += size;
        return output;
    }

    /// <summary>
    ///     Converts the byte array into a given Enum.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array.</param>
    /// <typeparam name="T">Type of Enum.</typeparam>
    /// <returns>Converted value.</returns>
    public static T ToEnum<T>(this byte[] array, int position = 0) where T : Enum
    {
        return ToEnum<T>(array, ref position);
    }

    /// <summary>
    ///     Converts the byte array into a given Enum.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array. Byref : is auto-incremented by the size of the output type.</param>
    /// <param name="defaultValue">Default value for that type, can be overriden.</param>
    /// <typeparam name="T">Type of Enum.</typeparam>
    /// <returns>Converted value.</returns>
    public static T ToEnumOrDefault<T>(this byte[] array, ref int position, T? defaultValue = default) where T : Enum
    {
        defaultValue ??= Enum.GetValues(typeof(T)).Cast<T>().First();
        try
        {
            return ToEnum<T>(array, ref position);
        }
        catch (IndexOutOfRangeException)
        {
            // If reading fails, return the default value
            return defaultValue;
        }
        catch (ArgumentException)
        {
            // If conversion fails, return the default value
            return defaultValue;
        }
    }

    /// <summary>
    ///     Converts the byte array into a given Enum.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array.</param>
    /// <param name="defaultValue">Default value for that type, can be overriden.</param>
    /// <typeparam name="T">Type of Enum.</typeparam>
    /// <returns>Converted value.</returns>
    public static T ToEnumOrDefault<T>(this byte[] array, int position = 0, T? defaultValue = default) where T : Enum
    {
        return ToEnumOrDefault(array, ref position, defaultValue);
    }

    #endregion

}
