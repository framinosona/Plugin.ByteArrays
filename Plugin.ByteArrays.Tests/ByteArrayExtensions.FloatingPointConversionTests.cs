using System;
using System.Linq;

using FluentAssertions;

using Plugin.ByteArrays;

using Xunit;

namespace Plugin.ByteArrays.Tests;

public class ByteArrayExtensions_FloatingPointConversionTests
{
    [Fact]
    public void ToDouble_Works()
    {
        var value = Math.PI;
        var bytes = BitConverter.GetBytes(value);
        var p = 0;
        bytes.ToDouble(ref p).Should().Be(value);
        p.Should().Be(sizeof(double));
    }

    [Fact]
    public void ToDouble_NonRef_Overload_Works()
    {
        var value = 42.5;
        var bytes = BitConverter.GetBytes(value);
        bytes.ToDouble().Should().Be(value);
        bytes.ToDouble(0).Should().Be(value);
    }

    [Fact]
    public void ToDoubleOrDefault_Success_And_Failure()
    {
        var value = 3.14159;
        var bytes = BitConverter.GetBytes(value);
        var p = 0;

        // Success case
        bytes.ToDoubleOrDefault(ref p).Should().Be(value);
        p.Should().Be(sizeof(double));

        // Failure case - position out of bounds
        p = 0;
        var empty = Array.Empty<byte>();
        empty.ToDoubleOrDefault(ref p, 99.9).Should().Be(99.9);
        p.Should().Be(0); // Position unchanged on failure

        // Non-ref overload
        empty.ToDoubleOrDefault(0, 123.45).Should().Be(123.45);
    }

    [Fact]
    public void ToSingle_Works()
    {
        var value = 123.456f;
        var bytes = BitConverter.GetBytes(value);
        var p = 0;
        bytes.ToSingle(ref p).Should().Be(value);
        p.Should().Be(sizeof(float));
    }

    [Fact]
    public void ToSingle_NonRef_Overload_Works()
    {
        var value = 42.5f;
        var bytes = BitConverter.GetBytes(value);
        bytes.ToSingle().Should().Be(value);
        bytes.ToSingle(0).Should().Be(value);
    }

    [Fact]
    public void ToSingleOrDefault_OutOfBounds_ReturnsDefault_NoAdvance()
    {
        var bytes = new byte[] { 1 };
        var p = 0;
        bytes.ToSingleOrDefault(ref p, 42f).Should().Be(42f);
        p.Should().Be(0);

        // Test non-ref overload
        bytes.ToSingleOrDefault(0, 99f).Should().Be(99f);

        // Success case
        var validBytes = BitConverter.GetBytes(1.5f);
        p = 0;
        validBytes.ToSingleOrDefault(ref p).Should().Be(1.5f);
        p.Should().Be(sizeof(float));
    }

    [Fact]
    public void ToHalf_AllVariants_Work()
    {
        var halfValue = (Half)3.14;
        var bytes = BitConverter.GetBytes(halfValue);
        var p = 0;

        // Test ref version
        bytes.ToHalf(ref p).Should().Be(halfValue);
        p.Should().Be(2);

        // Test non-ref overload
        bytes.ToHalf().Should().Be(halfValue);
        bytes.ToHalf(0).Should().Be(halfValue);

        // Test OrDefault success
        p = 0;
        bytes.ToHalfOrDefault(ref p).Should().Be(halfValue);
        p.Should().Be(2);

        // Test OrDefault failure
        p = 0;
        var empty = Array.Empty<byte>();
        var defaultHalf = (Half)99;
        empty.ToHalfOrDefault(ref p, defaultHalf).Should().Be(defaultHalf);
        p.Should().Be(0);

        // Test non-ref OrDefault overload
        empty.ToHalfOrDefault(0, defaultHalf).Should().Be(defaultHalf);
    }

    [Fact]
    public void ToDouble_ToSingle_ToHalf_Sequential_Reading()
    {
        var d = Math.PI;
        var f = 123.456f;
        var h = (Half)7.5;
        var bytes = BitConverter.GetBytes(d)
            .Concat(BitConverter.GetBytes(f))
            .Concat(BitConverter.GetBytes(h))
            .ToArray();
        var p = 0;
        bytes.ToDouble(ref p).Should().Be(d);
        bytes.ToSingle(ref p).Should().Be(f);
        bytes.ToHalf(ref p).Should().Be(h);
        p.Should().Be(bytes.Length);
    }

    [Fact]
    public void Floating_Point_Edge_Cases()
    {
        // Test with special floating point values
        var testCases = new[]
        {
            (double.NaN, float.NaN, (Half)float.NaN),
            (double.PositiveInfinity, float.PositiveInfinity, (Half)float.PositiveInfinity),
            (double.NegativeInfinity, float.NegativeInfinity, (Half)float.NegativeInfinity),
            (0.0, 0.0f, (Half)0.0),
            (-0.0, -0.0f, (Half)(-0.0)),
            (double.MaxValue, float.MaxValue, Half.MaxValue),
            (double.MinValue, float.MinValue, Half.MinValue)
        };

        foreach (var (doubleVal, floatVal, halfVal) in testCases)
        {
            // Test double
            var doubleBytes = BitConverter.GetBytes(doubleVal);
            var p = 0;
            var result = doubleBytes.ToDouble(ref p);
            if (double.IsNaN(doubleVal))
            {
                result.Should().Be(double.NaN);
            }
            else
            {
                result.Should().Be(doubleVal);
            }

            // Test float
            var floatBytes = BitConverter.GetBytes(floatVal);
            p = 0;
            var floatResult = floatBytes.ToSingle(ref p);
            if (float.IsNaN(floatVal))
            {
                floatResult.Should().Be(float.NaN);
            }
            else
            {
                floatResult.Should().Be(floatVal);
            }

            // Test half
            var halfBytes = BitConverter.GetBytes(halfVal);
            p = 0;
            var halfResult = halfBytes.ToHalf(ref p);
            if (Half.IsNaN(halfVal))
            {
                halfResult.Should().Be(Half.NaN);
            }
            else
            {
                halfResult.Should().Be(halfVal);
            }
        }
    }

