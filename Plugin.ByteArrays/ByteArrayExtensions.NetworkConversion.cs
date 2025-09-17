using System.Net;

namespace Plugin.ByteArrays;

/// <summary>
///     Functions converting network-related types and big-endian numeric values from byte arrays.
/// </summary>
public static partial class ByteArrayExtensions
{
    #region IP Address Conversions

    /// <summary>
    ///     Converts the byte array into an IPAddress.
    ///     Size = 4 bytes for IPv4, 16 bytes for IPv6.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array. Byref : is auto-incremented by the size of the output type.</param>
    /// <param name="isIPv6">True for IPv6 (16 bytes), false for IPv4 (4 bytes).</param>
    /// <returns>Converted IPAddress value.</returns>
    public static IPAddress ToIPAddress(this byte[] array, ref int position, bool isIPv6 = false)
    {
        ArgumentNullException.ThrowIfNull(array);
        var addressSize = isIPv6 ? 16 : 4;

        if (position < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(position), $"Position must be greater than or equal to 0. Was {position}.");
        }

        if (array.Length < position + addressSize)
        {
            throw new ArgumentOutOfRangeException(nameof(position),
                $"Array {array.ToDebugString()} is too small. Reading {addressSize} bytes from position {position} is not possible in array of {array.Length}");
        }

