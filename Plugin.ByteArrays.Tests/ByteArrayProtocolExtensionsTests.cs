using FluentAssertions;

namespace Plugin.ByteArrays.Tests;

public class ByteArrayProtocolExtensionsTests
{
    #region TLV Operations Tests

    [Fact]
    public void ParseTlvRecord_WithValidData_ShouldReturnCorrectRecord()
    {
        // Arrange
        var type = (byte)0x01;
        var length = (ushort)4;
        var value = new byte[] { 0x12, 0x34, 0x56, 0x78 };
        var data = new byte[] { type }
            .Concat(BitConverter.GetBytes(length))
            .Concat(value)
            .ToArray();
        var position = 0;

        // Act
        var result = data.ParseTlvRecord(ref position);

        // Assert
        result.Type.Should().Be(type);
        result.Length.Should().Be(length);
        result.Value.Should().BeEquivalentTo(value);
        position.Should().Be(7); // 1 + 2 + 4
    }

    [Fact]
    public void ParseTlvRecord_WithEmptyValue_ShouldReturnEmptyValue()
    {
        // Arrange
        var type = (byte)0x02;
        var length = (ushort)0;
        var data = new byte[] { type }
            .Concat(BitConverter.GetBytes(length))
            .ToArray();
        var position = 0;

        // Act
        var result = data.ParseTlvRecord(ref position);

        // Assert
        result.Type.Should().Be(type);
        result.Length.Should().Be(0);
        result.Value.Should().BeEmpty();
        position.Should().Be(3);
    }

    [Fact]
    public void ParseAllTlvRecords_WithMultipleRecords_ShouldReturnAllRecords()
    {
        // Arrange
        var record1 = new byte[] { 0x01, 0x02, 0x00, 0xAA, 0xBB };
        var record2 = new byte[] { 0x02, 0x01, 0x00, 0xCC };
        var record3 = new byte[] { 0x03, 0x03, 0x00, 0xDD, 0xEE, 0xFF };
        var data = record1.Concat(record2).Concat(record3).ToArray();

        // Act
        var results = data.ParseAllTlvRecords().ToList();

        // Assert
        results.Should().HaveCount(3);
        results[0].Type.Should().Be(0x01);
        results[0].Length.Should().Be(2);
        results[0].Value.Should().BeEquivalentTo(new byte[] { 0xAA, 0xBB });

        results[1].Type.Should().Be(0x02);
        results[1].Length.Should().Be(1);
        results[1].Value.Should().BeEquivalentTo(new byte[] { 0xCC });

        results[2].Type.Should().Be(0x03);
        results[2].Length.Should().Be(3);
        results[2].Value.Should().BeEquivalentTo(new byte[] { 0xDD, 0xEE, 0xFF });
    }

    [Fact]
    public void CreateTlvRecord_WithValidData_ShouldCreateCorrectRecord()
    {
        // Arrange
        var type = (byte)0x42;
        var value = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05 };

        // Act
        var result = ByteArrayProtocolExtensions.CreateTlvRecord(type, value);

