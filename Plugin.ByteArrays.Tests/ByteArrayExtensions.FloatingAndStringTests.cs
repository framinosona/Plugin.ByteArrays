using System;
using System.Linq;
using System.Text;
using Plugin.ByteArrays;
using Xunit;
using FluentAssertions;

namespace Plugin.ByteArrays.Tests;

public class ByteArrayExtensions_FloatingAndStringTests
{
    [Fact]
    public void ToDouble_ToSingle_Roundtrip()
    {
        var d = Math.PI;
        var f = 123.456f;
        var bytes = BitConverter.GetBytes(d)
            .Concat(BitConverter.GetBytes(f))
            .ToArray();
        var p = 0;
        bytes.ToDouble(ref p).Should().Be(d);
        bytes.ToSingle(ref p).Should().Be(f);
        p.Should().Be(bytes.Length);
    }

    [Fact]
    public void ToSingleOrDefault_OutOfBounds_ReturnsDefault_NoAdvance()
    {
        var bytes = new byte[] {1};
        var p = 0;
        bytes.ToSingleOrDefault(ref p, 42f).Should().Be(42f);
        p.Should().Be(0);
    }

    [Fact]
    public void Utf8_And_Ascii_String_Conversions_Work()
    {
        var s = "héllo"; // non-ascii character included
        var utf8 = Encoding.UTF8.GetBytes(s);
        var p = 0;
        utf8.ToUtf8String(ref p).Should().Be(s);
        p.Should().Be(utf8.Length);

        var ascii = Encoding.ASCII.GetBytes("ABC");
        p = 0;
        ascii.ToAsciiString(ref p).Should().Be("ABC");
        p.Should().Be(ascii.Length);
    }

    [Fact]
    public void Utf8_Ascii_Defaults_And_Lengths()
    {
        var bytes = Encoding.UTF8.GetBytes("helloWorld");
        var p = 0;
        bytes.ToUtf8String(ref p, 5).Should().Be("hello");
        p.Should().Be(5);
        bytes.ToUtf8StringOrDefault(ref p, 100, "fallback").Should().Be("fallback");
        p.Should().Be(5);

        var a = Array.Empty<byte>();
        var pa = 0;
        a.ToAsciiString(ref pa, 0).Should().BeEmpty();
    }

    [Fact]
    public void Hex_String_To_And_From()
    {
        var data = new byte[] {0, 10, 255, 0x42};
        data.ToHexString().Should().Be("000AFF42");
        data.ToHexString("-", "0x").Should().Be("0x00-0x0A-0xFF-0x42");
        data.ToHexString(":", prefix: "", upperCase: false).Should().Be("00:0a:ff:42");

        ByteArrayExtensions.FromHexString("00 0A-FF:42").Should().BeEquivalentTo(data);
        Action act = () => ByteArrayExtensions.FromHexString("0");
        act.Should().Throw<ArgumentException>();
        Action act2 = () => ByteArrayExtensions.FromHexString("GG");
        act2.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Base64_To_And_From()
    {
        var data = Encoding.UTF8.GetBytes("hello");
        var b64 = data.ToBase64String();
        b64.Should().NotBeNullOrEmpty();
        ByteArrayExtensions.FromBase64String(b64).Should().BeEquivalentTo(data);
        ByteArrayExtensions.FromBase64String("").Should().BeEmpty();
    }
}
