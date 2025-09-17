using FluentAssertions;
using System.Text;

namespace Plugin.ByteArrays.Tests;

public class ByteArrayUtilitiesTests
{
    #region Binary String Representation Tests

    [Fact]
    public void ToBinaryString_WithValidData_ShouldReturnCorrectBinaryString()
    {
        // Arrange
        var data = new byte[] { 0b10101010, 0b11110000 };

        // Act
        var result = data.ToBinaryString();

        // Assert
        result.Should().Be("10101010 11110000");
    }

    [Fact]
    public void ToBinaryString_WithCustomSeparator_ShouldUseCustomSeparator()
    {
        // Arrange
        var data = new byte[] { 0b00001111, 0b11110000 };
        var separator = "-";

        // Act
        var result = data.ToBinaryString(separator);

        // Assert
        result.Should().Be("00001111-11110000");
    }

    [Fact]
    public void ToBinaryString_WithEmptyData_ShouldReturnEmptyString()
    {
        // Arrange
        var data = Array.Empty<byte>();

        // Act
        var result = data.ToBinaryString();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void FromBinaryString_WithValidBinaryString_ShouldReturnCorrectByteArray()
    {
        // Arrange
        var binaryString = "10101010 11110000";

        // Act
        var result = ByteArrayUtilities.FromBinaryString(binaryString);

        // Assert
        result.Should().BeEquivalentTo(new byte[] { 0b10101010, 0b11110000 });
    }

    [Fact]
    public void FromBinaryString_WithCustomSeparator_ShouldParseCorrectly()
    {
        // Arrange
        var binaryString = "00001111|11110000";
        var separator = "|";

        // Act
        var result = ByteArrayUtilities.FromBinaryString(binaryString, separator);

        // Assert
        result.Should().BeEquivalentTo(new byte[] { 0b00001111, 0b11110000 });
    }

    [Fact]
    public void BinaryString_Roundtrip_ShouldPreserveOriginalData()
    {
        // Arrange
        var originalData = new byte[] { 0, 1, 127, 128, 255 };

        // Act
        var binaryString = originalData.ToBinaryString();
        var roundtripData = ByteArrayUtilities.FromBinaryString(binaryString);

        // Assert
        roundtripData.Should().BeEquivalentTo(originalData);
    }

    [Fact]
    public void FromBinaryString_WithInvalidBitCount_ShouldThrowArgumentException()
    {
        // Arrange
        var invalidBinaryString = "1010101 11110000"; // First part has 7 bits instead of 8

        // Act
        Action act = () => ByteArrayUtilities.FromBinaryString(invalidBinaryString);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Each byte must be exactly 8 bits*");
    }

    [Fact]
    public void FromBinaryString_WithInvalidCharacters_ShouldThrowFormatException()
    {
        // Arrange
        var invalidBinaryString = "1010X010 11110000"; // Contains 'X'

        // Act
        Action act = () => ByteArrayUtilities.FromBinaryString(invalidBinaryString);

        // Assert
        act.Should().Throw<FormatException>();
    }

    #endregion

    #region Hex Dump Tests

    [Fact]
    public void ToHexDump_WithValidData_ShouldReturnFormattedHexDump()
    {
        // Arrange
        var data = new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x20, 0x57, 0x6F, 0x72, 0x6C, 0x64, 0x21 }; // "Hello World!"

        // Act
        var result = data.ToHexDump();

        // Assert
        result.Should().Contain("00000000:");
        result.Should().Contain("48 65 6C 6C 6F 20 57 6F");
        result.Should().Contain("72 6C 64 21");
        result.Should().Contain("|Hello World!|");
    }

    [Fact]
    public void ToHexDump_WithCustomBytesPerLine_ShouldUseCustomLineLength()
    {
        // Arrange
        var data = Enumerable.Range(0, 20).Select(i => (byte)i).ToArray();

        // Act
        var result = data.ToHexDump(bytesPerLine: 8);

        // Assert
        var lines = result.Split('\n');
        lines.Should().HaveCountGreaterThan(2); // Should span multiple lines
        lines[0].Should().Contain("00 01 02 03").And.Contain("04 05 06 07");
    }

