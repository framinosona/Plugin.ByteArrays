using System.Globalization;
using System.Net;
using System.Text;

namespace Plugin.ByteArrays;

/// <summary>
///     Provides a builder for constructing byte arrays from various types and encodings.
/// </summary>
public sealed class ByteArrayBuilder : IDisposable, IAsyncDisposable
{
    /// <summary>
    ///     The underlying memory stream used to build the byte array.
    /// </summary>
    private readonly MemoryStream _memoryStream;

    /// <summary>
    ///     Initializes a new instance of the ByteArrayBuilder class.
    /// </summary>
    public ByteArrayBuilder()
    {
        _memoryStream = new MemoryStream();
    }

    /// <summary>
    ///     Initializes a new instance of the ByteArrayBuilder class with the specified initial capacity.
    /// </summary>
    /// <param name="initialCapacity">The initial capacity of the underlying stream.</param>
    public ByteArrayBuilder(int initialCapacity)
    {
        _memoryStream = new MemoryStream(initialCapacity);
    }

    /// <summary>
    ///     Gets the current length of the byte array being built.
    /// </summary>
    public int Length => (int)_memoryStream.Length;

    /// <summary>
    ///     Gets the current capacity of the underlying stream.
    /// </summary>
    public int Capacity => _memoryStream.Capacity;

    /// <summary>
    ///     Creates a new ByteArrayBuilder with the specified initial capacity.
    /// </summary>
    /// <param name="initialCapacity">The initial capacity to allocate.</param>
    /// <returns>A new ByteArrayBuilder instance.</returns>
    public static ByteArrayBuilder WithCapacity(int initialCapacity)
    {
        return new(initialCapacity);
    }

    /// <summary>
    ///     Clears the contents of the builder, resetting it to an empty state.
    /// </summary>
    /// <returns>The current ByteArrayBuilder instance for method chaining.</returns>
    public ByteArrayBuilder Clear()
    {
        _memoryStream.SetLength(0);
        return this;
    }

    #region IAsyncDisposable Members

    /// <summary>
    ///     Asynchronously releases the resources used by the <see cref="ByteArrayBuilder"/>.
    /// </summary>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    public async ValueTask DisposeAsync()
    {
        await _memoryStream.DisposeAsync().ConfigureAwait(false);
    }

    #endregion

    #region IDisposable Members

    /// <summary>
    ///     Releases the resources used by the <see cref="ByteArrayBuilder"/>.
    /// </summary>
    public void Dispose()
    {
        _memoryStream.Dispose();
    }

    #endregion

    /// <summary>
    ///     Returns the byte array built by this instance.
    /// </summary>
    /// <param name="maxSize">The maximum allowed size of the byte array. If exceeded, an exception is thrown.</param>
    /// <returns>The constructed byte array.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the resulting array exceeds the specified maxSize.</exception>
    public byte[] ToByteArray(int? maxSize = null)
    {
        var output = _memoryStream.ToArray();
        if (maxSize.HasValue && output.Length > maxSize.Value)
        {
            throw new InvalidOperationException($"Array [{this}] is too big. {output.Length} bytes > {maxSize.Value} bytes");
        }

        return output;
    }

    /// <summary>
    ///     Returns a string representation of the byte array.
    /// </summary>
    /// <returns>A comma-separated string of byte values.</returns>
    public override string ToString()
    {
        return string.Join(",", _memoryStream.ToArray());
    }


