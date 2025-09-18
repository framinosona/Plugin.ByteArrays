# ByteArrayExtensions.NetworkConversion

This documentation covers network-related conversion extension methods in `ByteArrayExtensions`. These methods handle IP addresses, endpoints, and big-endian integer conversions commonly used in network protocols.

## Overview

Network conversion methods provide specialized support for:
- **IP Address parsing** - IPv4 and IPv6 address handling
- **IPEndPoint conversion** - Network endpoint with IP and port
- **Big-endian integers** - Network byte order conversions
- **Protocol compliance** - Standard network data formats

## IP Address Conversion

### ToIPAddress
<xref:Plugin.ByteArrays.ByteArrayExtensions.ToIPAddress*>

Converts byte arrays to IPAddress objects supporting both IPv4 (4 bytes) and IPv6 (16 bytes).

#### Usage Examples

```csharp
// IPv4 address conversion (4 bytes)
byte[] ipv4Data = { 192, 168, 1, 1 };
var position = 0;

IPAddress ipv4 = ipv4Data.ToIPAddress(ref position, isIPv6: false);
Console.WriteLine($"IPv4: {ipv4}"); // 192.168.1.1
Console.WriteLine($"Position: {position}"); // 4

// IPv6 address conversion (16 bytes)
byte[] ipv6Data = {
    0x20, 0x01, 0x0d, 0xb8, // 2001:db8:
    0x85, 0xa3, 0x00, 0x00, // 85a3:0000:
    0x00, 0x00, 0x8a, 0x2e, // 0000:8a2e:
    0x03, 0x70, 0x73, 0x34  // 0370:7334
};
position = 0;

IPAddress ipv6 = ipv6Data.ToIPAddress(ref position, isIPv6: true);
Console.WriteLine($"IPv6: {ipv6}"); // 2001:db8:85a3::8a2e:370:7334

// Network packet parsing
byte[] packetData = GetNetworkPacket();
position = 0;
byte version = packetData.ToByte(ref position);
IPAddress sourceIP = packetData.ToIPAddress(ref position, isIPv6: version == 6);
IPAddress destIP = packetData.ToIPAddress(ref position, isIPv6: version == 6);

Console.WriteLine($"Packet v{version}: {sourceIP} -> {destIP}");

// Bulk IP address processing
byte[] ipListData = GetIPAddressList();
var ipAddresses = new List<IPAddress>();
position = 0;

while (position <= ipListData.Length - 4) // Ensure 4 bytes for IPv4
{
    ipAddresses.Add(ipListData.ToIPAddress(ref position, isIPv6: false));
}

Console.WriteLine($"Processed {ipAddresses.Count} IP addresses");

// DHCP lease data
byte[] dhcpData = GetDHCPLeaseData();
position = 0;
IPAddress assignedIP = dhcpData.ToIPAddress(ref position, false);
IPAddress subnetMask = dhcpData.ToIPAddress(ref position, false);
IPAddress gateway = dhcpData.ToIPAddress(ref position, false);
IPAddress dnsServer = dhcpData.ToIPAddress(ref position, false);

Console.WriteLine($"DHCP Lease:\");
Console.WriteLine($"  IP: {assignedIP}\");
Console.WriteLine($"  Mask: {subnetMask}\");
Console.WriteLine($"  Gateway: {gateway}\");
Console.WriteLine($"  DNS: {dnsServer}\");
```

### ToIPAddressOrDefault
<xref:Plugin.ByteArrays.ByteArrayExtensions.ToIPAddressOrDefault*>

Safe IP address conversion with default value fallback.

#### Usage Examples

```csharp
byte[] corruptedData = { 192, 168 }; // Only 2 bytes, need 4 for IPv4
var position = 0;

IPAddress defaultIP = IPAddress.Loopback;
IPAddress parsed = corruptedData.ToIPAddressOrDefault(ref position, false, defaultIP);
Console.WriteLine($"Parsed: {parsed}"); // 127.0.0.1 (loopback)
Console.WriteLine($"Position: {position}"); // 0 (not advanced on failure)

// Safe network configuration parsing
byte[] configData = GetNetworkConfig();
position = 0;
IPAddress serverIP = configData.ToIPAddressOrDefault(ref position, false, IPAddress.Parse("127.0.0.1"));
IPAddress backupIP = configData.ToIPAddressOrDefault(ref position, false, IPAddress.Parse("127.0.0.1"));

Console.WriteLine($"Primary server: {serverIP}");
Console.WriteLine($"Backup server: {backupIP}");
```

