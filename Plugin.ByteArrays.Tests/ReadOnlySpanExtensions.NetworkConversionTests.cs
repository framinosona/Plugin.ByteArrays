using FluentAssertions;
using System.Net;
using Xunit;

namespace Plugin.ByteArrays.Tests;

public class ReadOnlySpanExtensions_NetworkConversionTests
{
    [Fact]
    public void ToIPv4Address_Works()
    {
        var expected = IPAddress.Parse("192.168.1.1");
        var bytes = expected.GetAddressBytes();
        ReadOnlySpan<byte> span = bytes;
        var p = 0;
        span.ToIPv4Address(ref p).Should().Be(expected);
        p.Should().Be(4);

        // Test non-ref overload
        span.ToIPv4Address().Should().Be(expected);
        span.ToIPv4Address(0).Should().Be(expected);
    }

    [Fact]
    public void ToIPv6Address_Works()
    {
        var expected = IPAddress.Parse("2001:0db8:85a3:0000:0000:8a2e:0370:7334");
        var bytes = expected.GetAddressBytes();
        ReadOnlySpan<byte> span = bytes;
        var p = 0;
        span.ToIPv6Address(ref p).Should().Be(expected);
        p.Should().Be(16);

        // Test non-ref overload
        span.ToIPv6Address().Should().Be(expected);
        span.ToIPv6Address(0).Should().Be(expected);
    }

    [Fact]
    public void ToIPv4AddressOrDefault_OutOfBounds_ReturnsDefault()
    {
        ReadOnlySpan<byte> span = new byte[] { 1, 2 }; // Only 2 bytes, need 4
        var p = 0;
        var defaultValue = IPAddress.Parse("127.0.0.1");
        span.ToIPv4AddressOrDefault(ref p, defaultValue).Should().Be(defaultValue);
        p.Should().Be(0);
    }

    [Fact]
    public void ToIPv6AddressOrDefault_OutOfBounds_ReturnsDefault()
    {
        ReadOnlySpan<byte> span = new byte[] { 1, 2, 3, 4 }; // Only 4 bytes, need 16
        var p = 0;
        var defaultValue = IPAddress.IPv6Loopback;
        span.ToIPv6AddressOrDefault(ref p, defaultValue).Should().Be(defaultValue);
        p.Should().Be(0);
    }

    [Fact]
    public void ToUInt16NetworkOrder_Works()
    {
        ushort expected = 0x1234;
        var networkOrderBytes = new byte[] { 0x12, 0x34 }; // Big-endian
        ReadOnlySpan<byte> span = networkOrderBytes;
        var p = 0;
        
        if (BitConverter.IsLittleEndian)
        {
            // On little-endian systems, conversion should swap bytes
            span.ToUInt16NetworkOrder(ref p).Should().Be(expected);
        }
        else
        {
            // On big-endian systems, no conversion needed
            span.ToUInt16NetworkOrder(ref p).Should().Be(expected);
        }
        p.Should().Be(2);
    }

    [Fact]
    public void ToUInt32NetworkOrder_Works()
    {
        uint expected = 0x12345678;
        var networkOrderBytes = new byte[] { 0x12, 0x34, 0x56, 0x78 }; // Big-endian
        ReadOnlySpan<byte> span = networkOrderBytes;
        var p = 0;
        
        if (BitConverter.IsLittleEndian)
        {
            span.ToUInt32NetworkOrder(ref p).Should().Be(expected);
        }
        else
        {
            span.ToUInt32NetworkOrder(ref p).Should().Be(expected);
        }
        p.Should().Be(4);
    }

    [Fact]
    public void ToUInt64NetworkOrder_Works()
    {
        ulong expected = 0x123456789ABCDEF0;
        var networkOrderBytes = new byte[] { 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0 }; // Big-endian
        ReadOnlySpan<byte> span = networkOrderBytes;
        var p = 0;
        
        if (BitConverter.IsLittleEndian)
        {
            span.ToUInt64NetworkOrder(ref p).Should().Be(expected);
        }
        else
        {
            span.ToUInt64NetworkOrder(ref p).Should().Be(expected);
        }
        p.Should().Be(8);
    }

    [Fact]
    public void IPv4_Loopback_Works()
    {
        var expected = IPAddress.Loopback;
        var bytes = expected.GetAddressBytes();
        ReadOnlySpan<byte> span = bytes;
        span.ToIPv4Address().Should().Be(expected);
    }

    [Fact]
    public void IPv6_Loopback_Works()
    {
        var expected = IPAddress.IPv6Loopback;
        var bytes = expected.GetAddressBytes();
        ReadOnlySpan<byte> span = bytes;
        span.ToIPv6Address().Should().Be(expected);
    }
}
