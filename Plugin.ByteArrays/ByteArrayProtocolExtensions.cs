using System.Diagnostics.CodeAnalysis;

namespace Plugin.ByteArrays;

/// <summary>
///     Represents a TLV (Type-Length-Value) structure.
/// </summary>
public readonly struct TlvRecord : IEquatable<TlvRecord>
{
    /// <summary>
    ///     Gets the type field of the TLV record.
    /// </summary>
    public byte Type { get; }

    /// <summary>
    ///     Gets the length field of the TLV record.
    /// </summary>
    public ushort Length { get; }

    /// <summary>
    ///     Gets the value field of the TLV record.
    /// </summary>
    public IReadOnlyList<byte> Value { get; }

    /// <summary>
    ///     Initializes a new instance of the TlvRecord struct.
    /// </summary>
    /// <param name="type">The type field.</param>
    /// <param name="length">The length field.</param>
    /// <param name="value">The value field.</param>
    public TlvRecord(byte type, ushort length, byte[] value)
    {
        Type = type;
        Length = length;
        Value = value ?? Array.Empty<byte>();
    }

    /// <summary>
    ///     Determines whether the specified TlvRecord is equal to the current TlvRecord.
    /// </summary>
    /// <param name="other">The TlvRecord to compare with the current TlvRecord.</param>
    /// <returns>true if the specified TlvRecord is equal to the current TlvRecord; otherwise, false.</returns>
    public bool Equals(TlvRecord other)
    {
        return Type == other.Type && Length == other.Length && Value.SequenceEqual(other.Value);
    }

    /// <summary>
    ///     Determines whether the specified object is equal to the current TlvRecord.
    /// </summary>
    /// <param name="obj">The object to compare with the current TlvRecord.</param>
    /// <returns>true if the specified object is equal to the current TlvRecord; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        return obj is TlvRecord other && Equals(other);
    }

    /// <summary>
    ///     Returns the hash code for this TlvRecord.
    /// </summary>
    /// <returns>A 32-bit signed integer hash code.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(Type, Length, Value.Count);
    }

    /// <summary>
    ///     Determines whether two TlvRecord instances are equal.
    /// </summary>
    /// <param name="left">The first TlvRecord to compare.</param>
    /// <param name="right">The second TlvRecord to compare.</param>
    /// <returns>true if the TlvRecord instances are equal; otherwise, false.</returns>
    public static bool operator ==(TlvRecord left, TlvRecord right)
    {
        return left.Equals(right);
    }

    /// <summary>
    ///     Determines whether two TlvRecord instances are not equal.
    /// </summary>
    /// <param name="left">The first TlvRecord to compare.</param>
    /// <param name="right">The second TlvRecord to compare.</param>
    /// <returns>true if the TlvRecord instances are not equal; otherwise, false.</returns>
    public static bool operator !=(TlvRecord left, TlvRecord right)
    {
        return !left.Equals(right);
    }
}

/// <summary>
///     Protocol-specific extensions for byte arrays including TLV parsing, framing, and checksums.
/// </summary>
public static class ByteArrayProtocolExtensions
{
    #region TLV (Type-Length-Value) Operations

    /// <summary>
    ///     Parses a TLV record from the byte array at the specified position.
    ///     Format: 1-byte type + 2-byte length + variable-length value.
    /// </summary>
    /// <param name="array">The byte array containing TLV data.</param>
    /// <param name="position">The position to start parsing from. Will be advanced by the TLV record size.</param>
    /// <returns>The parsed TLV record.</returns>
    /// <exception cref="ArgumentException">Thrown when the array doesn't contain enough data for a complete TLV record.</exception>
    public static TlvRecord ParseTlvRecord(this byte[] array, ref int position)
    {
        ArgumentNullException.ThrowIfNull(array);

        if (position + 3 > array.Length)
        {
            throw new ArgumentException("Array doesn't contain enough data for TLV header", nameof(array));
        }

        var type = array[position++];
        var length = BitConverter.ToUInt16(array, position);
        position += 2;

        if (position + length > array.Length)
        {
            throw new ArgumentException($"Array doesn't contain enough data for TLV value (expected {length} bytes)", nameof(array));
        }

        var value = new byte[length];
        Array.Copy(array, position, value, 0, length);
        position += length;

        return new TlvRecord(type, length, value);
    }

