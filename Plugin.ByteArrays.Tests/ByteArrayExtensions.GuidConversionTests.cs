using FluentAssertions;

namespace Plugin.ByteArrays.Tests;

public class ByteArrayExtensionsGuidConversionTests
{
    #region GUID Conversion Tests

    [Fact]
    public void ToGuid_WithValidData_ShouldReturnCorrectGuid()
    {
        // Arrange
        var originalGuid = Guid.NewGuid();
        var data = originalGuid.ToByteArray();
        var position = 0;

        // Act
        var result = data.ToGuid(ref position);

        // Assert
        result.Should().Be(originalGuid);
        position.Should().Be(16);
    }

    [Fact]
    public void ToGuid_WithFixedPosition_ShouldReturnCorrectGuid()
    {
        // Arrange
        var originalGuid = Guid.NewGuid();
        var data = originalGuid.ToByteArray();

        // Act
        var result = data.ToGuid(0);

        // Assert
        result.Should().Be(originalGuid);
    }

    [Fact]
    public void ToGuid_WithKnownGuid_ShouldReturnExpectedValue()
    {
        // Arrange
        var knownGuid = new Guid("12345678-1234-5678-9ABC-123456789ABC");
        var data = knownGuid.ToByteArray();
        var position = 0;

        // Act
        var result = data.ToGuid(ref position);

        // Assert
        result.Should().Be(knownGuid);
        position.Should().Be(16);
    }

    [Fact]
    public void ToGuidOrDefault_WithValidData_ShouldReturnCorrectGuid()
    {
        // Arrange
        var originalGuid = Guid.NewGuid();
        var data = originalGuid.ToByteArray();
        var position = 0;

        // Act
        var result = data.ToGuidOrDefault(ref position);

        // Assert
        result.Should().Be(originalGuid);
        position.Should().Be(16);
    }

    [Fact]
    public void ToGuidOrDefault_WithFixedPosition_ShouldReturnCorrectGuid()
    {
        // Arrange
        var originalGuid = Guid.NewGuid();
        var data = originalGuid.ToByteArray();

        // Act
        var result = data.ToGuidOrDefault(0);

        // Assert
        result.Should().Be(originalGuid);
    }

    [Fact]
    public void ToGuidOrDefault_WithInsufficientData_ShouldReturnDefault()
    {
        // Arrange
        var data = new byte[8]; // Too short for GUID
        var position = 0;
        var defaultValue = Guid.Empty;

        // Act
        var result = data.ToGuidOrDefault(ref position, defaultValue);

        // Assert
        result.Should().Be(defaultValue);
        position.Should().Be(0); // Position should not advance on failure
    }

    [Fact]
    public void ToGuidOrDefault_WithCustomDefault_ShouldReturnCustomDefault()
    {
        // Arrange
        var data = new byte[8]; // Too short for GUID
        var position = 0;
        var customDefault = Guid.NewGuid();

        // Act
        var result = data.ToGuidOrDefault(ref position, customDefault);

        // Assert
        result.Should().Be(customDefault);
        position.Should().Be(0);
    }

    #endregion

    #region Error Handling Tests

    [Fact]
    public void ToGuid_WithNullArray_ShouldThrowArgumentNullException()
    {
        // Arrange
        byte[] data = null!;
        var position = 0;

        // Act
        Action act = () => data.ToGuid(ref position);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ToGuidOrDefault_WithValidData_ShouldReturnCorrectValue()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var data = guid.ToByteArray();
        var position = 0;

        // Act
        var result = data.ToGuidOrDefault(ref position);

        // Assert
        result.Should().Be(guid);
        position.Should().Be(16);
    }

    [Fact]
    public void ToGuid_WithInsufficientData_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var data = new byte[15]; // One byte short
        var position = 0;

