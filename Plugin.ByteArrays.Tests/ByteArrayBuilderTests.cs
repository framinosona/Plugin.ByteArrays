using System;
using Plugin.ByteArrays;
using Xunit;
using FluentAssertions;

namespace Plugin.ByteArrays.Tests;

public class ByteArrayBuilderTests
{
    private enum Small : byte { A = 1, B = 2 }
    private enum Big : ulong { Max = ulong.MaxValue }

    [Fact]
    public void ToByteArray_MaxSize_Throws_When_Exceeded()
    {
        using var b = new ByteArrayBuilder();
        b.Append(new byte[5]);
        Action act = () => b.ToByteArray(maxSize: 4);
        act.Should().Throw<IndexOutOfRangeException>();
        b.ToByteArray(maxSize: 5).Should().HaveCount(5);
    }

    [Fact]
    public void ToString_Prints_CommaSeparated()
    {
        using var b = new ByteArrayBuilder();
        b.Append(new byte[] {1,2,3});
        b.ToString().Should().Be("1,2,3");
    }

    [Fact]
    public void Append_Primitives_And_Enums()
    {
        using var b = new ByteArrayBuilder();
        b.Append((byte)1)
         .Append((sbyte)-1)
         .Append(true)
         .Append('Z')
         .Append((short)-2)
         .Append((ushort)3)
         .Append(4)
         .Append((uint)5)
         .Append((long)6)
         .Append((ulong)7)
         .Append(1.23f)
         .Append(4.56)
         .Append(new byte[] {9,9})
         .Append((System.Collections.Generic.IEnumerable<byte>)new byte[] {8})
         .Append(Small.B)
         .Append(Big.Max);

        var arr = b.ToByteArray();
        arr.Should().NotBeEmpty();
    }

    [Fact]
    public void Append_Decimal_Writes16Bytes()
    {
        using var b = new ByteArrayBuilder();
        b.Append(123.456m);
        b.ToByteArray().Should().HaveCount(16);
    }

    [Fact]
    public void AppendUtf8_Ascii_Hex_Base64()
    {
        using var b = new ByteArrayBuilder();
        b.AppendUtf8String("hé").ToByteArray().Should().NotBeEmpty();
        b.AppendAsciiString("AB").ToByteArray().Should().NotBeEmpty();

        using var b2 = new ByteArrayBuilder();
        b2.AppendHexString("0A0B").ToByteArray().Should().Equal(0x0A, 0x0B);
        using var b3 = new ByteArrayBuilder();
        var b64 = Convert.ToBase64String(new byte[] {1,2});
        b3.AppendBase64String(b64).ToByteArray().Should().Equal(1,2);
    }

    [Fact]
    public void AppendHex_Invalid_Throws()
    {
        using var b = new ByteArrayBuilder();
        Action act = () => b.AppendHexString("ABC");
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void AppendBase64_Invalid_Throws()
    {
        using var b = new ByteArrayBuilder();
        Action act = () => b.AppendBase64String("not base64");
        act.Should().Throw<FormatException>();
    }
}