    /// <summary>
    ///     Appends a value of any supported type to the byte array.
    /// </summary>
    /// <typeparam name="T">The type of the value to append.</typeparam>
    /// <param name="value">The value to append. Supported types include primitives, enums, and byte arrays.</param>
    /// <returns>The current <see cref="ByteArrayBuilder"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown if the type is not supported.</exception>
    public ByteArrayBuilder Append<T>(T value)
    {
        if (value is null)
        {
            return this;
        }

        switch (value)
        {
            case byte b:
                _memoryStream.WriteByte(b);
                return this;
            case sbyte sb:
                _memoryStream.WriteByte(unchecked((byte)sb));
                return this;
            case bool boolean:
                _memoryStream.WriteByte((byte)(boolean ? 1 : 0));
                return this;
            case byte[] byteArray:
                _memoryStream.Write(byteArray, 0, byteArray.Length);
                return this;
            case char c:
                return Append(BitConverter.GetBytes(c)); // Cyclic call to Append(byte[])
            case short s:
                return Append(BitConverter.GetBytes(s)); // Cyclic call to Append(byte[])
            case ushort us:
                return Append(BitConverter.GetBytes(us)); // Cyclic call to Append(byte[])
            case int i:
                return Append(BitConverter.GetBytes(i)); // Cyclic call to Append(byte[])
            case uint ui:
                return Append(BitConverter.GetBytes(ui)); // Cyclic call to Append(byte[])
            case long l:
                return Append(BitConverter.GetBytes(l)); // Cyclic call to Append(byte[])
            case ulong ul:
                return Append(BitConverter.GetBytes(ul)); // Cyclic call to Append(byte[])
            case float f:
                return Append(BitConverter.GetBytes(f)); // Cyclic call to Append(byte[])
            case double d:
                return Append(BitConverter.GetBytes(d)); // Cyclic call to Append(byte[])
            case Half half:
                return Append(BitConverter.GetBytes(half)); // Cyclic call to Append(byte[])
            case decimal dec:
                {
                    var bits = decimal.GetBits(dec);
                    foreach (var bit in bits)
                    {
                        _memoryStream.Write(BitConverter.GetBytes(bit));
                    }
                    return this;
                }
            case IEnumerable<byte> enumerable:
                foreach (var b in enumerable)
                {
                    _memoryStream.WriteByte(b);
                }
                return this;
            case Enum en:
                return AppendEnumValue(en);
            default:
                throw new ArgumentException($"Unsupported type: {typeof(T)}");
        }
    }


    /// <summary>
    ///     Append a UTF-8 encoded string to the ByteArrayBuilder.
    /// </summary>
    /// <param name="value">The input string.</param>
    /// <returns>The ByteArrayBuilder.</returns>
    /// <remarks>
    /// If the input string is empty or null, nothing is appended.
    /// </remarks>
    public ByteArrayBuilder AppendUtf8String(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return this;
        }

