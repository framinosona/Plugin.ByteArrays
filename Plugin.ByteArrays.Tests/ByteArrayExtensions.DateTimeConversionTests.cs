using FluentAssertions;

namespace Plugin.ByteArrays.Tests;

public class ByteArrayExtensionsDateTimeConversionTests
{
    #region DateTime Conversion Tests

    [Fact]
    public void ToDateTime_WithValidData_ShouldReturnCorrectDateTime()
    {
        // Arrange
        var dateTime = new DateTime(2023, 12, 25, 15, 30, 45);
        var data = BitConverter.GetBytes(dateTime.ToBinary());
        var position = 0;

        // Act
        var result = data.ToDateTime(ref position);

        // Assert
        result.Should().Be(dateTime);
        position.Should().Be(8);
    }

    [Fact]
    public void ToDateTime_WithFixedPosition_ShouldReturnCorrectDateTime()
    {
        // Arrange
        var dateTime = new DateTime(2023, 12, 25, 15, 30, 45);
        var data = BitConverter.GetBytes(dateTime.ToBinary());

        // Act
        var result = data.ToDateTime(0);

        // Assert
        result.Should().Be(dateTime);
    }

    [Fact]
    public void ToDateTimeOrDefault_WithInvalidData_ShouldReturnDefault()
    {
        // Arrange
        var data = new byte[4]; // Too short
        var position = 0;
        var defaultValue = new DateTime(2000, 1, 1);

        // Act
        var result = data.ToDateTimeOrDefault(ref position, defaultValue);

        // Assert
        result.Should().Be(defaultValue);
        position.Should().Be(0); // Position should not advance on failure
    }

    [Fact]
    public void ToDateTimeOrDefault_WithFixedPosition_ShouldReturnDefault()
    {
        // Arrange
        var data = new byte[4]; // Too short
        var defaultValue = new DateTime(2000, 1, 1);

        // Act
        var result = data.ToDateTimeOrDefault(0, defaultValue);

        // Assert
        result.Should().Be(defaultValue);
    }

    [Fact]
    public void ToDateTimeOrDefault_WithCustomDefault_ShouldReturnCustomDefault()
    {
        // Arrange
        var data = new byte[2]; // Too short
        var position = 0;
        var customDefault = new DateTime(1999, 12, 31, 23, 59, 59);

        // Act
        var result = data.ToDateTimeOrDefault(ref position, customDefault);

        // Assert
        result.Should().Be(customDefault);
        position.Should().Be(0);
    }

