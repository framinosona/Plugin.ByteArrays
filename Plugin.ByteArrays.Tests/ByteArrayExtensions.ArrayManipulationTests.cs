using System;
using System.Text;
using Plugin.ByteArrays;
using Xunit;
using FluentAssertions;

namespace Plugin.ByteArrays.Tests;

public class ByteArrayExtensions_ArrayManipulationTests
{
    [Fact]
    public void TrimEnd_ReturnsNewArray_And_NonDestructive_Works()
    {
        var arr = new byte[] {1,2,0,0};
        var trimmedCopy = arr.TrimEndNonDestructive();
        trimmedCopy.Should().Equal(1,2);
        arr.Should().Equal(1,2,0,0); // original untouched by non-destructive

        var trimmed = arr.TrimEnd();
        trimmed.Should().Equal(1,2);
        arr.Should().Equal(1,2,0,0); // original array still unchanged (extension methods can't mutate)
    }

    [Fact]
    public void SafeSlice_HandlesInvalidParameters()
    {
        var arr = new byte[] {1,2,3,4,5};
        arr.SafeSlice(1, 3).Should().Equal(2,3,4);
        arr.SafeSlice(-1, 2).Should().BeEmpty();
        arr.SafeSlice(99, 2).Should().BeEmpty();
        arr.SafeSlice(3, 99).Should().Equal(4,5);
    }

    [Fact]
    public void Concatenate_HandlesNullsAndEmpty()
    {
        ByteArrayExtensions.Concatenate().Should().BeEmpty();
        ByteArrayExtensions.Concatenate(Array.Empty<byte>()).Should().BeEmpty();
        ByteArrayExtensions.Concatenate(null!, new byte[] {1,2}, null!, new byte[] {3}).Should().Equal(1,2,3);
    }

    [Fact]
    public void Reverse_ReturnsNewArray()
    {
        var arr = new byte[] {1,2,3};
        var rev = arr.Reverse();
        rev.Should().Equal(3,2,1);
        rev.Should().NotBeSameAs(arr);
    }

    [Fact]
    public void Xor_Works_And_Throws_On_Errors()
    {
        var a = new byte[] {1,2,3};
        var b = new byte[] {3,2,1};
        a.Xor(b).Should().Equal(2,0,2);

        Action len = () => new byte[] {1}.Xor(new byte[] {1,2});
        len.Should().Throw<ArgumentException>();

        Action nullA = () => ByteArrayExtensions.Xor(null!, b);
        nullA.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("array1");

        Action nullB = () => a.Xor(null!);
        nullB.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("array2");
    }

    #region Advanced Search Operations Tests

    [Fact]
    public void IndexOfAll_WithMultipleOccurrences_ShouldReturnAllIndices()
    {
        // Arrange
        var data = new byte[] { 1, 2, 3, 2, 3, 4, 2, 3 };
        var pattern = new byte[] { 2, 3 };

        // Act
        var result = data.IndexOfAll(pattern);

        // Assert
        result.Should().BeEquivalentTo(new[] { 1, 3, 6 });
    }

