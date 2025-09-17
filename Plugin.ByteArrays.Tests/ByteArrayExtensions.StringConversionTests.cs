using System;
using System.Text;
using Plugin.ByteArrays;
using Xunit;
using FluentAssertions;

namespace Plugin.ByteArrays.Tests;

public class ByteArrayExtensions_StringConversionTests
{
    [Fact]
    public void ToUtf8String_Works()
    {
        var s = "héllo"; // non-ascii character included
        var utf8 = Encoding.UTF8.GetBytes(s);
        var p = 0;
        utf8.ToUtf8String(ref p).Should().Be(s);
        p.Should().Be(utf8.Length);
    }

    [Fact]
    public void ToUtf8String_NonRef_Overload_Works()
    {
        var utf8 = Encoding.UTF8.GetBytes("hello");
        utf8.ToUtf8String().Should().Be("hello"); // non-ref overload
        utf8.ToUtf8String(0).Should().Be("hello");
    }

    [Fact]
    public void ToUtf8String_WithLength()
    {
        var bytes = Encoding.UTF8.GetBytes("helloWorld");
        var p = 0;
        bytes.ToUtf8String(ref p, 5).Should().Be("hello");
        p.Should().Be(5);

        // Test non-ref overload with length
        bytes.ToUtf8String(0, 5).Should().Be("hello");
        bytes.ToUtf8String(5, 5).Should().Be("World");
    }

    [Fact]
    public void ToUtf8String_ZeroLength_ReturnsEmpty()
    {
        var arr = Encoding.UTF8.GetBytes("abc");
        var p = 0;
        arr.ToUtf8String(ref p, 0).Should().BeEmpty();
        p.Should().Be(0);

        // Test non-ref overload
        arr.ToUtf8String(0, 0).Should().BeEmpty();
    }

    [Fact]
    public void ToUtf8StringOrDefault_Success_And_Failure()
    {
        var bytes = Encoding.UTF8.GetBytes("test");
        var p = 0;

        // Success case
        bytes.ToUtf8StringOrDefault(ref p).Should().Be("test");
        p.Should().Be(4);

        // Failure case - length too large
        p = 0;
        bytes.ToUtf8StringOrDefault(ref p, 100, "fallback").Should().Be("fallback");
        p.Should().Be(0); // Position unchanged on failure

        // Non-ref overload
        bytes.ToUtf8StringOrDefault(0, 100, "default").Should().Be("default");

        // Success with non-ref
        bytes.ToUtf8StringOrDefault(0, 4, "unused").Should().Be("test");
    }

    [Fact]
    public void ToAsciiString_Works()
    {
        var ascii = Encoding.ASCII.GetBytes("ABC");
        var p = 0;
        ascii.ToAsciiString(ref p).Should().Be("ABC");
        p.Should().Be(ascii.Length);
    }

    [Fact]
    public void ToAsciiString_NonRef_Overload_Works()
    {
        var ascii = Encoding.ASCII.GetBytes("abc");
        ascii.ToAsciiString().Should().Be("abc");
        ascii.ToAsciiString(0).Should().Be("abc");
    }

    [Fact]
    public void ToAsciiString_WithLength()
    {
        var bytes = Encoding.ASCII.GetBytes("HelloWorld");
        var p = 0;
        bytes.ToAsciiString(ref p, 5).Should().Be("Hello");
        p.Should().Be(5);

        // Test non-ref overload with length
        bytes.ToAsciiString(0, 5).Should().Be("Hello");
        bytes.ToAsciiString(5, 5).Should().Be("World");
    }

    [Fact]
    public void ToAsciiString_ZeroLength_ReturnsEmpty()
    {
        var a = Array.Empty<byte>();
        var pa = 0;
        a.ToAsciiString(ref pa, 0).Should().BeEmpty();
        pa.Should().Be(0);

        // Test with non-empty array but zero length
        var bytes = Encoding.ASCII.GetBytes("test");
        bytes.ToAsciiString(0, 0).Should().BeEmpty();
    }

    [Fact]
    public void ToAsciiStringOrDefault_Success_And_Failure()
    {
        var bytes = Encoding.ASCII.GetBytes("test");
        var p = 0;

        // Success case
        bytes.ToAsciiStringOrDefault(ref p).Should().Be("test");
        p.Should().Be(4);

        // Failure case - length too large
        p = 0;
        bytes.ToAsciiStringOrDefault(ref p, 100, "fallback").Should().Be("fallback");
        p.Should().Be(0);

        // Non-ref overload
        bytes.ToAsciiStringOrDefault(0, 100, "default").Should().Be("default");

        // Success with non-ref
        bytes.ToAsciiStringOrDefault(0, 4, "unused").Should().Be("test");
    }

    [Fact]
    public void String_NonRef_Overloads_Work()
    {
        var utf8 = Encoding.UTF8.GetBytes("hello");
        utf8.ToUtf8StringOrDefault(defaultValue: "d").Should().Be("hello");

        var ascii = Encoding.ASCII.GetBytes("abc");
        ascii.ToAsciiStringOrDefault(defaultValue: "x").Should().Be("abc");
    }

    [Fact]
    public void ToHexString_Works()
    {
        var data = new byte[] {0, 10, 255, 0x42};
        data.ToHexString().Should().Be("000AFF42");
        data.ToHexString("-", "0x").Should().Be("0x00-0x0A-0xFF-0x42");
        data.ToHexString(":", prefix: "", upperCase: false).Should().Be("00:0a:ff:42");
    }