        var addressBytes = array.Skip(position).Take(addressSize).ToArray();
        var ipAddress = new IPAddress(addressBytes);
        position += addressSize;
        return ipAddress;
    }

    /// <summary>
    ///     Converts the byte array into an IPAddress.
    ///     Size = 4 bytes for IPv4, 16 bytes for IPv6.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array.</param>
    /// <param name="isIPv6">True for IPv6 (16 bytes), false for IPv4 (4 bytes).</param>
    /// <returns>Converted IPAddress value.</returns>
    public static IPAddress ToIPAddress(this byte[] array, int position = 0, bool isIPv6 = false)
    {
        return ToIPAddress(array, ref position, isIPv6);
    }

    /// <summary>
    ///     Converts the byte array into an IPAddress.
    ///     Returns the default value if conversion fails.
    ///     Size = 4 bytes for IPv4, 16 bytes for IPv6.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array. Byref : is auto-incremented by the size of the output type.</param>
    /// <param name="isIPv6">True for IPv6 (16 bytes), false for IPv4 (4 bytes).</param>
    /// <param name="defaultValue">Default value for that type, can be overridden.</param>
    /// <returns>Converted IPAddress value.</returns>
    public static IPAddress ToIPAddressOrDefault(this byte[] array, ref int position, bool isIPv6 = false, IPAddress? defaultValue = null)
    {
        defaultValue ??= isIPv6 ? IPAddress.IPv6Loopback : IPAddress.Loopback;

        try
        {
            return ToIPAddress(array, ref position, isIPv6);
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
    ///     Converts the byte array into an IPAddress.
    ///     Returns the default value if conversion fails.
    ///     Size = 4 bytes for IPv4, 16 bytes for IPv6.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array.</param>
    /// <param name="isIPv6">True for IPv6 (16 bytes), false for IPv4 (4 bytes).</param>
    /// <param name="defaultValue">Default value for that type, can be overridden.</param>
    /// <returns>Converted IPAddress value.</returns>
    public static IPAddress ToIPAddressOrDefault(this byte[] array, int position = 0, bool isIPv6 = false, IPAddress? defaultValue = null)
    {
        return ToIPAddressOrDefault(array, ref position, isIPv6, defaultValue);
    }

    #endregion

    #region IPEndPoint Conversions

    /// <summary>
    ///     Converts the byte array into an IPEndPoint.
    ///     Size = 6 bytes for IPv4 (4 bytes IP + 2 bytes port), 18 bytes for IPv6 (16 bytes IP + 2 bytes port).
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array. Byref : is auto-incremented by the size of the output type.</param>
    /// <param name="isIPv6">True for IPv6, false for IPv4.</param>
    /// <returns>Converted IPEndPoint value.</returns>
    public static IPEndPoint ToIPEndPoint(this byte[] array, ref int position, bool isIPv6 = false)
    {
        var ipAddress = ToIPAddress(array, ref position, isIPv6);
        var port = ExecuteConversionToType(array, ref position, sizeof(ushort),
            (bytes, offset) => BitConverter.IsLittleEndian
                ? BitConverter.ToUInt16(bytes.Reverse().ToArray(), 0) // Convert to big-endian
                : BitConverter.ToUInt16(bytes, offset));

        return new IPEndPoint(ipAddress, port);
    }

    /// <summary>
    ///     Converts the byte array into an IPEndPoint.
    ///     Size = 6 bytes for IPv4 (4 bytes IP + 2 bytes port), 18 bytes for IPv6 (16 bytes IP + 2 bytes port).
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array.</param>
    /// <param name="isIPv6">True for IPv6, false for IPv4.</param>
    /// <returns>Converted IPEndPoint value.</returns>
    public static IPEndPoint ToIPEndPoint(this byte[] array, int position = 0, bool isIPv6 = false)
    {
        return ToIPEndPoint(array, ref position, isIPv6);
    }

    /// <summary>
    ///     Converts the byte array into an IPEndPoint.
    ///     Returns the default value if conversion fails.
    ///     Size = 6 bytes for IPv4 (4 bytes IP + 2 bytes port), 18 bytes for IPv6 (16 bytes IP + 2 bytes port).
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array. Byref : is auto-incremented by the size of the output type.</param>
    /// <param name="isIPv6">True for IPv6, false for IPv4.</param>
    /// <param name="defaultValue">Default value for that type, can be overridden.</param>
    /// <returns>Converted IPEndPoint value.</returns>
    public static IPEndPoint ToIPEndPointOrDefault(this byte[] array, ref int position, bool isIPv6 = false, IPEndPoint? defaultValue = null)
    {
        defaultValue ??= new IPEndPoint(isIPv6 ? IPAddress.IPv6Loopback : IPAddress.Loopback, 0);

        try
        {
            return ToIPEndPoint(array, ref position, isIPv6);
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
    ///     Converts the byte array into an IPEndPoint.
    ///     Returns the default value if conversion fails.
    ///     Size = 6 bytes for IPv4 (4 bytes IP + 2 bytes port), 18 bytes for IPv6 (16 bytes IP + 2 bytes port).
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array.</param>
    /// <param name="isIPv6">True for IPv6, false for IPv4.</param>
    /// <param name="defaultValue">Default value for that type, can be overridden.</param>
    /// <returns>Converted IPEndPoint value.</returns>
    public static IPEndPoint ToIPEndPointOrDefault(this byte[] array, int position = 0, bool isIPv6 = false, IPEndPoint? defaultValue = null)
    {
        return ToIPEndPointOrDefault(array, ref position, isIPv6, defaultValue);
    }

    #endregion

    #region Big-Endian Integer Conversions

    /// <summary>
    ///     Converts the byte array into a 16-bit integer in big-endian format.
    ///     Size = 2 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array. Byref : is auto-incremented by the size of the output type.</param>
    /// <returns>Converted value.</returns>
    public static short ToInt16BigEndian(this byte[] array, ref int position)
    {
        return ExecuteConversionToType(array, ref position, sizeof(short), (bytes, offset) =>
        {
            if (BitConverter.IsLittleEndian)
            {
                var reversed = bytes.Reverse().ToArray();
                return BitConverter.ToInt16(reversed, 0);
            }
            return BitConverter.ToInt16(bytes, offset);
        });
    }

    /// <summary>
    ///     Converts the byte array into a 16-bit integer in big-endian format.
    ///     Size = 2 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array.</param>
    /// <returns>Converted value.</returns>
    public static short ToInt16BigEndian(this byte[] array, int position = 0)
    {
        return ToInt16BigEndian(array, ref position);
    }

    /// <summary>
    ///     Converts the byte array into a 32-bit integer in big-endian format.
    ///     Size = 4 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array. Byref : is auto-incremented by the size of the output type.</param>
    /// <returns>Converted value.</returns>
    public static int ToInt32BigEndian(this byte[] array, ref int position)
    {
        return ExecuteConversionToType(array, ref position, sizeof(int), (bytes, offset) =>
        {
            if (BitConverter.IsLittleEndian)
            {
                var reversed = bytes.Reverse().ToArray();
                return BitConverter.ToInt32(reversed, 0);
            }
            return BitConverter.ToInt32(bytes, offset);
        });
    }

    /// <summary>
    ///     Converts the byte array into a 32-bit integer in big-endian format.
    ///     Size = 4 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array.</param>
    /// <returns>Converted value.</returns>
    public static int ToInt32BigEndian(this byte[] array, int position = 0)
    {
        return ToInt32BigEndian(array, ref position);
    }

    /// <summary>
    ///     Converts the byte array into a 64-bit integer in big-endian format.
    ///     Size = 8 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array. Byref : is auto-incremented by the size of the output type.</param>
    /// <returns>Converted value.</returns>
    public static long ToInt64BigEndian(this byte[] array, ref int position)
    {
        return ExecuteConversionToType(array, ref position, sizeof(long), (bytes, offset) =>
        {
            if (BitConverter.IsLittleEndian)
            {
                var reversed = bytes.Reverse().ToArray();
                return BitConverter.ToInt64(reversed, 0);
            }
            return BitConverter.ToInt64(bytes, offset);
        });
    }

    /// <summary>
    ///     Converts the byte array into a 64-bit integer in big-endian format.
    ///     Size = 8 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array.</param>
    /// <returns>Converted value.</returns>
    public static long ToInt64BigEndian(this byte[] array, int position = 0)
    {
        return ToInt64BigEndian(array, ref position);
    }

    /// <summary>
    ///     Converts the byte array into a 16-bit unsigned integer in big-endian format.
    ///     Size = 2 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array. Byref : is auto-incremented by the size of the output type.</param>
    /// <returns>Converted value.</returns>
    public static ushort ToUInt16BigEndian(this byte[] array, ref int position)
    {
        return ExecuteConversionToType(array, ref position, sizeof(ushort), (bytes, offset) =>
        {
            if (BitConverter.IsLittleEndian)
            {
                var reversed = bytes.Reverse().ToArray();
                return BitConverter.ToUInt16(reversed, 0);
            }
            return BitConverter.ToUInt16(bytes, offset);
        });
    }

    /// <summary>
    ///     Converts the byte array into a 16-bit unsigned integer in big-endian format.
    ///     Size = 2 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array.</param>
    /// <returns>Converted value.</returns>
    public static ushort ToUInt16BigEndian(this byte[] array, int position = 0)
    {
        return ToUInt16BigEndian(array, ref position);
    }

    /// <summary>
    ///     Converts the byte array into a 32-bit unsigned integer in big-endian format.
    ///     Size = 4 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array. Byref : is auto-incremented by the size of the output type.</param>
    /// <returns>Converted value.</returns>
    public static uint ToUInt32BigEndian(this byte[] array, ref int position)
    {
        return ExecuteConversionToType(array, ref position, sizeof(uint), (bytes, offset) =>
        {
            if (BitConverter.IsLittleEndian)
            {
                var reversed = bytes.Reverse().ToArray();
                return BitConverter.ToUInt32(reversed, 0);
            }
            return BitConverter.ToUInt32(bytes, offset);
        });
    }

    /// <summary>
    ///     Converts the byte array into a 32-bit unsigned integer in big-endian format.
    ///     Size = 4 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array.</param>
    /// <returns>Converted value.</returns>
    public static uint ToUInt32BigEndian(this byte[] array, int position = 0)
    {
        return ToUInt32BigEndian(array, ref position);
    }

    /// <summary>
    ///     Converts the byte array into a 64-bit unsigned integer in big-endian format.
    ///     Size = 8 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array. Byref : is auto-incremented by the size of the output type.</param>
    /// <returns>Converted value.</returns>
    public static ulong ToUInt64BigEndian(this byte[] array, ref int position)
    {
        return ExecuteConversionToType(array, ref position, sizeof(ulong), (bytes, offset) =>
        {
            if (BitConverter.IsLittleEndian)
            {
                var reversed = bytes.Reverse().ToArray();
                return BitConverter.ToUInt64(reversed, 0);
            }
            return BitConverter.ToUInt64(bytes, offset);
        });
    }

    /// <summary>
    ///     Converts the byte array into a 64-bit unsigned integer in big-endian format.
    ///     Size = 8 bytes.
    /// </summary>
    /// <param name="array">The byte array.</param>
    /// <param name="position">The position within the array.</param>
    /// <returns>Converted value.</returns>
    public static ulong ToUInt64BigEndian(this byte[] array, int position = 0)
    {
        return ToUInt64BigEndian(array, ref position);
    }

    #endregion
}