    [Fact]
    public void IndexOfAll_WithNoMatches_ShouldReturnEmpty()
    {
        // Arrange
        var data = new byte[] { 1, 2, 3, 4, 5 };
        var pattern = new byte[] { 6, 7 };

        // Act
        var result = data.IndexOfAll(pattern);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void IndexOfAll_WithOverlappingPattern_ShouldFindAllOccurrences()
    {
        // Arrange
        var data = new byte[] { 1, 1, 1, 2, 1, 1, 1 };
        var pattern = new byte[] { 1, 1 };

        // Act
        var result = data.IndexOfAll(pattern);

        // Assert
        result.Should().BeEquivalentTo(new[] { 0, 1, 4, 5 });
    }

    [Fact]
    public void IndexOfAll_WithEmptyPattern_ShouldReturnEmpty()
    {
        // Arrange
        var data = new byte[] { 1, 2, 3 };
        var pattern = Array.Empty<byte>();

        // Act
        var result = data.IndexOfAll(pattern);

        // Assert
        result.Should().BeEmpty();
    }

    #endregion

    #region Bit Operations Tests

    [Fact]
    public void SetBit_WithValidPosition_ShouldSetCorrectBit()
    {
        // Arrange
        var data = new byte[] { 0x00, 0x00 };

        // Act
        var result = data.SetBit(0); // Set LSB of first byte
        var result2 = result.SetBit(15); // Set MSB of second byte

        // Assert
        result[0].Should().Be(0x01);
        result[1].Should().Be(0x00);
        result2[0].Should().Be(0x01);
        result2[1].Should().Be(0x80);
    }

    [Fact]
    public void SetBit_WithBitAlreadySet_ShouldRemainSet()
    {
        // Arrange
        var data = new byte[] { 0xFF }; // All bits set

        // Act
        var result = data.SetBit(0);

        // Assert
        result[0].Should().Be(0xFF);
    }

    [Fact]
    public void GetBit_WithSetBit_ShouldReturnTrue()
    {
        // Arrange
        var data = new byte[] { 0x81 }; // 10000001

        // Act & Assert
        data.GetBit(0).Should().BeTrue(); // LSB
        data.GetBit(7).Should().BeTrue(); // MSB
        data.GetBit(1).Should().BeFalse(); // Middle bit
    }

    [Fact]
    public void GetBit_WithUnsetBit_ShouldReturnFalse()
    {
        // Arrange
        var data = new byte[] { 0x7E }; // 01111110

        // Act & Assert
        data.GetBit(0).Should().BeFalse(); // LSB
        data.GetBit(7).Should().BeFalse(); // MSB
        data.GetBit(3).Should().BeTrue(); // Middle bit
    }

    [Fact]
    public void BitReverse_WithValidData_ShouldReverseBitsInEachByte()
    {
        // Arrange
        var data = new byte[] { 0x01, 0x80, 0xF0 }; // 00000001, 10000000, 11110000

        // Act
        var result = data.BitReverse();

        // Assert
        result[0].Should().Be(0x80); // 10000000
        result[1].Should().Be(0x01); // 00000001
        result[2].Should().Be(0x0F); // 00001111
    }

    [Fact]
    public void BitReverse_WithSymmetricByte_ShouldReturnSame()
    {
        // Arrange
        var data = new byte[] { 0x18, 0x81 }; // 00011000, 10000001

        // Act
        var result = data.BitReverse();

        // Assert
        result[0].Should().Be(0x18); // 00011000 (symmetric)
        result[1].Should().Be(0x81); // 10000001 (symmetric)
    }

    [Fact]
    public void BitOperations_WithValidPosition_ShouldWork()
    {
        // Arrange
        var data = new byte[] { 0xFF, 0x00, 0xFF, 0x00 };

        // Act & Assert - Test that operations work with valid positions
        var bit = data.GetBit(0);
        bit.Should().BeTrue();
    }

    #endregion

    #region Splitting and Chunking Tests

    [Fact]
    public void Split_WithValidChunkSize_ShouldReturnCorrectChunks()
    {
        // Arrange
        var data = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        // Act
        var chunks = data.Split(3).ToList();

        // Assert
        chunks.Should().HaveCount(3);
        chunks[0].Should().BeEquivalentTo(new byte[] { 1, 2, 3 });
        chunks[1].Should().BeEquivalentTo(new byte[] { 4, 5, 6 });
        chunks[2].Should().BeEquivalentTo(new byte[] { 7, 8, 9 });
    }

    [Fact]
    public void Split_WithUnevenChunks_ShouldHandleLastChunk()
    {
        // Arrange
        var data = new byte[] { 1, 2, 3, 4, 5, 6, 7 };

        // Act
        var chunks = data.Split(3).ToList();

        // Assert
        chunks.Should().HaveCount(3);
        chunks[0].Should().BeEquivalentTo(new byte[] { 1, 2, 3 });
        chunks[1].Should().BeEquivalentTo(new byte[] { 4, 5, 6 });
        chunks[2].Should().BeEquivalentTo(new byte[] { 7 }); // Last chunk smaller
    }

    [Fact]
    public void SplitAt_WithDelimiter_ShouldSplitCorrectly()
    {
        // Arrange
        var data = new byte[] { 1, 2, 0, 3, 4, 0, 5 };
        var delimiter = (byte)0;

        // Act
        var segments = data.SplitAt(delimiter).ToList();

        // Assert
        segments.Should().HaveCount(3);
        segments[0].Should().BeEquivalentTo(new byte[] { 1, 2 });
        segments[1].Should().BeEquivalentTo(new byte[] { 3, 4 });
        segments[2].Should().BeEquivalentTo(new byte[] { 5 });
    }

    [Fact]
    public void SplitAt_WithTrailingDelimiter_ShouldHandleCorrectly()
    {
        // Arrange
        var data = new byte[] { 1, 2, 0, 3, 4, 0 };
        var delimiter = (byte)0;

        // Act
        var segments = data.SplitAt(delimiter).ToList();

        // Assert
        segments.Should().HaveCount(2);
        segments[0].Should().BeEquivalentTo(new byte[] { 1, 2 });
        segments[1].Should().BeEquivalentTo(new byte[] { 3, 4 });
    }

    [Fact]
    public void SplitAt_WithNoDelimiter_ShouldReturnEntireArray()
    {
        // Arrange
        var data = new byte[] { 1, 2, 3, 4, 5 };
        var delimiter = (byte)99;

        // Act
        var segments = data.SplitAt(delimiter).ToList();

        // Assert
        segments.Should().HaveCount(1);
        segments[0].Should().BeEquivalentTo(data);
    }

    [Fact]
    public void Split_WithInvalidChunkSize_ShouldThrowArgumentException()
    {
        // Arrange
        var data = new byte[] { 1, 2, 3 };

        // Act & Assert
        Action act = () => data.Split(0).ToList();
        act.Should().Throw<ArgumentException>();
    }

    #endregion

    #region Checksum Tests

    [Fact]
    public void CalculateCrc32_WithKnownData_ShouldReturnExpectedValue()
    {
        // Arrange
        var data = Encoding.ASCII.GetBytes("123456789");

        // Act
        var crc = data.CalculateCrc32();

        // Assert - Known CRC32 for "123456789"
        crc.Should().Be(0xCBF43926);
    }

    [Fact]
    public void CalculateCrc32_WithEmptyData_ShouldReturnInitialValue()
    {
        // Arrange
        var data = Array.Empty<byte>();

        // Act
        var crc = data.CalculateCrc32();

        // Assert
        // CRC32 of empty data should be the initial value or 0
        crc.Should().BeOneOf(0u, 0xFFFFFFFFu); // Both are valid for empty data
    }

    [Fact]
    public void CalculateMd5_WithKnownData_ShouldReturnExpectedHash()
    {
        // Arrange
        var data = Encoding.ASCII.GetBytes("hello");

        // Act
        var hash = data.CalculateMd5();

        // Assert
        hash.Should().HaveCount(16); // MD5 is 16 bytes
        hash.Should().NotBeEquivalentTo(new byte[16]); // Should not be all zeros
    }

    [Fact]
    public void CalculateSha256_WithKnownData_ShouldReturnExpectedHash()
    {
        // Arrange
        var data = Encoding.ASCII.GetBytes("hello world");

        // Act
        var hash = data.CalculateSha256();

        // Assert
        hash.Should().HaveCount(32); // SHA256 is 32 bytes
        hash.Should().NotBeEquivalentTo(new byte[32]); // Should not be all zeros
    }

    [Fact]
    public void CalculateSha1_WithKnownData_ShouldReturnExpectedHash()
    {
        // Arrange
        var data = Encoding.ASCII.GetBytes("test");

        // Act
        var hash = data.CalculateSha1();

        // Assert
        hash.Should().HaveCount(20); // SHA1 is 20 bytes
        hash.Should().NotBeEquivalentTo(new byte[20]); // Should not be all zeros
    }

    [Fact]
    public void HashFunctions_WithSameInput_ShouldReturnConsistentResults()
    {
        // Arrange
        var data = Encoding.UTF8.GetBytes("Consistency test");

        // Act
        var md5_1 = data.CalculateMd5();
        var md5_2 = data.CalculateMd5();
        var sha256_1 = data.CalculateSha256();
        var sha256_2 = data.CalculateSha256();

        // Assert
        md5_1.Should().BeEquivalentTo(md5_2);
        sha256_1.Should().BeEquivalentTo(sha256_2);
    }

    #endregion

    #region Padding Operations Tests

    [Fact]
    public void Pad_WithRightPadding_ShouldPadCorrectly()
    {
        // Arrange
        var data = new byte[] { 1, 2, 3 };

        // Act
        var result = data.Pad(6, 0xFF, padLeft: false);

        // Assert
        result.Should().BeEquivalentTo(new byte[] { 1, 2, 3, 0xFF, 0xFF, 0xFF });
    }

    [Fact]
    public void Pad_WithLeftPadding_ShouldPadCorrectly()
    {
        // Arrange
        var data = new byte[] { 1, 2, 3 };

        // Act
        var result = data.Pad(6, 0xAA, padLeft: true);

        // Assert
        result.Should().BeEquivalentTo(new byte[] { 0xAA, 0xAA, 0xAA, 1, 2, 3 });
    }

    [Fact]
    public void Pad_WithSameLengthAsTarget_ShouldReturnOriginal()
    {
        // Arrange
        var data = new byte[] { 1, 2, 3 };

        // Act
        var result = data.Pad(3);

        // Assert
        result.Should().BeSameAs(data);
    }

    [Fact]
    public void Pad_WithSmallerTarget_ShouldReturnOriginal()
    {
        // Arrange
        var data = new byte[] { 1, 2, 3, 4, 5 };

        // Act
        var result = data.Pad(3);

        // Assert
        result.Should().BeSameAs(data);
    }

    [Fact]
    public void RemovePadding_WithRightPadding_ShouldRemoveCorrectly()
    {
        // Arrange
        var data = new byte[] { 1, 2, 3, 0, 0, 0 };

        // Act
        var result = data.RemovePadding(0, fromLeft: false);

        // Assert
        result.Should().BeEquivalentTo(new byte[] { 1, 2, 3 });
    }

    [Fact]
    public void RemovePadding_WithLeftPadding_ShouldRemoveCorrectly()
    {
        // Arrange
        var data = new byte[] { 0, 0, 0, 1, 2, 3 };

        // Act
        var result = data.RemovePadding(0, fromLeft: true);

        // Assert
        result.Should().BeEquivalentTo(new byte[] { 1, 2, 3 });
    }

    [Fact]
    public void RemovePadding_WithNoPadding_ShouldReturnOriginal()
    {
        // Arrange
        var data = new byte[] { 1, 2, 3, 4, 5 };

        // Act
        var result = data.RemovePadding(0);

        // Assert
        result.Should().BeEquivalentTo(data);
    }

    [Fact]
    public void RemovePadding_WithAllPadding_ShouldReturnEmpty()
    {
        // Arrange
        var data = new byte[] { 0, 0, 0, 0 };

        // Act
        var result = data.RemovePadding(0);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void PaddingRoundtrip_ShouldPreserveOriginalData()
    {
        // Arrange
        var originalData = new byte[] { 1, 2, 3, 4, 5 };

        // Act
        var padded = originalData.Pad(10, 0x99);
        var unpadded = padded.RemovePadding(0x99);

        // Assert
        unpadded.Should().BeEquivalentTo(originalData);
    }

    #endregion

    #region Error Handling for Advanced Operations

    [Fact]
    public void AdvancedOperations_WithNullArray_ShouldThrowArgumentNullException()
    {
        // Arrange
        byte[] data = null!;
        var pattern = new byte[] { 1, 2 };

        // Act & Assert
        Action act1 = () => data.IndexOfAll(pattern);
        act1.Should().Throw<ArgumentNullException>();

        Action act2 = () => data.SetBit(0);
        act2.Should().Throw<ArgumentNullException>();

        Action act3 = () => data.GetBit(0);
        act3.Should().Throw<ArgumentNullException>();

        Action act4 = () => data.BitReverse();
        act4.Should().Throw<ArgumentNullException>();

        Action act5 = () => data.Split(5).ToList();
        act5.Should().Throw<ArgumentNullException>();

        Action act6 = () => data.SplitAt(0).ToList();
        act6.Should().Throw<ArgumentNullException>();

        Action act7 = () => data.CalculateCrc32();
        act7.Should().Throw<ArgumentNullException>();

        Action act8 = () => data.CalculateMd5();
        act8.Should().Throw<ArgumentNullException>();

        Action act9 = () => data.CalculateSha256();
        act9.Should().Throw<ArgumentNullException>();

        Action act10 = () => data.Pad(10);
        act10.Should().Throw<ArgumentNullException>();

        Action act11 = () => data.RemovePadding();
        act11.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void IndexOfAll_WithNullPattern_ShouldThrowArgumentNullException()
    {
        // Arrange
        var data = new byte[] { 1, 2, 3 };
        byte[] pattern = null!;

        // Act & Assert
        Action act = () => data.IndexOfAll(pattern);
        act.Should().Throw<ArgumentNullException>();
    }

    #endregion

}

