using FluentAssertions;
using System.Linq;
using Xunit;

namespace Plugin.ByteArrays.Tests;

public class ReadOnlySpanExtensions_DateTimeConversionTests
{
    #region Unix Timestamp Tests

    [Fact]
    public void ToDateTimeFromUnixTimestamp_WithValidTimestamp_ShouldReturnCorrectDateTime()
    {
        // Arrange
        var unixTimestamp = 1672502400; // Jan 1, 2023, 00:00:00 UTC
        var data = BitConverter.GetBytes(unixTimestamp);
        ReadOnlySpan<byte> span = data;
        var expected = DateTimeOffset.FromUnixTimeSeconds(unixTimestamp).DateTime;
        var position = 0;

        // Act
        var result = span.ToDateTimeFromUnixTimestamp(ref position);

        // Assert
        result.Should().Be(expected);
        position.Should().Be(4);
    }

    [Fact]
    public void ToDateTimeFromUnixTimestamp_WithFixedPosition_ShouldReturnCorrectDateTime()
    {
        // Arrange
        var unixTimestamp = 1672502400; // Jan 1, 2023, 00:00:00 UTC
        var data = BitConverter.GetBytes(unixTimestamp);
        ReadOnlySpan<byte> span = data;
        var expected = DateTimeOffset.FromUnixTimeSeconds(unixTimestamp).DateTime;

        // Act
        var result = span.ToDateTimeFromUnixTimestamp(0);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void ToDateTimeFromUnixTimestampOrDefault_WithInvalidData_ShouldReturnDefault()
    {
        // Arrange
        var data = new byte[2]; // Too short
        ReadOnlySpan<byte> span = data;
        var position = 0;
        var defaultValue = new DateTime(2000, 1, 1);

        // Act
        var result = span.ToDateTimeFromUnixTimestampOrDefault(ref position, defaultValue);

        // Assert
        result.Should().Be(defaultValue);
        position.Should().Be(0); // Position should not advance on failure
    }

    [Fact]
    public void ToDateTimeFromUnixTimestampOrDefault_WithFixedPosition_ShouldReturnDefault()
    {
        // Arrange
        var data = new byte[2]; // Too short
        ReadOnlySpan<byte> span = data;
        var defaultValue = new DateTime(2000, 1, 1);

        // Act
        var result = span.ToDateTimeFromUnixTimestampOrDefault(0, defaultValue);

        // Assert
        result.Should().Be(defaultValue);
    }

    [Fact]
    public void ToDateTimeFromUnixTimestamp_WithZeroTimestamp_ShouldReturnUnixEpoch()
    {
        // Arrange
        var data = BitConverter.GetBytes(0); // Unix epoch
        ReadOnlySpan<byte> span = data;
        var expected = DateTimeOffset.FromUnixTimeSeconds(0).DateTime;
        var position = 0;

        // Act
        var result = span.ToDateTimeFromUnixTimestamp(ref position);

        // Assert
        result.Should().Be(expected);
        position.Should().Be(4);
    }

    [Fact]
    public void ToDateTimeFromUnixTimestamp_WithNegativeTimestamp_ShouldReturnPreEpochDate()
    {
        // Arrange
        var negativeTimestamp = -86400; // One day before Unix epoch
        var data = BitConverter.GetBytes(negativeTimestamp);
        ReadOnlySpan<byte> span = data;
        var expected = DateTimeOffset.FromUnixTimeSeconds(negativeTimestamp).DateTime;
        var position = 0;

        // Act
        var result = span.ToDateTimeFromUnixTimestamp(ref position);

        // Assert
        result.Should().Be(expected);
        position.Should().Be(4);
    }

    [Fact]
    public void ToDateTimeFromUnixTimestamp_WithMaxInt32Value_ShouldWork()
    {
        // Arrange
        var maxTimestamp = int.MaxValue;
        var data = BitConverter.GetBytes(maxTimestamp);
        ReadOnlySpan<byte> span = data;
        var expected = DateTimeOffset.FromUnixTimeSeconds(maxTimestamp).DateTime;
        var position = 0;

        // Act
        var result = span.ToDateTimeFromUnixTimestamp(ref position);

        // Assert
        result.Should().Be(expected);
        position.Should().Be(4);
    }

    [Fact]
    public void ToDateTimeFromUnixTimestampOrDefault_WithValidData_ShouldReturnCorrectValue()
    {
        // Arrange
        var unixTimestamp = 1609459200; // Jan 1, 2021, 00:00:00 UTC
        var data = BitConverter.GetBytes(unixTimestamp);
        ReadOnlySpan<byte> span = data;
        var expected = DateTimeOffset.FromUnixTimeSeconds(unixTimestamp).DateTime;
        var position = 0;

        // Act
        var result = span.ToDateTimeFromUnixTimestampOrDefault(ref position);

        // Assert
        result.Should().Be(expected);
        position.Should().Be(4);
    }

    [Fact]
    public void ToDateTimeFromUnixTimestamp_WithEmptySpan_ShouldThrowException()
    {
        // Arrange
        var emptyArray = Array.Empty<byte>();

        // Act & Assert
        var act = () =>
        {
            var tempSpan = new ReadOnlySpan<byte>(emptyArray);
            var tempPos = 0;
            return tempSpan.ToDateTimeFromUnixTimestamp(ref tempPos);
        };
        
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void ToDateTimeFromUnixTimestamp_MultipleConversions_ShouldWork()
    {
        // Arrange
        var timestamp1 = 1672502400; // Jan 1, 2023
        var timestamp2 = 1704038400; // Jan 1, 2024
        var data = BitConverter.GetBytes(timestamp1).Concat(BitConverter.GetBytes(timestamp2)).ToArray();
        ReadOnlySpan<byte> span = data;
        var position = 0;

        // Act
        var result1 = span.ToDateTimeFromUnixTimestamp(ref position);
        var result2 = span.ToDateTimeFromUnixTimestamp(ref position);

        // Assert
        result1.Should().Be(DateTimeOffset.FromUnixTimeSeconds(timestamp1).DateTime);
        result2.Should().Be(DateTimeOffset.FromUnixTimeSeconds(timestamp2).DateTime);
        position.Should().Be(8);
    }

    #endregion
}