    /// <summary>
    ///     Parses all TLV records from the byte array.
    /// </summary>
    /// <param name="array">The byte array containing TLV data.</param>
    /// <returns>An enumerable of TLV records.</returns>
    public static IEnumerable<TlvRecord> ParseAllTlvRecords(this byte[] array)
    {
        ArgumentNullException.ThrowIfNull(array);

        var position = 0;
        while (position < array.Length)
        {
            if (position + 3 <= array.Length)
            {
                yield return ParseTlvRecord(array, ref position);
            }
            else
            {
                break; // Not enough data for another TLV record
            }
        }
    }

    /// <summary>
    ///     Creates a TLV record as a byte array.
    /// </summary>
    /// <param name="type">The type field.</param>
    /// <param name="value">The value field.</param>
    /// <returns>A byte array representing the TLV record.</returns>
    public static byte[] CreateTlvRecord(byte type, byte[] value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value.Length > ushort.MaxValue)
        {
            throw new ArgumentException($"Value length cannot exceed {ushort.MaxValue} bytes", nameof(value));
        }

        var length = (ushort)value.Length;
        var result = new byte[3 + length];

        result[0] = type;
        var lengthBytes = BitConverter.GetBytes(length);
        result[1] = lengthBytes[0];
        result[2] = lengthBytes[1];

        Array.Copy(value, 0, result, 3, length);