        // Act
        Action act = () => data.ToGuid(ref position);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(17)] // Beyond array bounds for GUID
    public void ToGuid_WithInvalidPosition_ShouldThrowArgumentOutOfRangeException(int invalidPosition)
    {
        // Arrange
        var data = new byte[16];
        var position = invalidPosition;

        // Act
        Action act = () => data.ToGuid(ref position);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void ToGuid_WithPositionTooCloseToEnd_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var data = new byte[20];
        var position = 10; // Only 10 bytes left, need 16

        // Act
        Action act = () => data.ToGuid(ref position);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void ToGuid_WithEmptyGuid_ShouldReturnEmptyGuid()
    {
        // Arrange
        var data = Guid.Empty.ToByteArray();
        var position = 0;

        // Act
        var result = data.ToGuid(ref position);

        // Assert
        result.Should().Be(Guid.Empty);
        position.Should().Be(16);
    }

    [Fact]
    public void ToGuid_WithAllOnesGuid_ShouldReturnCorrectGuid()
    {
        // Arrange
        var data = new byte[16];
        Array.Fill(data, (byte)0xFF);
        var position = 0;

        // Act
        var result = data.ToGuid(ref position);

        // Assert
        result.Should().NotBe(Guid.Empty);
        position.Should().Be(16);
    }

    [Fact]
    public void ToGuid_WithAllZerosData_ShouldReturnEmptyGuid()
    {
        // Arrange
        var data = new byte[16]; // All zeros by default
        var position = 0;

        // Act
        var result = data.ToGuid(ref position);

        // Assert
        result.Should().Be(Guid.Empty);
        position.Should().Be(16);
    }

    #endregion

    #region Multiple GUIDs

    [Fact]
    public void ToGuid_WithMultipleGuidsInArray_ShouldProcessSequentially()
    {
        // Arrange
        var guid1 = Guid.NewGuid();
        var guid2 = Guid.NewGuid();
        var guid3 = Guid.NewGuid();

        var data = guid1.ToByteArray()
            .Concat(guid2.ToByteArray())
            .Concat(guid3.ToByteArray())
            .ToArray();

        var position = 0;

        // Act
        var result1 = data.ToGuid(ref position);
        var result2 = data.ToGuid(ref position);
        var result3 = data.ToGuid(ref position);

        // Assert
        result1.Should().Be(guid1);
        result2.Should().Be(guid2);
        result3.Should().Be(guid3);
        position.Should().Be(48);
    }

    [Fact]
    public void ToGuid_WithMixedData_ShouldExtractGuidCorrectly()
    {
        // Arrange
        var prefix = new byte[] { 0x00, 0x01, 0x02, 0x03 };
        var guid = Guid.NewGuid();
        var suffix = new byte[] { 0x04, 0x05, 0x06, 0x07 };

        var data = prefix.Concat(guid.ToByteArray()).Concat(suffix).ToArray();
        var position = 4; // Skip prefix

        // Act
        var result = data.ToGuid(ref position);

        // Assert
        result.Should().Be(guid);
        position.Should().Be(20); // 4 (prefix) + 16 (GUID)
    }

    #endregion

    #region Roundtrip Tests

    [Fact]
    public void ToGuid_Roundtrip_ShouldPreserveOriginalValue()
    {
        // Arrange
        var originalGuid = Guid.NewGuid();
        var data = originalGuid.ToByteArray();
        var position = 0;

        // Act
        var roundtripGuid = data.ToGuid(ref position);
        var roundtripData = roundtripGuid.ToByteArray();

        // Assert
        roundtripGuid.Should().Be(originalGuid);
        roundtripData.Should().BeEquivalentTo(data);
    }

    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    [InlineData("12345678-1234-5678-9ABC-123456789ABC")]
    [InlineData("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF")]
    public void ToGuid_WithSpecificGuids_ShouldReturnCorrectValues(string guidString)
    {
        // Arrange
        var originalGuid = Guid.Parse(guidString);
        var data = originalGuid.ToByteArray();
        var position = 0;

        // Act
        var result = data.ToGuid(ref position);

        // Assert
        result.Should().Be(originalGuid);
        position.Should().Be(16);
    }

    #endregion

    #region Position Management

    [Fact]
    public void ToGuid_AfterOtherConversions_ShouldUseCorrectPosition()
    {
        // Arrange
        var intValue = 12345;
        var guid = Guid.NewGuid();
        var data = BitConverter.GetBytes(intValue).Concat(guid.ToByteArray()).ToArray();
        var position = 0;

        // Act
        var extractedInt = data.ToInt32(ref position);
        var extractedGuid = data.ToGuid(ref position);

        // Assert
        extractedInt.Should().Be(intValue);
        extractedGuid.Should().Be(guid);
        position.Should().Be(20); // 4 (int) + 16 (GUID)
    }

    #endregion
}
