using System;
using System.Linq;
using System.Runtime.InteropServices;
using Plugin.ByteArrays;
using Xunit;
using FluentAssertions;

namespace Plugin.ByteArrays.Tests;

public class ByteArrayExtensions_ComplexTypeTests
{
    private enum Color : byte { Red = 1, Green = 2, Blue = 3 }

    [Flags]
    private enum Perms : ushort { None = 0, Read = 1, Write = 2, Execute = 4 }

    private enum Level : int { Low = 1, High = 2 }
    private enum BigEnum : ulong { A = 1UL, B = 2UL }

    [Fact]
    public void ToVersion_And_Default()
    {
        var ver = new Version(1, 2, 3);
        var bytes = System.Text.Encoding.UTF8.GetBytes(ver.ToString());
        var p = 0;
        bytes.ToVersion(ref p).Should().Be(ver);
        p.Should().Be(bytes.Length);

        var bad = System.Text.Encoding.UTF8.GetBytes("not.a.version");
        p = 0;
        bad.ToVersionOrDefault(ref p, defaultValue: new Version(9,9,9)).Should().Be(new Version(9,9,9));
        p.Should().Be(bad.Length);
    }

    [Fact]
    public void ToEnum_NonFlags_ValidAndInvalid()
    {
        // valid
        var data = new byte[] { (byte)Color.Green };
        var p = 0;
        data.ToEnum<Color>(ref p).Should().Be(Color.Green);
        p.Should().Be(1);

        // invalid value for enum should throw
        Action act = () => new byte[] { 9 }.ToEnum<Color>(0);
        act.Should().Throw<ArgumentException>();

        // OrDefault returns provided default
        p = 0;
        new byte[] { 9 }.ToEnumOrDefault<Color>(ref p, defaultValue: Color.Blue).Should().Be(Color.Blue);
        p.Should().Be(0);
    }

    [Fact]
    public void ToEnum_Flags_ValidAndInvalidBits()
    {
        // Read|Write valid
        var combined = (ushort)(Perms.Read | Perms.Write);
        var bytes = BitConverter.GetBytes(combined);
        var p = 0;
        bytes.ToEnum<Perms>(ref p).Should().Be(Perms.Read | Perms.Write);
        p.Should().Be(Marshal.SizeOf(typeof(ushort)));

        // invalid bit (8) should throw
        var invalid = BitConverter.GetBytes((ushort)8);
        Action act = () => invalid.ToEnum<Perms>(0);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ToEnum_Int32_And_UInt64_Sizes()
    {
        // 4-byte enum
        var e4 = (int)Level.High;
        var bytes4 = BitConverter.GetBytes(e4);
        var p = 0;
        bytes4.ToEnum<Level>(ref p).Should().Be(Level.High);
        p.Should().Be(sizeof(int));

        // 8-byte enum
        var e8 = (ulong)BigEnum.B;
        var bytes8 = BitConverter.GetBytes(e8);
        p = 0;
        bytes8.ToEnum<BigEnum>(ref p).Should().Be(BigEnum.B);
        p.Should().Be(sizeof(ulong));
    }
}
