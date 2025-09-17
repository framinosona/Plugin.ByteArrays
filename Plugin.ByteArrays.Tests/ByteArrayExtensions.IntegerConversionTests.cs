using System;
using System.Linq;
using Plugin.ByteArrays;
using Xunit;
using FluentAssertions;

namespace Plugin.ByteArrays.Tests;

public class ByteArrayExtensions_IntegerConversionTests
{
    [Fact]
    public void ToInt16_Works()
    {
        var expected = (short)-12345;
        var bytes = BitConverter.GetBytes(expected);
        var p = 0;
        bytes.ToInt16(ref p).Should().Be(expected);
        p.Should().Be(sizeof(short));

        // Test non-ref overload
        bytes.ToInt16().Should().Be(expected);
        bytes.ToInt16(0).Should().Be(expected);
    }

    [Fact]
    public void ToUInt16_Works()
    {
        var expected = (ushort)65535;
        var bytes = BitConverter.GetBytes(expected);
        var p = 0;
        bytes.ToUInt16(ref p).Should().Be(expected);
        p.Should().Be(sizeof(ushort));

        // Test non-ref overload
        bytes.ToUInt16().Should().Be(expected);
        bytes.ToUInt16(0).Should().Be(expected);
    }

    [Fact]
    public void ToInt32_Works()
    {
        var expected = 0x1234_5678;
        var bytes = BitConverter.GetBytes(expected);
        var p = 0;
        bytes.ToInt32(ref p).Should().Be(expected);
        p.Should().Be(sizeof(int));

        // Test non-ref overload
        bytes.ToInt32().Should().Be(expected);
        bytes.ToInt32(0).Should().Be(expected);
    }

    [Fact]
    public void ToUInt32_Works()
    {
        var expected = 0x89ABCDEFu;
        var bytes = BitConverter.GetBytes(expected);
        var p = 0;
        bytes.ToUInt32(ref p).Should().Be(expected);
        p.Should().Be(sizeof(uint));

        // Test non-ref overload
        bytes.ToUInt32().Should().Be(expected);
        bytes.ToUInt32(0).Should().Be(expected);
    }

    [Fact]
    public void ToInt64_Works()
    {
        var expected = 0x123456789ABCDEF0L;
        var bytes = BitConverter.GetBytes(expected);
        var p = 0;
        bytes.ToInt64(ref p).Should().Be(expected);
        p.Should().Be(sizeof(long));

        // Test non-ref overload
        bytes.ToInt64().Should().Be(expected);
        bytes.ToInt64(0).Should().Be(expected);
    }

    [Fact]
    public void ToUInt64_Works()
    {
        var expected = 0xFEDCBA9876543210UL;
        var bytes = BitConverter.GetBytes(expected);
        var p = 0;
        bytes.ToUInt64(ref p).Should().Be(expected);
        p.Should().Be(sizeof(ulong));

        // Test non-ref overload
        bytes.ToUInt64().Should().Be(expected);
        bytes.ToUInt64(0).Should().Be(expected);
    }