    [Fact]
    public void ToDateTime_WithInsufficientData_ShouldThrowException()
    {
        // Arrange
        var data = new byte[4]; // Too short for DateTime
        var position = 0;

        // Act
        Action act = () => data.ToDateTime(ref position);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    #endregion

    #region Unix Timestamp Tests

    [Fact]
    public void ToDateTimeFromUnixTimestamp_WithValidTimestamp_ShouldReturnCorrectDateTime()
    {
        // Arrange
        var unixTimestamp = 1672502400; // 2023-01-01 00:00:00 UTC
        var data = BitConverter.GetBytes(unixTimestamp);
        var position = 0;
        var expected = DateTimeOffset.FromUnixTimeSeconds(unixTimestamp).DateTime;

        // Act
        var result = data.ToDateTimeFromUnixTimestamp(ref position);

        // Assert
        result.Should().Be(expected);
        position.Should().Be(4);
    }

    [Fact]
    public void ToDateTimeFromUnixTimestamp_WithFixedPosition_ShouldReturnCorrectDateTime()
    {
        // Arrange
        var unixTimestamp = 1672502400; // 2023-01-01 00:00:00 UTC
        var data = BitConverter.GetBytes(unixTimestamp);
        var expected = DateTimeOffset.FromUnixTimeSeconds(unixTimestamp).DateTime;

        // Act
        var result = data.ToDateTimeFromUnixTimestamp(0);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void ToDateTimeFromUnixTimestampOrDefault_WithInvalidData_ShouldReturnDefault()
    {
        // Arrange
        var data = new byte[2]; // Too short
        var position = 0;
        var defaultValue = new DateTime(2000, 1, 1);

        // Act
        var result = data.ToDateTimeFromUnixTimestampOrDefault(ref position, defaultValue);

        // Assert
        result.Should().Be(defaultValue);
        position.Should().Be(0);
    }

    [Fact]
    public void ToDateTimeFromUnixTimestampOrDefault_WithFixedPosition_ShouldReturnDefault()
    {
        // Arrange
        var data = new byte[2]; // Too short
        var defaultValue = new DateTime(2000, 1, 1);

        // Act
        var result = data.ToDateTimeFromUnixTimestampOrDefault(0, defaultValue);

        // Assert
        result.Should().Be(defaultValue);
    }

    [Fact]
    public void ToDateTimeFromUnixTimestamp_WithInsufficientData_ShouldThrowException()
    {
        // Arrange
        var data = new byte[2]; // Too short for int32
        var position = 0;

        // Act
        Action act = () => data.ToDateTimeFromUnixTimestamp(ref position);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    #endregion

    #region TimeSpan Conversion Tests

    [Fact]
    public void ToTimeSpan_WithValidData_ShouldReturnCorrectTimeSpan()
    {
        // Arrange
        var timeSpan = new TimeSpan(1, 2, 3, 4, 5);
        var data = BitConverter.GetBytes(timeSpan.Ticks);
        var position = 0;

        // Act
        var result = data.ToTimeSpan(ref position);

        // Assert
        result.Should().Be(timeSpan);
        position.Should().Be(8);
    }

    [Fact]
    public void ToTimeSpan_WithFixedPosition_ShouldReturnCorrectTimeSpan()
    {
        // Arrange
        var timeSpan = new TimeSpan(1, 2, 3, 4, 5);
        var data = BitConverter.GetBytes(timeSpan.Ticks);

        // Act
        var result = data.ToTimeSpan(0);

        // Assert
        result.Should().Be(timeSpan);
    }

    [Fact]
    public void ToTimeSpanOrDefault_WithInvalidData_ShouldReturnDefault()
    {
        // Arrange
        var data = new byte[4]; // Too short
        var position = 0;
        var defaultValue = TimeSpan.FromHours(1);

        // Act
        var result = data.ToTimeSpanOrDefault(ref position, defaultValue);

        // Assert
        result.Should().Be(defaultValue);
        position.Should().Be(0);
    }

    [Fact]
    public void ToTimeSpanOrDefault_WithFixedPosition_ShouldReturnDefault()
    {
        // Arrange
        var data = new byte[4]; // Too short
        var defaultValue = TimeSpan.FromHours(1);

        // Act
        var result = data.ToTimeSpanOrDefault(0, defaultValue);

        // Assert
        result.Should().Be(defaultValue);
    }

    [Fact]
    public void ToTimeSpan_WithInsufficientData_ShouldThrowException()
    {
        // Arrange
        var data = new byte[4]; // Too short for TimeSpan (needs 8 bytes)
        var position = 0;

        // Act
        Action act = () => data.ToTimeSpan(ref position);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    #endregion

    #region DateTimeOffset Conversion Tests

    [Fact]
    public void ToDateTimeOffset_WithValidData_ShouldReturnCorrectDateTimeOffset()
    {
        // Arrange
        var dateTimeOffset = new DateTimeOffset(2023, 12, 25, 15, 30, 45, TimeSpan.FromHours(2));
        var dateTimeTicks = BitConverter.GetBytes(dateTimeOffset.DateTime.ToBinary());
        var offsetTicks = BitConverter.GetBytes(dateTimeOffset.Offset.Ticks);
        var data = dateTimeTicks.Concat(offsetTicks).ToArray();
        var position = 0;

        // Act
        var result = data.ToDateTimeOffset(ref position);

        // Assert
        result.Should().Be(dateTimeOffset);
        position.Should().Be(16);
    }

    [Fact]
    public void ToDateTimeOffset_WithFixedPosition_ShouldReturnCorrectDateTimeOffset()
    {
        // Arrange
        var dateTimeOffset = new DateTimeOffset(2023, 12, 25, 15, 30, 45, TimeSpan.FromHours(-5));
        var dateTimeTicks = BitConverter.GetBytes(dateTimeOffset.DateTime.ToBinary());
        var offsetTicks = BitConverter.GetBytes(dateTimeOffset.Offset.Ticks);
        var data = dateTimeTicks.Concat(offsetTicks).ToArray();

        // Act
        var result = data.ToDateTimeOffset(0);

        // Assert
        result.Should().Be(dateTimeOffset);
    }

    [Fact]
    public void ToDateTimeOffsetOrDefault_WithInvalidData_ShouldReturnDefault()
    {
        // Arrange
        var data = new byte[8]; // Too short for DateTimeOffset (needs 16 bytes)
        var position = 0;
        var defaultValue = new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero);

        // Act
        var result = data.ToDateTimeOffsetOrDefault(ref position, defaultValue);

        // Assert
        result.Should().Be(defaultValue);
        // Note: Position advances by 8 because DateTime portion succeeds before offset fails
        position.Should().Be(8);
    }

    [Fact]
    public void ToDateTimeOffsetOrDefault_WithFixedPosition_ShouldReturnDefault()
    {
        // Arrange
        var data = new byte[10]; // Too short for DateTimeOffset
        var defaultValue = new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero);

        // Act
        var result = data.ToDateTimeOffsetOrDefault(0, defaultValue);

        // Assert
        result.Should().Be(defaultValue);
    }

    [Fact]
    public void ToDateTimeOffset_WithInsufficientData_ShouldThrowException()
    {
        // Arrange
        var data = new byte[12]; // Too short for DateTimeOffset (needs 16 bytes)
        var position = 0;

        // Act
        Action act = () => data.ToDateTimeOffset(ref position);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    #endregion

    #region Edge Cases and Error Handling

    [Fact]
    public void ToDateTime_WithNullArray_ShouldThrowArgumentNullException()
    {
        // Arrange
        byte[] data = null!;
        var position = 0;

        // Act
        Action act = () => data.ToDateTime(ref position);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ToTimeSpan_WithNullArray_ShouldThrowArgumentNullException()
    {
        // Arrange
        byte[] data = null!;
        var position = 0;

        // Act
        Action act = () => data.ToTimeSpan(ref position);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ToDateTimeOffset_WithNullArray_ShouldThrowArgumentNullException()
    {
        // Arrange
        byte[] data = null!;
        var position = 0;

        // Act
        Action act = () => data.ToDateTimeOffset(ref position);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ToDateTimeFromUnixTimestamp_WithNullArray_ShouldThrowArgumentNullException()
    {
        // Arrange
        byte[] data = null!;
        var position = 0;

        // Act
        Action act = () => data.ToDateTimeFromUnixTimestamp(ref position);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(100)]
    public void ToDateTime_WithInvalidPosition_ShouldThrowArgumentOutOfRangeException(int invalidPosition)
    {
        // Arrange
        var data = new byte[8];
        var position = invalidPosition;

        // Act
        Action act = () => data.ToDateTime(ref position);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void ToDateTime_WithPositionAtEndOfArray_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var data = new byte[8];
        var position = 8; // At the end

        // Act
        Action act = () => data.ToDateTime(ref position);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void ToDateTimeFromUnixTimestamp_WithZeroTimestamp_ShouldReturnUnixEpoch()
    {
        // Arrange
        var data = BitConverter.GetBytes(0); // Unix epoch
        var position = 0;
        var expected = DateTimeOffset.FromUnixTimeSeconds(0).DateTime;

        // Act
        var result = data.ToDateTimeFromUnixTimestamp(ref position);

        // Assert
        result.Should().Be(expected);
        position.Should().Be(4);
    }

    [Fact]
    public void ToTimeSpan_WithZeroTicks_ShouldReturnZeroTimeSpan()
    {
        // Arrange
        var data = BitConverter.GetBytes(0L); // Zero ticks
        var position = 0;

        // Act
        var result = data.ToTimeSpan(ref position);

        // Assert
        result.Should().Be(TimeSpan.Zero);
        position.Should().Be(8);
    }

    [Fact]
    public void ToTimeSpan_WithMaxTicks_ShouldReturnMaxTimeSpan()
    {
        // Arrange
        var maxTicks = TimeSpan.MaxValue.Ticks;
        var data = BitConverter.GetBytes(maxTicks);
        var position = 0;

        // Act
        var result = data.ToTimeSpan(ref position);

        // Assert
        result.Should().Be(TimeSpan.MaxValue);
        position.Should().Be(8);
    }

    #endregion

    #region Multiple Conversions

    [Fact]
    public void MultipleConversions_WithSequentialData_ShouldWorkCorrectly()
    {
        // Arrange
        var dateTime1 = new DateTime(2023, 1, 1);
        var dateTime2 = new DateTime(2023, 12, 31);
        var timeSpan = new TimeSpan(1, 2, 3);

        var data = BitConverter.GetBytes(dateTime1.ToBinary())
            .Concat(BitConverter.GetBytes(dateTime2.ToBinary()))
            .Concat(BitConverter.GetBytes(timeSpan.Ticks))
            .ToArray();

        var position = 0;

        // Act
        var result1 = data.ToDateTime(ref position);
        var result2 = data.ToDateTime(ref position);
        var result3 = data.ToTimeSpan(ref position);

        // Assert
        result1.Should().Be(dateTime1);
        result2.Should().Be(dateTime2);
        result3.Should().Be(timeSpan);
        position.Should().Be(24);
    }

    [Fact]
    public void MixedConversions_WithUnixTimestampAndTimeSpan_ShouldWorkCorrectly()
    {
        // Arrange
        var unixTimestamp = 1672502400; // 2023-01-01 00:00:00 UTC
        var timeSpan = new TimeSpan(12, 30, 45);

        var data = BitConverter.GetBytes(unixTimestamp)
            .Concat(BitConverter.GetBytes(timeSpan.Ticks))
            .ToArray();

        var position = 0;
        var expectedDateTime = DateTimeOffset.FromUnixTimeSeconds(unixTimestamp).DateTime;

        // Act
        var resultDateTime = data.ToDateTimeFromUnixTimestamp(ref position);
        var resultTimeSpan = data.ToTimeSpan(ref position);

        // Assert
        resultDateTime.Should().Be(expectedDateTime);
        resultTimeSpan.Should().Be(timeSpan);
        position.Should().Be(12); // 4 bytes for int32 + 8 bytes for long
    }

    [Fact]
    public void OrDefaultMethods_WithPartiallyValidData_ShouldHandleGracefully()
    {
        // Arrange - valid data for first conversion, invalid for second
        var validDateTime = new DateTime(2023, 6, 15);
        var data = BitConverter.GetBytes(validDateTime.ToBinary())
            .Concat(new byte[4]) // Only 4 bytes for what should be 8-byte TimeSpan
            .ToArray();

        var position = 0;
        var defaultTimeSpan = TimeSpan.FromMinutes(30);

        // Act
        var resultDateTime = data.ToDateTime(ref position); // Should succeed
        var resultTimeSpan = data.ToTimeSpanOrDefault(ref position, defaultTimeSpan); // Should use default

        // Assert
        resultDateTime.Should().Be(validDateTime);
        resultTimeSpan.Should().Be(defaultTimeSpan);
        position.Should().Be(8); // Only the first conversion should advance position
    }

    [Fact]
    public void ToDateTimeFromUnixTimestamp_WithNegativeTimestamp_ShouldReturnPreEpochDate()
    {
        // Arrange
        var negativeTimestamp = -86400; // One day before epoch
        var data = BitConverter.GetBytes(negativeTimestamp);
        var position = 0;
        var expected = DateTimeOffset.FromUnixTimeSeconds(negativeTimestamp).DateTime;

        // Act
        var result = data.ToDateTimeFromUnixTimestamp(ref position);

        // Assert
        result.Should().Be(expected);
        position.Should().Be(4);
    }

    [Fact]
    public void ToTimeSpan_WithNegativeTicks_ShouldReturnNegativeTimeSpan()
    {
        // Arrange
        var negativeTimeSpan = new TimeSpan(-1, -2, -3, -4, -5);
        var data = BitConverter.GetBytes(negativeTimeSpan.Ticks);
        var position = 0;

        // Act
        var result = data.ToTimeSpan(ref position);

        // Assert
        result.Should().Be(negativeTimeSpan);
        position.Should().Be(8);
    }

    [Fact]
    public void ToDateTimeOffset_WithNegativeOffset_ShouldWorkCorrectly()
    {
        // Arrange
        var dateTimeOffset = new DateTimeOffset(2023, 6, 15, 14, 30, 0, TimeSpan.FromHours(-8));
        var dateTimeTicks = BitConverter.GetBytes(dateTimeOffset.DateTime.ToBinary());
        var offsetTicks = BitConverter.GetBytes(dateTimeOffset.Offset.Ticks);
        var data = dateTimeTicks.Concat(offsetTicks).ToArray();
        var position = 0;

        // Act
        var result = data.ToDateTimeOffset(ref position);

        // Assert
        result.Should().Be(dateTimeOffset);
        position.Should().Be(16);
    }

    #endregion
}
