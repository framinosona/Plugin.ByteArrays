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
    private readonly MemoryStream _memoryStream = new MemoryStream();

    #region IAsyncDisposable Members

    /// <summary>
    ///     Asynchronously releases the resources used by the <see cref="ByteArrayBuilder"/>.
    /// </summary>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    public async ValueTask DisposeAsync()
    {
        await _memoryStream.DisposeAsync();
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
    /// <exception cref="IndexOutOfRangeException">Thrown if the byte array is longer than <paramref name="maxSize"/>.</exception>
    public byte[] ToByteArray(int? maxSize = null)
    {
        var output = _memoryStream.ToArray();
        if (maxSize.HasValue && output.Length > maxSize.Value)
        {
            throw new IndexOutOfRangeException($"Array [{this}] is too big. {output.Length} bytes > {maxSize.Value} bytes");
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

    private ByteArrayBuilder AppendEnumValue(Enum enumValue)
    {
        var enumType = enumValue.GetType();
        var underlyingType = Enum.GetUnderlyingType(enumType);
        var bytes = underlyingType switch
        {
            not null when underlyingType == typeof(byte) => [Convert.ToByte(enumValue)],
            not null when underlyingType == typeof(sbyte) => [unchecked((byte)Convert.ToSByte(enumValue))],
            not null when underlyingType == typeof(short) => BitConverter.GetBytes(Convert.ToInt16(enumValue)),
            not null when underlyingType == typeof(ushort) => BitConverter.GetBytes(Convert.ToUInt16(enumValue)),
            not null when underlyingType == typeof(int) => BitConverter.GetBytes(Convert.ToInt32(enumValue)),
            not null when underlyingType == typeof(uint) => BitConverter.GetBytes(Convert.ToUInt32(enumValue)),
            not null when underlyingType == typeof(long) => BitConverter.GetBytes(Convert.ToInt64(enumValue)),
            not null when underlyingType == typeof(ulong) => BitConverter.GetBytes(Convert.ToUInt64(enumValue)),
            var _ => throw new ArgumentException("Unsupported enum underlying type"),
        };
        return Append(bytes);
    }

    private ByteArrayBuilder AppendEnum<T>(T enumValue) where T : Enum
    {
        var underlyingType = Enum.GetUnderlyingType(typeof(T));
        var bytes = underlyingType switch
        {
            not null when underlyingType == typeof(byte) => [Convert.ToByte(enumValue)],
            not null when underlyingType == typeof(sbyte) => [unchecked((byte)Convert.ToSByte(enumValue))],
            not null when underlyingType == typeof(short) => BitConverter.GetBytes(Convert.ToInt16(enumValue)),
            not null when underlyingType == typeof(ushort) => BitConverter.GetBytes(Convert.ToUInt16(enumValue)),
            not null when underlyingType == typeof(int) => BitConverter.GetBytes(Convert.ToInt32(enumValue)),
            not null when underlyingType == typeof(uint) => BitConverter.GetBytes(Convert.ToUInt32(enumValue)),
            not null when underlyingType == typeof(long) => BitConverter.GetBytes(Convert.ToInt64(enumValue)),
            not null when underlyingType == typeof(ulong) => BitConverter.GetBytes(Convert.ToUInt64(enumValue)),
            var _ => throw new ArgumentException("Unsupported enum underlying type"),
        };
        return Append(bytes);
    }
}
