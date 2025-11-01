using FluentAssertions;
using System.Linq;
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

    [Fact]
    public void ToInt16NetworkOrder_Works()
    {
        short expected = 0x1234;
        var networkOrderBytes = new byte[] { 0x12, 0x34 }; // Big-endian
        ReadOnlySpan<byte> span = networkOrderBytes;
        var p = 0;
        
        span.ToInt16NetworkOrder(ref p).Should().Be(expected);
        p.Should().Be(2);
        
        // Test non-ref overload
        span.ToInt16NetworkOrder().Should().Be(expected);
    }

    [Fact]
    public void ToInt32NetworkOrder_Works()
    {
        int expected = 0x12345678;
        var networkOrderBytes = new byte[] { 0x12, 0x34, 0x56, 0x78 }; // Big-endian
        ReadOnlySpan<byte> span = networkOrderBytes;
        var p = 0;
        
        span.ToInt32NetworkOrder(ref p).Should().Be(expected);
        p.Should().Be(4);
        
        // Test non-ref overload
        span.ToInt32NetworkOrder().Should().Be(expected);
    }

    [Fact]
    public void ToInt64NetworkOrder_Works()
    {
        long expected = 0x123456789ABCDEF0;
        var networkOrderBytes = new byte[] { 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0 }; // Big-endian
        ReadOnlySpan<byte> span = networkOrderBytes;
        var p = 0;
        
        span.ToInt64NetworkOrder(ref p).Should().Be(expected);
        p.Should().Be(8);
        
        // Test non-ref overload
        span.ToInt64NetworkOrder().Should().Be(expected);
    }

    [Fact]
    public void ToInt16NetworkOrder_NegativeValue_Works()
    {
        short expected = -12345;
        var bytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(expected));
        ReadOnlySpan<byte> span = bytes;
        
        span.ToInt16NetworkOrder().Should().Be(expected);
    }

    [Fact]
    public void ToInt32NetworkOrder_NegativeValue_Works()
    {
        int expected = -1234567890;
        var bytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(expected));
        ReadOnlySpan<byte> span = bytes;
        
        span.ToInt32NetworkOrder().Should().Be(expected);
    }

    [Fact]
    public void ToInt64NetworkOrder_NegativeValue_Works()
    {
        long expected = -1234567890123456789;
        var bytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(expected));
        ReadOnlySpan<byte> span = bytes;
        
        span.ToInt64NetworkOrder().Should().Be(expected);
    }

    [Fact]
    public void ToIPv4EndPoint_Works()
    {
        var expectedAddress = IPAddress.Parse("192.168.1.1");
        ushort expectedPort = 8080;
        var addressBytes = expectedAddress.GetAddressBytes();
        var portBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)expectedPort));
        var bytes = addressBytes.Concat(portBytes).ToArray();
        ReadOnlySpan<byte> span = bytes;
        var p = 0;
        
        var result = span.ToIPv4EndPoint(ref p);
        result.Address.Should().Be(expectedAddress);
        result.Port.Should().Be(expectedPort);
        p.Should().Be(6);
        
        // Test non-ref overload
        var result2 = span.ToIPv4EndPoint();
        result2.Address.Should().Be(expectedAddress);
        result2.Port.Should().Be(expectedPort);
    }

    [Fact]
    public void ToIPv6EndPoint_Works()
    {
        var expectedAddress = IPAddress.Parse("2001:0db8:85a3::8a2e:0370:7334");
        ushort expectedPort = 443;
        var addressBytes = expectedAddress.GetAddressBytes();
        var portBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)expectedPort));
        var bytes = addressBytes.Concat(portBytes).ToArray();
        ReadOnlySpan<byte> span = bytes;
        var p = 0;
        
        var result = span.ToIPv6EndPoint(ref p);
        result.Address.Should().Be(expectedAddress);
        result.Port.Should().Be(expectedPort);
        p.Should().Be(18);
        
        // Test non-ref overload
        var result2 = span.ToIPv6EndPoint();
        result2.Address.Should().Be(expectedAddress);
        result2.Port.Should().Be(expectedPort);
    }

    [Fact]
    public void ToIPv4EndPointOrDefault_OutOfBounds_ReturnsDefault()
    {
        ReadOnlySpan<byte> span = new byte[] { 1, 2, 3 }; // Only 3 bytes, need 6
        var p = 0;
        var defaultValue = new IPEndPoint(IPAddress.Loopback, 9999);
        
        var result = span.ToIPv4EndPointOrDefault(ref p, defaultValue);
        result.Should().Be(defaultValue);
        p.Should().Be(0);
        
        // Test non-ref overload
        var result2 = span.ToIPv4EndPointOrDefault(0, defaultValue);
        result2.Should().Be(defaultValue);
    }

    [Fact]
    public void ToIPv6EndPointOrDefault_OutOfBounds_ReturnsDefault()
    {
        ReadOnlySpan<byte> span = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }; // Only 8 bytes, need 18
        var p = 0;
        var defaultValue = new IPEndPoint(IPAddress.IPv6Loopback, 8888);
        
        var result = span.ToIPv6EndPointOrDefault(ref p, defaultValue);
        result.Should().Be(defaultValue);
        p.Should().Be(0);
        
        // Test non-ref overload
        var result2 = span.ToIPv6EndPointOrDefault(0, defaultValue);
        result2.Should().Be(defaultValue);
    }
}
