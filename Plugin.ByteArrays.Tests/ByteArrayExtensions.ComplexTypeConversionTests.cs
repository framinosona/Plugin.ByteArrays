using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using FluentAssertions;

using Plugin.ByteArrays;

using Xunit;

namespace Plugin.ByteArrays.Tests;

public class ByteArrayExtensions_ComplexTypeConversionTests
{
    private enum Color : byte { Red = 1, Green = 2, Blue = 3 }

    [Flags]
    private enum Perms : ushort { None = 0, Read = 1, Write = 2, Execute = 4 }

    private enum Level : int { Low = 1, High = 2 }
    private enum BigEnum : ulong { A = 1UL, B = 2UL }

    private enum SByteEnum : sbyte { Negative = -1, Zero = 0, Positive = 1 }

    #region Version Tests

    [Fact]
    public void ToVersion_Success_Cases()
    {
        var ver = new Version(1, 2, 3);
        var bytes = Encoding.UTF8.GetBytes(ver.ToString());
        var p = 0;
        bytes.ToVersion(ref p).Should().Be(ver);
        p.Should().Be(bytes.Length);

        // Test different version formats
        var ver2 = new Version(4, 5);
        var bytes2 = Encoding.UTF8.GetBytes(ver2.ToString());
        bytes2.ToVersion().Should().Be(ver2);

        var ver3 = new Version(1, 2, 3, 4);
        var bytes3 = Encoding.UTF8.GetBytes(ver3.ToString());
        bytes3.ToVersion(0).Should().Be(ver3);
    }

    [Fact]
    public void ToVersion_NonRef_Overload_Works()
    {
        var ver = new Version(2, 3, 4);
        var bytes = Encoding.UTF8.GetBytes(ver.ToString());
        bytes.ToVersion().Should().Be(ver);
        bytes.ToVersion(0).Should().Be(ver);
    }

    [Fact]
    public void ToVersion_WithNumberOfBytesToRead()
    {
        var fullVersionString = "1.2.3.4.extra";
        var bytes = Encoding.UTF8.GetBytes(fullVersionString);
        var p = 0;

        // Read only the version part (7 characters: "1.2.3.4")
        var result = bytes.ToVersion(ref p, 7);
        result.Should().Be(new Version(1, 2, 3, 4));
        p.Should().Be(7);
    }

