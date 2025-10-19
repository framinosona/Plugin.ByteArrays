using FluentAssertions;
using Xunit;

namespace Plugin.ByteArrays.Tests;

public class ReadOnlySpanExtensions_CoreTests
{
    [Fact]
    public void ToDebugString_EmptySpan_ReturnsEmptyBrackets()
    {
        ReadOnlySpan<byte> span = ReadOnlySpan<byte>.Empty;
        span.ToDebugString().Should().Be("[]");
    }

    [Fact]
    public void ToDebugString_WithBytes_ReturnsFormattedString()
    {
        ReadOnlySpan<byte> span = new byte[] { 1, 2, 3, 255 };
        span.ToDebugString().Should().Be("[1,2,3,255]");
    }

    [Fact]
    public void ToHexDebugString_EmptySpan_ReturnsEmptyBrackets()
    {
        ReadOnlySpan<byte> span = ReadOnlySpan<byte>.Empty;
        span.ToHexDebugString().Should().Be("[]");
    }

    [Fact]
    public void ToHexDebugString_WithBytes_ReturnsFormattedString()
    {
        ReadOnlySpan<byte> span = new byte[] { 1, 2, 15, 255 };
        span.ToHexDebugString().Should().Be("[01,02,0F,FF]");
    }

    [Fact]
    public void StartsWith_EmptyPattern_ReturnsTrue()
    {
        ReadOnlySpan<byte> span = new byte[] { 1, 2, 3 };
        ReadOnlySpan<byte> pattern = ReadOnlySpan<byte>.Empty;
        span.StartsWith(pattern).Should().BeTrue();
    }

    [Fact]
    public void StartsWith_MatchingPattern_ReturnsTrue()
    {
        ReadOnlySpan<byte> span = new byte[] { 1, 2, 3, 4, 5 };
        ReadOnlySpan<byte> pattern = new byte[] { 1, 2, 3 };
        span.StartsWith(pattern).Should().BeTrue();
    }

    [Fact]
    public void StartsWith_NonMatchingPattern_ReturnsFalse()
    {
        ReadOnlySpan<byte> span = new byte[] { 1, 2, 3, 4, 5 };
        ReadOnlySpan<byte> pattern = new byte[] { 2, 3, 4 };
        span.StartsWith(pattern).Should().BeFalse();
    }

    [Fact]
    public void StartsWith_PatternLongerThanSpan_ReturnsFalse()
    {
        ReadOnlySpan<byte> span = new byte[] { 1, 2 };
        ReadOnlySpan<byte> pattern = new byte[] { 1, 2, 3 };
        span.StartsWith(pattern).Should().BeFalse();
    }

    [Fact]
    public void EndsWith_EmptyPattern_ReturnsTrue()
    {
        ReadOnlySpan<byte> span = new byte[] { 1, 2, 3 };
        ReadOnlySpan<byte> pattern = ReadOnlySpan<byte>.Empty;
        span.EndsWith(pattern).Should().BeTrue();
    }

    [Fact]
    public void EndsWith_MatchingPattern_ReturnsTrue()
    {
        ReadOnlySpan<byte> span = new byte[] { 1, 2, 3, 4, 5 };
        ReadOnlySpan<byte> pattern = new byte[] { 3, 4, 5 };
        span.EndsWith(pattern).Should().BeTrue();
    }

    [Fact]
    public void EndsWith_NonMatchingPattern_ReturnsFalse()
    {
        ReadOnlySpan<byte> span = new byte[] { 1, 2, 3, 4, 5 };
        ReadOnlySpan<byte> pattern = new byte[] { 2, 3, 4 };
        span.EndsWith(pattern).Should().BeFalse();
    }

    [Fact]
    public void EndsWith_PatternLongerThanSpan_ReturnsFalse()
    {
        ReadOnlySpan<byte> span = new byte[] { 1, 2 };
        ReadOnlySpan<byte> pattern = new byte[] { 1, 2, 3 };
        span.EndsWith(pattern).Should().BeFalse();
    }

    [Fact]
    public void IndexOf_EmptyPattern_ReturnsZero()
    {
        ReadOnlySpan<byte> span = new byte[] { 1, 2, 3 };
        ReadOnlySpan<byte> pattern = ReadOnlySpan<byte>.Empty;
        span.IndexOf(pattern).Should().Be(0);
    }

    [Fact]
    public void IndexOf_PatternAtBeginning_ReturnsZero()
    {
        ReadOnlySpan<byte> span = new byte[] { 1, 2, 3, 4, 5 };
        ReadOnlySpan<byte> pattern = new byte[] { 1, 2 };
        span.IndexOf(pattern).Should().Be(0);
    }

    [Fact]
    public void IndexOf_PatternInMiddle_ReturnsCorrectIndex()
    {
        ReadOnlySpan<byte> span = new byte[] { 1, 2, 3, 4, 5 };
        ReadOnlySpan<byte> pattern = new byte[] { 3, 4 };
        span.IndexOf(pattern).Should().Be(2);
    }

    [Fact]
    public void IndexOf_PatternAtEnd_ReturnsCorrectIndex()
    {
        ReadOnlySpan<byte> span = new byte[] { 1, 2, 3, 4, 5 };
        ReadOnlySpan<byte> pattern = new byte[] { 4, 5 };
        span.IndexOf(pattern).Should().Be(3);
    }

    [Fact]
    public void IndexOf_PatternNotFound_ReturnsMinusOne()
    {
        ReadOnlySpan<byte> span = new byte[] { 1, 2, 3, 4, 5 };
        ReadOnlySpan<byte> pattern = new byte[] { 6, 7 };
        span.IndexOf(pattern).Should().Be(-1);
    }

    [Fact]
    public void IndexOf_PatternLongerThanSpan_ReturnsMinusOne()
    {
        ReadOnlySpan<byte> span = new byte[] { 1, 2 };
        ReadOnlySpan<byte> pattern = new byte[] { 1, 2, 3 };
        span.IndexOf(pattern).Should().Be(-1);
    }

    [Fact]
    public void IsIdenticalTo_IdenticalSpans_ReturnsTrue()
    {
        ReadOnlySpan<byte> span1 = new byte[] { 1, 2, 3, 4, 5 };
        ReadOnlySpan<byte> span2 = new byte[] { 1, 2, 3, 4, 5 };
        span1.IsIdenticalTo(span2).Should().BeTrue();
    }

    [Fact]
    public void IsIdenticalTo_DifferentSpans_ReturnsFalse()
    {
        ReadOnlySpan<byte> span1 = new byte[] { 1, 2, 3, 4, 5 };
        ReadOnlySpan<byte> span2 = new byte[] { 1, 2, 3, 4, 6 };
        span1.IsIdenticalTo(span2).Should().BeFalse();
    }

    [Fact]
    public void IsIdenticalTo_DifferentLengths_ReturnsFalse()
    {
        ReadOnlySpan<byte> span1 = new byte[] { 1, 2, 3 };
        ReadOnlySpan<byte> span2 = new byte[] { 1, 2, 3, 4 };
        span1.IsIdenticalTo(span2).Should().BeFalse();
    }

    [Fact]
    public void IsIdenticalTo_BothEmpty_ReturnsTrue()
    {
        ReadOnlySpan<byte> span1 = ReadOnlySpan<byte>.Empty;
        ReadOnlySpan<byte> span2 = ReadOnlySpan<byte>.Empty;
        span1.IsIdenticalTo(span2).Should().BeTrue();
    }
}