    [Fact]
    public void Integer_Sequential_Reading()
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
    }

    [Fact]
    public void Integer_OrDefault_Methods_Success()
    {
        var int16Val = (short)-1000;
        var uint16Val = (ushort)2000;
        var int32Val = -3000;
        var uint32Val = 4000u;
        var int64Val = -5000L;
        var uint64Val = 6000UL;

        var bytes = BitConverter.GetBytes(int16Val)
            .Concat(BitConverter.GetBytes(uint16Val))
            .Concat(BitConverter.GetBytes(int32Val))
            .Concat(BitConverter.GetBytes(uint32Val))
            .Concat(BitConverter.GetBytes(int64Val))
            .Concat(BitConverter.GetBytes(uint64Val))
            .ToArray();

        var p = 0;
        bytes.ToInt16OrDefault(ref p).Should().Be(int16Val);
        bytes.ToUInt16OrDefault(ref p).Should().Be(uint16Val);
        bytes.ToInt32OrDefault(ref p).Should().Be(int32Val);
        bytes.ToUInt32OrDefault(ref p).Should().Be(uint32Val);
        bytes.ToInt64OrDefault(ref p).Should().Be(int64Val);
        bytes.ToUInt64OrDefault(ref p).Should().Be(uint64Val);
        p.Should().Be(bytes.Length);
    }

    [Fact]
    public void Integer_OrDefault_Methods_Failure_Returns_Defaults()
    {
        var empty = Array.Empty<byte>();
        var pos = 0;

        // Test with custom defaults
        empty.ToInt16OrDefault(ref pos, 7).Should().Be(7);
        pos.Should().Be(0);

        empty.ToUInt16OrDefault(ref pos, 8).Should().Be(8);
        pos.Should().Be(0);

        empty.ToInt32OrDefault(ref pos, 9).Should().Be(9);
        pos.Should().Be(0);

        empty.ToUInt32OrDefault(ref pos, 10u).Should().Be(10u);
        pos.Should().Be(0);

        empty.ToInt64OrDefault(ref pos, 11L).Should().Be(11L);
        pos.Should().Be(0);

        empty.ToUInt64OrDefault(ref pos, 12UL).Should().Be(12UL);
        pos.Should().Be(0);
    }

    [Fact]
    public void Integer_OrDefault_NonRef_Overloads()
    {
        var empty = Array.Empty<byte>();

        empty.ToInt16OrDefault().Should().Be(0);
        empty.ToUInt16OrDefault().Should().Be(0);
        empty.ToInt32OrDefault().Should().Be(0);
        empty.ToUInt32OrDefault().Should().Be(0u);
        empty.ToInt64OrDefault().Should().Be(0L);
        empty.ToUInt64OrDefault().Should().Be(0UL);

        // With custom defaults
        empty.ToInt16OrDefault(0, -1).Should().Be(-1);
        empty.ToUInt16OrDefault(0, 1).Should().Be(1);
        empty.ToInt32OrDefault(0, -2).Should().Be(-2);
        empty.ToUInt32OrDefault(0, 2u).Should().Be(2u);
        empty.ToInt64OrDefault(0, -3L).Should().Be(-3L);
        empty.ToUInt64OrDefault(0, 3UL).Should().Be(3UL);
    }

    [Fact]
    public void Integer_NonRef_Overloads_With_Complex_Buffer()
    {
        var buf = BitConverter.GetBytes((short)-2)
            .Concat(BitConverter.GetBytes((ushort)3))
            .Concat(BitConverter.GetBytes(4))
            .Concat(BitConverter.GetBytes((uint)5))
            .Concat(BitConverter.GetBytes((long)6))
            .Concat(BitConverter.GetBytes((ulong)7))
            .ToArray();

        // Test non-ref overloads with calculated positions
        buf.ToInt16().Should().Be(-2);
        buf.ToUInt16(sizeof(short)).Should().Be(3);
        buf.ToInt32(buf.IndexOf(BitConverter.GetBytes(4))).Should().Be(4);
        buf.ToUInt32(buf.IndexOf(BitConverter.GetBytes((uint)5))).Should().Be(5u);
        buf.ToInt64(buf.IndexOf(BitConverter.GetBytes((long)6))).Should().Be(6L);
        buf.ToUInt64(buf.IndexOf(BitConverter.GetBytes((ulong)7))).Should().Be(7UL);
    }

    [Fact]
    public void ExecuteConversion_Guards_Throw_On_Invalid_Integers()
    {
        var data = new byte[] {1, 2, 3, 4};

        // Test negative position
        Action act = () => data.ToInt32(-1);
        act.Should().Throw<ArgumentOutOfRangeException>();

        // Test out of bounds
        var p = 3;
        Action act2 = () => data.ToInt32(ref p);
        act2.Should().Throw<ArgumentOutOfRangeException>();

        // Test other integer types
        Action act3 = () => data.ToInt16(-1);
        act3.Should().Throw<ArgumentOutOfRangeException>();

        Action act4 = () => data.ToUInt16(-1);
        act4.Should().Throw<ArgumentOutOfRangeException>();

        Action act5 = () => data.ToUInt32(-1);
        act5.Should().Throw<ArgumentOutOfRangeException>();

        Action act6 = () => data.ToInt64(-1);
        act6.Should().Throw<ArgumentOutOfRangeException>();

        Action act7 = () => data.ToUInt64(-1);
        act7.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Integer_Edge_Cases()
    {
        // Test with min/max values for Int16
        var int16Bytes = BitConverter.GetBytes(short.MinValue).Concat(BitConverter.GetBytes(short.MaxValue)).ToArray();
        var p = 0;
        int16Bytes.ToInt16(ref p).Should().Be(short.MinValue);
        int16Bytes.ToInt16(ref p).Should().Be(short.MaxValue);

        // Test with min/max values for UInt16
        var uint16Bytes = BitConverter.GetBytes(ushort.MinValue).Concat(BitConverter.GetBytes(ushort.MaxValue)).ToArray();
        p = 0;
        uint16Bytes.ToUInt16(ref p).Should().Be(ushort.MinValue);
        uint16Bytes.ToUInt16(ref p).Should().Be(ushort.MaxValue);

        // Test with min/max values for Int32
        var int32Bytes = BitConverter.GetBytes(int.MinValue).Concat(BitConverter.GetBytes(int.MaxValue)).ToArray();
        p = 0;
        int32Bytes.ToInt32(ref p).Should().Be(int.MinValue);
        int32Bytes.ToInt32(ref p).Should().Be(int.MaxValue);

        // Test with min/max values for UInt32
        var uint32Bytes = BitConverter.GetBytes(uint.MinValue).Concat(BitConverter.GetBytes(uint.MaxValue)).ToArray();
        p = 0;
        uint32Bytes.ToUInt32(ref p).Should().Be(uint.MinValue);
        uint32Bytes.ToUInt32(ref p).Should().Be(uint.MaxValue);

        // Test with min/max values for Int64
        var int64Bytes = BitConverter.GetBytes(long.MinValue).Concat(BitConverter.GetBytes(long.MaxValue)).ToArray();
        p = 0;
        int64Bytes.ToInt64(ref p).Should().Be(long.MinValue);
        int64Bytes.ToInt64(ref p).Should().Be(long.MaxValue);

        // Test with min/max values for UInt64
        var uint64Bytes = BitConverter.GetBytes(ulong.MinValue).Concat(BitConverter.GetBytes(ulong.MaxValue)).ToArray();
        p = 0;
        uint64Bytes.ToUInt64(ref p).Should().Be(ulong.MinValue);
        uint64Bytes.ToUInt64(ref p).Should().Be(ulong.MaxValue);
    }
}