## IPEndPoint Conversion

### ToIPEndPoint
<xref:Plugin.ByteArrays.ByteArrayExtensions.ToIPEndPoint*>

Converts byte arrays to IPEndPoint objects (IP address + port) with network byte order handling.

#### Usage Examples

```csharp
// IPv4 endpoint conversion (6 bytes: 4 IP + 2 port)
byte[] endpointData = {
    192, 168, 1, 100,  // IP address
    0x1F, 0x90         // Port 8080 in big-endian (network byte order)
};
var position = 0;

IPEndPoint endpoint = endpointData.ToIPEndPoint(ref position, isIPv6: false);
Console.WriteLine($"Endpoint: {endpoint}"); // 192.168.1.100:8080
Console.WriteLine($"Position: {position}"); // 6

// Server discovery protocol
byte[] discoveryResponse = GetDiscoveryResponse();
position = 0;
byte serviceType = discoveryResponse.ToByte(ref position);
IPEndPoint serviceEndpoint = discoveryResponse.ToIPEndPoint(ref position, false);
string serviceName = discoveryResponse.ToUtf8String(ref position, 50);

Console.WriteLine($"Discovered service '{serviceName}' at {serviceEndpoint}");

// Load balancer configuration
byte[] lbConfig = GetLoadBalancerConfig();
var endpoints = new List<IPEndPoint>();
position = 0;

byte endpointCount = lbConfig.ToByte(ref position);
for (int i = 0; i < endpointCount; i++)
{
    endpoints.Add(lbConfig.ToIPEndPoint(ref position, false));
}

Console.WriteLine($"Load balancer endpoints:\");
foreach (var ep in endpoints)
{
    Console.WriteLine($"  {ep}\");
}

// Network monitoring data
byte[] monitorData = GetNetworkMonitoringData();
position = 0;
DateTime timestamp = monitorData.ToDateTime(ref position);
IPEndPoint source = monitorData.ToIPEndPoint(ref position, false);
IPEndPoint destination = monitorData.ToIPEndPoint(ref position, false);
long bytesTransferred = monitorData.ToInt64(ref position);

Console.WriteLine($\"[{timestamp}] {source} -> {destination}: {bytesTransferred:N0} bytes\");

// IPv6 endpoint (18 bytes: 16 IP + 2 port)
byte[] ipv6EndpointData = {
    // IPv6 address (16 bytes)
    0x20, 0x01, 0x0d, 0xb8, 0x85, 0xa3, 0x00, 0x00,
    0x00, 0x00, 0x8a, 0x2e, 0x03, 0x70, 0x73, 0x34,
    // Port 443 in big-endian (2 bytes)
    0x01, 0xBB
};

IPEndPoint ipv6Endpoint = ipv6EndpointData.ToIPEndPoint(0, isIPv6: true);
Console.WriteLine($"IPv6 Endpoint: {ipv6Endpoint}");
```

## Big-Endian Integer Conversion

### ToInt16BigEndian, ToInt32BigEndian, ToInt64BigEndian
<xref:Plugin.ByteArrays.ByteArrayExtensions.ToInt16BigEndian*>
<xref:Plugin.ByteArrays.ByteArrayExtensions.ToInt32BigEndian*>
<xref:Plugin.ByteArrays.ByteArrayExtensions.ToInt64BigEndian*>

Converts byte arrays to integers using network byte order (big-endian).

#### Usage Examples

