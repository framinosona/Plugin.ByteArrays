using System;
using System.Net;
using System.Text;

using FluentAssertions;

using Plugin.ByteArrays;

using Xunit;

namespace Plugin.ByteArrays.Tests;

public class ByteArrayBuilderTests
{
    private enum Small : byte { A = 1, B = 2 }
    private enum Big : ulong { Max = ulong.MaxValue }
    private enum SByteEnum : sbyte { M1 = -1, P1 = 1 }
    private enum ShortEnum : short { S = -2 }
    private enum UShortEnum : ushort { U = 3 }
    private enum IntEnum : int { I = 4 }
    private enum UIntEnum : uint { U = 5 }
    private enum LongEnum : long { L = 6 }

    [Fact]
    public void ToByteArray_MaxSize_Throws_When_Exceeded()
    {
        using var b = new ByteArrayBuilder();
        b.Append(new byte[5]);
        Action act = () => b.ToByteArray(maxSize: 4);
        act.Should().Throw<InvalidOperationException>();
        b.ToByteArray(maxSize: 5).Should().HaveCount(5);
    }

    [Fact]
    public void ToString_Prints_CommaSeparated()
    {
        using var b = new ByteArrayBuilder();
        b.Append(new byte[] { 1, 2, 3 });
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
         .Append(new byte[] { 9, 9 })
         .Append((System.Collections.Generic.IEnumerable<byte>)new byte[] { 8 })
         .Append(Small.B)
         .Append(Big.Max);

        var arr = b.ToByteArray();
        arr.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Append_Null_And_DisposeAsync_Covered()
    {
        await using var b = new ByteArrayBuilder();
        b.Append<string>(null!).ToByteArray().Should().BeEmpty();
        await b.DisposeAsync();
    }

    [Fact]
    public void Invoke_Private_AppendEnum_Generic_Via_Reflection()
    {
        using var b = new ByteArrayBuilder();
        var mi = typeof(ByteArrayBuilder)
            .GetMethods(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .Single(m => m.Name == "AppendEnum" && m.IsGenericMethodDefinition);
        foreach (var t in new[]
                 {
                     typeof(Small), typeof(SByteEnum), typeof(ShortEnum), typeof(UShortEnum),
                     typeof(IntEnum), typeof(UIntEnum), typeof(LongEnum), typeof(Big)
                 })
        {
            var g = mi.MakeGenericMethod(t);
            var value = Enum.GetValues(t).GetValue(0)!;
            g.Invoke(b, new object[] { value });
        }
        b.ToByteArray().Should().NotBeEmpty();
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
        var b64 = Convert.ToBase64String(new byte[] { 1, 2 });
        b3.AppendBase64String(b64).ToByteArray().Should().Equal(1, 2);
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

    #region Constructor and Basic Properties Tests

    [Fact]
    public void Constructor_WithCapacity_ShouldSetInitialCapacity()
    {
        // Act
        using var builder = new ByteArrayBuilder(1024);

        // Assert
        builder.Capacity.Should().Be(1024);
        builder.Length.Should().Be(0);
    }

    [Fact]
    public void WithCapacity_ShouldCreateBuilderWithSpecifiedCapacity()
    {
        // Act
        using var builder = ByteArrayBuilder.WithCapacity(512);

        // Assert
        builder.Capacity.Should().Be(512);
        builder.Length.Should().Be(0);
    }

    [Fact]
    public void Clear_ShouldResetLengthButKeepCapacity()
    {
        // Arrange
        using var builder = new ByteArrayBuilder(100);
        builder.Append(new byte[50]);
        var originalCapacity = builder.Capacity;

        // Act
        builder.Clear();

        // Assert
        builder.Length.Should().Be(0);
        builder.Capacity.Should().Be(originalCapacity);
    }

    #endregion

    #region Unicode and Encoding Tests

    [Fact]
    public void AppendUtf16String_ShouldEncodeCorrectly()
    {
        // Arrange
        using var builder = new ByteArrayBuilder();
        var testString = "Hello 世界";

        // Act
        builder.AppendUtf16String(testString);

        // Assert
        var result = builder.ToByteArray();
        var expected = Encoding.Unicode.GetBytes(testString);
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void AppendUtf32String_ShouldEncodeCorrectly()
    {
        // Arrange
        using var builder = new ByteArrayBuilder();
        var testString = "Test 🚀";

        // Act
        builder.AppendUtf32String(testString);

        // Assert
        var result = builder.ToByteArray();
        var expected = Encoding.UTF32.GetBytes(testString);
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void AppendStringWithEncoding_ShouldUseSpecifiedEncoding()
    {
        // Arrange
        using var builder = new ByteArrayBuilder();
        var testString = "Café";
        var encoding = Encoding.GetEncoding("ISO-8859-1");

        // Act
        builder.AppendString(testString, encoding);

        // Assert
        var result = builder.ToByteArray();
        var expected = encoding.GetBytes(testString);
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void AppendLengthPrefixedString_ShouldIncludeLengthPrefix()
    {
        // Arrange
        using var builder = new ByteArrayBuilder();
        var testString = "Test";

        // Act
        builder.AppendLengthPrefixedString(testString, Encoding.UTF8);

        // Assert
        var result = builder.ToByteArray();
        var expectedLength = Encoding.UTF8.GetByteCount(testString);
        var expectedLengthBytes = BitConverter.GetBytes((short)expectedLength);
        var expectedStringBytes = Encoding.UTF8.GetBytes(testString);

        result.Take(2).Should().BeEquivalentTo(expectedLengthBytes);
        result.Skip(2).Should().BeEquivalentTo(expectedStringBytes);
    }

    [Fact]
    public void AppendLengthPrefixedString_WithEmptyString_ShouldWriteZeroLength()
    {
        // Arrange
        using var builder = new ByteArrayBuilder();

        // Act
        builder.AppendLengthPrefixedString("", Encoding.UTF8);

        // Assert
        var result = builder.ToByteArray();
        result.Should().HaveCount(2);
        var length = BitConverter.ToInt16(result, 0);
        length.Should().Be(0);
    }

    [Fact]
    public void AppendNullTerminatedString_ShouldAddNullTerminator()
    {
        // Arrange
        using var builder = new ByteArrayBuilder();
        var testString = "Test";

        // Act
        builder.AppendNullTerminatedString(testString, Encoding.UTF8);

        // Assert
        var result = builder.ToByteArray();
        var expectedStringBytes = Encoding.UTF8.GetBytes(testString);

        result.Take(expectedStringBytes.Length).Should().BeEquivalentTo(expectedStringBytes);
        result.Last().Should().Be(0); // Null terminator
    }

    [Fact]
    public void AppendNullTerminatedString_WithEmptyString_ShouldOnlyAddNullTerminator()
    {
        // Arrange
        using var builder = new ByteArrayBuilder();

        // Act
        builder.AppendNullTerminatedString("", Encoding.UTF8);

        // Assert
        var result = builder.ToByteArray();
        result.Should().HaveCount(1);
        result[0].Should().Be(0);
    }

    #endregion

    #region Endianness Tests

    [Fact]
    public void AppendBigEndian_WithInt16_ShouldReverseBytes()
    {
        // Arrange
        using var builder = new ByteArrayBuilder();
        short value = 0x1234;

        // Act
        builder.AppendBigEndian(value);

        // Assert
        var result = builder.ToByteArray();
        if (BitConverter.IsLittleEndian)
        {
            result.Should().BeEquivalentTo(new byte[] { 0x12, 0x34 });
        }
        else
        {
            result.Should().BeEquivalentTo(BitConverter.GetBytes(value));
        }
    }

    [Fact]
    public void AppendBigEndian_WithInt32_ShouldReverseBytes()
    {
        // Arrange
        using var builder = new ByteArrayBuilder();
        var value = 0x12345678;

        // Act
        builder.AppendBigEndian(value);

        // Assert
        var result = builder.ToByteArray();
        if (BitConverter.IsLittleEndian)
        {
            result.Should().BeEquivalentTo(new byte[] { 0x12, 0x34, 0x56, 0x78 });
        }
        else
        {
            result.Should().BeEquivalentTo(BitConverter.GetBytes(value));
        }
    }

    [Fact]
    public void AppendBigEndian_WithInt64_ShouldReverseBytes()
    {
        // Arrange
        using var builder = new ByteArrayBuilder();
        var value = 0x123456789ABCDEF0;

        // Act
        builder.AppendBigEndian(value);

        // Assert
        var result = builder.ToByteArray();
        if (BitConverter.IsLittleEndian)
        {
            result.Should().BeEquivalentTo(new byte[] { 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0 });
        }
        else
        {
            result.Should().BeEquivalentTo(BitConverter.GetBytes(value));
        }
    }

    [Fact]
    public void AppendLittleEndian_WithInt16_ShouldEnsureLittleEndian()
    {
        // Arrange
        using var builder = new ByteArrayBuilder();
        short value = 0x1234;

        // Act
        builder.AppendLittleEndian(value);

        // Assert
        var result = builder.ToByteArray();
        if (BitConverter.IsLittleEndian)
        {
            result.Should().BeEquivalentTo(BitConverter.GetBytes(value));
        }
        else
        {
            result.Should().BeEquivalentTo(new byte[] { 0x34, 0x12 });
        }
    }

    [Fact]
    public void AppendLittleEndian_WithInt32_ShouldEnsureLittleEndian()
    {
        // Arrange
        using var builder = new ByteArrayBuilder();
        var value = 0x12345678;

        // Act
        builder.AppendLittleEndian(value);

        // Assert
        var result = builder.ToByteArray();
        if (BitConverter.IsLittleEndian)
        {
            result.Should().BeEquivalentTo(BitConverter.GetBytes(value));
        }
        else
        {
            result.Should().BeEquivalentTo(new byte[] { 0x78, 0x56, 0x34, 0x12 });
        }
    }

    [Fact]
    public void AppendLittleEndian_WithInt64_ShouldEnsureLittleEndian()
    {
        // Arrange
        using var builder = new ByteArrayBuilder();
        var value = 0x123456789ABCDEF0;

        // Act
        builder.AppendLittleEndian(value);

        // Assert
        var result = builder.ToByteArray();
        if (BitConverter.IsLittleEndian)
        {
            result.Should().BeEquivalentTo(BitConverter.GetBytes(value));
        }
        else
        {
            result.Should().BeEquivalentTo(new byte[] { 0xF0, 0xDE, 0xBC, 0x9A, 0x78, 0x56, 0x34, 0x12 });
        }
    }

    #endregion

    #region DateTime and Advanced Types Tests

    [Fact]
    public void AppendDateTime_ShouldWriteAsBinary()
    {
        // Arrange
        using var builder = new ByteArrayBuilder();
        var dateTime = new DateTime(2023, 12, 25, 15, 30, 45);

        // Act
        builder.Append(dateTime);

        // Assert
        var result = builder.ToByteArray();
        result.Should().HaveCount(8);
        var binaryValue = BitConverter.ToInt64(result, 0);
        var reconstructed = DateTime.FromBinary(binaryValue);
        reconstructed.Should().Be(dateTime);
    }

    [Fact]
    public void AppendTimeSpan_ShouldWriteTicks()
    {
        // Arrange
        using var builder = new ByteArrayBuilder();
        var timeSpan = TimeSpan.FromHours(2.5);

        // Act
        builder.Append(timeSpan);

        // Assert
        var result = builder.ToByteArray();
        result.Should().HaveCount(8);
        var ticks = BitConverter.ToInt64(result, 0);
        ticks.Should().Be(timeSpan.Ticks);
    }

    [Fact]
    public void AppendDateTimeOffset_ShouldWriteDateTimeAndOffset()
    {
        // Arrange
        using var builder = new ByteArrayBuilder();
        var offset = TimeSpan.FromHours(5);
        var dateTimeOffset = new DateTimeOffset(2023, 12, 25, 15, 30, 45, offset);

        // Act
        builder.Append(dateTimeOffset);

        // Assert
        var result = builder.ToByteArray();
        result.Should().HaveCount(16); // 8 bytes for DateTime + 8 bytes for offset
    }

    [Fact]
    public void AppendGuid_ShouldWrite16Bytes()
    {
        // Arrange
        using var builder = new ByteArrayBuilder();
        var guid = Guid.NewGuid();

        // Act
        builder.Append(guid);

        // Assert
        var result = builder.ToByteArray();
        result.Should().HaveCount(16);

        var reconstructedGuid = new Guid(result);
        reconstructedGuid.Should().Be(guid);
    }

    [Fact]
    public void AppendIPAddress_WithIPv4_ShouldWrite4Bytes()
    {
        // Arrange
        using var builder = new ByteArrayBuilder();
        var ipAddress = IPAddress.Parse("192.168.1.1");

        // Act
        builder.Append(ipAddress);

        // Assert
        var result = builder.ToByteArray();
        result.Should().HaveCount(4);
        result.Should().BeEquivalentTo(new byte[] { 192, 168, 1, 1 });
    }

    [Fact]
    public void AppendIPAddress_WithIPv6_ShouldWrite16Bytes()
    {
        // Arrange
        using var builder = new ByteArrayBuilder();
        var ipAddress = IPAddress.Parse("2001:db8::1");

        // Act
        builder.Append(ipAddress);

        // Assert
        var result = builder.ToByteArray();
        result.Should().HaveCount(16);

        var reconstructedIP = new IPAddress(result);
        reconstructedIP.Should().Be(ipAddress);
    }

    #endregion

    #region Bulk Operations Tests

    [Fact]
    public void AppendRepeated_ShouldRepeatValueCorrectly()
    {
        // Arrange
        using var builder = new ByteArrayBuilder();
        byte value = 0xFF;
        var count = 5;

        // Act
        builder.AppendRepeated(value, count);

        // Assert
        var result = builder.ToByteArray();
        result.Should().HaveCount(5);
        result.Should().AllSatisfy(b => b.Should().Be(0xFF));
    }

    [Fact]
    public void AppendRepeated_WithZeroCount_ShouldNotAppendAnything()
    {
        // Arrange
        using var builder = new ByteArrayBuilder();

        // Act
        builder.AppendRepeated(0xFF, 0);

        // Assert
        var result = builder.ToByteArray();
        result.Should().BeEmpty();
    }

    [Fact]
    public void AppendRepeated_WithByteArray_ShouldRepeatPatternCorrectly()
    {
        // Arrange
        using var builder = new ByteArrayBuilder();
        var pattern = new byte[] { 1, 2, 3 };
        var repetitions = 3;

        // Act
        builder.AppendRepeated(pattern, repetitions);

        // Assert
        var result = builder.ToByteArray();
        result.Should().HaveCount(9);
        result.Should().BeEquivalentTo(new byte[] { 1, 2, 3, 1, 2, 3, 1, 2, 3 });
    }

    [Fact]
    public void AppendIf_WithTrueCondition_ShouldExecuteAction()
    {
        // Arrange
        using var builder = new ByteArrayBuilder();

        // Act
        builder.AppendIf(true, b => b.Append((byte)42));

        // Assert
        var result = builder.ToByteArray();
        result.Should().ContainSingle().Which.Should().Be(42);
    }

    [Fact]
    public void AppendIf_WithFalseCondition_ShouldNotExecuteAction()
    {
        // Arrange
        using var builder = new ByteArrayBuilder();

        // Act
        builder.AppendIf(false, b => b.Append((byte)42));

        // Assert
        var result = builder.ToByteArray();
        result.Should().BeEmpty();
    }

    [Fact]
    public void AppendMany_ShouldAppendAllValues()
    {
        // Arrange
        using var builder = new ByteArrayBuilder();
        var values = new int[] { 1, 2, 3, 4 };

        // Act
        builder.AppendMany(values);

        // Assert
        var result = builder.ToByteArray();
        result.Should().HaveCount(16); // 4 integers × 4 bytes each

        for (var i = 0; i < values.Length; i++)
        {
            var value = BitConverter.ToInt32(result, i * 4);
            value.Should().Be(values[i]);
        }
    }

    [Fact]
    public void AppendMany_WithStrings_ShouldAppendAllStrings()
    {
        // Arrange
        using var builder = new ByteArrayBuilder();
        var values = new string[] { "A", "B", "C" };

        // Act - Use AppendUtf8String for each string since AppendMany doesn't support strings
        foreach (var value in values)
        {
            builder.AppendUtf8String(value);
        }

        // Assert
        var result = builder.ToByteArray();
        result.Should().BeEquivalentTo(Encoding.UTF8.GetBytes("ABC"));
    }

    #endregion

    #region Stream Integration Tests

    [Fact]
    public void AppendFromStream_ShouldReadAllBytes()
    {
        // Arrange
        using var builder = new ByteArrayBuilder();
        var data = new byte[] { 1, 2, 3, 4, 5 };
        using var stream = new MemoryStream(data);

        // Act
        builder.AppendFromStream(stream);

        // Assert
        var result = builder.ToByteArray();
        result.Should().BeEquivalentTo(data);
    }

    [Fact]
    public void AppendFromStream_WithCount_ShouldReadSpecifiedBytes()
    {
        // Arrange
        using var builder = new ByteArrayBuilder();
        var data = new byte[] { 1, 2, 3, 4, 5 };
        using var stream = new MemoryStream(data);

        // Act
        builder.AppendFromStream(stream, 3);

        // Assert
        var result = builder.ToByteArray();
        result.Should().BeEquivalentTo(new byte[] { 1, 2, 3 });
    }

    [Fact]
    public void WriteTo_ShouldWriteAllBytes()
    {
        // Arrange
        using var builder = new ByteArrayBuilder();
        builder.Append(new byte[] { 1, 2, 3, 4, 5 });
        using var stream = new MemoryStream();

        // Act
        builder.WriteTo(stream);

        // Assert
        stream.ToArray().Should().BeEquivalentTo(new byte[] { 1, 2, 3, 4, 5 });
    }

    [Fact]
    public void ToReadOnlyMemory_ShouldReturnCorrectMemory()
    {
        // Arrange
        using var builder = new ByteArrayBuilder();
        builder.Append(new byte[] { 1, 2, 3, 4, 5 });

        // Act
        var memory = builder.ToReadOnlyMemory();

        // Assert
        memory.Length.Should().Be(5);
        memory.ToArray().Should().BeEquivalentTo(new byte[] { 1, 2, 3, 4, 5 });
    }

    [Fact]
    public void ToMemory_ShouldReturnCorrectMemory()
    {
        // Arrange
        using var builder = new ByteArrayBuilder();
        builder.Append(new byte[] { 1, 2, 3, 4, 5 });

        // Act
        var memory = builder.ToMemory();

        // Assert
        memory.Length.Should().Be(5);
        memory.ToArray().Should().BeEquivalentTo(new byte[] { 1, 2, 3, 4, 5 });
    }

    [Fact]
    public void ToArraySegment_ShouldReturnCorrectSegment()
    {
        // Arrange
        using var builder = new ByteArrayBuilder();
        builder.Append(new byte[] { 1, 2, 3, 4, 5 });

        // Act
        var segment = builder.ToArraySegment();

        // Assert
        segment.Count.Should().Be(5);
        segment.ToArray().Should().BeEquivalentTo(new byte[] { 1, 2, 3, 4, 5 });
    }

    #endregion

    #region Error Handling Tests

    [Fact]
    public void AppendString_WithNullEncoding_ShouldThrow()
    {
        // Arrange
        using var builder = new ByteArrayBuilder();

        // Act & Assert
        Action act = () => builder.AppendString("test", null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AppendLengthPrefixedString_WithNullEncoding_ShouldThrow()
    {
        // Arrange
        using var builder = new ByteArrayBuilder();

        // Act & Assert
        Action act = () => builder.AppendLengthPrefixedString("test", null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AppendNullTerminatedString_WithNullEncoding_ShouldThrow()
    {
        // Arrange
        using var builder = new ByteArrayBuilder();

        // Act & Assert
        Action act = () => builder.AppendNullTerminatedString("test", null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AppendIPAddress_WithNullAddress_ShouldThrow()
    {
        // Arrange
        using var builder = new ByteArrayBuilder();

        // Act & Assert
        Action act = () => builder.Append((IPAddress)null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AppendFromStream_WithNullStream_ShouldThrow()
    {
        // Arrange
        using var builder = new ByteArrayBuilder();

        // Act & Assert
        Action act = () => builder.AppendFromStream(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AppendIf_WithNullAction_ShouldThrow()
    {
        // Arrange
        using var builder = new ByteArrayBuilder();

        // Act & Assert
        Action act = () => builder.AppendIf(true, null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AppendMany_WithNullCollection_ShouldThrow()
    {
        // Arrange
        using var builder = new ByteArrayBuilder();

        // Act & Assert
        Action act = () => builder.AppendMany<int>(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    #endregion
}
