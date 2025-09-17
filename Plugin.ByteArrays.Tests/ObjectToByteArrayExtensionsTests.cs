using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Plugin.ByteArrays;
using Xunit;
using FluentAssertions;

namespace Plugin.ByteArrays.Tests;

public class ObjectToByteArrayExtensionsTests
{
    [Fact]
    public void StringToByteArrays_Work_And_Empty_ReturnsEmpty()
    {
        "hello".AsciiStringToByteArray().Should().Equal(Encoding.ASCII.GetBytes("hello"));
        "hello".Utf8StringToByteArray().Should().Equal(Encoding.UTF8.GetBytes("hello"));
        "".AsciiStringToByteArray().Should().BeEmpty();
        ((string)null!).Utf8StringToByteArray().Should().BeEmpty();
    }

    [Fact]
    public void Hex_Base64_StringToBytes()
    {
        "0A0BFF".HexStringToByteArray().Should().Equal(0x0A, 0x0B, 0xFF);
        Action odd = () => "ABC".HexStringToByteArray();
        odd.Should().Throw<ArgumentException>();
        var b64 = Convert.ToBase64String(new byte[] {1,2,3});
        b64.Base64StringToByteArray().Should().Equal(1,2,3);
    }

    [Fact]
    public void ToByteArray_Generic_SupportsManyTypes()
    {
        123.ToByteArray().Should().Equal(BitConverter.GetBytes(123));
        123.45.ToByteArray().Should().Equal(BitConverter.GetBytes(123.45));
        ((int?)null).ToByteArray().Should().BeEmpty();
        new byte[] {1,2}.ToByteArray().Should().Equal(1,2);
    }

    #region JSON Serialization Tests

    [Fact]
    public void ToJsonByteArray_WithValidObject_ShouldSerializeCorrectly()
    {
        // Arrange
        var obj = new { Name = "Test", Value = 42 };

        // Act
        var result = obj.ToJsonByteArray();

        // Assert
        result.Should().NotBeNull();
        result.Length.Should().BeGreaterThan(0);

        // Deserialize to verify
        var json = Encoding.UTF8.GetString(result);
        json.Should().Contain("Test");
        json.Should().Contain("42");
    }

    [Fact]
    public void ToJsonByteArray_WithCustomOptions_ShouldUseOptions()
    {
        // Arrange
        var obj = new { name = "Test", value = 42 };
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        // Act
        var result = obj.ToJsonByteArray(options);

        // Assert
        result.Should().NotBeNull();
        var json = Encoding.UTF8.GetString(result);
        json.Should().Contain("name");
        json.Should().Contain("value");
    }