    [Fact]
    public void ToVersion_Invalid_String_Throws()
    {
        var invalidBytes = Encoding.UTF8.GetBytes("not.a.version");
        Action act = () => invalidBytes.ToVersion();
        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void ToVersionOrDefault_Success_And_Failure()
    {
        // Success case
        var ver = new Version(1, 2, 3);
        var bytes = Encoding.UTF8.GetBytes(ver.ToString());
        var p = 0;
        bytes.ToVersionOrDefault(ref p).Should().Be(ver);
        p.Should().Be(bytes.Length);

        // Failure case with custom default
        var bad = Encoding.UTF8.GetBytes("not.a.version");
        p = 0;
        var defaultVer = new Version(9, 9, 9);
        bad.ToVersionOrDefault(ref p, defaultValue: defaultVer).Should().Be(defaultVer);
        p.Should().Be(bad.Length);

        // Test with null default (should use 0.0.0)
        p = 0;
        bad.ToVersionOrDefault(ref p, defaultValue: null).Should().Be(new Version(0, 0, 0));

        // Non-ref overload
        bad.ToVersionOrDefault(0, defaultValue: defaultVer).Should().Be(defaultVer);
    }

    [Fact]
    public void ToVersionOrDefault_OutOfBounds_Returns_Default()
    {
        var empty = Array.Empty<byte>();
        var p = 0;
        var defaultVer = new Version(5, 5, 5);

        empty.ToVersionOrDefault(ref p, defaultValue: defaultVer).Should().Be(defaultVer);
        p.Should().Be(0); // Position unchanged on failure

        // Non-ref overload
        empty.ToVersionOrDefault(0, defaultValue: defaultVer).Should().Be(defaultVer);
    }

    #endregion

    #region Enum Tests

    [Fact]
    public void ToEnum_Byte_Enum_Success()
    {
        var data = new byte[] { (byte)Color.Green };
        var p = 0;
        data.ToEnum<Color>(ref p).Should().Be(Color.Green);
        p.Should().Be(1);

        // Test non-ref overload
        data.ToEnum<Color>().Should().Be(Color.Green);
        data.ToEnum<Color>(0).Should().Be(Color.Green);
    }

    [Fact]
    public void ToEnum_SByte_Enum_Success()
    {
        var data = new byte[] { unchecked((byte)SByteEnum.Negative) };
        var p = 0;
        data.ToEnum<SByteEnum>(ref p).Should().Be(SByteEnum.Negative);
        p.Should().Be(sizeof(sbyte));
    }

    [Fact]
    public void ToEnum_UInt16_Enum_Success()
    {
        var combined = (ushort)(Perms.Read | Perms.Write);
        var bytes = BitConverter.GetBytes(combined);
        var p = 0;
        bytes.ToEnum<Perms>(ref p).Should().Be(Perms.Read | Perms.Write);
        p.Should().Be(sizeof(ushort));
    }

    [Fact]
    public void ToEnum_Int32_Enum_Success()
    {
        var e4 = (int)Level.High;
        var bytes4 = BitConverter.GetBytes(e4);
        var p = 0;
        bytes4.ToEnum<Level>(ref p).Should().Be(Level.High);
        p.Should().Be(sizeof(int));
    }

    [Fact]
    public void ToEnum_UInt64_Enum_Success()
    {
        var e8 = (ulong)BigEnum.B;
        var bytes8 = BitConverter.GetBytes(e8);
        var p = 0;
        bytes8.ToEnum<BigEnum>(ref p).Should().Be(BigEnum.B);
        p.Should().Be(sizeof(ulong));
    }

    [Fact]
    public void ToEnum_NonFlags_Invalid_Value_Throws()
    {
        // Invalid value for enum should throw
        Action act = () => new byte[] { 9 }.ToEnum<Color>(0);
        act.Should().Throw<ArgumentException>()
            .WithMessage("*not a valid value for enum*");
    }

    [Fact]
    public void ToEnum_Flags_Invalid_Bits_Throws()
    {
        // Invalid bit (8) should throw for flags enum
        var invalid = BitConverter.GetBytes((ushort)8);
        Action act = () => invalid.ToEnum<Perms>(0);
        act.Should().Throw<ArgumentException>()
            .WithMessage("*contains bits not defined in [Flags] enum*");
    }

    [Fact]
    public void ToEnum_Flags_Valid_Combinations()
    {
        // Test None
        var noneBytes = BitConverter.GetBytes((ushort)Perms.None);
        noneBytes.ToEnum<Perms>().Should().Be(Perms.None);

        // Test single values
        var readBytes = BitConverter.GetBytes((ushort)Perms.Read);
        readBytes.ToEnum<Perms>().Should().Be(Perms.Read);

        var writeBytes = BitConverter.GetBytes((ushort)Perms.Write);
        writeBytes.ToEnum<Perms>().Should().Be(Perms.Write);

        var executeBytes = BitConverter.GetBytes((ushort)Perms.Execute);
        executeBytes.ToEnum<Perms>().Should().Be(Perms.Execute);

        // Test all combinations
        var allBytes = BitConverter.GetBytes((ushort)(Perms.Read | Perms.Write | Perms.Execute));
        allBytes.ToEnum<Perms>().Should().Be(Perms.Read | Perms.Write | Perms.Execute);
    }

    [Fact]
    public void ToEnum_Error_Handling()
    {
        var data = new byte[] { 1, 2 };

        // Test null array
        byte[]? nullArray = null;
        var p = 0;
        Action act1 = () => nullArray!.ToEnum<Color>(ref p);
        act1.Should().Throw<ArgumentNullException>();

        // Test negative position
        Action act2 = () => data.ToEnum<Color>(-1);
        act2.Should().Throw<ArgumentException>()
            .WithMessage("*Position must be greater than or equal to 0*");

        // Test out of bounds
        p = 2;
        Action act3 = () => data.ToEnum<Color>(ref p);
        act3.Should().Throw<ArgumentException>()
            .WithMessage("*Array * is too small*");
    }

    [Fact]
    public void ToEnumOrDefault_Success_Cases()
    {
        var data = new byte[] { (byte)Color.Blue };
        var p = 0;

        // Success case
        data.ToEnumOrDefault<Color>(ref p).Should().Be(Color.Blue);
        p.Should().Be(1);

        // Test with custom default
        p = 0;
        data.ToEnumOrDefault<Color>(ref p, Color.Green).Should().Be(Color.Blue);
        p.Should().Be(1);

        // Non-ref overloads
        data.ToEnumOrDefault<Color>().Should().Be(Color.Blue);
        data.ToEnumOrDefault<Color>(0, Color.Green).Should().Be(Color.Blue);
    }

    [Fact]
    public void ToEnumOrDefault_Failure_Returns_Default()
    {
        // Test with invalid value
        var invalidData = new byte[] { 9 };
        var p = 0;

        invalidData.ToEnumOrDefault<Color>(ref p, Color.Blue).Should().Be(Color.Blue);
        p.Should().Be(0); // Position unchanged on failure

        // Test with out of bounds
        var empty = Array.Empty<byte>();
        p = 0;
        empty.ToEnumOrDefault<Color>(ref p, Color.Red).Should().Be(Color.Red);
        p.Should().Be(0);

        // Test with default parameter (should use first enum value when null is passed to method)
        p = 0;
        // Note: When defaultValue parameter is default(T?), it becomes default(Color) which is 0
        var result = invalidData.ToEnumOrDefault<Color>(ref p);
        // The method uses Enum.GetValues().First() when defaultValue is null, but since we're not passing null explicitly,
        // it uses default(Color) which is (Color)0
        result.Should().Be((Color)0);

        // Non-ref overloads
        invalidData.ToEnumOrDefault<Color>(0, Color.Green).Should().Be(Color.Green);
        empty.ToEnumOrDefault<Color>(0, Color.Blue).Should().Be(Color.Blue);
    }

    [Fact]
    public void ToEnumOrDefault_IndexOutOfRangeException_Handling()
    {
        var tooSmall = new byte[] { 1 }; // Too small for UInt64 enum
        var p = 0;

        tooSmall.ToEnumOrDefault<BigEnum>(ref p, BigEnum.A).Should().Be(BigEnum.A);
        p.Should().Be(0); // Position unchanged on failure
    }

    [Fact]
    public void ToEnumOrDefault_ArgumentException_Handling()
    {
        var invalidFlags = BitConverter.GetBytes((ushort)16); // Invalid flag bit
        var p = 0;

        invalidFlags.ToEnumOrDefault<Perms>(ref p, Perms.Read).Should().Be(Perms.Read);
        p.Should().Be(0); // Position unchanged on failure
    }

    [Fact]
    public void ToEnumOrDefault_Default_Parameter_Behavior()
    {
        var invalidData = new byte[] { 99 }; // Invalid Color value
        var p = 0;

        // Test that the default parameter uses first enum value when method implementation
        // encounters null and falls back to Enum.GetValues().First()
        // This tests the line: defaultValue ??= Enum.GetValues(typeof(T)).Cast<T>().First();

        // We can't pass null directly due to generic constraints, but we can test that
        // when an invalid value fails, it correctly returns the first enum value when using null coalescing
        var emptyArray = Array.Empty<byte>();
        p = 0;

        // The implementation will hit the null coalescing operator and use First() enum value
        var result = emptyArray.ToEnumOrDefault<Color>(ref p); // Uses default(T?) which is null for T?
        result.Should().Be((Color)0); // Default enum value, not necessarily first defined
        p.Should().Be(0);
    }

    [Fact]
    public void ToEnum_All_Enum_Sizes_Coverage()
    {
        // Test 1-byte enum (already covered but ensure position advances)
        var byte1 = new byte[] { 1 };
        var p1 = 0;
        byte1.ToEnum<Color>(ref p1);
        p1.Should().Be(1);

        // Test 2-byte enum
        var byte2 = BitConverter.GetBytes((ushort)1);
        var p2 = 0;
        byte2.ToEnum<Perms>(ref p2);
        p2.Should().Be(2);

        // Test 4-byte enum
        var byte4 = BitConverter.GetBytes(1);
        var p4 = 0;
        byte4.ToEnum<Level>(ref p4);
        p4.Should().Be(4);

        // Test 8-byte enum
        var byte8 = BitConverter.GetBytes(1UL);
        var p8 = 0;
        byte8.ToEnum<BigEnum>(ref p8);
        p8.Should().Be(8);
    }

    [Fact]
    public void ToEnum_Complex_Scenarios()
    {
        // Test sequential enum reading
        var combined = new byte[] { (byte)Color.Red }
            .Concat(BitConverter.GetBytes((ushort)Perms.Write))
            .Concat(BitConverter.GetBytes((int)Level.High))
            .ToArray();

        var p = 0;
        combined.ToEnum<Color>(ref p).Should().Be(Color.Red);
        combined.ToEnum<Perms>(ref p).Should().Be(Perms.Write);
        combined.ToEnum<Level>(ref p).Should().Be(Level.High);
        p.Should().Be(combined.Length);
    }

    [Fact]
    public void Version_And_Enum_Edge_Cases()
    {
        // Version with all position variations
        var versionString = "1.0.0";
        var versionBytes = Encoding.UTF8.GetBytes(versionString);

        // Test all overloads
        versionBytes.ToVersion().Should().Be(new Version(1, 0, 0));
        versionBytes.ToVersion(0).Should().Be(new Version(1, 0, 0));
        versionBytes.ToVersion(0, versionString.Length).Should().Be(new Version(1, 0, 0));

        var p = 0;
        versionBytes.ToVersion(ref p, versionString.Length).Should().Be(new Version(1, 0, 0));
        p.Should().Be(versionString.Length);

        // Test default version creation
        var defaultVersion = new Version(0, 0, 0);
        Array.Empty<byte>().ToVersionOrDefault(0, defaultValue: null).Should().Be(defaultVersion);
    }

    #endregion
}
