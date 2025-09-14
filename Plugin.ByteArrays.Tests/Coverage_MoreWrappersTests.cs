using System;
using System.Text;
using FluentAssertions;
using Plugin.ByteArrays;
using Xunit;

namespace Plugin.ByteArrays.Tests;

public class Coverage_MoreWrappersTests
{
    [Fact]
    public void String_NonRef_Wrappers_Covered()
    {
        var utf8 = Encoding.UTF8.GetBytes("hello");
        utf8.ToUtf8String(); // non-ref
        utf8.ToUtf8StringOrDefault(defaultValue: "d");

        var ascii = Encoding.ASCII.GetBytes("abc");
        ascii.ToAsciiString();
        ascii.ToAsciiStringOrDefault(defaultValue: "x");
    }

    [Fact]
    public void TrimEnd_NoTrimmingNeeded()
    {
        var arr = new byte[] {1,2,3};
        var beforeRef = arr;
        var result = arr.TrimEnd(0);
        result.Should().BeSameAs(beforeRef);
        result.Should().Equal(1,2,3);
    }
}