        return result;
    }

    #endregion

    #region Frame Operations

    /// <summary>
    ///     Adds a simple frame around the data with start/end markers.
    /// </summary>
    /// <param name="data">The data to frame.</param>
    /// <param name="startMarker">The start marker byte (default 0x7E).</param>
    /// <param name="endMarker">The end marker byte (default 0x7E).</param>
    /// <returns>The framed data.</returns>
    public static byte[] AddSimpleFrame(this byte[] data, byte startMarker = 0x7E, byte endMarker = 0x7E)
    {
        ArgumentNullException.ThrowIfNull(data);

        var result = new byte[data.Length + 2];
        result[0] = startMarker;
        Array.Copy(data, 0, result, 1, data.Length);
        result[^1] = endMarker;

        return result;
    }

    /// <summary>
    ///     Removes a simple frame from the data.
    /// </summary>
    /// <param name="framedData">The framed data.</param>
    /// <param name="startMarker">The expected start marker byte (default 0x7E).</param>
    /// <param name="endMarker">The expected end marker byte (default 0x7E).</param>
    /// <returns>The data without framing.</returns>
    /// <exception cref="ArgumentException">Thrown when frame markers are invalid.</exception>
    public static byte[] RemoveSimpleFrame(this byte[] framedData, byte startMarker = 0x7E, byte endMarker = 0x7E)
    {
        ArgumentNullException.ThrowIfNull(framedData);

        if (framedData.Length < 2)
        {
            throw new ArgumentException("Framed data must be at least 2 bytes long", nameof(framedData));
        }

        if (framedData[0] != startMarker)
        {
            throw new ArgumentException($"Invalid start marker: expected 0x{startMarker:X2}, got 0x{framedData[0]:X2}", nameof(framedData));
        }

        if (framedData[^1] != endMarker)
        {
            throw new ArgumentException($"Invalid end marker: expected 0x{endMarker:X2}, got 0x{framedData[^1]:X2}", nameof(framedData));
        }

        var result = new byte[framedData.Length - 2];
        Array.Copy(framedData, 1, result, 0, result.Length);

        return result;
    }

    /// <summary>
    ///     Adds a length-prefixed frame to the data.
    ///     Format: 2-byte length + data.
    /// </summary>
    /// <param name="data">The data to frame.</param>
    /// <returns>The length-prefixed frame.</returns>
    public static byte[] AddLengthPrefixedFrame(this byte[] data)
    {
        ArgumentNullException.ThrowIfNull(data);

        if (data.Length > ushort.MaxValue)
        {
            throw new ArgumentException($"Data length cannot exceed {ushort.MaxValue} bytes", nameof(data));
        }

        var length = (ushort)data.Length;
        var result = new byte[2 + data.Length];

        var lengthBytes = BitConverter.GetBytes(length);
        result[0] = lengthBytes[0];
        result[1] = lengthBytes[1];

        Array.Copy(data, 0, result, 2, data.Length);

        return result;
    }

    /// <summary>
    ///     Removes a length-prefixed frame from the data.
    /// </summary>
    /// <param name="framedData">The length-prefixed framed data.</param>
    /// <returns>The data without the length prefix.</returns>
    /// <exception cref="ArgumentException">Thrown when the frame format is invalid.</exception>
    public static byte[] RemoveLengthPrefixedFrame(this byte[] framedData)
    {
        ArgumentNullException.ThrowIfNull(framedData);

        if (framedData.Length < 2)
        {
            throw new ArgumentException("Length-prefixed frame must be at least 2 bytes long", nameof(framedData));
        }

        var length = BitConverter.ToUInt16(framedData, 0);

        if (framedData.Length != 2 + length)
        {
            throw new ArgumentException($"Frame length mismatch: header indicates {length} bytes, but frame contains {framedData.Length - 2} bytes", nameof(framedData));
        }

        var result = new byte[length];
        Array.Copy(framedData, 2, result, 0, length);

        return result;
    }

    #endregion

    #region Protocol Checksums

    /// <summary>
    ///     Calculates a simple 8-bit checksum (sum of all bytes).
    /// </summary>
    /// <param name="data">The data to calculate checksum for.</param>
    /// <returns>The 8-bit checksum.</returns>
    public static byte CalculateSimpleChecksum(this byte[] data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var sum = 0;
        foreach (var b in data)
        {
            sum += b;
        }

        return (byte)(sum & 0xFF);
    }

    /// <summary>
    ///     Calculates a XOR checksum of all bytes.
    /// </summary>
    /// <param name="data">The data to calculate checksum for.</param>
    /// <returns>The XOR checksum.</returns>
    public static byte CalculateXorChecksum(this byte[] data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var xor = (byte)0;
        foreach (var b in data)
        {
            xor ^= b;
        }

        return xor;
    }

    /// <summary>
    ///     Validates data against its checksum.
    /// </summary>
    /// <param name="dataWithChecksum">The data including the checksum at the end.</param>
    /// <param name="checksumFunction">The function to calculate the checksum.</param>
    /// <returns>True if the checksum is valid, false otherwise.</returns>
    public static bool ValidateChecksum(this byte[] dataWithChecksum, Func<byte[], byte> checksumFunction)
    {
        ArgumentNullException.ThrowIfNull(dataWithChecksum);
        ArgumentNullException.ThrowIfNull(checksumFunction);

        if (dataWithChecksum.Length == 0)
        {
            return false;
        }

        var data = new byte[dataWithChecksum.Length - 1];
        Array.Copy(dataWithChecksum, 0, data, 0, data.Length);

        var expectedChecksum = dataWithChecksum[^1];
        var actualChecksum = checksumFunction(data);

        return expectedChecksum == actualChecksum;
    }

    /// <summary>
    ///     Appends a checksum to the data.
    /// </summary>
    /// <param name="data">The data to add checksum to.</param>
    /// <param name="checksumFunction">The function to calculate the checksum.</param>
    /// <returns>The data with checksum appended.</returns>
    public static byte[] AppendChecksum(this byte[] data, Func<byte[], byte> checksumFunction)
    {
        ArgumentNullException.ThrowIfNull(data);
        ArgumentNullException.ThrowIfNull(checksumFunction);

        var checksum = checksumFunction(data);
        var result = new byte[data.Length + 1];

        Array.Copy(data, 0, result, 0, data.Length);
        result[^1] = checksum;

        return result;
    }

    #endregion
}
