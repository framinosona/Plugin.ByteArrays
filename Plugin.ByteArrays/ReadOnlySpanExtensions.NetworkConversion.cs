using System.Net;
using System.Net.Sockets;

namespace Plugin.ByteArrays;

/// <summary>
///     Functions for working with ReadOnlySpan&lt;byte&gt; providing zero-allocation operations for reading and manipulating byte data.
/// </summary>
public static partial class ReadOnlySpanExtensions
{
    #region Network Conversions

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into an IPv4 address.
    ///     Size = 4 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span. By reference; is auto-incremented by the size of the output type.</param>
    /// <returns>Converted value.</returns>
    public static IPAddress ToIPv4Address(this ReadOnlySpan<byte> span, ref int position)
    {
        const int ipv4Size = 4;
        return ExecuteConversionToType(span, ref position, ipv4Size, s => new IPAddress(s));
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into an IPv4 address.
    ///     Size = 4 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span.</param>
    /// <returns>Converted value.</returns>
    public static IPAddress ToIPv4Address(this ReadOnlySpan<byte> span, int position = 0)
    {
        return ToIPv4Address(span, ref position);
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into an IPv4 address.
    ///     Size = 4 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span. By reference; is auto-incremented by the size of the output type.</param>
    /// <param name="defaultValue">Value returned if out of bounds.</param>
    /// <returns>Converted value.</returns>
    public static IPAddress ToIPv4AddressOrDefault(this ReadOnlySpan<byte> span, ref int position, IPAddress? defaultValue = null)
    {
        const int ipv4Size = 4;
        return ExecuteConversionToTypeOrDefault(span, ref position, ipv4Size, s => new IPAddress(s), defaultValue ?? IPAddress.Any);
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into an IPv4 address.
    ///     Size = 4 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span.</param>
    /// <param name="defaultValue">Default value for that type, can be overridden.</param>
    /// <returns>Converted value.</returns>
    public static IPAddress ToIPv4AddressOrDefault(this ReadOnlySpan<byte> span, int position = 0, IPAddress? defaultValue = null)
    {
        return ToIPv4AddressOrDefault(span, ref position, defaultValue);
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into an IPv6 address.
    ///     Size = 16 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span. By reference; is auto-incremented by the size of the output type.</param>
    /// <returns>Converted value.</returns>
    public static IPAddress ToIPv6Address(this ReadOnlySpan<byte> span, ref int position)
    {
        const int ipv6Size = 16;
        return ExecuteConversionToType(span, ref position, ipv6Size, s => new IPAddress(s));
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into an IPv6 address.
    ///     Size = 16 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span.</param>
    /// <returns>Converted value.</returns>
    public static IPAddress ToIPv6Address(this ReadOnlySpan<byte> span, int position = 0)
    {
        return ToIPv6Address(span, ref position);
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into an IPv6 address.
    ///     Size = 16 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span. By reference; is auto-incremented by the size of the output type.</param>
    /// <param name="defaultValue">Value returned if out of bounds.</param>
    /// <returns>Converted value.</returns>
    public static IPAddress ToIPv6AddressOrDefault(this ReadOnlySpan<byte> span, ref int position, IPAddress? defaultValue = null)
    {
        const int ipv6Size = 16;
        return ExecuteConversionToTypeOrDefault(span, ref position, ipv6Size, s => new IPAddress(s), defaultValue ?? IPAddress.IPv6Any);
    }

    /// <summary>
    ///     Converts the ReadOnlySpan&lt;byte&gt; into an IPv6 address.
    ///     Size = 16 bytes.
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span.</param>
    /// <param name="defaultValue">Default value for that type, can be overridden.</param>
    /// <returns>Converted value.</returns>
    public static IPAddress ToIPv6AddressOrDefault(this ReadOnlySpan<byte> span, int position = 0, IPAddress? defaultValue = null)
    {
        return ToIPv6AddressOrDefault(span, ref position, defaultValue);
    }

    /// <summary>
    ///     Converts a 16-bit value from network byte order (big-endian) to host byte order (little-endian on most systems).
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span. By reference; is auto-incremented by the size of the output type.</param>
    /// <returns>Converted value in host byte order.</returns>
    public static ushort ToUInt16NetworkOrder(this ReadOnlySpan<byte> span, ref int position)
    {
        return ExecuteConversionToType(span, ref position, sizeof(ushort), s =>
        {
            var value = BitConverter.ToUInt16(s);
            return (ushort)IPAddress.NetworkToHostOrder((short)value);
        });
    }

    /// <summary>
    ///     Converts a 16-bit value from network byte order (big-endian) to host byte order (little-endian on most systems).
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span.</param>
    /// <returns>Converted value in host byte order.</returns>
    public static ushort ToUInt16NetworkOrder(this ReadOnlySpan<byte> span, int position = 0)
    {
        return ToUInt16NetworkOrder(span, ref position);
    }

    /// <summary>
    ///     Converts a 32-bit value from network byte order (big-endian) to host byte order (little-endian on most systems).
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span. By reference; is auto-incremented by the size of the output type.</param>
    /// <returns>Converted value in host byte order.</returns>
    public static uint ToUInt32NetworkOrder(this ReadOnlySpan<byte> span, ref int position)
    {
        return ExecuteConversionToType(span, ref position, sizeof(uint), s =>
        {
            var value = BitConverter.ToUInt32(s);
            return (uint)IPAddress.NetworkToHostOrder((int)value);
        });
    }

    /// <summary>
    ///     Converts a 32-bit value from network byte order (big-endian) to host byte order (little-endian on most systems).
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span.</param>
    /// <returns>Converted value in host byte order.</returns>
    public static uint ToUInt32NetworkOrder(this ReadOnlySpan<byte> span, int position = 0)
    {
        return ToUInt32NetworkOrder(span, ref position);
    }

    /// <summary>
    ///     Converts a 64-bit value from network byte order (big-endian) to host byte order (little-endian on most systems).
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span. By reference; is auto-incremented by the size of the output type.</param>
    /// <returns>Converted value in host byte order.</returns>
    public static ulong ToUInt64NetworkOrder(this ReadOnlySpan<byte> span, ref int position)
    {
        return ExecuteConversionToType(span, ref position, sizeof(ulong), s =>
        {
            var value = BitConverter.ToUInt64(s);
            return (ulong)IPAddress.NetworkToHostOrder((long)value);
        });
    }

    /// <summary>
    ///     Converts a 64-bit value from network byte order (big-endian) to host byte order (little-endian on most systems).
    /// </summary>
    /// <param name="span">The ReadOnlySpan&lt;byte&gt;.</param>
    /// <param name="position">The position within the span.</param>
    /// <returns>Converted value in host byte order.</returns>
    public static ulong ToUInt64NetworkOrder(this ReadOnlySpan<byte> span, int position = 0)
    {
        return ToUInt64NetworkOrder(span, ref position);
    }

    #endregion
}
