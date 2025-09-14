using System;
using System.Text;
using Plugin.ByteArrays;
using Xunit;
using FluentAssertions;

namespace Plugin.ByteArrays.Tests;

public class ObjectToByteArrayExtensionsTests
{
    [Fact]
    public void StringToByteArrays_Work_And_Empty_ReturnsEmpty()
    {
        "hello".AsciiStringToByteArray().Should().Equal(Encoding.ASCII.GetBytes("hello"));
        "hello".Utf8StringToByteArray().Should().Equal(Encoding.UTF8.GetBytes("hello"));
        "".AsciiStringToByteArray().Should().BeEmpty();
        ((string)null!).Utf8StringToByteArray().Should().BeEmpty();
    }

    [Fact]
    public void Hex_Base64_StringToBytes()
    {
        "0A0BFF".HexStringToByteArray().Should().Equal(0x0A, 0x0B, 0xFF);
        Action odd = () => "ABC".HexStringToByteArray();
        odd.Should().Throw<ArgumentException>();
        var b64 = Convert.ToBase64String(new byte[] {1,2,3});
        b64.Base64StringToByteArray().Should().Equal(1,2,3);
    }

    [Fact]
    public void ToByteArray_Generic_SupportsManyTypes()
    {
        123.ToByteArray().Should().Equal(BitConverter.GetBytes(123));
        123.45.ToByteArray().Should().Equal(BitConverter.GetBytes(123.45));
        ((int?)null).ToByteArray().Should().BeEmpty();
        new byte[] {1,2}.ToByteArray().Should().Equal(1,2);
    }
}