    [Fact]
    public void Floating_Point_Precision_Tests()
    {
        // Test precision boundaries
        var preciseDouble = 1.7976931348623157E+308; // Near double max
        var preciseFloat = 3.4028235E+38f; // Near float max
        var preciseHalf = (Half)65504; // Half max value

        var doubleBytes = BitConverter.GetBytes(preciseDouble);
        var floatBytes = BitConverter.GetBytes(preciseFloat);
        var halfBytes = BitConverter.GetBytes(preciseHalf);

        doubleBytes.ToDouble().Should().Be(preciseDouble);
        floatBytes.ToSingle().Should().Be(preciseFloat);
        halfBytes.ToHalf().Should().Be(preciseHalf);
    }

    [Fact]
    public void Floating_Point_NonRef_Overloads_Work()
    {
        var buf = BitConverter.GetBytes(1.5f)
            .Concat(BitConverter.GetBytes(2.5))
            .Concat(BitConverter.GetBytes((Half)3.5))
            .ToArray();

        buf.ToSingle().Should().Be(1.5f);
        buf.ToDouble(sizeof(float)).Should().Be(2.5);
        buf.ToHalf(sizeof(float) + sizeof(double)).Should().Be((Half)3.5);

        // OrDefault variants with empty arrays
        Array.Empty<byte>().ToSingleOrDefault().Should().Be(0f);
        Array.Empty<byte>().ToDoubleOrDefault().Should().Be(0.0);
        Array.Empty<byte>().ToHalfOrDefault().Should().Be((Half)0);
    }

    [Fact]
    public void Floating_Point_Error_Handling()
    {
        var data = new byte[] { 1, 2, 3 };

        // Test negative position errors
        Action act1 = () => data.ToDouble(-1);
        act1.Should().Throw<ArgumentOutOfRangeException>();

        Action act2 = () => data.ToSingle(-1);
        act2.Should().Throw<ArgumentOutOfRangeException>();

        Action act3 = () => data.ToHalf(-1);
        act3.Should().Throw<ArgumentOutOfRangeException>();

        // Test out of bounds errors
        var p = 0;
        Action act4 = () => data.ToDouble(ref p);
        act4.Should().Throw<ArgumentOutOfRangeException>();

        Action act5 = () => data.ToSingle(ref p);
        act5.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Floating_Point_OrDefault_With_Custom_Defaults()
    {
        var empty = Array.Empty<byte>();
        var p = 0;

        // Test custom default values
        empty.ToDoubleOrDefault(ref p, -1.5).Should().Be(-1.5);
        p.Should().Be(0);

        empty.ToSingleOrDefault(ref p, -2.5f).Should().Be(-2.5f);
        p.Should().Be(0);

        empty.ToHalfOrDefault(ref p, (Half)(-3.5)).Should().Be((Half)(-3.5));
        p.Should().Be(0);

        // Test non-ref overloads with custom defaults
        empty.ToDoubleOrDefault(0, 99.99).Should().Be(99.99);
        empty.ToSingleOrDefault(0, 88.88f).Should().Be(88.88f);
        empty.ToHalfOrDefault(0, (Half)77.77).Should().Be((Half)77.77);
    }

    [Fact]
    public void ToDecimal_Works()
    {
        var value = 123.456789m;
        var bits = decimal.GetBits(value);
        var bytes = new byte[16];
        Buffer.BlockCopy(bits, 0, bytes, 0, 16);
        
        var p = 0;
        bytes.ToDecimal(ref p).Should().Be(value);
        p.Should().Be(16);
    }

    [Fact]
    public void ToDecimal_NonRef_Overload_Works()
    {
        var value = 987.654321m;
        var bits = decimal.GetBits(value);
        var bytes = new byte[16];
        Buffer.BlockCopy(bits, 0, bytes, 0, 16);
        
        bytes.ToDecimal().Should().Be(value);
        bytes.ToDecimal(0).Should().Be(value);
    }

    [Fact]
    public void ToDecimalOrDefault_Success_And_Failure()
    {
        var value = 123456.789m;
        var bits = decimal.GetBits(value);
        var bytes = new byte[16];
        Buffer.BlockCopy(bits, 0, bytes, 0, 16);
        
        var p = 0;

        // Success case
        bytes.ToDecimalOrDefault(ref p).Should().Be(value);
        p.Should().Be(16);

        // Failure case - position out of bounds
        p = 0;
        var empty = Array.Empty<byte>();
        empty.ToDecimalOrDefault(ref p, 99.99m).Should().Be(99.99m);
        p.Should().Be(0); // Position unchanged on failure

        // Non-ref overload
        empty.ToDecimalOrDefault(0, 123.45m).Should().Be(123.45m);
    }

    [Fact]
    public void ToDecimal_WithDifferentValues()
    {
        var values = new[] { 0m, -1m, 1m, decimal.MaxValue, decimal.MinValue, 3.14159265358979323846m };
        
        foreach (var value in values)
        {
            var bits = decimal.GetBits(value);
            var bytes = new byte[16];
            Buffer.BlockCopy(bits, 0, bytes, 0, 16);
            
            bytes.ToDecimal().Should().Be(value);
        }
    }
}
