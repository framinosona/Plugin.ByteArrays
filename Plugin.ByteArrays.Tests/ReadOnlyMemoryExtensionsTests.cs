using FluentAssertions;
using Xunit;

namespace Plugin.ByteArrays.Tests;

public class ReadOnlyMemoryExtensionsTests
{
    [Fact]
    public void ToDebugString_EmptyMemory_ReturnsEmptyBrackets()
    {
        ReadOnlyMemory<byte> memory = ReadOnlyMemory<byte>.Empty;
        memory.ToDebugString().Should().Be("[]");
    }

    [Fact]
    public void ToDebugString_WithBytes_ReturnsFormattedString()
    {
        ReadOnlyMemory<byte> memory = new byte[] { 1, 2, 3, 255 };
        memory.ToDebugString().Should().Be("[1,2,3,255]");
    }

    [Fact]
    public void ToHexDebugString_EmptyMemory_ReturnsEmptyBrackets()
    {
        ReadOnlyMemory<byte> memory = ReadOnlyMemory<byte>.Empty;
        memory.ToHexDebugString().Should().Be("[]");
    }

    [Fact]
    public void ToHexDebugString_WithBytes_ReturnsFormattedString()
    {
        ReadOnlyMemory<byte> memory = new byte[] { 1, 2, 15, 255 };
        memory.ToHexDebugString().Should().Be("[01,02,0F,FF]");
    }

    [Fact]
    public void StartsWith_MatchingPattern_ReturnsTrue()
    {
        ReadOnlyMemory<byte> memory = new byte[] { 1, 2, 3, 4, 5 };
        ReadOnlySpan<byte> pattern = new byte[] { 1, 2, 3 };
        memory.StartsWith(pattern).Should().BeTrue();
    }

    [Fact]
    public void StartsWith_NonMatchingPattern_ReturnsFalse()
    {
        ReadOnlyMemory<byte> memory = new byte[] { 1, 2, 3, 4, 5 };
        ReadOnlySpan<byte> pattern = new byte[] { 2, 3, 4 };
        memory.StartsWith(pattern).Should().BeFalse();
    }

    [Fact]
    public void EndsWith_MatchingPattern_ReturnsTrue()
    {
        ReadOnlyMemory<byte> memory = new byte[] { 1, 2, 3, 4, 5 };
        ReadOnlySpan<byte> pattern = new byte[] { 3, 4, 5 };
        memory.EndsWith(pattern).Should().BeTrue();
    }

    [Fact]
    public void EndsWith_NonMatchingPattern_ReturnsFalse()
    {
        ReadOnlyMemory<byte> memory = new byte[] { 1, 2, 3, 4, 5 };
        ReadOnlySpan<byte> pattern = new byte[] { 2, 3, 4 };
        memory.EndsWith(pattern).Should().BeFalse();
    }

    [Fact]
    public void IndexOf_PatternInMiddle_ReturnsCorrectIndex()
    {
        ReadOnlyMemory<byte> memory = new byte[] { 1, 2, 3, 4, 5 };
        ReadOnlySpan<byte> pattern = new byte[] { 3, 4 };
        memory.IndexOf(pattern).Should().Be(2);
    }

    [Fact]
    public void IndexOf_PatternNotFound_ReturnsMinusOne()
    {
        ReadOnlyMemory<byte> memory = new byte[] { 1, 2, 3, 4, 5 };
        ReadOnlySpan<byte> pattern = new byte[] { 6, 7 };
        memory.IndexOf(pattern).Should().Be(-1);
    }

    [Fact]
    public void IsIdenticalTo_IdenticalMemories_ReturnsTrue()
    {
        ReadOnlyMemory<byte> memory1 = new byte[] { 1, 2, 3, 4, 5 };
        ReadOnlyMemory<byte> memory2 = new byte[] { 1, 2, 3, 4, 5 };
        memory1.IsIdenticalTo(memory2).Should().BeTrue();
    }

    [Fact]
    public void IsIdenticalTo_DifferentMemories_ReturnsFalse()
    {
        ReadOnlyMemory<byte> memory1 = new byte[] { 1, 2, 3, 4, 5 };
        ReadOnlyMemory<byte> memory2 = new byte[] { 1, 2, 3, 4, 6 };
        memory1.IsIdenticalTo(memory2).Should().BeFalse();
    }

    [Fact]
    public void IsIdenticalTo_BothEmpty_ReturnsTrue()
    {
        ReadOnlyMemory<byte> memory1 = ReadOnlyMemory<byte>.Empty;
        ReadOnlyMemory<byte> memory2 = ReadOnlyMemory<byte>.Empty;
        memory1.IsIdenticalTo(memory2).Should().BeTrue();
    }
}