        // Assert
        result.Should().HaveCount(8); // 1 + 2 + 5
        result[0].Should().Be(type);
        BitConverter.ToUInt16(result, 1).Should().Be((ushort)value.Length);
        result.Skip(3).Should().BeEquivalentTo(value);
    }

    [Fact]
    public void CreateTlvRecord_WithEmptyValue_ShouldCreateRecordWithZeroLength()
    {
        // Arrange
        var type = (byte)0x99;
        var value = Array.Empty<byte>();

        // Act
        var result = ByteArrayProtocolExtensions.CreateTlvRecord(type, value);

        // Assert
        result.Should().HaveCount(3); // 1 + 2 + 0
        result[0].Should().Be(type);
        BitConverter.ToUInt16(result, 1).Should().Be(0);
    }

    [Fact]
    public void TlvRecord_Equality_ShouldWorkCorrectly()
    {
        // Arrange
        var record1 = new TlvRecord(0x01, 3, new byte[] { 1, 2, 3 });
        var record2 = new TlvRecord(0x01, 3, new byte[] { 1, 2, 3 });
        var record3 = new TlvRecord(0x02, 3, new byte[] { 1, 2, 3 });

        // Act & Assert
        record1.Should().Be(record2);
        record1.Should().NotBe(record3);
        (record1 == record2).Should().BeTrue();
        (record1 != record3).Should().BeTrue();
    }

    #endregion

    #region Frame Operations Tests

    [Fact]
    public void AddSimpleFrame_WithValidData_ShouldAddFrameMarkers()
    {
        // Arrange
        var data = new byte[] { 0x01, 0x02, 0x03 };
        var startMarker = (byte)0x7E;
        var endMarker = (byte)0x7E;

        // Act
        var result = data.AddSimpleFrame(startMarker, endMarker);

        // Assert
        result.Should().HaveCount(5);
        result[0].Should().Be(startMarker);
        result[^1].Should().Be(endMarker);
        result.Skip(1).Take(3).Should().BeEquivalentTo(data);
    }

    [Fact]
    public void RemoveSimpleFrame_WithValidFrame_ShouldExtractData()
    {
        // Arrange
        var originalData = new byte[] { 0x01, 0x02, 0x03 };
        var framedData = originalData.AddSimpleFrame();

        // Act
        var result = framedData.RemoveSimpleFrame();

        // Assert
        result.Should().BeEquivalentTo(originalData);
    }

    [Fact]
    public void AddLengthPrefixedFrame_WithValidData_ShouldAddLengthPrefix()
    {
        // Arrange
        var data = new byte[] { 0xAA, 0xBB, 0xCC, 0xDD };

        // Act
        var result = data.AddLengthPrefixedFrame();

        // Assert
        result.Should().HaveCount(6); // 2 bytes length + 4 bytes data
        BitConverter.ToUInt16(result, 0).Should().Be((ushort)data.Length);
        result.Skip(2).Should().BeEquivalentTo(data);
    }

    [Fact]
    public void RemoveLengthPrefixedFrame_WithValidFrame_ShouldExtractData()
    {
        // Arrange
        var originalData = new byte[] { 0xAA, 0xBB, 0xCC, 0xDD };
        var framedData = originalData.AddLengthPrefixedFrame();

        // Act
        var result = framedData.RemoveLengthPrefixedFrame();

        // Assert
        result.Should().BeEquivalentTo(originalData);
    }

    [Fact]
    public void AddSimpleFrame_WithCustomMarkers_ShouldUseCustomMarkers()
    {
        // Arrange
        var data = new byte[] { 0x01, 0x02 };
        var startMarker = (byte)0xAA;
        var endMarker = (byte)0xBB;

        // Act
        var result = data.AddSimpleFrame(startMarker, endMarker);

        // Assert
        result[0].Should().Be(startMarker);
        result[^1].Should().Be(endMarker);
    }

    #endregion

    #region Checksum Tests

    [Fact]
    public void CalculateSimpleChecksum_WithValidData_ShouldReturnCorrectChecksum()
    {
        // Arrange
        var data = new byte[] { 0x01, 0x02, 0x03, 0x04 };
        var expectedChecksum = (byte)(0x01 + 0x02 + 0x03 + 0x04);

        // Act
        var result = data.CalculateSimpleChecksum();

        // Assert
        result.Should().Be(expectedChecksum);
    }

    [Fact]
    public void CalculateSimpleChecksum_WithOverflow_ShouldWrapAround()
    {
        // Arrange
        var data = new byte[] { 0xFF, 0xFF, 0xFF };
        var expectedChecksum = (byte)((0xFF + 0xFF + 0xFF) & 0xFF);

        // Act
        var result = data.CalculateSimpleChecksum();

        // Assert
        result.Should().Be(expectedChecksum);
    }

    [Fact]
    public void CalculateXorChecksum_WithValidData_ShouldReturnCorrectChecksum()
    {
        // Arrange
        var data = new byte[] { 0x01, 0x02, 0x03, 0x04 };
        var expectedChecksum = (byte)(0x01 ^ 0x02 ^ 0x03 ^ 0x04);

        // Act
        var result = data.CalculateXorChecksum();

        // Assert
        result.Should().Be(expectedChecksum);
    }

    [Fact]
    public void CalculateXorChecksum_WithIdenticalBytes_ShouldReturnZero()
    {
        // Arrange
        var data = new byte[] { 0xAA, 0xAA }; // XOR of identical values is 0

        // Act
        var result = data.CalculateXorChecksum();

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public void AppendChecksum_WithValidData_ShouldAppendCorrectChecksum()
    {
        // Arrange
        var data = new byte[] { 0x01, 0x02, 0x03 };

        // Act
        var result = data.AppendChecksum(d => d.CalculateSimpleChecksum());

        // Assert
        result.Should().HaveCount(4);
        result[^1].Should().Be(data.CalculateSimpleChecksum());
        result.Take(3).Should().BeEquivalentTo(data);
    }

    [Fact]
    public void ValidateChecksum_WithValidChecksum_ShouldReturnTrue()
    {
        // Arrange
        var data = new byte[] { 0x01, 0x02, 0x03 };
        var dataWithChecksum = data.AppendChecksum(d => d.CalculateSimpleChecksum());

        // Act
        var result = dataWithChecksum.ValidateChecksum(d => d.CalculateSimpleChecksum());

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ValidateChecksum_WithInvalidChecksum_ShouldReturnFalse()
    {
        // Arrange
        var data = new byte[] { 0x01, 0x02, 0x03 };
        var dataWithChecksum = data.AppendChecksum(d => d.CalculateSimpleChecksum());
        dataWithChecksum[^1] = 0xFF; // Corrupt the checksum

        // Act
        var result = dataWithChecksum.ValidateChecksum(d => d.CalculateSimpleChecksum());

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region Error Handling Tests

    [Fact]
    public void ParseTlvRecord_WithNullArray_ShouldThrowArgumentNullException()
    {
        // Arrange
        byte[] data = null!;
        var position = 0;

        // Act
        Action act = () => data.ParseTlvRecord(ref position);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ParseTlvRecord_WithInsufficientHeaderData_ShouldThrowArgumentException()
    {
        // Arrange
        var data = new byte[] { 0x01, 0x02 }; // Missing length byte
        var position = 0;

        // Act
        Action act = () => data.ParseTlvRecord(ref position);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ParseTlvRecord_WithInsufficientValueData_ShouldThrowArgumentException()
    {
        // Arrange
        var data = new byte[] { 0x01, 0x05, 0x00, 0xAA }; // Says length 5, but only 1 byte follows
        var position = 0;

        // Act
        Action act = () => data.ParseTlvRecord(ref position);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void CreateTlvRecord_WithNullValue_ShouldThrowArgumentNullException()
    {
        // Arrange
        var type = (byte)0x01;
        byte[] value = null!;

        // Act
        Action act = () => ByteArrayProtocolExtensions.CreateTlvRecord(type, value);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CreateTlvRecord_WithValueTooLarge_ShouldThrowArgumentException()
    {
        // Arrange
        var type = (byte)0x01;
        var value = new byte[ushort.MaxValue + 1]; // Too large for ushort length

        // Act
        Action act = () => ByteArrayProtocolExtensions.CreateTlvRecord(type, value);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void RemoveSimpleFrame_WithInvalidStartMarker_ShouldThrowArgumentException()
    {
        // Arrange
        var data = new byte[] { 0x00, 0x01, 0x02, 0x7E }; // Wrong start marker

        // Act
        Action act = () => data.RemoveSimpleFrame();

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void RemoveSimpleFrame_WithInvalidEndMarker_ShouldThrowArgumentException()
    {
        // Arrange
        var data = new byte[] { 0x7E, 0x01, 0x02, 0x00 }; // Wrong end marker

        // Act
        Action act = () => data.RemoveSimpleFrame();

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void RemoveLengthPrefixedFrame_WithInvalidLength_ShouldThrowArgumentException()
    {
        // Arrange
        var data = new byte[] { 0x05, 0x00, 0x01, 0x02 }; // Says length 5, but only 2 bytes follow

        // Act
        Action act = () => data.RemoveLengthPrefixedFrame();

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ValidateChecksum_WithEmptyArray_ShouldReturnFalse()
    {
        // Arrange
        var data = Array.Empty<byte>();

        // Act
        var result = data.ValidateChecksum(d => d.CalculateSimpleChecksum());

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void ParseAllTlvRecords_WithEmptyArray_ShouldReturnEmpty()
    {
        // Arrange
        var data = Array.Empty<byte>();

        // Act
        var results = data.ParseAllTlvRecords().ToList();

        // Assert
        results.Should().BeEmpty();
    }

    [Fact]
    public void ParseAllTlvRecords_WithPartialRecord_ShouldStopAtPartialRecord()
    {
        // Arrange
        var record1 = new byte[] { 0x01, 0x01, 0x00, 0xAA };
        var partialRecord = new byte[] { 0x02, 0x05 }; // Missing length byte and value
        var data = record1.Concat(partialRecord).ToArray();

        // Act
        var results = data.ParseAllTlvRecords().ToList();

        // Assert
        results.Should().HaveCount(1);
        results[0].Type.Should().Be(0x01);
    }

    [Fact]
    public void AddSimpleFrame_WithEmptyData_ShouldOnlyHaveMarkers()
    {
        // Arrange
        var data = Array.Empty<byte>();

        // Act
        var result = data.AddSimpleFrame();

        // Assert
        result.Should().HaveCount(2);
        result[0].Should().Be(0x7E);
        result[1].Should().Be(0x7E);
    }

    [Fact]
    public void AddLengthPrefixedFrame_WithEmptyData_ShouldHaveZeroLength()
    {
        // Arrange
        var data = Array.Empty<byte>();

        // Act
        var result = data.AddLengthPrefixedFrame();

        // Assert
        result.Should().HaveCount(2);
        BitConverter.ToUInt16(result, 0).Should().Be(0);
    }

    [Fact]
    public void Checksums_WithEmptyData_ShouldReturnZero()
    {
        // Arrange
        var data = Array.Empty<byte>();

        // Act
        var simpleChecksum = data.CalculateSimpleChecksum();
        var xorChecksum = data.CalculateXorChecksum();

        // Assert
        simpleChecksum.Should().Be(0);
        xorChecksum.Should().Be(0);
    }

    #endregion
}