```csharp
// Big-endian 16-bit integer (network byte order)
byte[] int16Data = { 0x12, 0x34 }; // 0x1234 = 4660
short bigEndianShort = int16Data.ToInt16BigEndian(0);
Console.WriteLine($"Big-endian short: {bigEndianShort}\"); // 4660

// Compare with little-endian
short littleEndianShort = int16Data.ToInt16(0);
Console.WriteLine($"Little-endian short: {littleEndianShort}\"); // 13330 (0x3412)

// Big-endian 32-bit integer
byte[] int32Data = { 0x12, 0x34, 0x56, 0x78 }; // 0x12345678 = 305419896
int bigEndianInt = int32Data.ToInt32BigEndian(0);
Console.WriteLine($"Big-endian int: {bigEndianInt}\"); // 305419896

// Network protocol parsing
byte[] protocolHeader = GetProtocolHeader();
var position = 0;
short version = protocolHeader.ToInt16BigEndian(ref position);
int messageLength = protocolHeader.ToInt32BigEndian(ref position);
long sessionId = protocolHeader.ToInt64BigEndian(ref position);

Console.WriteLine($"Protocol v{version}, Message: {messageLength} bytes, Session: {sessionId}\");

// TCP header parsing
byte[] tcpHeader = GetTCPHeader();
position = 0;
short sourcePort = tcpHeader.ToInt16BigEndian(ref position);
short destPort = tcpHeader.ToInt16BigEndian(ref position);
int sequenceNumber = tcpHeader.ToInt32BigEndian(ref position);
int acknowledgmentNumber = tcpHeader.ToInt32BigEndian(ref position);

Console.WriteLine($"TCP: {sourcePort} -> {destPort}\");
Console.WriteLine($"SEQ: {sequenceNumber}, ACK: {acknowledgmentNumber}\");

// HTTP/2 frame parsing
byte[] http2Frame = GetHTTP2Frame();
position = 0;
int frameLength = http2Frame.ToInt32BigEndian(ref position) >> 8; // 24-bit length
byte frameType = http2Frame.ToByte(ref position);
byte flags = http2Frame.ToByte(ref position);
int streamId = http2Frame.ToInt32BigEndian(ref position) & 0x7FFFFFFF; // 31-bit stream ID

Console.WriteLine($"HTTP/2 Frame: Type {frameType}, Length {frameLength}, Stream {streamId}\");
```

## Advanced Usage Patterns

### Network Protocol Stack Parsing
```csharp
byte[] networkPacket = CaptureNetworkPacket();
var position = 0;

// Ethernet header
byte[] destMac = networkPacket.SafeSlice(position, 6);
position += 6;
byte[] srcMac = networkPacket.SafeSlice(position, 6);
position += 6;
short etherType = networkPacket.ToInt16BigEndian(ref position);

Console.WriteLine($"Ethernet: {BitConverter.ToString(srcMac)} -> {BitConverter.ToString(destMac)}\");
Console.WriteLine($"EtherType: 0x{etherType:X4}\");

if (etherType == 0x0800) // IPv4
{
    // IPv4 header
    byte versionAndHeaderLength = networkPacket.ToByte(ref position);
    byte version = (byte)(versionAndHeaderLength >> 4);
    byte headerLength = (byte)((versionAndHeaderLength & 0x0F) * 4);

    position += 1; // Skip type of service
    short totalLength = networkPacket.ToInt16BigEndian(ref position);
    position += 6; // Skip identification, flags, fragment offset
    byte ttl = networkPacket.ToByte(ref position);
    byte protocol = networkPacket.ToByte(ref position);
    position += 2; // Skip checksum

    IPAddress sourceIP = networkPacket.ToIPAddress(ref position, false);
    IPAddress destIP = networkPacket.ToIPAddress(ref position, false);

    Console.WriteLine($"IPv{version}: {sourceIP} -> {destIP}\");
    Console.WriteLine($"Protocol: {protocol}, TTL: {ttl}, Length: {totalLength}\");

    if (protocol == 6) // TCP
    {
        short sourcePort = networkPacket.ToInt16BigEndian(ref position);
        short destPort = networkPacket.ToInt16BigEndian(ref position);
        int seqNum = networkPacket.ToInt32BigEndian(ref position);
        int ackNum = networkPacket.ToInt32BigEndian(ref position);

        Console.WriteLine($"TCP: {sourcePort} -> {destPort}\");
        Console.WriteLine($"SEQ: {seqNum}, ACK: {ackNum}\");
    }
}
```