    [Fact]
    public void ToHexDump_WithoutAscii_ShouldNotIncludeAsciiRepresentation()
    {
        // Arrange
        var data = new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F }; // "Hello"

        // Act
        var result = data.ToHexDump(showAscii: false);

        // Assert
        result.Should().NotContain("|");
        result.Should().NotContain("Hello");
        result.Should().Contain("48 65 6C 6C 6F");
    }

    [Fact]
    public void ToHexDump_WithoutOffsets_ShouldNotIncludeOffsets()
    {
        // Arrange
        var data = new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F };

        // Act
        var result = data.ToHexDump(showOffsets: false);

        // Assert
        result.Should().NotContain("00000000:");
        result.Should().Contain("48 65 6C 6C 6F");
    }

    [Fact]
    public void ToHexDump_WithNonPrintableCharacters_ShouldShowDots()
    {
        // Arrange
        var data = new byte[] { 0x00, 0x01, 0x1F, 0x20, 0x7E, 0x7F, 0xFF };

        // Act
        var result = data.ToHexDump();

        // Assert
        result.Should().Contain("... ~..");
    }

    [Fact]
    public void ToHexDump_WithEmptyData_ShouldReturnEmptyString()
    {
        // Arrange
        var data = Array.Empty<byte>();

        // Act
        var result = data.ToHexDump();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void ToHexDump_WithLargeData_ShouldHandleMultipleLines()
    {
        // Arrange
        var data = Enumerable.Range(0, 256).Select(i => (byte)i).ToArray();

        // Act
        var result = data.ToHexDump();

        // Assert
        var lines = result.Split('\n');
        lines.Should().HaveCount(16); // 256 bytes / 16 bytes per line = 16 lines
        lines[0].Should().StartWith("00000000:");
        lines[15].Should().StartWith("000000F0:");
    }

    #endregion

    #region Basic Analysis Tests

    [Fact]
    public void CalculateEntropy_WithRandomData_ShouldReturnHighEntropy()
    {
        // Arrange
        var random = new Random(42);
        var data = new byte[10000];
        random.NextBytes(data);

        // Act
        var entropy = data.CalculateEntropy();

        // Assert
        entropy.Should().BeGreaterThan(7.0); // Random data should have high entropy
    }

    [Fact]
    public void CalculateEntropy_WithRepeatingData_ShouldReturnLowEntropy()
    {
        // Arrange
        var data = Enumerable.Repeat((byte)0xAA, 1000).ToArray();

        // Act
        var entropy = data.CalculateEntropy();

        // Assert
        entropy.Should().Be(0.0); // All same bytes = zero entropy
    }

    [Fact]
    public void CalculateEntropy_WithTwoDifferentBytes_ShouldReturnOne()
    {
        // Arrange
        var data = new byte[1000];
        for (var i = 0; i < data.Length; i++)
        {
            data[i] = (byte)(i % 2); // Alternating 0 and 1
        }

        // Act
        var entropy = data.CalculateEntropy();

        // Assert
        entropy.Should().BeApproximately(1.0, 0.01); // Perfect 50/50 distribution = 1 bit entropy
    }

    [Fact]
    public void CalculateEntropy_WithEmptyData_ShouldReturnZero()
    {
        // Arrange
        var data = Array.Empty<byte>();

        // Act
        var entropy = data.CalculateEntropy();

        // Assert
        entropy.Should().Be(0.0);
    }

    [Fact]
    public void GetByteFrequency_WithVariousBytes_ShouldReturnCorrectFrequencies()
    {
        // Arrange
        var data = new byte[] { 1, 2, 2, 3, 3, 3, 4, 4, 4, 4 };

        // Act
        var frequency = data.GetByteFrequency();

        // Assert
        frequency.Should().HaveCount(4);
        frequency[1].Should().Be(1);
        frequency[2].Should().Be(2);
        frequency[3].Should().Be(3);
        frequency[4].Should().Be(4);
    }

    [Fact]
    public void GetByteFrequency_WithEmptyData_ShouldReturnEmptyDictionary()
    {
        // Arrange
        var data = Array.Empty<byte>();

        // Act
        var frequency = data.GetByteFrequency();

        // Assert
        frequency.Should().BeEmpty();
    }

    [Fact]
    public void AppearsCompressed_WithHighEntropyData_ShouldReturnTrue()
    {
        // Arrange
        var random = new Random(42);
        var data = new byte[1000];
        random.NextBytes(data);

        // Act
        var result = data.AppearsCompressed();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void AppearsCompressed_WithLowEntropyData_ShouldReturnFalse()
    {
        // Arrange
        var data = Enumerable.Repeat((byte)0x55, 1000).ToArray();

        // Act
        var result = data.AppearsCompressed();

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region Performance Measurement Tests

    [Fact]
    public void MeasurePerformance_WithSimpleOperation_ShouldReturnValidMeasurement()
    {
        // Arrange
        var data = new byte[1000];
        new Random(42).NextBytes(data);

        // Act
        var (result, totalTime, averageTime) = data.MeasurePerformance(
            array => array.Sum(b => (int)b),
            iterations: 10);

        // Assert
        result.Should().BeGreaterThan(0);
        totalTime.Should().BeGreaterThan(TimeSpan.Zero);
        averageTime.Should().BeGreaterThan(TimeSpan.Zero);
        averageTime.Should().BeLessOrEqualTo(totalTime);
    }

    [Fact]
    public void MeasurePerformance_WithSingleIteration_ShouldReturnSameTimesForTotalAndAverage()
    {
        // Arrange
        var data = new byte[] { 1, 2, 3, 4, 5 };

        // Act
        var (result, totalTime, averageTime) = data.MeasurePerformance(
            array => array.Length,
            iterations: 1);

        // Assert
        result.Should().Be(5);
        totalTime.Should().Be(averageTime);
    }

    [Fact]
    public void CalculateThroughput_WithValidData_ShouldReturnCorrectThroughput()
    {
        // Arrange
        var data = new byte[1000];
        var elapsedTime = TimeSpan.FromSeconds(1);

        // Act
        var throughput = data.CalculateThroughput(elapsedTime);

        // Assert
        throughput.Should().Be(1000.0); // 1000 bytes per second
    }

    [Fact]
    public void CalculateThroughput_WithZeroTime_ShouldReturnZero()
    {
        // Arrange
        var data = new byte[1000];
        var elapsedTime = TimeSpan.Zero;

        // Act
        var throughput = data.CalculateThroughput(elapsedTime);

        // Assert
        throughput.Should().Be(0.0);
    }

    [Fact]
    public void CalculateThroughput_WithNegativeTime_ShouldReturnZero()
    {
        // Arrange
        var data = new byte[1000];
        var elapsedTime = TimeSpan.FromSeconds(-1);

        // Act
        var throughput = data.CalculateThroughput(elapsedTime);

        // Assert
        throughput.Should().Be(0.0);
    }

    #endregion

    #region Error Handling Tests

    [Fact]
    public void ToBinaryString_WithNullArray_ShouldThrowArgumentNullException()
    {
        // Arrange
        byte[] data = null!;

        // Act
        Action act = () => data.ToBinaryString();

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ToBinaryString_WithNullSeparator_ShouldThrowArgumentNullException()
    {
        // Arrange
        var data = new byte[] { 1, 2, 3 };

        // Act
        Action act = () => data.ToBinaryString(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void FromBinaryString_WithNullString_ShouldThrowArgumentException()
    {
        // Arrange
        string binaryString = null!;

        // Act
        Action act = () => ByteArrayUtilities.FromBinaryString(binaryString);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void FromBinaryString_WithEmptyString_ShouldThrowArgumentException()
    {
        // Arrange
        var binaryString = "";

        // Act
        Action act = () => ByteArrayUtilities.FromBinaryString(binaryString);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void FromBinaryString_WithNullSeparator_ShouldThrowArgumentNullException()
    {
        // Arrange
        var binaryString = "10101010 11110000";

        // Act
        Action act = () => ByteArrayUtilities.FromBinaryString(binaryString, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ToHexDump_WithNullArray_ShouldThrowArgumentNullException()
    {
        // Arrange
        byte[] data = null!;

        // Act
        Action act = () => data.ToHexDump();

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ToHexDump_WithInvalidBytesPerLine_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var data = new byte[] { 1, 2, 3 };

        // Act
        Action act = () => data.ToHexDump(bytesPerLine: 0);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void CalculateEntropy_WithNullArray_ShouldThrowArgumentNullException()
    {
        // Arrange
        byte[] data = null!;

        // Act
        Action act = () => data.CalculateEntropy();

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void GetByteFrequency_WithNullArray_ShouldThrowArgumentNullException()
    {
        // Arrange
        byte[] data = null!;

        // Act
        Action act = () => data.GetByteFrequency();

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void MeasurePerformance_WithNullArray_ShouldThrowArgumentNullException()
    {
        // Arrange
        byte[] data = null!;

        // Act
        Action act = () => data.MeasurePerformance(array => array.Length);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void MeasurePerformance_WithNullOperation_ShouldThrowArgumentNullException()
    {
        // Arrange
        var data = new byte[] { 1, 2, 3 };
        Func<byte[], int> operation = null!;

        // Act
        Action act = () => data.MeasurePerformance(operation);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void MeasurePerformance_WithInvalidIterations_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var data = new byte[] { 1, 2, 3 };

        // Act
        Action act = () => data.MeasurePerformance(array => array.Length, iterations: 0);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void CalculateThroughput_WithNullArray_ShouldThrowArgumentNullException()
    {
        // Arrange
        byte[] data = null!;

        // Act
        Action act = () => data.CalculateThroughput(TimeSpan.FromSeconds(1));

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void BinaryStringAndHexDump_Integration_ShouldWorkTogether()
    {
        // Arrange
        var originalData = new byte[] { 0xAA, 0xBB, 0xCC, 0xDD };

        // Act
        var binaryString = originalData.ToBinaryString();
        var hexDump = originalData.ToHexDump();
        var roundtripData = ByteArrayUtilities.FromBinaryString(binaryString);

        // Assert
        roundtripData.Should().BeEquivalentTo(originalData);
        hexDump.Should().Contain("AA BB CC DD");
        binaryString.Should().Contain("10101010"); // 0xAA in binary
    }

    [Fact]
    public void EntropyAndFrequency_Integration_ShouldBeConsistent()
    {
        // Arrange
        var data = new byte[] { 0, 0, 1, 1, 2, 2, 3, 3 }; // Even distribution

        // Act
        var entropy = data.CalculateEntropy();
        var frequency = data.GetByteFrequency();

        // Assert
        entropy.Should().BeApproximately(2.0, 0.01); // log2(4) = 2 for perfect 4-way distribution
        frequency.Should().HaveCount(4);
        frequency.Values.Should().AllSatisfy(x => x.Should().Be(2)); // Each value appears twice
    }

    [Fact]
    public void PerformanceAndThroughput_Integration_ShouldBeConsistent()
    {
        // Arrange
        var data = new byte[10000];

        // Act
        var (result, totalTime, averageTime) = data.MeasurePerformance(
            array => array.Sum(b => (int)b),
            iterations: 5);

        var throughput = data.CalculateThroughput(averageTime);

        // Assert
        result.Should().BeGreaterOrEqualTo(0);
        throughput.Should().BeGreaterThan(0);
        (throughput * averageTime.TotalSeconds).Should().BeApproximately(data.Length, data.Length * 0.1);
    }

    #endregion
}