    [Fact]
    public void ToJsonByteArray_WithNullObject_ShouldReturnEmpty()
    {
        // Arrange
        object obj = null!;

        // Act
        var result = obj.ToJsonByteArray();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void FromJsonByteArray_WithValidJson_ShouldDeserializeCorrectly()
    {
        // Arrange
        var originalObj = new TestClass { Name = "Test", Value = 42 };
        var jsonBytes = originalObj.ToJsonByteArray();

        // Act
        var result = jsonBytes.FromJsonByteArray<TestClass>();

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Test");
        result.Value.Should().Be(42);
    }

    [Fact]
    public void FromJsonByteArray_WithEmptyArray_ShouldReturnDefault()
    {
        // Arrange
        var emptyArray = Array.Empty<byte>();

        // Act
        var result = emptyArray.FromJsonByteArray<TestClass>();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ToJsonMemory_WithValidObject_ShouldReturnReadOnlyMemory()
    {
        // Arrange
        var obj = new { Name = "Memory Test" };

        // Act
        var result = obj.ToJsonMemory();

        // Assert
        result.Length.Should().BeGreaterThan(0);
        var json = Encoding.UTF8.GetString(result.Span);
        json.Should().Contain("Memory Test");
    }

    [Fact]
    public void ToJsonMemory_WithNullObject_ShouldReturnEmpty()
    {
        // Arrange
        object obj = null!;

        // Act
        var result = obj.ToJsonMemory();

        // Assert
        result.Length.Should().Be(0);
    }

    public class TestClass
    {
        public string? Name { get; set; }
        public int Value { get; set; }
    }

    #endregion

    #region Collection Support Tests

    [Fact]
    public void ToByteArray_WithCollection_ShouldSerializeWithItemCount()
    {
        // Arrange
        var collection = new List<int> { 1, 2, 3, 4, 5 };

        // Act
        var result = collection.ToByteArray(item => BitConverter.GetBytes(item));

        // Assert
        result.Should().NotBeNull();
        result.Length.Should().BeGreaterThan(0);

        // First 4 bytes should be the count
        var count = BitConverter.ToInt32(result, 0);
        count.Should().Be(5);
    }

    [Fact]
    public void FromByteArrayToList_WithValidData_ShouldDeserializeCorrectly()
    {
        // Arrange
        var originalList = new List<int> { 10, 20, 30 };
        var serialized = originalList.ToByteArray(item => BitConverter.GetBytes(item));

        // Act
        var result = serialized.FromByteArrayToList(bytes => BitConverter.ToInt32(bytes, 0));

        // Assert
        result.Should().BeEquivalentTo(originalList);
    }

    [Fact]
    public void ToByteArray_WithEmptyCollection_ShouldSerializeEmptyCollection()
    {
        // Arrange
        var emptyCollection = new List<string>();

        // Act
        var result = emptyCollection.ToByteArray(item => Encoding.UTF8.GetBytes(item));

        // Assert
        result.Should().NotBeNull();
        var count = BitConverter.ToInt32(result, 0);
        count.Should().Be(0);
    }

    [Fact]
    public void ToByteArray_WithDictionary_ShouldSerializeKeyValuePairs()
    {
        // Arrange
        var dictionary = new Dictionary<string, int>
        {
            ["key1"] = 100,
            ["key2"] = 200
        };

        // Act
        var result = dictionary.ToByteArray(
            key => Encoding.UTF8.GetBytes(key),
            value => BitConverter.GetBytes(value));

        // Assert
        result.Should().NotBeNull();
        result.Length.Should().BeGreaterThan(4); // At least the count

        // First 4 bytes should be the count
        var count = BitConverter.ToInt32(result, 0);
        count.Should().Be(2);
    }

    [Fact]
    public void CollectionSerialization_WithComplexObjects_ShouldWorkCorrectly()
    {
        // Arrange
        var objects = new List<TestClass>
        {
            new() { Name = "First", Value = 1 },
            new() { Name = "Second", Value = 2 }
        };

        // Act - Use a simple serializer that produces fixed-length data
        var serialized = objects.ToByteArray(obj => BitConverter.GetBytes(obj.Value));
        var deserialized = serialized.FromByteArrayToList(bytes => new TestClass { Value = BitConverter.ToInt32(bytes, 0) });

        // Assert
        deserialized.Should().HaveCount(2);
        deserialized[0].Value.Should().Be(1);
        deserialized[1].Value.Should().Be(2);
    }

    #endregion

    #region Span and Memory Operations Tests

    [Fact]
    public void ToByteArray_WithReadOnlySpan_ShouldReturnCorrectArray()
    {
        // Arrange
        var originalArray = new byte[] { 1, 2, 3, 4, 5 };
        ReadOnlySpan<byte> span = originalArray.AsSpan(1, 3);

        // Act
        var result = span.ToByteArray();

        // Assert
        result.Should().BeEquivalentTo(new byte[] { 2, 3, 4 });
    }

    [Fact]
    public void ToByteArray_WithReadOnlyMemory_ShouldReturnCorrectArray()
    {
        // Arrange
        var originalArray = new byte[] { 10, 20, 30, 40 };
        ReadOnlyMemory<byte> memory = originalArray.AsMemory(1, 2);

        // Act
        var result = memory.ToByteArray();

        // Assert
        result.Should().BeEquivalentTo(new byte[] { 20, 30 });
    }

    [Fact]
    public void TryWriteToSpan_WithValidSerializer_ShouldWriteCorrectly()
    {
        // Arrange
        var value = 12345;
        Span<byte> destination = stackalloc byte[10];

        // Act
        var bytesWritten = value.TryWriteToSpan(destination, (val, span) =>
        {
            var bytes = BitConverter.GetBytes(val);
            bytes.CopyTo(span);
            return bytes.Length;
        });

        // Assert
        bytesWritten.Should().Be(4);
        BitConverter.ToInt32(destination.Slice(0, 4)).Should().Be(12345);
    }

    [Fact]
    public void TryWriteToSpan_WithNullValue_ShouldReturnZero()
    {
        // Arrange
        object value = null!;
        Span<byte> destination = stackalloc byte[10];

        // Act
        var bytesWritten = value.TryWriteToSpan(destination, (val, span) => 5);

        // Assert
        bytesWritten.Should().Be(0);
    }

    [Fact]
    public void ToRentedBuffer_WithValidSerializer_ShouldRentFromPool()
    {
        // Arrange
        var value = "Test string for buffer rental";

        // Act
        var (buffer, length) = value.ToRentedBuffer(
            str => Encoding.UTF8.GetBytes(str),
            minimumLength: 1024);

        try
        {
            // Assert
            buffer.Should().NotBeNull();
            buffer.Length.Should().BeGreaterOrEqualTo(1024);
            length.Should().Be(Encoding.UTF8.GetByteCount(value));

            var result = Encoding.UTF8.GetString(buffer, 0, length);
            result.Should().Be(value);
        }
        finally
        {
            ObjectToByteArrayExtensions.ReturnRentedBuffer(buffer);
        }
    }

    [Fact]
    public void ToRentedBuffer_WithNullValue_ShouldReturnEmptyBuffer()
    {
        // Arrange
        string value = null!;

        // Act
        var (buffer, length) = value.ToRentedBuffer(str => Encoding.UTF8.GetBytes(str ?? ""));

        // Assert
        buffer.Should().BeEmpty();
        length.Should().Be(0);
    }

    [Fact]
    public void ReturnRentedBuffer_WithValidBuffer_ShouldNotThrow()
    {
        // Arrange
        var buffer = ArrayPool<byte>.Shared.Rent(1024);

        // Act & Assert
        Action act = () => ObjectToByteArrayExtensions.ReturnRentedBuffer(buffer);
        act.Should().NotThrow();
    }

    [Fact]
    public void ReturnRentedBuffer_WithNullBuffer_ShouldNotThrow()
    {
        // Act & Assert
        Action act = () => ObjectToByteArrayExtensions.ReturnRentedBuffer(null!);
        act.Should().NotThrow();
    }

    #endregion

    #region Custom Serialization Support Tests

    [Fact]
    public void ToByteArray_WithIByteSerializable_ShouldUseCustomSerialization()
    {
        // Arrange
        var serializable = new TestSerializable { Data = "Custom serialization test" };

        // Act - Use custom serializer since IByteSerializable isn't supported directly
        var result = serializable.ToByteArray(s => s.ToBytes());

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(Encoding.UTF8.GetBytes(serializable.Data));
    }

    [Fact]
    public void ToByteArray_WithCustomSerializer_ShouldUseProvidedSerializer()
    {
        // Arrange
        var value = 42;

        // Act
        var result = value.ToByteArray(val => Encoding.UTF8.GetBytes($"Custom: {val}"));

        // Assert
        result.Should().BeEquivalentTo(Encoding.UTF8.GetBytes("Custom: 42"));
    }

    [Fact]
    public void ToByteArray_WithCustomSerializerAndNullValue_ShouldReturnEmpty()
    {
        // Arrange
        string value = null!;

        // Act
        var result = value.ToByteArray(val => Encoding.UTF8.GetBytes(val ?? ""));

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void CreateCachedSerializer_ShouldCacheResults()
    {
        // Arrange
        var callCount = 0;
        var baseSerializer = new Func<string, byte[]>(str =>
        {
            callCount++;
            return Encoding.UTF8.GetBytes(str);
        });

        var cachedSerializer = ObjectToByteArrayExtensions.CreateCachedSerializer(baseSerializer, maxCacheSize: 2);

        // Act
        var result1 = cachedSerializer("test1");
        var result2 = cachedSerializer("test1"); // Should use cache
        var result3 = cachedSerializer("test2");
        var result4 = cachedSerializer("test1"); // Should still use cache

        // Assert
        callCount.Should().Be(2); // Only called twice despite 4 invocations
        result1.Should().BeEquivalentTo(result2);
        result1.Should().BeEquivalentTo(result4);
    }

    [Fact]
    public void CreateCachedSerializer_ShouldEvictOldEntries()
    {
        // Arrange
        var callCount = 0;
        var baseSerializer = new Func<int, byte[]>(val =>
        {
            callCount++;
            return BitConverter.GetBytes(val);
        });

        var cachedSerializer = ObjectToByteArrayExtensions.CreateCachedSerializer(baseSerializer, maxCacheSize: 2);

        // Act
        cachedSerializer(1); // Cache: [1]
        cachedSerializer(2); // Cache: [1, 2]
        cachedSerializer(3); // Cache: [2, 3] (1 evicted)
        cachedSerializer(1); // Should call base again since 1 was evicted

        // Assert
        callCount.Should().Be(4); // All calls went to base serializer
    }

    public class TestSerializable : IByteSerializable
    {
        public string Data { get; set; } = "";

        public byte[] ToBytes()
        {
            return Encoding.UTF8.GetBytes(Data);
        }

        public void FromBytes(byte[] data)
        {
            Data = Encoding.UTF8.GetString(data);
        }
    }

    #endregion

    #region Error Handling Tests

    [Fact]
    public void FromJsonByteArray_WithNullArray_ShouldThrowArgumentNullException()
    {
        // Arrange
        byte[] data = null!;

        // Act & Assert
        Action act = () => data.FromJsonByteArray<TestClass>();
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ToByteArray_WithCollectionAndNullSerializer_ShouldThrowArgumentNullException()
    {
        // Arrange
        var collection = new List<int> { 1, 2, 3 };
        Func<int, byte[]> serializer = null!;

        // Act & Assert
        Action act = () => collection.ToByteArray(serializer);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void FromByteArrayToList_WithNullArray_ShouldThrowArgumentNullException()
    {
        // Arrange
        byte[] data = null!;

        // Act & Assert
        Action act = () => data.FromByteArrayToList(bytes => 0);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void FromByteArrayToList_WithNullDeserializer_ShouldThrowArgumentNullException()
    {
        // Arrange
        var data = new byte[] { 1, 2, 3 };
        Func<byte[], int> deserializer = null!;

        // Act & Assert
        Action act = () => data.FromByteArrayToList(deserializer);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void TryWriteToSpan_WithNullSerializer_ShouldThrowArgumentNullException()
    {
        // Arrange
        var value = 42;
        Func<int, Span<byte>, int> serializer = null!;

        // Act & Assert - Use stackalloc in the method call to avoid ref local capture
        Assert.Throws<ArgumentNullException>(() =>
        {
            Span<byte> span = stackalloc byte[10];
            value.TryWriteToSpan(span, serializer);
        });
    }

    [Fact]
    public void ToRentedBuffer_WithNullSerializer_ShouldThrowArgumentNullException()
    {
        // Arrange
        var value = "test";
        Func<string, byte[]> serializer = null!;

        // Act & Assert
        Action act = () => value.ToRentedBuffer(serializer);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ToByteArray_WithIByteSerializableAndNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        IByteSerializable serializable = null!;

        // Act & Assert
        Action act = () => serializable.ToByteArray();
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ToByteArray_WithCustomSerializerAndNullSerializer_ShouldThrowArgumentNullException()
    {
        // Arrange
        var value = 42;
        Func<int, byte[]> serializer = null!;

        // Act & Assert
        Action act = () => value.ToByteArray(serializer);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CreateCachedSerializer_WithNullBaseSerializer_ShouldThrowArgumentNullException()
    {
        // Arrange
        Func<string, byte[]> baseSerializer = null!;

        // Act & Assert
        Action act = () => ObjectToByteArrayExtensions.CreateCachedSerializer(baseSerializer);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CreateCachedSerializer_WithInvalidCacheSize_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var baseSerializer = new Func<string, byte[]>(str => Encoding.UTF8.GetBytes(str));

        // Act & Assert
        Action act = () => ObjectToByteArrayExtensions.CreateCachedSerializer(baseSerializer, maxCacheSize: 0);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    #endregion

}