### DNS Message Parsing
```csharp
byte[] dnsMessage = GetDNSMessage();
var position = 0;

// DNS Header (12 bytes)
short transactionId = dnsMessage.ToInt16BigEndian(ref position);
short flags = dnsMessage.ToInt16BigEndian(ref position);
short questionCount = dnsMessage.ToInt16BigEndian(ref position);
short answerCount = dnsMessage.ToInt16BigEndian(ref position);
short authorityCount = dnsMessage.ToInt16BigEndian(ref position);
short additionalCount = dnsMessage.ToInt16BigEndian(ref position);

Console.WriteLine($"DNS Transaction ID: {transactionId}\");
Console.WriteLine($"Questions: {questionCount}, Answers: {answerCount}\");

bool isResponse = (flags & 0x8000) != 0;
byte opcode = (byte)((flags >> 11) & 0x0F);
byte responseCode = (byte)(flags & 0x0F);

Console.WriteLine($"Type: {(isResponse ? \"Response\" : \"Query\")}, Opcode: {opcode}\");

// Parse DNS questions
for (int i = 0; i < questionCount; i++)
{
    string domainName = ParseDNSName(dnsMessage, ref position);
    short queryType = dnsMessage.ToInt16BigEndian(ref position);
    short queryClass = dnsMessage.ToInt16BigEndian(ref position);

    Console.WriteLine($"Question {i + 1}: {domainName} (Type: {queryType}, Class: {queryClass})\");
}

// Parse DNS answers
for (int i = 0; i < answerCount; i++)
{
    string name = ParseDNSName(dnsMessage, ref position);
    short type = dnsMessage.ToInt16BigEndian(ref position);
    short classCode = dnsMessage.ToInt16BigEndian(ref position);
    int ttl = dnsMessage.ToInt32BigEndian(ref position);
    short dataLength = dnsMessage.ToInt16BigEndian(ref position);

    if (type == 1 && dataLength == 4) // A record
    {
        IPAddress ip = dnsMessage.ToIPAddress(ref position, false);
        Console.WriteLine($"Answer {i + 1}: {name} -> {ip} (TTL: {ttl})\");
    }
    else
    {
        position += dataLength; // Skip unknown record types
    }
}

string ParseDNSName(byte[] data, ref int pos)
{
    var parts = new List<string>();

    while (pos < data.Length)
    {
        byte length = data.ToByte(ref pos);
        if (length == 0) break; // End of name

        if ((length & 0xC0) == 0xC0) // Compression pointer
        {
            short pointer = (short)(((length & 0x3F) << 8) | data.ToByte(ref pos));
            // Handle compression (simplified)
            break;
        }

        string part = data.ToUtf8String(ref pos, length);
        parts.Add(part);
    }

    return string.Join(\".\", parts);
}
```

### SNMP Protocol Parsing
```csharp
byte[] snmpPacket = GetSNMPPacket();
var position = 0;

// SNMP is ASN.1 encoded, but we can parse basic structure
byte sequenceTag = snmpPacket.ToByte(ref position); // 0x30 for SEQUENCE
byte messageLength = snmpPacket.ToByte(ref position);

// SNMP version
position += 2; // Skip version tag and length
int snmpVersion = snmpPacket.ToInt32BigEndian(ref position);

// Community string
position += 1; // Skip string tag
byte communityLength = snmpPacket.ToByte(ref position);
string community = snmpPacket.ToUtf8String(ref position, communityLength);

Console.WriteLine($"SNMP v{snmpVersion + 1}, Community: {community}\");

// PDU type
byte pduType = snmpPacket.ToByte(ref position);
string pduTypeName = pduType switch
{
    0xA0 => \"GetRequest\",
    0xA1 => \"GetNextRequest\",
    0xA2 => \"GetResponse\",
    0xA3 => \"SetRequest\",
    0xA4 => \"Trap\",
    _ => $\"Unknown ({pduType:X2})\"
};

Console.WriteLine($"PDU Type: {pduTypeName}\");
```

