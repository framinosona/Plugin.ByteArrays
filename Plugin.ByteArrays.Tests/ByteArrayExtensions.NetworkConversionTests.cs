using FluentAssertions;
using System.Net;

namespace Plugin.ByteArrays.Tests;

public class ByteArrayExtensionsNetworkConversionTests
{
    #region IPAddress Conversion Tests

    [Fact]
    public void ToIPAddress_WithIPv4_ShouldReturnCorrectAddress()
    {
        // Arrange
        var ipAddress = IPAddress.Parse("192.168.1.1");
        var data = ipAddress.GetAddressBytes();
        var position = 0;

        // Act
        var result = data.ToIPAddress(ref position);

        // Assert
        result.Should().Be(ipAddress);
        position.Should().Be(4);
    }

    [Fact]
    public void ToIPAddress_WithIPv6_ShouldReturnCorrectAddress()
    {
        // Arrange
        var ipAddress = IPAddress.Parse("2001:0db8:85a3:0000:0000:8a2e:0370:7334");
        var data = ipAddress.GetAddressBytes();
        var position = 0;

        // Act
        var result = data.ToIPAddress(ref position, isIPv6: true);

        // Assert
        result.Should().Be(ipAddress);
        position.Should().Be(16);
    }

    [Fact]
    public void ToIPAddressOrDefault_WithInvalidData_ShouldReturnDefault()
    {
        // Arrange
        var data = new byte[2]; // Too short for IPv4
        var position = 0;

        // Act
        var result = data.ToIPAddressOrDefault(ref position);

        // Assert
        result.Should().Be(IPAddress.Loopback);
        position.Should().Be(0);
    }

    #endregion

    #region IPEndPoint Conversion Tests

    [Fact]
    public void ToIPEndPoint_WithIPv4_ShouldReturnCorrectEndPoint()
    {
        // Arrange
        var ipAddress = IPAddress.Parse("192.168.1.100");
        var port = (ushort)8080;
        var addressBytes = ipAddress.GetAddressBytes();
        var portBytes = BitConverter.GetBytes(port);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(portBytes); // Convert to big-endian for network byte order
        }
        var data = addressBytes.Concat(portBytes).ToArray();
        var position = 0;

        // Act
        var result = data.ToIPEndPoint(ref position);

