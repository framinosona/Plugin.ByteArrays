using System.Text;
using FluentAssertions;
using Xunit;

namespace Plugin.ByteArrays.Tests;

public class ReadOnlySpanExtensions_StringConversionTests
{
    [Fact]
    public void ToUtf8String_Works()
    {
        var expected = "Hello, World!";
        var bytes = Encoding.UTF8.GetBytes(expected);
        ReadOnlySpan<byte> span = bytes;
        var p = 0;
        span.ToUtf8String(ref p, bytes.Length).Should().Be(expected);
        p.Should().Be(bytes.Length);

        // Test non-ref overload
        span.ToUtf8String(0, bytes.Length).Should().Be(expected);
    }

    [Fact]
    public void ToUtf8String_WithUnicodeCharacters_Works()
    {
        var expected = "Hello 世界 🌍";
        var bytes = Encoding.UTF8.GetBytes(expected);
        ReadOnlySpan<byte> span = bytes;
        span.ToUtf8String().Should().Be(expected);
    }

    [Fact]
    public void ToUtf8String_ReadToEnd_Works()
    {
        var expected = "Test";
        var bytes = Encoding.UTF8.GetBytes(expected);
        ReadOnlySpan<byte> span = bytes;
        var p = 0;
        span.ToUtf8String(ref p, -1).Should().Be(expected);
        p.Should().Be(bytes.Length);
    }

    [Fact]
    public void ToUtf8String_EmptyString_Works()
    {
        ReadOnlySpan<byte> span = new byte[] { 65, 66, 67 };
        var p = 0;
        span.ToUtf8String(ref p, 0).Should().Be(string.Empty);
        p.Should().Be(0);
    }

    [Fact]
    public void ToAsciiString_Works()
    {
        var expected = "Hello ASCII";
        var bytes = Encoding.ASCII.GetBytes(expected);
        ReadOnlySpan<byte> span = bytes;
        var p = 0;
        span.ToAsciiString(ref p, bytes.Length).Should().Be(expected);
        p.Should().Be(bytes.Length);

        // Test non-ref overload
        span.ToAsciiString(0, bytes.Length).Should().Be(expected);
    }

    [Fact]
    public void ToAsciiString_ReadToEnd_Works()
    {
        var expected = "Test";
        var bytes = Encoding.ASCII.GetBytes(expected);
        ReadOnlySpan<byte> span = bytes;
        var p = 0;
        span.ToAsciiString(ref p, -1).Should().Be(expected);
        p.Should().Be(bytes.Length);
    }

    [Fact]
    public void ToHexString_Works()
    {
        ReadOnlySpan<byte> span = new byte[] { 0xAB, 0xCD, 0xEF, 0x12 };
        span.ToHexString().Should().Be("ABCDEF12");
    }

    [Fact]
    public void ToHexString_EmptySpan_ReturnsEmptyString()
    {
        ReadOnlySpan<byte> span = ReadOnlySpan<byte>.Empty;
        span.ToHexString().Should().Be(string.Empty);
    }

    [Fact]
    public void ToBase64String_Works()
    {
        ReadOnlySpan<byte> span = new byte[] { 0x01, 0x02, 0x03, 0x04 };
        var expected = Convert.ToBase64String(new byte[] { 0x01, 0x02, 0x03, 0x04 });
        span.ToBase64String().Should().Be(expected);
    }

    [Fact]
    public void ToBase64String_EmptySpan_ReturnsEmptyString()
    {
        ReadOnlySpan<byte> span = ReadOnlySpan<byte>.Empty;
        span.ToBase64String().Should().Be(string.Empty);
    }

    [Fact]
    public void ToUtf8StringOrDefault_OutOfBounds_ReturnsDefault()
    {
        ReadOnlySpan<byte> span = new byte[] { 65, 66 }; // 2 bytes
        var p = 0;
        span.ToUtf8StringOrDefault(ref p, 10, "DEFAULT").Should().Be("DEFAULT");
        p.Should().Be(0); // Position not advanced on failure
    }

    [Fact]
    public void ToAsciiStringOrDefault_OutOfBounds_ReturnsDefault()
    {
        ReadOnlySpan<byte> span = new byte[] { 65, 66 }; // 2 bytes
        var p = 0;
        span.ToAsciiStringOrDefault(ref p, 10, "DEFAULT").Should().Be("DEFAULT");
        p.Should().Be(0);
    }

    [Fact]
    public void String_Sequential_Reading()
    {
        var builder = new List<byte>();
        builder.AddRange(Encoding.UTF8.GetBytes("ABC")); // 3 bytes
        builder.AddRange(Encoding.ASCII.GetBytes("DE"));  // 2 bytes

        ReadOnlySpan<byte> span = builder.ToArray();
        var p = 0;

        span.ToUtf8String(ref p, 3).Should().Be("ABC");
        p.Should().Be(3);

        span.ToAsciiString(ref p, 2).Should().Be("DE");
        p.Should().Be(5);
    }

    [Fact]
    public void ToUtf8String_WithPosition_Works()
    {
        var bytes = Encoding.UTF8.GetBytes("ABCDEFGH");
        ReadOnlySpan<byte> span = bytes;
        
        span.ToUtf8String(2, 3).Should().Be("CDE");
    }

    [Fact]
    public void ToAsciiString_WithPosition_Works()
    {
        var bytes = Encoding.ASCII.GetBytes("ABCDEFGH");
        ReadOnlySpan<byte> span = bytes;
        
        span.ToAsciiString(2, 3).Should().Be("CDE");
    }
}