### Performance Optimized Network Data Processing
```csharp
byte[] networkStream = GetLargeNetworkStream();
var connections = new Dictionary<IPEndPoint, NetworkConnection>();
var position = 0;

// Process network flow records efficiently
while (position <= networkStream.Length - 20) // Minimum record size
{
    var record = new NetworkFlowRecord
    {
        Timestamp = networkStream.ToDateTimeFromUnixTimestamp(ref position),
        SourceEndpoint = networkStream.ToIPEndPoint(ref position, false),
        DestEndpoint = networkStream.ToIPEndPoint(ref position, false),
        ByteCount = networkStream.ToInt32BigEndian(ref position),
        PacketCount = networkStream.ToInt32BigEndian(ref position)
    };

    // Aggregate connection data
    if (!connections.ContainsKey(record.SourceEndpoint))
    {
        connections[record.SourceEndpoint] = new NetworkConnection();
    }

    var connection = connections[record.SourceEndpoint];
    connection.TotalBytes += record.ByteCount;
    connection.TotalPackets += record.PacketCount;
    connection.LastSeen = record.Timestamp;
}

// Analyze top connections
var topConnections = connections
    .OrderByDescending(kvp => kvp.Value.TotalBytes)
    .Take(10);

Console.WriteLine(\"Top 10 connections by bytes:\");
foreach (var conn in topConnections)
{
    Console.WriteLine($\"  {conn.Key}: {conn.Value.TotalBytes:N0} bytes, {conn.Value.TotalPackets:N0} packets\");
}

class NetworkFlowRecord
{
    public DateTime Timestamp { get; set; }
    public IPEndPoint SourceEndpoint { get; set; }
    public IPEndPoint DestEndpoint { get; set; }
    public int ByteCount { get; set; }
    public int PacketCount { get; set; }
}

class NetworkConnection
{
    public long TotalBytes { get; set; }
    public long TotalPackets { get; set; }
    public DateTime LastSeen { get; set; }
}
```

## Performance Characteristics

| Operation | Size | Performance | Notes |
|-----------|------|-------------|-------|
| `ToIPAddress` (IPv4) | 4 bytes | Fast | Direct array constructor |
| `ToIPAddress` (IPv6) | 16 bytes | Fast | Direct array constructor |
| `ToIPEndPoint` | 6/18 bytes | Fast | IP + port with endian conversion |
| `ToBigEndian` integers | 2-8 bytes | Fast | Byte reversal on little-endian systems |

## Network Byte Order

Network protocols typically use **big-endian** byte order (most significant byte first):

```csharp
// Example: Port 8080 (0x1F90)
// Big-endian (network): [0x1F, 0x90]
// Little-endian (Intel): [0x90, 0x1F]

byte[] networkPort = { 0x1F, 0x90 };
short port = networkPort.ToInt16BigEndian(0); // 8080 (correct)
short wrongPort = networkPort.ToInt16(0);     // 36895 (incorrect on little-endian)
```

## Common Use Cases

1. **Network Protocol Analysis**: Parsing packet headers and payloads
2. **Server Configuration**: Reading network endpoint configurations
3. **Load Balancing**: Managing server endpoint lists
4. **Network Monitoring**: Processing flow and connection data
5. **Protocol Implementation**: Building custom network protocols
6. **Security Analysis**: Parsing network traffic for threats
7. **DNS Processing**: Handling DNS queries and responses
8. **SNMP Management**: Network device monitoring

## Best Practices

- **Use big-endian methods** for network protocol compliance
- **Validate IP address formats** before processing
- **Handle both IPv4 and IPv6** in modern applications
- **Consider endianness** when interfacing with network protocols
- **Use OrDefault variants** for robust parsing of network data
- **Cache IPAddress objects** for frequently used addresses
- **Validate port ranges** (1-65535) for endpoint data

## Protocol Standards

- **RFC 791**: Internet Protocol (IPv4)
- **RFC 2460**: Internet Protocol Version 6 (IPv6)
- **RFC 793**: Transmission Control Protocol (TCP)
- **RFC 768**: User Datagram Protocol (UDP)
- **RFC 1035**: Domain Name System (DNS)

## See Also

- <xref:Plugin.ByteArrays.ByteArrayExtensions> - For little-endian integers and network string data
- <xref:Plugin.ByteArrays.ByteArrayBuilder> - For constructing network packets
- <xref:Plugin.ByteArrays.ByteArrayUtilities> - For network data analysis
