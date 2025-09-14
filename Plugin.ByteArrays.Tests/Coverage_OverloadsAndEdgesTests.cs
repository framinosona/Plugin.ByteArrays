using System;
using System.Linq;
using System.Text;
using FluentAssertions;
using Plugin.ByteArrays;
using Xunit;

namespace Plugin.ByteArrays.Tests;

public class Coverage_OverloadsAndEdgesTests
{
    private enum LocalColor : byte { Red = 1, Green = 2, Blue = 3 }

    [Fact]
    public void NonRef_Overloads_Are_Invoked()
    {
        var buf = new byte[] { 1, 255, 128 }
            .Concat(BitConverter.GetBytes('A'))
            .Concat(BitConverter.GetBytes((short)-2))
            .Concat(BitConverter.GetBytes((ushort)3))
            .Concat(BitConverter.GetBytes(4))
            .Concat(BitConverter.GetBytes((uint)5))
            .Concat(BitConverter.GetBytes((long)6))
            .Concat(BitConverter.GetBytes((ulong)7))
            .Concat(BitConverter.GetBytes(1.5f))
            .Concat(BitConverter.GetBytes(2.5))
            .ToArray();

        // call non-ref overloads to exercise delegating wrappers
        buf.ToBoolean();
        buf.ToByte();
        buf.ToSByte();
        buf.ToChar(sizeof(bool) + sizeof(byte));
        buf.ToInt16(sizeof(bool) + sizeof(byte) + sizeof(sbyte) + sizeof(char));
        buf.ToUInt16(sizeof(bool) + sizeof(byte) + sizeof(sbyte) + sizeof(char) + sizeof(short));
        buf.ToInt32(buf.IndexOf(BitConverter.GetBytes(4))); // reuse IndexOf to get position
        buf.ToUInt32(buf.IndexOf(BitConverter.GetBytes((uint)5)));
        buf.ToInt64(buf.IndexOf(BitConverter.GetBytes((long)6)));
        buf.ToUInt64(buf.IndexOf(BitConverter.GetBytes((ulong)7)));
        buf.ToSingle(buf.IndexOf(BitConverter.GetBytes(1.5f)));
        buf.ToDouble(buf.IndexOf(BitConverter.GetBytes(2.5)));

        // OrDefault non-ref overloads
        Array.Empty<byte>().ToBooleanOrDefault();
        Array.Empty<byte>().ToByteOrDefault();
        Array.Empty<byte>().ToSByteOrDefault();
        Array.Empty<byte>().ToCharOrDefault();
        Array.Empty<byte>().ToInt16OrDefault();
        Array.Empty<byte>().ToUInt16OrDefault();
        Array.Empty<byte>().ToInt32OrDefault();
        Array.Empty<byte>().ToUInt32OrDefault();
        Array.Empty<byte>().ToInt64OrDefault();
        Array.Empty<byte>().ToUInt64OrDefault();
        Array.Empty<byte>().ToSingleOrDefault();
        Array.Empty<byte>().ToDoubleOrDefault();
    }

    [Fact]
    public void StringConversions_ExtraEdges()
    {
        // Hex empty input returns empty bytes
        ByteArrayExtensions.FromHexString("   ").Should().BeEmpty();
        // Base64 invalid throws
        Action badB64 = () => ByteArrayExtensions.FromBase64String("not base64!");
        badB64.Should().Throw<FormatException>();
        // Utf8 0-length read returns empty
        var arr = Encoding.UTF8.GetBytes("abc");
        var p = 0;
        arr.ToUtf8String(ref p, 0).Should().BeEmpty();
        p.Should().Be(0);
        // ToHexString empty
        Array.Empty<byte>().ToHexString().Should().BeEmpty();
    }

    [Fact]
    public void ArrayManipulation_ExtraBranches()
    {
        // TrimEnd: all bytes equal to trim value
        var all = new byte[] {0,0,0};
        var mutated = all.TrimEnd();
        mutated.Should().BeEmpty();

        // TrimEnd: no trimming needed
        var none = new byte[] {1,2,3};
        var noneCopy = none.TrimEndNonDestructive();
        noneCopy.Should().BeSameAs(none);

        // Empty arrays
        Array.Empty<byte>().TrimEnd().Should().BeEmpty();
        Array.Empty<byte>().TrimEndNonDestructive().Should().BeEmpty();
    }

    [Fact]
    public void Complex_Overloads_Version_Enum_Defaults()
    {
        var v = new Version(2,3,4);
        var vb = Encoding.UTF8.GetBytes(v.ToString());
        vb.ToVersion().Should().Be(v);
        var bad = Encoding.UTF8.GetBytes("not.a.version");
        bad.ToVersionOrDefault(defaultValue: new Version(1,1,1)).Should().Be(new Version(1,1,1));

        // Enum OrDefault for out-of-range
        var pos = 0;
        Array.Empty<byte>().ToEnumOrDefault<LocalColor>(ref pos, defaultValue: LocalColor.Blue)
            .Should().Be(LocalColor.Blue);
        pos.Should().Be(0);
    }

    [Fact]
    public void ByteArrayBuilder_ExtraBranches()
    {
        using var b = new ByteArrayBuilder();
        b.Append((Half)1.5);

        using var b2 = new ByteArrayBuilder();
        Action unsupported = () => b2.Append(new object());
        unsupported.Should().Throw<ArgumentException>();
    }
}