        return Append(Encoding.UTF8.GetBytes(value));
    }

    /// <summary>
    ///     Append an ASCII encoded string to the ByteArrayBuilder.
    /// </summary>
    /// <param name="value">The input string.</param>
    /// <returns>The ByteArrayBuilder.</returns>
    /// <remarks>
    /// If the input string is empty or null, nothing is appended.
    /// </remarks>
    public ByteArrayBuilder AppendAsciiString(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return this;
        }

        return Append(Encoding.ASCII.GetBytes(value));
    }

    /// <summary>
    ///     Append a hexadecimal string to the ByteArrayBuilder.
    /// </summary>
    /// <param name="value">The input hexadecimal string.</param>
    /// <returns>The ByteArrayBuilder.</returns>
    /// <exception cref="ArgumentException">Thrown if the input string is not a valid hexadecimal string.</exception>
    /// <remarks>
    /// The input string must have an even number of characters, as each pair of characters represents a byte.
    /// If the string is empty or null, nothing is appended.
    /// </remarks>
    public ByteArrayBuilder AppendHexString(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return this;
        }

        if (value.Length % 2 != 0)
        {
            throw new ArgumentException("Hex string must have an even number of characters.");
        }

        var byteArray = Convert.FromHexString(value);
        return Append(byteArray);
    }

    /// <summary>
    ///     Append a Base64 string to the ByteArrayBuilder.
    /// </summary>
    /// <param name="value">The input Base64 string.</param>
    /// <returns>The ByteArrayBuilder.</returns>
    /// <exception cref="FormatException">Thrown if the input string is not a valid Base64 string.</exception>
    /// <remarks>
    /// If the input string is empty or null, nothing is appended.
    /// </remarks>
    public ByteArrayBuilder AppendBase64String(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return this;
        }

        var byteArray = Convert.FromBase64String(value);
        return Append(byteArray);
    }

    #region Advanced String Encodings

    /// <summary>
    ///     Append a UTF-16 encoded string to the ByteArrayBuilder.
    /// </summary>
    /// <param name="value">The input string.</param>
    /// <returns>The ByteArrayBuilder.</returns>
    public ByteArrayBuilder AppendUtf16String(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return this;
        }

        return Append(Encoding.Unicode.GetBytes(value));
    }

    /// <summary>
    ///     Append a UTF-32 encoded string to the ByteArrayBuilder.
    /// </summary>
    /// <param name="value">The input string.</param>
    /// <returns>The ByteArrayBuilder.</returns>
    public ByteArrayBuilder AppendUtf32String(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return this;
        }

        return Append(Encoding.UTF32.GetBytes(value));
    }

    /// <summary>
    ///     Append a string with custom encoding to the ByteArrayBuilder.
    /// </summary>
    /// <param name="value">The input string.</param>
    /// <param name="encoding">The encoding to use.</param>
    /// <returns>The ByteArrayBuilder.</returns>
    public ByteArrayBuilder AppendString(string value, Encoding encoding)
    {
        ArgumentNullException.ThrowIfNull(encoding);

        if (string.IsNullOrEmpty(value))
        {
            return this;
        }

        return Append(encoding.GetBytes(value));
    }

    /// <summary>
    ///     Append a length-prefixed string to the ByteArrayBuilder.
    /// </summary>
    /// <param name="value">The input string.</param>
    /// <param name="encoding">The encoding to use.</param>
    /// <returns>The ByteArrayBuilder.</returns>
    public ByteArrayBuilder AppendLengthPrefixedString(string value, Encoding encoding)
    {
        ArgumentNullException.ThrowIfNull(encoding);

        if (string.IsNullOrEmpty(value))
        {
            Append((short)0); // Zero length
            return this;
        }

        var bytes = encoding.GetBytes(value);
        Append((short)bytes.Length);
        return Append(bytes);
    }

    /// <summary>
    ///     Append a null-terminated string to the ByteArrayBuilder.
    /// </summary>
    /// <param name="value">The input string.</param>
    /// <param name="encoding">The encoding to use.</param>
    /// <returns>The ByteArrayBuilder.</returns>
    public ByteArrayBuilder AppendNullTerminatedString(string value, Encoding encoding)
    {
        ArgumentNullException.ThrowIfNull(encoding);

        if (!string.IsNullOrEmpty(value))
        {
            Append(encoding.GetBytes(value));
        }
        return Append((byte)0); // Null terminator
    }

    #endregion

    #region Endianness Support

    /// <summary>
    ///     Append a 16-bit integer in big-endian format.
    /// </summary>
    /// <param name="value">The value to append.</param>
    /// <returns>The ByteArrayBuilder.</returns>
    public ByteArrayBuilder AppendBigEndian(short value)
    {
        var bytes = BitConverter.GetBytes(value);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }
        return Append(bytes);
    }

    /// <summary>
    ///     Append a 32-bit integer in big-endian format.
    /// </summary>
    /// <param name="value">The value to append.</param>
    /// <returns>The ByteArrayBuilder.</returns>
    public ByteArrayBuilder AppendBigEndian(int value)
    {
        var bytes = BitConverter.GetBytes(value);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }
        return Append(bytes);
    }

    /// <summary>
    ///     Append a 64-bit integer in big-endian format.
    /// </summary>
    /// <param name="value">The value to append.</param>
    /// <returns>The ByteArrayBuilder.</returns>
    public ByteArrayBuilder AppendBigEndian(long value)
    {
        var bytes = BitConverter.GetBytes(value);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }
        return Append(bytes);
    }

    /// <summary>
    ///     Append a 16-bit integer in little-endian format.
    /// </summary>
    /// <param name="value">The value to append.</param>
    /// <returns>The ByteArrayBuilder.</returns>
    public ByteArrayBuilder AppendLittleEndian(short value)
    {
        var bytes = BitConverter.GetBytes(value);
        if (!BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }
        return Append(bytes);
    }

    /// <summary>
    ///     Append a 32-bit integer in little-endian format.
    /// </summary>
    /// <param name="value">The value to append.</param>
    /// <returns>The ByteArrayBuilder.</returns>
    public ByteArrayBuilder AppendLittleEndian(int value)
    {
        var bytes = BitConverter.GetBytes(value);
        if (!BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }
        return Append(bytes);
    }

    /// <summary>
    ///     Append a 64-bit integer in little-endian format.
    /// </summary>
    /// <param name="value">The value to append.</param>
    /// <returns>The ByteArrayBuilder.</returns>
    public ByteArrayBuilder AppendLittleEndian(long value)
    {
        var bytes = BitConverter.GetBytes(value);
        if (!BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }
        return Append(bytes);
    }

    #endregion

    #region Advanced Data Types

    /// <summary>
    ///     Append a DateTime to the ByteArrayBuilder.
    /// </summary>
    /// <param name="dateTime">The DateTime to append.</param>
    /// <returns>The ByteArrayBuilder.</returns>
    public ByteArrayBuilder Append(DateTime dateTime)
    {
        return Append(dateTime.ToBinary());
    }

    /// <summary>
    ///     Append a TimeSpan to the ByteArrayBuilder.
    /// </summary>
    /// <param name="timeSpan">The TimeSpan to append.</param>
    /// <returns>The ByteArrayBuilder.</returns>
    public ByteArrayBuilder Append(TimeSpan timeSpan)
    {
        return Append(timeSpan.Ticks);
    }

    /// <summary>
    ///     Append a DateTimeOffset to the ByteArrayBuilder.
    /// </summary>
    /// <param name="dateTimeOffset">The DateTimeOffset to append.</param>
    /// <returns>The ByteArrayBuilder.</returns>
    public ByteArrayBuilder Append(DateTimeOffset dateTimeOffset)
    {
        Append(dateTimeOffset.DateTime);
        return Append(dateTimeOffset.Offset.Ticks);
    }

    /// <summary>
    ///     Append a GUID to the ByteArrayBuilder.
    /// </summary>
    /// <param name="guidValue">The GUID to append.</param>
    /// <returns>The ByteArrayBuilder.</returns>
    public ByteArrayBuilder Append(Guid guidValue)
    {
        return Append(guidValue.ToByteArray());
    }

    /// <summary>
    ///     Append an IPAddress to the ByteArrayBuilder.
    /// </summary>
    /// <param name="ipAddress">The IPAddress to append.</param>
    /// <returns>The ByteArrayBuilder.</returns>
    public ByteArrayBuilder Append(IPAddress ipAddress)
    {
        ArgumentNullException.ThrowIfNull(ipAddress);
        return Append(ipAddress.GetAddressBytes());
    }

    #endregion

    #region Bulk Operations

    /// <summary>
    ///     Append a byte value repeated a specified number of times.
    /// </summary>
    /// <param name="value">The byte value to repeat.</param>
    /// <param name="count">The number of times to repeat the value.</param>
    /// <returns>The ByteArrayBuilder.</returns>
    public ByteArrayBuilder AppendRepeated(byte value, int count)
    {
        for (var i = 0; i < count; i++)
        {
            _memoryStream.WriteByte(value);
        }
        return this;
    }

    /// <summary>
    ///     Append a byte pattern repeated a specified number of times.
    /// </summary>
    /// <param name="pattern">The pattern to repeat.</param>
    /// <param name="times">The number of times to repeat the pattern.</param>
    /// <returns>The ByteArrayBuilder.</returns>
    public ByteArrayBuilder AppendRepeated(byte[] pattern, int times)
    {
        for (var i = 0; i < times; i++)
        {
            Append(pattern);
        }
        return this;
    }

    /// <summary>
    ///     Conditionally append data based on a condition.
    /// </summary>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="action">The action to perform if condition is true.</param>
    /// <returns>The ByteArrayBuilder.</returns>
    public ByteArrayBuilder AppendIf(bool condition, Func<ByteArrayBuilder, ByteArrayBuilder> action)
    {
        ArgumentNullException.ThrowIfNull(action);
        return condition ? action(this) : this;
    }

    /// <summary>
    ///     Append multiple values from a collection.
    /// </summary>
    /// <typeparam name="T">The type of values to append.</typeparam>
    /// <param name="values">The collection of values.</param>
    /// <returns>The ByteArrayBuilder.</returns>
    public ByteArrayBuilder AppendMany<T>(IEnumerable<T> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        foreach (var value in values)
        {
            Append(value);
        }
        return this;
    }

    #endregion

    #region Stream Integration

    /// <summary>
    ///     Append data from a stream.
    /// </summary>
    /// <param name="stream">The stream to read from.</param>
    /// <param name="count">The number of bytes to read, or -1 to read all.</param>
    /// <returns>The ByteArrayBuilder.</returns>
    public ByteArrayBuilder AppendFromStream(Stream stream, int count = -1)
    {
        ArgumentNullException.ThrowIfNull(stream);

        if (count == -1)
        {
            stream.CopyTo(_memoryStream);
        }
        else
        {
            var buffer = new byte[Math.Min(count, 8192)];
            var remaining = count;
            while (remaining > 0)
            {
                var bytesToRead = Math.Min(buffer.Length, remaining);
                var bytesRead = stream.Read(buffer, 0, bytesToRead);
                if (bytesRead == 0)
                {
                    break;
                }

                _memoryStream.Write(buffer, 0, bytesRead);
                remaining -= bytesRead;
            }
        }
        return this;
    }

    /// <summary>
    ///     Write the contents to a stream.
    /// </summary>
    /// <param name="stream">The stream to write to.</param>
    public void WriteTo(Stream stream)
    {
        _memoryStream.WriteTo(stream);
    }

    /// <summary>
    ///     Convert to ReadOnlyMemory.
    /// </summary>
    /// <returns>A ReadOnlyMemory representing the built data.</returns>
    public ReadOnlyMemory<byte> ToReadOnlyMemory()
    {
        return new ReadOnlyMemory<byte>(_memoryStream.GetBuffer(), 0, (int)_memoryStream.Length);
    }

    /// <summary>
    ///     Convert to Memory.
    /// </summary>
    /// <returns>A Memory representing the built data.</returns>
    public Memory<byte> ToMemory()
    {
        return new Memory<byte>(_memoryStream.GetBuffer(), 0, (int)_memoryStream.Length);
    }

    /// <summary>
    ///     Convert to ArraySegment.
    /// </summary>
    /// <returns>An ArraySegment representing the built data.</returns>
    public ArraySegment<byte> ToArraySegment()
    {
        return new ArraySegment<byte>(_memoryStream.GetBuffer(), 0, (int)_memoryStream.Length);
    }

    #endregion

    private ByteArrayBuilder AppendEnumValue(Enum enumValue)
    {
        var enumType = enumValue.GetType();
        var underlyingType = Enum.GetUnderlyingType(enumType);
        var bytes = underlyingType switch
        {
            not null when underlyingType == typeof(byte) => [Convert.ToByte(enumValue, CultureInfo.InvariantCulture)],
            not null when underlyingType == typeof(sbyte) => [unchecked((byte)Convert.ToSByte(enumValue, CultureInfo.InvariantCulture))],
            not null when underlyingType == typeof(short) => BitConverter.GetBytes(Convert.ToInt16(enumValue, CultureInfo.InvariantCulture)),
            not null when underlyingType == typeof(ushort) => BitConverter.GetBytes(Convert.ToUInt16(enumValue, CultureInfo.InvariantCulture)),
            not null when underlyingType == typeof(int) => BitConverter.GetBytes(Convert.ToInt32(enumValue, CultureInfo.InvariantCulture)),
            not null when underlyingType == typeof(uint) => BitConverter.GetBytes(Convert.ToUInt32(enumValue, CultureInfo.InvariantCulture)),
            not null when underlyingType == typeof(long) => BitConverter.GetBytes(Convert.ToInt64(enumValue, CultureInfo.InvariantCulture)),
            not null when underlyingType == typeof(ulong) => BitConverter.GetBytes(Convert.ToUInt64(enumValue, CultureInfo.InvariantCulture)),
            var _ => throw new ArgumentException("Unsupported enum underlying type"),
        };
        return Append(bytes);
    }

    private ByteArrayBuilder AppendEnum<T>(T enumValue) where T : Enum
    {
        var underlyingType = Enum.GetUnderlyingType(typeof(T));
        var bytes = underlyingType switch
        {
            not null when underlyingType == typeof(byte) => [Convert.ToByte(enumValue, CultureInfo.InvariantCulture)],
            not null when underlyingType == typeof(sbyte) => [unchecked((byte)Convert.ToSByte(enumValue, CultureInfo.InvariantCulture))],
            not null when underlyingType == typeof(short) => BitConverter.GetBytes(Convert.ToInt16(enumValue, CultureInfo.InvariantCulture)),
            not null when underlyingType == typeof(ushort) => BitConverter.GetBytes(Convert.ToUInt16(enumValue, CultureInfo.InvariantCulture)),
            not null when underlyingType == typeof(int) => BitConverter.GetBytes(Convert.ToInt32(enumValue, CultureInfo.InvariantCulture)),
            not null when underlyingType == typeof(uint) => BitConverter.GetBytes(Convert.ToUInt32(enumValue, CultureInfo.InvariantCulture)),
            not null when underlyingType == typeof(long) => BitConverter.GetBytes(Convert.ToInt64(enumValue, CultureInfo.InvariantCulture)),
            not null when underlyingType == typeof(ulong) => BitConverter.GetBytes(Convert.ToUInt64(enumValue, CultureInfo.InvariantCulture)),
            var _ => throw new ArgumentException("Unsupported enum underlying type"),
        };
        return Append(bytes);
    }
}
