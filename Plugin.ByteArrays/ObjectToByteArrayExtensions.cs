using System.Buffers;
using System.Text.Json;

namespace Plugin.ByteArrays;

/// <summary>
///     Interface for types that can serialize themselves to bytes.
/// </summary>
public interface IByteSerializable
{
    /// <summary>
    ///     Serializes the object to a byte array.
    /// </summary>
    /// <returns>The serialized byte array.</returns>
    byte[] ToBytes();

    /// <summary>
    ///     Deserializes the object from a byte array.
    /// </summary>
    /// <param name="data">The byte array to deserialize from.</param>
    void FromBytes(byte[] data);
}

/// <summary>
///     Provides extension methods for converting objects and strings to byte arrays using various encodings and formats.
/// </summary>
public static class ObjectToByteArrayExtensions
{
    /// <summary>
    ///     Creates a byte array from an ASCII-encoded string.
    /// </summary>
    /// <param name="value">The input string to convert to a byte array.</param>
    /// <returns>A new byte array containing the ASCII-encoded bytes of the input string, or an empty array if the input is null or empty.</returns>
    public static byte[] AsciiStringToByteArray(this string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return [];
        }

        using var byteArrayBuilder = new ByteArrayBuilder();
        return byteArrayBuilder.AppendAsciiString(value).ToByteArray();
    }

    /// <summary>
    ///     Creates a byte array from a UTF-8 encoded string.
    /// </summary>
    /// <param name="value">The input string to convert to a byte array.</param>
    /// <returns>A new byte array containing the UTF-8 encoded bytes of the input string, or an empty array if the input is null or empty.</returns>
    public static byte[] Utf8StringToByteArray(this string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return [];
        }

        using var byteArrayBuilder = new ByteArrayBuilder();
        return byteArrayBuilder.AppendUtf8String(value).ToByteArray();
    }

    /// <summary>
    ///     Creates a byte array from a hexadecimal string.
    /// </summary>
    /// <param name="hexString">The input hexadecimal string to convert to a byte array.</param>
    /// <returns>A new byte array containing the bytes represented by the hexadecimal string, or an empty array if the input is null or empty.</returns>
    /// <exception cref="ArgumentException">Thrown if the input string is not a valid hexadecimal string.</exception>
    /// <remarks>
    /// The input string must have an even number of characters, as each pair of characters represents a byte.
    /// If the string is empty or null, an empty byte array is returned.
    /// </remarks>
    public static byte[] HexStringToByteArray(this string hexString)
    {
        if (string.IsNullOrEmpty(hexString))
        {
            return [];
        }

        using var byteArrayBuilder = new ByteArrayBuilder();
        return byteArrayBuilder.AppendHexString(hexString).ToByteArray();
    }

    /// <summary>
    ///     Creates a byte array from a Base64-encoded string.
    /// </summary>
    /// <param name="base64String">The input Base64 string to convert to a byte array.</param>
    /// <returns>A new byte array containing the decoded bytes of the Base64 string, or an empty array if the input is null or empty.</returns>
    public static byte[] Base64StringToByteArray(this string base64String)
    {
        if (string.IsNullOrEmpty(base64String))
        {
            return [];
        }

        using var byteArrayBuilder = new ByteArrayBuilder();
        return byteArrayBuilder.AppendBase64String(base64String).ToByteArray();
    }

    /// <summary>
    ///     Creates a byte array from an object of any supported type.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <param name="value">The input value to convert to a byte array.</param>
    /// <returns>A new byte array representing the input value, or an empty array if the input is null.</returns>
    public static byte[] ToByteArray<T>(this T value)
    {
        if (value is null)
        {
            return [];
        }

        using var byteArrayBuilder = new ByteArrayBuilder();
        return byteArrayBuilder.Append(value).ToByteArray();
    }

    #region JSON Serialization

    /// <summary>
    ///     Serializes an object to a byte array using System.Text.Json.
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize.</typeparam>
    /// <param name="value">The object to serialize.</param>
    /// <param name="options">Optional JsonSerializerOptions for customizing serialization.</param>
    /// <returns>A byte array containing the JSON representation of the object.</returns>
    public static byte[] ToJsonByteArray<T>(this T value, JsonSerializerOptions? options = null)
    {
        if (value is null)
        {
            return [];
        }

        return JsonSerializer.SerializeToUtf8Bytes(value, options);
    }

    /// <summary>
    ///     Deserializes a byte array from JSON into an object of the specified type.
    /// </summary>
    /// <typeparam name="T">The type to deserialize to.</typeparam>
    /// <param name="jsonBytes">The JSON byte array to deserialize.</param>
    /// <param name="options">Optional JsonSerializerOptions for customizing deserialization.</param>
    /// <returns>The deserialized object.</returns>
    public static T? FromJsonByteArray<T>(this byte[] jsonBytes, JsonSerializerOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(jsonBytes);

        if (jsonBytes.Length == 0)
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(jsonBytes, options);
    }

    /// <summary>
    ///     Serializes an object to a byte array using System.Text.Json with ReadOnlySpan support.
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize.</typeparam>
    /// <param name="value">The object to serialize.</param>
    /// <param name="options">Optional JsonSerializerOptions for customizing serialization.</param>
    /// <returns>A ReadOnlyMemory containing the JSON representation of the object.</returns>
    public static ReadOnlyMemory<byte> ToJsonMemory<T>(this T value, JsonSerializerOptions? options = null)
    {
        if (value is null)
        {
            return ReadOnlyMemory<byte>.Empty;
        }

        return JsonSerializer.SerializeToUtf8Bytes(value, options).AsMemory();
    }

    #endregion

    #region Collection Support

    /// <summary>
    ///     Converts a collection of objects to a byte array where each object is serialized and prefixed with its length.
    /// </summary>
    /// <typeparam name="T">The type of objects in the collection.</typeparam>
    /// <param name="collection">The collection to serialize.</param>
    /// <param name="itemSerializer">A function to serialize each item to bytes.</param>
    /// <returns>A byte array containing the serialized collection.</returns>
    public static byte[] ToByteArray<T>(this IEnumerable<T> collection, Func<T, byte[]> itemSerializer)
    {
        ArgumentNullException.ThrowIfNull(collection);
        ArgumentNullException.ThrowIfNull(itemSerializer);

        using var builder = new ByteArrayBuilder();

        var items = collection.ToList();
        builder.Append(items.Count); // Write item count

        foreach (var item in items)
        {
            var itemBytes = itemSerializer(item);
            builder.Append((ushort)itemBytes.Length); // Write item length
            builder.Append(itemBytes); // Write item data
        }

        return builder.ToByteArray();
    }

    /// <summary>
    ///     Deserializes a byte array back to a collection of objects.
    /// </summary>
    /// <typeparam name="T">The type of objects in the collection.</typeparam>
    /// <param name="data">The byte array containing the serialized collection.</param>
    /// <param name="itemDeserializer">A function to deserialize each item from bytes.</param>
    /// <returns>The deserialized collection.</returns>
    public static IList<T> FromByteArrayToList<T>(this byte[] data, Func<byte[], T> itemDeserializer)
    {
        ArgumentNullException.ThrowIfNull(data);
        ArgumentNullException.ThrowIfNull(itemDeserializer);

        var position = 0;
        var count = data.ToInt32(ref position);
        var result = new List<T>(count);

        for (var i = 0; i < count; i++)
        {
            var itemLength = data.ToUInt16(ref position);
            var itemBytes = new byte[itemLength];
            Array.Copy(data, position, itemBytes, 0, itemLength);
            position += itemLength;

            result.Add(itemDeserializer(itemBytes));
        }

        return result;
    }

    /// <summary>
    ///     Converts a dictionary to a byte array with key-value pairs serialized.
    /// </summary>
    /// <typeparam name="TKey">The type of dictionary keys.</typeparam>
    /// <typeparam name="TValue">The type of dictionary values.</typeparam>
    /// <param name="dictionary">The dictionary to serialize.</param>
    /// <param name="keySerializer">A function to serialize keys to bytes.</param>
    /// <param name="valueSerializer">A function to serialize values to bytes.</param>
    /// <returns>A byte array containing the serialized dictionary.</returns>
    public static byte[] ToByteArray<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary,
        Func<TKey, byte[]> keySerializer,
        Func<TValue, byte[]> valueSerializer)
    {
        ArgumentNullException.ThrowIfNull(dictionary);
        ArgumentNullException.ThrowIfNull(keySerializer);
        ArgumentNullException.ThrowIfNull(valueSerializer);

        using var builder = new ByteArrayBuilder();

        builder.Append(dictionary.Count); // Write pair count

        foreach (var kvp in dictionary)
        {
            var keyBytes = keySerializer(kvp.Key);
            var valueBytes = valueSerializer(kvp.Value);

            builder.Append((ushort)keyBytes.Length); // Write key length
            builder.Append(keyBytes); // Write key data
            builder.Append((ushort)valueBytes.Length); // Write value length
            builder.Append(valueBytes); // Write value data
        }

        return builder.ToByteArray();
    }

    #endregion

    #region Span and Memory Operations

    /// <summary>
    ///     Converts a span of bytes to a new byte array.
    /// </summary>
    /// <param name="span">The span to convert.</param>
    /// <returns>A new byte array containing the span data.</returns>
    public static byte[] ToByteArray(this ReadOnlySpan<byte> span)
    {
        return span.ToArray();
    }

    /// <summary>
    ///     Converts a memory segment to a new byte array.
    /// </summary>
    /// <param name="memory">The memory segment to convert.</param>
    /// <returns>A new byte array containing the memory data.</returns>
    public static byte[] ToByteArray(this ReadOnlyMemory<byte> memory)
    {
        return memory.Span.ToArray();
    }

    /// <summary>
    ///     Writes an object to a provided span using a custom serializer.
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize.</typeparam>
    /// <param name="value">The value to serialize.</param>
    /// <param name="destination">The span to write to.</param>
    /// <param name="serializer">A function that writes the object to a span and returns bytes written.</param>
    /// <returns>The number of bytes written to the span.</returns>
    public static int TryWriteToSpan<T>(this T value, Span<byte> destination, Func<T, Span<byte>, int> serializer)
    {
        ArgumentNullException.ThrowIfNull(serializer);

        if (value is null)
        {
            return 0;
        }

        return serializer(value, destination);
    }

    /// <summary>
    ///     Rents a buffer from ArrayPool, writes data to it, and returns the relevant portion.
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize.</typeparam>
    /// <param name="value">The value to serialize.</param>
    /// <param name="serializer">A function that serializes the object and returns the byte array.</param>
    /// <param name="minimumLength">The minimum buffer length to rent.</param>
    /// <returns>A rented buffer and the actual length of data written.</returns>
    public static (byte[] buffer, int length) ToRentedBuffer<T>(
        this T value,
        Func<T, byte[]> serializer,
        int minimumLength = 1024)
    {
        ArgumentNullException.ThrowIfNull(serializer);

        if (value is null)
        {
            return (Array.Empty<byte>(), 0);
        }

        var data = serializer(value);
        var bufferLength = Math.Max(minimumLength, data.Length);
        var buffer = ArrayPool<byte>.Shared.Rent(bufferLength);

        Array.Copy(data, 0, buffer, 0, data.Length);

        return (buffer, data.Length);
    }

    /// <summary>
    ///     Returns a rented buffer to the ArrayPool.
    /// </summary>
    /// <param name="buffer">The buffer to return.</param>
    /// <param name="clearArray">Whether to clear the buffer before returning.</param>
    public static void ReturnRentedBuffer(byte[] buffer, bool clearArray = false)
    {
        if (buffer != null && buffer.Length > 0)
        {
            ArrayPool<byte>.Shared.Return(buffer, clearArray);
        }
    }

    #endregion

    #region Custom Serialization Support

    /// <summary>
    ///     Serializes an object that implements IByteSerializable.
    /// </summary>
    /// <param name="serializable">The object to serialize.</param>
    /// <returns>The serialized byte array.</returns>
    public static byte[] ToByteArray(this IByteSerializable serializable)
    {
        ArgumentNullException.ThrowIfNull(serializable);
        return serializable.ToBytes();
    }

    /// <summary>
    ///     Serializes an object using a provided custom serialization function.
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize.</typeparam>
    /// <param name="value">The object to serialize.</param>
    /// <param name="customSerializer">The custom serialization function.</param>
    /// <returns>The serialized byte array.</returns>
    public static byte[] ToByteArray<T>(this T value, Func<T, byte[]> customSerializer)
    {
        ArgumentNullException.ThrowIfNull(customSerializer);

        if (value is null)
        {
            return [];
        }

        return customSerializer(value);
    }

    /// <summary>
    ///     Creates a cacheable serializer that remembers serialization results for specific values.
    /// </summary>
    /// <typeparam name="T">The type of objects to serialize.</typeparam>
    /// <param name="baseSerializer">The base serialization function.</param>
    /// <param name="maxCacheSize">The maximum number of items to cache (default 100).</param>
    /// <returns>A cached serializer function.</returns>
    public static Func<T, byte[]> CreateCachedSerializer<T>(Func<T, byte[]> baseSerializer, int maxCacheSize = 100)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(baseSerializer);

        if (maxCacheSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxCacheSize), "Cache size must be positive");
        }

        var cache = new Dictionary<T, byte[]>();
        var accessOrder = new Queue<T>();

        return value =>
        {
            if (cache.TryGetValue(value, out var cachedResult))
            {
                return cachedResult;
            }

            var result = baseSerializer(value);

            // Manage cache size
            if (cache.Count >= maxCacheSize)
            {
                var oldest = accessOrder.Dequeue();
                cache.Remove(oldest);
            }

            cache[value] = result;
            accessOrder.Enqueue(value);

            return result;
        };
    }

    #endregion
}
