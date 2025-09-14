using System;
using System.Linq;
using Plugin.ByteArrays;
using Xunit;
using FluentAssertions;

namespace Plugin.ByteArrays.Tests;

public class ByteArrayExtensions_CoreTests
{
    [Fact]
    public void ToDebugString_Null_ReturnsNullTag()
    {
        IEnumerable<byte>? input = null;
        input.ToDebugString().Should().Be("<null>");
    }

    [Fact]
    public void ToDebugString_WithContent_ReturnsCommaSeparated()
    {
        new byte[] {1,2,3}.ToDebugString().Should().Be("[1,2,3]");
    }

    [Fact]
    public void ToHexDebugString_Null_ReturnsNullTag()
    {
        IEnumerable<byte>? input = null;
        input.ToHexDebugString().Should().Be("<null>");
    }

    [Fact]
    public void ToHexDebugString_WithContent_ReturnsHexPairs()
    {
        new byte[] {0, 10, 255}.ToHexDebugString().Should().Be("[00,0A,FF]");
    }

    [Theory]
    [InlineData(new byte[] {1,2,3,4}, new byte[] {1,2}, true)]
    [InlineData(new byte[] {1,2}, new byte[] {1,2,3}, false)]
    [InlineData(new byte[] {1,2,3}, new byte[] {}, true)]
    [InlineData(new byte[] {9,9,9}, new byte[] {1}, false)]
    public void StartsWith_Works(byte[] array, byte[] pattern, bool expected)
    {
        array.StartsWith(pattern).Should().Be(expected);
    }

    [Theory]
    [InlineData(new byte[] {1,2,3,4}, new byte[] {3,4}, true)]
    [InlineData(new byte[] {1,2}, new byte[] {1,2,3}, false)]
    [InlineData(new byte[] {1,2,3}, new byte[] {}, true)]
    [InlineData(new byte[] {9,9,9}, new byte[] {1}, false)]
    public void EndsWith_Works(byte[] array, byte[] pattern, bool expected)
    {
        array.EndsWith(pattern).Should().Be(expected);
    }

    [Fact]
    public void IndexOf_FindsPatternOrNot()
    {
        var arr = new byte[] {1,2,3,2,3,4};
        arr.IndexOf(new byte[] {2,3}).Should().Be(1);
        arr.IndexOf(new byte[] {3,4,5}).Should().Be(-1);
        arr.IndexOf(Array.Empty<byte>()).Should().Be(0);
    }

    [Fact]
    public void IsIdenticalTo_CoversNullRefAndContent()
    {
        byte[]? a = null;
        byte[]? b = null;
        a.IsIdenticalTo(b).Should().BeTrue();

        a = null;
        b = Array.Empty<byte>();
        a.IsIdenticalTo(b).Should().BeFalse();

        a = new byte[] {1,2,3};
        a.IsIdenticalTo(a).Should().BeTrue();
        a.IsIdenticalTo(new byte[] {1,2,3}).Should().BeTrue();
        a.IsIdenticalTo(new byte[] {1,2,4}).Should().BeFalse();
        a.IsIdenticalTo(new byte[] {1,2}).Should().BeFalse();
    }
}

