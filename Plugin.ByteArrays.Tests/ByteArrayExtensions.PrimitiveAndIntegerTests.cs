using System;
using System.Linq;
using Plugin.ByteArrays;
using Xunit;
using FluentAssertions;

namespace Plugin.ByteArrays.Tests;

public class ByteArrayExtensions_PrimitiveAndIntegerTests
{
    [Fact]
    public void ToBoolean_ReadsAndAdvances()
    {
        var data = new byte[] {1, 0};
        var pos = 0;
        data.ToBoolean(ref pos).Should().BeTrue();
        pos.Should().Be(1);
        data.ToBoolean(ref pos).Should().BeFalse();
        pos.Should().Be(2);
    }

    [Fact]
    public void ToBooleanOrDefault_OutOfBounds_ReturnsDefaultAndNoAdvance()
    {
        var data = new byte[] {1};
        var pos = 1;
        data.ToBooleanOrDefault(ref pos, defaultValue: true).Should().BeTrue();
        pos.Should().Be(1);
    }

    [Fact]
    public void ToByte_And_SByte()
    {
        var data = new byte[] {255, 128};
        var p = 0;
        data.ToByte(ref p).Should().Be(255);
        data.ToSByte(ref p).Should().Be(unchecked((sbyte)128));
        p.Should().Be(2);
    }

    [Fact]
    public void ToChar_Works()
    {
        var bytes = BitConverter.GetBytes('Z');
        var p = 0;
        bytes.ToChar(ref p).Should().Be('Z');
        p.Should().Be(sizeof(char));
    }

    [Fact]
    public void ToInt16_Int32_UInt64_AndDefaults()
    {
        var expected16 = (short)-12345;
        var expected32 = 0x1234_5678;
        var expectedU64 = 0xFEDCBA9876543210UL;

        var bytes = BitConverter.GetBytes(expected16)
            .Concat(BitConverter.GetBytes(expected32))
            .Concat(BitConverter.GetBytes(expectedU64))
            .ToArray();
        var p = 0;
        bytes.ToInt16(ref p).Should().Be(expected16);
        bytes.ToInt32(ref p).Should().Be(expected32);
        bytes.ToUInt64(ref p).Should().Be(expectedU64);
        p.Should().Be(bytes.Length);

        // Defaults
        var pos = 0;
        Array.Empty<byte>().ToInt16OrDefault(ref pos, 7).Should().Be(7);
        pos.Should().Be(0);
    }

    [Fact]
    public void ExecuteConversion_Guards_Throw_On_Invalid()
    {
        // negative position
        var data = new byte[] {1,2,3,4};
        Action act = () => ByteArrayExtensions.ToInt32(data, position: -1);
        act.Should().Throw<ArgumentOutOfRangeException>();

        // out of bounds
        var p = 3;
        Action act2 = () => data.ToInt32(ref p);
        act2.Should().Throw<ArgumentOutOfRangeException>();
    }
}
