using System;
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
}