        // Assert
        result.Address.Should().Be(ipAddress);
        result.Port.Should().Be(port);
        position.Should().Be(6); // 4 bytes for IPv4 + 2 bytes for port
    }

    [Fact]
    public void ToIPEndPointOrDefault_WithInvalidData_ShouldReturnDefault()
    {
        // Arrange
        var data = new byte[4]; // Too short for IPEndPoint (needs 6 bytes for IPv4)
        var position = 0;
        var defaultValue = new IPEndPoint(IPAddress.Any, 8080);

        // Act
        var result = data.ToIPEndPointOrDefault(ref position, defaultValue: defaultValue);

        // Assert
        result.Should().Be(defaultValue);
        position.Should().Be(4); // Position advances by 4 because IP portion succeeds before port fails
    }

    #endregion

    #region Big-Endian Numeric Conversions

    [Fact]
    public void ToInt16BigEndian_WithValidData_ShouldReturnCorrectValue()
    {
        // Arrange
        var value = (short)0x1234;
        var data = new byte[] { 0x12, 0x34 }; // Big-endian representation
        var position = 0;

        // Act
        var result = data.ToInt16BigEndian(ref position);

        // Assert
        result.Should().Be(value);
        position.Should().Be(2);
    }

    [Fact]
    public void ToInt32BigEndian_WithValidData_ShouldReturnCorrectValue()
    {
        // Arrange
        var value = 0x12345678;
        var data = new byte[] { 0x12, 0x34, 0x56, 0x78 }; // Big-endian representation
        var position = 0;

        // Act
        var result = data.ToInt32BigEndian(ref position);

        // Assert
        result.Should().Be(value);
        position.Should().Be(4);
    }

    [Fact]
    public void ToUInt16BigEndian_WithValidData_ShouldReturnCorrectValue()
    {
        // Arrange
        var value = (ushort)0xABCD;
        var data = new byte[] { 0xAB, 0xCD }; // Big-endian representation
        var position = 0;

        // Act
        var result = data.ToUInt16BigEndian(ref position);

        // Assert
        result.Should().Be(value);
        position.Should().Be(2);
    }

    [Fact]
    public void ToUInt32BigEndian_WithValidData_ShouldReturnCorrectValue()
    {
        // Arrange
        var value = 0xDEADBEEFU;
        var data = new byte[] { 0xDE, 0xAD, 0xBE, 0xEF }; // Big-endian representation
        var position = 0;

        // Act
        var result = data.ToUInt32BigEndian(ref position);

        // Assert
        result.Should().Be(value);
        position.Should().Be(4);
    }

    [Fact]
    public void ToUInt64BigEndian_WithValidData_ShouldReturnCorrectValue()
    {
        // Arrange
        var value = 0xFEDCBA9876543210UL;
        var data = new byte[] { 0xFE, 0xDC, 0xBA, 0x98, 0x76, 0x54, 0x32, 0x10 }; // Big-endian
        var position = 0;

        // Act
        var result = data.ToUInt64BigEndian(ref position);

        // Assert
        result.Should().Be(value);
        position.Should().Be(8);
    }

    [Fact]
    public void ToInt64BigEndian_WithValidData_ShouldReturnCorrectValue()
    {
        // Arrange
        var value = 0x123456789ABCDEF0L;
        var data = new byte[] { 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0 }; // Big-endian
        var position = 0;

        // Act
        var result = data.ToInt64BigEndian(ref position);

        // Assert
        result.Should().Be(value);
        position.Should().Be(8);
    }

    [Fact]
    public void ToIPEndPoint_WithIPv6_ShouldReturnCorrectEndPoint()
    {
        // Arrange
        var ipAddress = IPAddress.Parse("2001:db8::1");
        var port = (ushort)443;
        var addressBytes = ipAddress.GetAddressBytes();
        var portBytes = BitConverter.GetBytes(port);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(portBytes); // Convert to big-endian for network byte order
        }
        var data = addressBytes.Concat(portBytes).ToArray();
        var position = 0;

        // Act
        var result = data.ToIPEndPoint(ref position, isIPv6: true);

        // Assert
        result.Address.Should().Be(ipAddress);
        result.Port.Should().Be(port);
        position.Should().Be(18); // 16 bytes for IPv6 + 2 bytes for port
    }

    [Fact]
    public void ToIPAddress_WithFixedPosition_ShouldReturnCorrectAddress()
    {
        // Arrange
        var ipAddress = IPAddress.Parse("10.0.0.1");
        var data = ipAddress.GetAddressBytes();

        // Act
        var result = data.ToIPAddress(0);

        // Assert
        result.Should().Be(ipAddress);
    }

    [Fact]
    public void ToIPAddressOrDefault_WithIPv6InvalidData_ShouldReturnIPv6Default()
    {
        // Arrange
        var data = new byte[8]; // Too short for IPv6
        var position = 0;

        // Act
        var result = data.ToIPAddressOrDefault(ref position, isIPv6: true);

        // Assert
        result.Should().Be(IPAddress.IPv6Loopback);
        position.Should().Be(0); // Position should not advance on failure
    }

    [Fact]
    public void ToIPAddress_WithIPv6Loopback_ShouldReturnIPv6Loopback()
    {
        // Arrange
        var data = IPAddress.IPv6Loopback.GetAddressBytes();
        var position = 0;

        // Act
        var result = data.ToIPAddress(ref position, isIPv6: true);

        // Assert
        result.Should().Be(IPAddress.IPv6Loopback);
        position.Should().Be(16);
    }

    [Fact]
    public void BigEndian_vs_LittleEndian_ShouldProduceDifferentResults()
    {
        // Arrange
        var data = new byte[] { 0x12, 0x34, 0x56, 0x78 };
        var position1 = 0;
        var position2 = 0;

        // Act
        var bigEndianResult = data.ToInt32BigEndian(ref position1);
        var littleEndianResult = data.ToInt32(ref position2);

        // Assert
        bigEndianResult.Should().NotBe(littleEndianResult);
        bigEndianResult.Should().Be(0x12345678);
        if (BitConverter.IsLittleEndian)
        {
            littleEndianResult.Should().Be(0x78563412);
        }
    }

    #endregion

    #region Error Handling Tests

    [Fact]
    public void ToIPAddress_WithNullArray_ShouldThrowArgumentNullException()
    {
        // Arrange
        byte[] data = null!;
        var position = 0;

        // Act
        Action act = () => data.ToIPAddress(ref position);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ToInt32BigEndian_WithNullArray_ShouldThrowArgumentNullException()
    {
        // Arrange
        byte[] data = null!;
        var position = 0;

        // Act
        Action act = () => data.ToInt32BigEndian(ref position);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ToIPEndPoint_WithNullArray_ShouldThrowArgumentNullException()
    {
        // Arrange
        byte[] data = null!;
        var position = 0;

        // Act
        Action act = () => data.ToIPEndPoint(ref position);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ToIPAddressOrDefault_WithNullArray_ShouldReturnDefault()
    {
        // Arrange
        byte[] data = null!;
        var position = 0;
        var customDefault = IPAddress.Parse("127.0.0.1");

        // Act
        var result = data.ToIPAddressOrDefault(ref position, defaultValue: customDefault);

        // Assert
        result.Should().Be(customDefault);
        position.Should().Be(0);
    }

    [Fact]
    public void ToIPAddress_WithInsufficientData_ShouldThrowException()
    {
        // Arrange
        var data = new byte[2]; // Too short for IPv4
        var position = 0;

        // Act
        Action act = () => data.ToIPAddress(ref position);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void ToIPEndPoint_WithInsufficientData_ShouldThrowException()
    {
        // Arrange
        var data = new byte[5]; // Too short for IPEndPoint (needs 6 bytes)
        var position = 0;

        // Act
        Action act = () => data.ToIPEndPoint(ref position);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void ToInt16BigEndian_WithInvalidData_ShouldThrowException()
    {
        // Arrange
        var data = new byte[1]; // Too short for int16
        var position = 0;

        // Act
        Action act = () => data.ToInt16BigEndian(ref position);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(5)]
    public void ToInt16BigEndian_WithInvalidPosition_ShouldThrowArgumentOutOfRangeException(int invalidPosition)
    {
        // Arrange
        var data = new byte[4];
        var position = invalidPosition;

        // Act
        Action act = () => data.ToInt16BigEndian(ref position);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void ToIPAddress_WithAnyAddress_ShouldReturnAnyAddress()
    {
        // Arrange
        var data = IPAddress.Any.GetAddressBytes();
        var position = 0;

        // Act
        var result = data.ToIPAddress(ref position);

        // Assert
        result.Should().Be(IPAddress.Any);
        position.Should().Be(4);
    }

    [Fact]
    public void ToIPAddress_WithBroadcastAddress_ShouldReturnBroadcastAddress()
    {
        // Arrange
        var data = IPAddress.Broadcast.GetAddressBytes();
        var position = 0;

        // Act
        var result = data.ToIPAddress(ref position);

        // Assert
        result.Should().Be(IPAddress.Broadcast);
        position.Should().Be(4);
    }

    [Fact]
    public void ToBigEndian_WithZeroValue_ShouldReturnZero()
    {
        // Arrange
        var data = new byte[] { 0x00, 0x00, 0x00, 0x00 };
        var position = 0;

        // Act
        var result = data.ToInt32BigEndian(ref position);

        // Assert
        result.Should().Be(0);
        position.Should().Be(4);
    }

    [Fact]
    public void ToBigEndian_WithMaxValue_ShouldReturnMaxValue()
    {
        // Arrange
        var data = new byte[] { 0xFF, 0xFF };
        var position = 0;

        // Act
        var result = data.ToUInt16BigEndian(ref position);

        // Assert
        result.Should().Be(ushort.MaxValue);
        position.Should().Be(2);
    }

    [Fact]
    public void MultipleNetworkConversions_ShouldWorkSequentially()
    {
        // Arrange
        var ipBytes = IPAddress.Parse("10.0.0.1").GetAddressBytes();
        var port = (ushort)8080;
        var portBytes = BitConverter.GetBytes(port);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(portBytes); // Convert to big-endian
        }
        var sequenceNumber = 0x12345678;
        var seqBytes = new byte[] { 0x12, 0x34, 0x56, 0x78 }; // Big-endian

        var data = ipBytes.Concat(portBytes).Concat(seqBytes).ToArray();
        var position = 0;

        // Act
        var extractedIP = data.ToIPAddress(ref position);
        var extractedPort = data.ToUInt16BigEndian(ref position);
        var extractedSeq = data.ToUInt32BigEndian(ref position);

        // Assert
        extractedIP.Should().Be(IPAddress.Parse("10.0.0.1"));
        extractedPort.Should().Be(port);
        extractedSeq.Should().Be((uint)sequenceNumber);
        position.Should().Be(10); // 4 + 2 + 4
    }

    #endregion
}
