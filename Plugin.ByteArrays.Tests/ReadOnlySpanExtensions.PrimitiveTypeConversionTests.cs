using FluentAssertions;
using Xunit;

namespace Plugin.ByteArrays.Tests;

public class ReadOnlySpanExtensions_PrimitiveTypeConversionTests
{
    [Fact]
    public void ToBoolean_True_Works()
    {
        ReadOnlySpan<byte> span = new byte[] { 1 };
        var p = 0;
        span.ToBoolean(ref p).Should().BeTrue();
        p.Should().Be(sizeof(bool));

        // Test non-ref overload
        span.ToBoolean().Should().BeTrue();
        span.ToBoolean(0).Should().BeTrue();
    }

    [Fact]
    public void ToBoolean_False_Works()
    {
        ReadOnlySpan<byte> span = new byte[] { 0 };
        var p = 0;
        span.ToBoolean(ref p).Should().BeFalse();
        p.Should().Be(sizeof(bool));
    }

    [Fact]
    public void ToBooleanOrDefault_OutOfBounds_ReturnsDefault()
    {
        ReadOnlySpan<byte> span = ReadOnlySpan<byte>.Empty;
        var p = 0;
        span.ToBooleanOrDefault(ref p, true).Should().BeTrue();
        p.Should().Be(0); // Position not advanced on failure
    }

    [Fact]
    public void ToByte_Works()
    {
        byte expected = 42;
        ReadOnlySpan<byte> span = new byte[] { expected };
        var p = 0;
        span.ToByte(ref p).Should().Be(expected);
        p.Should().Be(sizeof(byte));

        // Test non-ref overload
        span.ToByte().Should().Be(expected);
        span.ToByte(0).Should().Be(expected);
    }

    [Fact]
    public void ToByte_MaxValue_Works()
    {
        byte expected = 255;
        ReadOnlySpan<byte> span = new byte[] { expected };
        span.ToByte().Should().Be(expected);
    }

    [Fact]
    public void ToByteOrDefault_OutOfBounds_ReturnsDefault()
    {
        ReadOnlySpan<byte> span = ReadOnlySpan<byte>.Empty;
        var p = 0;
        span.ToByteOrDefault(ref p, 99).Should().Be(99);
        p.Should().Be(0);
    }

    [Fact]
    public void ToSByte_Positive_Works()
    {
        sbyte expected = 42;
        ReadOnlySpan<byte> span = new byte[] { (byte)expected };
        var p = 0;
        span.ToSByte(ref p).Should().Be(expected);
        p.Should().Be(sizeof(sbyte));

        // Test non-ref overload
        span.ToSByte().Should().Be(expected);
        span.ToSByte(0).Should().Be(expected);
    }

    [Fact]
    public void ToSByte_Negative_Works()
    {
        sbyte expected = -42;
        ReadOnlySpan<byte> span = new byte[] { (byte)expected };
        span.ToSByte().Should().Be(expected);
    }

    [Fact]
    public void ToSByteOrDefault_OutOfBounds_ReturnsDefault()
    {
        ReadOnlySpan<byte> span = ReadOnlySpan<byte>.Empty;
        var p = 0;
        span.ToSByteOrDefault(ref p, -1).Should().Be(-1);
        p.Should().Be(0);
    }

    [Fact]
    public void ToChar_Works()
    {
        char expected = 'A';
        var bytes = BitConverter.GetBytes(expected);
        ReadOnlySpan<byte> span = bytes;
        var p = 0;
        span.ToChar(ref p).Should().Be(expected);
        p.Should().Be(sizeof(char));

        // Test non-ref overload
        span.ToChar().Should().Be(expected);
        span.ToChar(0).Should().Be(expected);
    }

    [Fact]
    public void ToChar_UnicodeCharacter_Works()
    {
        char expected = '€'; // Euro sign
        var bytes = BitConverter.GetBytes(expected);
        ReadOnlySpan<byte> span = bytes;
        span.ToChar().Should().Be(expected);
    }

    [Fact]
    public void ToCharOrDefault_OutOfBounds_ReturnsDefault()
    {
        ReadOnlySpan<byte> span = new byte[] { 65 }; // Only 1 byte, need 2
        var p = 0;
        span.ToCharOrDefault(ref p, 'X').Should().Be('X');
        p.Should().Be(0);
    }

    [Fact]
    public void SequentialReading_Works()
    {
        // Build a span with multiple values
        var bytes = new byte[4];
        bytes[0] = 1; // bool true
        bytes[1] = 42; // byte
        bytes[2] = 255; // byte
        bytes[3] = 0; // bool false

        ReadOnlySpan<byte> span = bytes;
        var p = 0;

        span.ToBoolean(ref p).Should().BeTrue();
        p.Should().Be(1);

        span.ToByte(ref p).Should().Be(42);
        p.Should().Be(2);

        span.ToByte(ref p).Should().Be(255);
        p.Should().Be(3);

        span.ToBoolean(ref p).Should().BeFalse();
        p.Should().Be(4);
    }

    [Fact]
    public void ToType_InvalidPosition_Throws()
    {
        var bytes = new byte[] { 1, 2, 3 };
        ReadOnlySpan<byte> span = bytes;
        var p = -1;

        try
        {
            span.ToByte(ref p);
            Assert.Fail("Expected ArgumentOutOfRangeException");
        }
        catch (ArgumentOutOfRangeException ex)
        {
            ex.ParamName.Should().Be("position");
        }
    }

    [Fact]
    public void ToType_PositionBeyondEnd_Throws()
    {
        var bytes = new byte[] { 1, 2, 3 };
        ReadOnlySpan<byte> span = bytes;
        var p = 10;

        try
        {
            span.ToByte(ref p);
            Assert.Fail("Expected ArgumentOutOfRangeException");
        }
        catch (ArgumentOutOfRangeException ex)
        {
            ex.ParamName.Should().Be("position");
        }
    }

    [Fact]
    public void ToType_NotEnoughBytesRemaining_Throws()
    {
        var bytes = new byte[] { 1, 2 };
        ReadOnlySpan<byte> span = bytes;
        var p = 1;

        // Try to read 2-byte char from position 1 (only 1 byte available)
        try
        {
            span.ToChar(ref p);
            Assert.Fail("Expected ArgumentOutOfRangeException");
        }
        catch (ArgumentOutOfRangeException ex)
        {
            ex.ParamName.Should().Be("position");
        }
    }
}
