using FluentAssertions;
using Xunit;

namespace Plugin.ByteArrays.Tests;

public class ReadOnlySpanExtensions_IntegerConversionTests
{
    [Fact]
    public void ToInt16_Works()
    {
        var expected = (short)-12345;
        var bytes = BitConverter.GetBytes(expected);
        ReadOnlySpan<byte> span = bytes;
        var p = 0;
        span.ToInt16(ref p).Should().Be(expected);
        p.Should().Be(sizeof(short));

        // Test non-ref overload
        span.ToInt16().Should().Be(expected);
        span.ToInt16(0).Should().Be(expected);
    }

    [Fact]
    public void ToUInt16_Works()
    {
        var expected = (ushort)65535;
        var bytes = BitConverter.GetBytes(expected);
        ReadOnlySpan<byte> span = bytes;
        var p = 0;
        span.ToUInt16(ref p).Should().Be(expected);
        p.Should().Be(sizeof(ushort));

        // Test non-ref overload
        span.ToUInt16().Should().Be(expected);
        span.ToUInt16(0).Should().Be(expected);
    }

    [Fact]
    public void ToInt32_Works()
    {
        var expected = 0x1234_5678;
        var bytes = BitConverter.GetBytes(expected);
        ReadOnlySpan<byte> span = bytes;
        var p = 0;
        span.ToInt32(ref p).Should().Be(expected);
        p.Should().Be(sizeof(int));

        // Test non-ref overload
        span.ToInt32().Should().Be(expected);
        span.ToInt32(0).Should().Be(expected);
    }

    [Fact]
    public void ToUInt32_Works()
    {
        var expected = 0x89ABCDEFu;
        var bytes = BitConverter.GetBytes(expected);
        ReadOnlySpan<byte> span = bytes;
        var p = 0;
        span.ToUInt32(ref p).Should().Be(expected);
        p.Should().Be(sizeof(uint));

        // Test non-ref overload
        span.ToUInt32().Should().Be(expected);
        span.ToUInt32(0).Should().Be(expected);
    }

    [Fact]
    public void ToInt64_Works()
    {
        var expected = 0x123456789ABCDEF0L;
        var bytes = BitConverter.GetBytes(expected);
        ReadOnlySpan<byte> span = bytes;
        var p = 0;
        span.ToInt64(ref p).Should().Be(expected);
        p.Should().Be(sizeof(long));

        // Test non-ref overload
        span.ToInt64().Should().Be(expected);
        span.ToInt64(0).Should().Be(expected);
    }

    [Fact]
    public void ToUInt64_Works()
    {
        var expected = 0xFEDCBA9876543210UL;
        var bytes = BitConverter.GetBytes(expected);
        ReadOnlySpan<byte> span = bytes;
        var p = 0;
        span.ToUInt64(ref p).Should().Be(expected);
        p.Should().Be(sizeof(ulong));

        // Test non-ref overload
        span.ToUInt64().Should().Be(expected);
        span.ToUInt64(0).Should().Be(expected);
    }

    [Fact]
    public void Integer_Sequential_Reading()
    {
        // Build a span with multiple integer values
        var builder = new List<byte>();
        builder.AddRange(BitConverter.GetBytes((short)100));
        builder.AddRange(BitConverter.GetBytes((int)1000));
        builder.AddRange(BitConverter.GetBytes((long)10000));

        ReadOnlySpan<byte> span = builder.ToArray();
        var p = 0;

        span.ToInt16(ref p).Should().Be(100);
        p.Should().Be(2);

        span.ToInt32(ref p).Should().Be(1000);
        p.Should().Be(6);

        span.ToInt64(ref p).Should().Be(10000);
        p.Should().Be(14);
    }

    [Fact]
    public void ToInt16OrDefault_OutOfBounds_ReturnsDefault()
    {
        ReadOnlySpan<byte> span = new byte[] { 1 }; // Only 1 byte, need 2
        var p = 0;
        span.ToInt16OrDefault(ref p, -1).Should().Be(-1);
        p.Should().Be(0); // Position not advanced on failure
    }

    [Fact]
    public void ToInt32OrDefault_OutOfBounds_ReturnsDefault()
    {
        ReadOnlySpan<byte> span = new byte[] { 1, 2 }; // Only 2 bytes, need 4
        var p = 0;
        span.ToInt32OrDefault(ref p, 999).Should().Be(999);
        p.Should().Be(0);
    }

    [Fact]
    public void ToInt64OrDefault_OutOfBounds_ReturnsDefault()
    {
        ReadOnlySpan<byte> span = new byte[] { 1, 2, 3, 4 }; // Only 4 bytes, need 8
        var p = 0;
        span.ToInt64OrDefault(ref p, -999).Should().Be(-999);
        p.Should().Be(0);
    }

    [Fact]
    public void ToUInt16OrDefault_Success_AdvancesPosition()
    {
        var expected = (ushort)1234;
        var bytes = BitConverter.GetBytes(expected);
        ReadOnlySpan<byte> span = bytes;
        var p = 0;
        span.ToUInt16OrDefault(ref p, 0).Should().Be(expected);
        p.Should().Be(2);
    }

    [Fact]
    public void ToUInt32OrDefault_Success_AdvancesPosition()
    {
        var expected = 0xABCDEF12u;
        var bytes = BitConverter.GetBytes(expected);
        ReadOnlySpan<byte> span = bytes;
        var p = 0;
        span.ToUInt32OrDefault(ref p, 0).Should().Be(expected);
        p.Should().Be(4);
    }

    [Fact]
    public void ToUInt64OrDefault_Success_AdvancesPosition()
    {
        var expected = 0xFEDCBA9876543210UL;
        var bytes = BitConverter.GetBytes(expected);
        ReadOnlySpan<byte> span = bytes;
        var p = 0;
        span.ToUInt64OrDefault(ref p, 0).Should().Be(expected);
        p.Should().Be(8);
    }
}
