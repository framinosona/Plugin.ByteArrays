using FluentAssertions;
using Xunit;

namespace Plugin.ByteArrays.Tests;

public class ReadOnlySpanExtensions_FloatingPointConversionTests
{
    [Fact]
    public void ToSingle_Works()
    {
        var expected = 3.14159f;
        var bytes = BitConverter.GetBytes(expected);
        ReadOnlySpan<byte> span = bytes;
        var p = 0;
        span.ToSingle(ref p).Should().BeApproximately(expected, 0.00001f);
        p.Should().Be(sizeof(float));

        // Test non-ref overload
        span.ToSingle().Should().BeApproximately(expected, 0.00001f);
        span.ToSingle(0).Should().BeApproximately(expected, 0.00001f);
    }

    [Fact]
    public void ToDouble_Works()
    {
        var expected = 3.141592653589793;
        var bytes = BitConverter.GetBytes(expected);
        ReadOnlySpan<byte> span = bytes;
        var p = 0;
        span.ToDouble(ref p).Should().BeApproximately(expected, 0.000000000001);
        p.Should().Be(sizeof(double));

        // Test non-ref overload
        span.ToDouble().Should().BeApproximately(expected, 0.000000000001);
        span.ToDouble(0).Should().BeApproximately(expected, 0.000000000001);
    }

    [Fact]
    public void ToHalf_Works()
    {
        var expected = (Half)3.5;
        var bytes = BitConverter.GetBytes(expected);
        ReadOnlySpan<byte> span = bytes;
        var p = 0;
        span.ToHalf(ref p).Should().Be(expected);
        p.Should().Be(2);

        // Test non-ref overload
        span.ToHalf().Should().Be(expected);
        span.ToHalf(0).Should().Be(expected);
    }

    [Fact]
    public void ToDecimal_Works()
    {
        var expected = 123456.789m;
        var bits = decimal.GetBits(expected);
        var bytes = new List<byte>();
        foreach (var bit in bits)
        {
            bytes.AddRange(BitConverter.GetBytes(bit));
        }
        ReadOnlySpan<byte> span = bytes.ToArray();
        var p = 0;
        span.ToDecimal(ref p).Should().Be(expected);
        p.Should().Be(16);

        // Test non-ref overload
        span.ToDecimal().Should().Be(expected);
        span.ToDecimal(0).Should().Be(expected);
    }

    [Fact]
    public void FloatingPoint_Sequential_Reading()
    {
        var builder = new List<byte>();
        builder.AddRange(BitConverter.GetBytes(1.5f));
        builder.AddRange(BitConverter.GetBytes(2.5));
        builder.AddRange(BitConverter.GetBytes((Half)3.5));

        ReadOnlySpan<byte> span = builder.ToArray();
        var p = 0;

        span.ToSingle(ref p).Should().BeApproximately(1.5f, 0.00001f);
        p.Should().Be(4);

        span.ToDouble(ref p).Should().BeApproximately(2.5, 0.000000001);
        p.Should().Be(12);

        span.ToHalf(ref p).Should().Be((Half)3.5);
        p.Should().Be(14);
    }

    [Fact]
    public void ToSingleOrDefault_OutOfBounds_ReturnsDefault()
    {
        ReadOnlySpan<byte> span = new byte[] { 1, 2 }; // Only 2 bytes, need 4
        var p = 0;
        span.ToSingleOrDefault(ref p, 99.9f).Should().BeApproximately(99.9f, 0.001f);
        p.Should().Be(0); // Position not advanced on failure
    }

    [Fact]
    public void ToDoubleOrDefault_OutOfBounds_ReturnsDefault()
    {
        ReadOnlySpan<byte> span = new byte[] { 1, 2, 3, 4 }; // Only 4 bytes, need 8
        var p = 0;
        span.ToDoubleOrDefault(ref p, 99.9).Should().BeApproximately(99.9, 0.001);
        p.Should().Be(0);
    }

    [Fact]
    public void ToHalfOrDefault_OutOfBounds_ReturnsDefault()
    {
        ReadOnlySpan<byte> span = new byte[] { 1 }; // Only 1 byte, need 2
        var p = 0;
        span.ToHalfOrDefault(ref p, (Half)7.5).Should().Be((Half)7.5);
        p.Should().Be(0);
    }

    [Fact]
    public void ToDecimalOrDefault_OutOfBounds_ReturnsDefault()
    {
        ReadOnlySpan<byte> span = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }; // Only 8 bytes, need 16
        var p = 0;
        span.ToDecimalOrDefault(ref p, 123.45m).Should().Be(123.45m);
        p.Should().Be(0);
    }

    [Fact]
    public void ToSingle_NegativeValue_Works()
    {
        var expected = -3.14159f;
        var bytes = BitConverter.GetBytes(expected);
        ReadOnlySpan<byte> span = bytes;
        span.ToSingle().Should().BeApproximately(expected, 0.00001f);
    }

    [Fact]
    public void ToDouble_NegativeValue_Works()
    {
        var expected = -3.141592653589793;
        var bytes = BitConverter.GetBytes(expected);
        ReadOnlySpan<byte> span = bytes;
        span.ToDouble().Should().BeApproximately(expected, 0.000000000001);
    }

    [Fact]
    public void ToDecimal_NegativeValue_Works()
    {
        var expected = -123456.789m;
        var bits = decimal.GetBits(expected);
        var bytes = new List<byte>();
        foreach (var bit in bits)
        {
            bytes.AddRange(BitConverter.GetBytes(bit));
        }
        ReadOnlySpan<byte> span = bytes.ToArray();
        span.ToDecimal().Should().Be(expected);
    }
}