    [Fact]
    public void ToHexString_EmptyArray()
    {
        Array.Empty<byte>().ToHexString().Should().BeEmpty();
    }

    [Fact]
    public void ToHexString_CustomFormatting()
    {
        var data = new byte[] {0xAB, 0xCD};

        // Test different separators
        data.ToHexString(" ").Should().Be("AB CD");
        data.ToHexString("").Should().Be("ABCD");
        data.ToHexString("-").Should().Be("AB-CD");

        // Test different prefixes
        data.ToHexString("", "0x").Should().Be("0xAB0xCD");
        data.ToHexString("-", "#").Should().Be("#AB-#CD");

        // Test case sensitivity
        data.ToHexString("", "", upperCase: true).Should().Be("ABCD");
        data.ToHexString("", "", upperCase: false).Should().Be("abcd");
    }

    [Fact]
    public void FromHexString_Works()
    {
        var data = new byte[] {0, 10, 255, 0x42};
        ByteArrayExtensions.FromHexString("00 0A-FF:42").Should().BeEquivalentTo(data);
        ByteArrayExtensions.FromHexString("0X0A").Should().BeEquivalentTo(new byte[] {0x0A});
    }

    [Fact]
    public void FromHexString_Edge_Cases()
    {
        // Empty/whitespace input returns empty bytes
        ByteArrayExtensions.FromHexString("   ").Should().BeEmpty();
        ByteArrayExtensions.FromHexString("").Should().BeEmpty();

        // Various prefixes and separators
        ByteArrayExtensions.FromHexString("0x41").Should().BeEquivalentTo(new byte[] {0x41});
        ByteArrayExtensions.FromHexString("41:42").Should().BeEquivalentTo(new byte[] {0x41, 0x42});
        ByteArrayExtensions.FromHexString("41-42").Should().BeEquivalentTo(new byte[] {0x41, 0x42});
        ByteArrayExtensions.FromHexString("41 42").Should().BeEquivalentTo(new byte[] {0x41, 0x42});
    }

    [Fact]
    public void FromHexString_Invalid_Input_Throws()
    {
        // Odd number of characters
        Action act = () => ByteArrayExtensions.FromHexString("0");
        act.Should().Throw<ArgumentException>();

        // Invalid hex characters
        Action act2 = () => ByteArrayExtensions.FromHexString("GG");
        act2.Should().Throw<ArgumentException>();

        // Invalid hex with valid length
        Action act3 = () => ByteArrayExtensions.FromHexString("0G");
        act3.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ToBase64String_Works()
    {
        var data = new byte[] {72, 101, 108, 108, 111}; // "Hello"
        data.ToBase64String().Should().Be("SGVsbG8=");

        var data2 = Encoding.UTF8.GetBytes("hello");
        var b64 = data2.ToBase64String();
        b64.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void ToBase64String_EmptyArray()
    {
        Array.Empty<byte>().ToBase64String().Should().BeEmpty();
    }

    [Fact]
    public void FromBase64String_Works()
    {
        var data = new byte[] {72, 101, 108, 108, 111}; // "Hello"
        ByteArrayExtensions.FromBase64String("SGVsbG8=").Should().BeEquivalentTo(data);
    }

    [Fact]
    public void FromBase64String_Edge_Cases()
    {
        ByteArrayExtensions.FromBase64String(string.Empty).Should().BeEmpty();
        ByteArrayExtensions.FromBase64String("").Should().BeEmpty();
    }

    [Fact]
    public void FromBase64String_Invalid_Input_Throws()
    {
        Action act = () => ByteArrayExtensions.FromBase64String("invalid base64");
        act.Should().Throw<FormatException>();

        // Additional edge case: invalid base64 format
        Action badB64 = () => ByteArrayExtensions.FromBase64String("not base64!");
        badB64.Should().Throw<FormatException>();
    }

    [Fact]
    public void String_Conversion_Error_Handling()
    {
        var data = new byte[] {1, 2, 3};

        // Test negative position errors
        Action act1 = () => data.ToUtf8String(-1);
        act1.Should().Throw<ArgumentOutOfRangeException>();

        Action act2 = () => data.ToAsciiString(-1);
        act2.Should().Throw<ArgumentOutOfRangeException>();

        // Test invalid length (numberOfBytesToRead less than -1 doesn't throw, but out of bounds will)
        Action act3 = () => data.ToUtf8String(0, 100); // Length too big
        act3.Should().Throw<ArgumentOutOfRangeException>();

        Action act4 = () => data.ToAsciiString(0, 100); // Length too big
        act4.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void String_Conversion_Null_Input_Handling()
    {
        // Test null array handling (should throw ArgumentNullException)
        byte[]? nullArray = null;
        var p = 0;

        Action act1 = () => nullArray!.ToUtf8String(ref p);
        act1.Should().Throw<ArgumentNullException>();

        Action act2 = () => nullArray!.ToAsciiString(ref p);
        act2.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void String_Conversion_Read_To_End()
    {
        var utf8Data = Encoding.UTF8.GetBytes("Hello World!");
        var asciiData = Encoding.ASCII.GetBytes("Hello World!");

        // Test reading to end with -1 length
        var p = 5; // Start from "World!"
        utf8Data.ToUtf8String(ref p, -1).Should().Be(" World!");
        p.Should().Be(utf8Data.Length);

        p = 6; // Start from "World!"
        asciiData.ToAsciiString(ref p, -1).Should().Be("World!");
        p.Should().Be(asciiData.Length);
    }
}
