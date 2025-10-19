using FluentAssertions;
using Xunit;

namespace Plugin.ByteArrays.Tests;

public enum TestEnum
{
    None = 0,
    First = 1,
    Second = 2,
    Third = 3
}

public class ReadOnlySpanExtensions_ComplexTypeConversionTests
{
    [Fact]
    public void ToEnum_Works()
    {
        var expected = TestEnum.Second;
        var bytes = BitConverter.GetBytes((int)expected);
        ReadOnlySpan<byte> span = bytes;
        var p = 0;
        span.ToEnum<TestEnum>(ref p).Should().Be(expected);
        p.Should().Be(4);

        // Test non-ref overload
        span.ToEnum<TestEnum>().Should().Be(expected);
        span.ToEnum<TestEnum>(0).Should().Be(expected);
    }

    [Fact]
    public void ToEnum_UndefinedValue_Throws()
    {
        var bytes = BitConverter.GetBytes(999); // Not a valid TestEnum value
        ReadOnlySpan<byte> span = bytes;

        try
        {
            span.ToEnum<TestEnum>();
            Assert.Fail("Expected ArgumentException");
        }
        catch (ArgumentException ex)
        {
            ex.Message.Should().Contain("not defined");
        }
    }

    [Fact]
    public void ToEnumOrDefault_Works()
    {
        var expected = TestEnum.Third;
        var bytes = BitConverter.GetBytes((int)expected);
        ReadOnlySpan<byte> span = bytes;
        var p = 0;
        span.ToEnumOrDefault(ref p, TestEnum.None).Should().Be(expected);
        p.Should().Be(4);
    }

    [Fact]
    public void ToEnumOrDefault_UndefinedValue_ReturnsDefault()
    {
        var bytes = BitConverter.GetBytes(999); // Not a valid TestEnum value
        ReadOnlySpan<byte> span = bytes;
        var p = 0;
        span.ToEnumOrDefault(ref p, TestEnum.First).Should().Be(TestEnum.First);
        p.Should().Be(4);
    }

    [Fact]
    public void ToEnumOrDefault_OutOfBounds_ReturnsDefault()
    {
        ReadOnlySpan<byte> span = new byte[] { 1, 2 }; // Only 2 bytes, need 4
        var p = 0;
        span.ToEnumOrDefault(ref p, TestEnum.Second).Should().Be(TestEnum.Second);
        p.Should().Be(0); // Position not advanced on failure
    }

    [Fact]
    public void ToVersion_FullVersion_Works()
    {
        var expected = new Version(1, 2, 3, 4);
        var bytes = new List<byte>();
        bytes.AddRange(BitConverter.GetBytes(1)); // major
        bytes.AddRange(BitConverter.GetBytes(2)); // minor
        bytes.AddRange(BitConverter.GetBytes(3)); // build
        bytes.AddRange(BitConverter.GetBytes(4)); // revision

        ReadOnlySpan<byte> span = bytes.ToArray();
        var p = 0;
        span.ToVersion(ref p).Should().Be(expected);
        p.Should().Be(16);

        // Test non-ref overload
        span.ToVersion().Should().Be(expected);
        span.ToVersion(0).Should().Be(expected);
    }

    [Fact]
    public void ToVersion_MajorMinorOnly_Works()
    {
        var expected = new Version(1, 2);
        var bytes = new List<byte>();
        bytes.AddRange(BitConverter.GetBytes(1)); // major
        bytes.AddRange(BitConverter.GetBytes(2)); // minor
        bytes.AddRange(BitConverter.GetBytes(-1)); // build (undefined)
        bytes.AddRange(BitConverter.GetBytes(-1)); // revision (undefined)

        ReadOnlySpan<byte> span = bytes.ToArray();
        span.ToVersion().Should().Be(expected);
    }

    [Fact]
    public void ToVersion_MajorMinorBuild_Works()
    {
        var expected = new Version(1, 2, 3);
        var bytes = new List<byte>();
        bytes.AddRange(BitConverter.GetBytes(1)); // major
        bytes.AddRange(BitConverter.GetBytes(2)); // minor
        bytes.AddRange(BitConverter.GetBytes(3)); // build
        bytes.AddRange(BitConverter.GetBytes(-1)); // revision (undefined)

        ReadOnlySpan<byte> span = bytes.ToArray();
        span.ToVersion().Should().Be(expected);
    }

    [Fact]
    public void ToVersionOrDefault_OutOfBounds_ReturnsDefault()
    {
        ReadOnlySpan<byte> span = new byte[] { 1, 2, 3, 4 }; // Only 4 bytes, need 16
        var p = 0;
        var defaultValue = new Version(9, 9);
        span.ToVersionOrDefault(ref p, defaultValue).Should().Be(defaultValue);
        p.Should().Be(0);
    }

    [Fact]
    public void ToVersionOrDefault_Works()
    {
        var expected = new Version(5, 6, 7, 8);
        var bytes = new List<byte>();
        bytes.AddRange(BitConverter.GetBytes(5));
        bytes.AddRange(BitConverter.GetBytes(6));
        bytes.AddRange(BitConverter.GetBytes(7));
        bytes.AddRange(BitConverter.GetBytes(8));

        ReadOnlySpan<byte> span = bytes.ToArray();
        var p = 0;
        span.ToVersionOrDefault(ref p).Should().Be(expected);
        p.Should().Be(16);
    }
}
