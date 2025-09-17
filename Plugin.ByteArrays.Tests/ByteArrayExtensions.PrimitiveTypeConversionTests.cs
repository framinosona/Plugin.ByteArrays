using System;

using FluentAssertions;

using Plugin.ByteArrays;

using Xunit;

namespace Plugin.ByteArrays.Tests;

public class ByteArrayExtensions_PrimitiveTypeConversionTests
{
    [Fact]
    public void ToBoolean_ReadsAndAdvances()
    {
        var data = new byte[] { 1, 0 };
        var pos = 0;
        data.ToBoolean(ref pos).Should().BeTrue();
        pos.Should().Be(1);
        data.ToBoolean(ref pos).Should().BeFalse();
        pos.Should().Be(2);
    }

    [Fact]
    public void ToBoolean_NonRef_Overload_Works()
    {
        var data = new byte[] { 1, 0 };
        data.ToBoolean().Should().BeTrue();
        data.ToBoolean(1).Should().BeFalse();
    }

    [Fact]
    public void ToBooleanOrDefault_OutOfBounds_ReturnsDefaultAndNoAdvance()
    {
        var data = new byte[] { 1 };
        var pos = 1;
        data.ToBooleanOrDefault(ref pos, defaultValue: true).Should().BeTrue();
        pos.Should().Be(1);

        // Test non-ref overload
        data.ToBooleanOrDefault(1, defaultValue: false).Should().BeFalse();

        // Test with empty array
        Array.Empty<byte>().ToBooleanOrDefault().Should().BeFalse();
    }

    [Fact]
    public void ToByte_And_SByte()
    {
        var data = new byte[] { 255, 128 };
        var p = 0;
        data.ToByte(ref p).Should().Be(255);
        data.ToSByte(ref p).Should().Be(unchecked((sbyte)128));
        p.Should().Be(2);
    }

    [Fact]
    public void ToByte_NonRef_Overloads_Work()
    {
        var data = new byte[] { 255, 128 };
        data.ToByte().Should().Be(255);
        data.ToByte(1).Should().Be(128);
        data.ToSByte().Should().Be(unchecked((sbyte)255));
        data.ToSByte(1).Should().Be(unchecked((sbyte)128));
    }

    [Fact]
    public void ToByteOrDefault_And_ToSByteOrDefault_Work()
    {
        var data = new byte[] { 42 };
        var p = 0;

        // Success cases
        data.ToByteOrDefault(ref p).Should().Be(42);
        p.Should().Be(1);

        p = 0;
        data.ToSByteOrDefault(ref p).Should().Be(42);
        p.Should().Be(1);

        // Failure cases
        var empty = Array.Empty<byte>();
        p = 0;
        empty.ToByteOrDefault(ref p, 99).Should().Be(99);
        p.Should().Be(0);

        empty.ToSByteOrDefault(ref p, -1).Should().Be(-1);
        p.Should().Be(0);

        // Non-ref overloads
        empty.ToByteOrDefault(0, 123).Should().Be(123);
        empty.ToSByteOrDefault(0, -99).Should().Be(-99);
    }

    [Fact]
    public void ToChar_Works()
    {
        var bytes = BitConverter.GetBytes('Z');
        var p = 0;
        bytes.ToChar(ref p).Should().Be('Z');
        p.Should().Be(sizeof(char));

        // Test non-ref overload
        bytes.ToChar().Should().Be('Z');
        bytes.ToChar(0).Should().Be('Z');
    }

    [Fact]
    public void ToCharOrDefault_Works()
    {
        var bytes = BitConverter.GetBytes('A');
        var p = 0;

        // Success case
        bytes.ToCharOrDefault(ref p).Should().Be('A');
        p.Should().Be(sizeof(char));

        // Failure case
        var empty = Array.Empty<byte>();
        p = 0;
        empty.ToCharOrDefault(ref p, 'X').Should().Be('X');
        p.Should().Be(0);

        // Non-ref overload
        empty.ToCharOrDefault(0, 'Y').Should().Be('Y');
    }

    [Fact]
    public void Primitive_Types_NonRef_Overloads_With_Empty_Arrays()
    {
        var empty = Array.Empty<byte>();

        empty.ToBooleanOrDefault().Should().BeFalse();
        empty.ToByteOrDefault().Should().Be(0);
        empty.ToSByteOrDefault().Should().Be(0);
        empty.ToCharOrDefault().Should().Be('\0');
    }

    [Fact]
    public void ExecuteConversion_Guards_Throw_On_Invalid_Primitives()
    {
        var data = new byte[] { 1 };

        // Test negative position
        Action act1 = () => data.ToBoolean(-1);
        act1.Should().Throw<ArgumentOutOfRangeException>();

        Action act2 = () => data.ToByte(-1);
        act2.Should().Throw<ArgumentOutOfRangeException>();

        Action act3 = () => data.ToSByte(-1);
        act3.Should().Throw<ArgumentOutOfRangeException>();

        Action act4 = () => data.ToChar(-1);
        act4.Should().Throw<ArgumentOutOfRangeException>();

        // Test out of bounds
        var p = 1;
        Action act5 = () => data.ToChar(ref p);
        act5.Should().Throw<ArgumentOutOfRangeException>();
    }
}
