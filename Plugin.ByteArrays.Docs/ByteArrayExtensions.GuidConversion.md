# ByteArrayExtensions.GuidConversion

This documentation covers GUID (Globally Unique Identifier) conversion extension methods in `ByteArrayExtensions`. These methods handle conversion between byte arrays and System.Guid objects for unique identifier management.

## Overview

GUID conversion methods provide reliable unique identifier handling:
- **16-byte precision** - Full GUID representation
- **Cross-platform compatibility** - Standard GUID format
- **Unique identification** - Database keys, object IDs, session tokens
- **Validation** - Automatic format verification

## GUID Conversion

### ToGuid
<xref:Plugin.ByteArrays.ByteArrayExtensions.ToGuid*>

Converts 16 bytes to a System.Guid object using the standard GUID byte layout.

#### Usage Examples

```csharp
// Basic GUID conversion
byte[] guidData = {
    0x12, 0x34, 0x56, 0x78, // Data1 (4 bytes)
    0x9A, 0xBC,             // Data2 (2 bytes)
    0xDE, 0xF0,             // Data3 (2 bytes)
    0x12, 0x34, 0x56, 0x78, // Data4 (8 bytes)
    0x9A, 0xBC, 0xDE, 0xF0
};
var position = 0;

Guid guid = guidData.ToGuid(ref position);
Console.WriteLine($"GUID: {guid}");
Console.WriteLine($"Position: {position}"); // 16

// Database record ID parsing
byte[] recordData = GetDatabaseRecord();
position = 0;
Guid recordId = recordData.ToGuid(ref position);
int recordVersion = recordData.ToInt32(ref position);
string recordName = recordData.ToUtf8String(ref position, 50);

Console.WriteLine($"Record {recordId} (v{recordVersion}): {recordName}");

// Session token parsing
byte[] sessionData = GetSessionData();
position = 0;
Guid sessionId = sessionData.ToGuid(ref position);
DateTime expiresAt = sessionData.ToDateTime(ref position);
long userId = sessionData.ToInt64(ref position);

Console.WriteLine($"Session {sessionId} for user {userId}, expires: {expiresAt}");

// File system unique identifiers
byte[] fileMetadata = GetFileMetadata();
position = 0;
Guid fileId = fileMetadata.ToGuid(ref position);
Guid parentDirectoryId = fileMetadata.ToGuid(ref position);
long fileSize = fileMetadata.ToInt64(ref position);
DateTime createdAt = fileMetadata.ToDateTime(ref position);

Console.WriteLine($"File {fileId} in directory {parentDirectoryId}");
Console.WriteLine($"Size: {fileSize:N0} bytes, Created: {createdAt}");

// Distributed system correlation IDs
byte[] messageData = ReceiveMessage();
position = 0;
Guid correlationId = messageData.ToGuid(ref position);
Guid requestId = messageData.ToGuid(ref position);
byte messageType = messageData.ToByte(ref position);
string payload = messageData.ToUtf8String(ref position);

Console.WriteLine($"Message {requestId} (correlation: {correlationId})");
Console.WriteLine($"Type: {messageType}, Payload: {payload}");

// Bulk GUID processing
byte[] guidArray = GetGuidArray();
var guids = new List<Guid>();
position = 0;

while (position <= guidArray.Length - 16) // Ensure 16 bytes remain
{
    guids.Add(guidArray.ToGuid(ref position));
}

Console.WriteLine($"Processed {guids.Count} GUIDs");
foreach (var g in guids)
{
    Console.WriteLine($"  {g}");
}
```

### ToGuidOrDefault
<xref:Plugin.ByteArrays.ByteArrayExtensions.ToGuidOrDefault*>

Safe GUID conversion with default value fallback for invalid or insufficient data.

#### Usage Examples

```csharp
byte[] corruptedData = { 0x01, 0x02, 0x03, 0x04 }; // Only 4 bytes, need 16
var position = 0;

// Safe conversion with default
Guid defaultGuid = Guid.Empty;
Guid parsed = corruptedData.ToGuidOrDefault(ref position, defaultGuid);
Console.WriteLine($"Parsed: {parsed}"); // 00000000-0000-0000-0000-000000000000
Console.WriteLine($"Position: {position}"); // 0 (not advanced on failure)

// Safe protocol parsing
byte[] unreliableMessage = GetUnreliableMessage();
position = 0;
Guid messageId = unreliableMessage.ToGuidOrDefault(ref position, Guid.NewGuid());

if (messageId == Guid.Empty)
{
    Console.WriteLine("Warning: Could not parse message ID, using generated ID");
}

// Optional field parsing
byte[] optionalData = GetDataWithOptionalGuid();
position = 0;
int mandatoryField = optionalData.ToInt32(ref position);
Guid optionalGuid = optionalData.ToGuidOrDefault(ref position, Guid.Empty);

if (optionalGuid != Guid.Empty)
{
    Console.WriteLine($"Optional GUID present: {optionalGuid}");
    ProcessWithGuid(mandatoryField, optionalGuid);
}
else
{
    Console.WriteLine("No optional GUID provided");
    ProcessWithoutGuid(mandatoryField);
}

// Graceful degradation for corrupted files
byte[] fileHeader = GetPotentiallyCorruptedFile();
position = 0;
string fileSignature = fileHeader.ToAsciiString(ref position, 4);
Guid fileId = fileHeader.ToGuidOrDefault(ref position, Guid.Empty);

if (fileId == Guid.Empty)
{
    Console.WriteLine("Warning: File ID corrupted, generating new ID");
    fileId = Guid.NewGuid();
    RegenerateFileMetadata(fileId);
}
else
{
    LoadExistingMetadata(fileId);
}
```

## Advanced Usage Patterns

### GUID-Based Hierarchical Systems
```csharp
byte[] hierarchyData = GetHierarchyData();
var position = 0;

// Parse hierarchical structure with GUID references
var nodes = new List<HierarchyNode>();
while (position <= hierarchyData.Length - 32) // 16 + 16 bytes minimum
{
    var node = new HierarchyNode
    {
        Id = hierarchyData.ToGuid(ref position),
        ParentId = hierarchyData.ToGuid(ref position)
    };

    // Additional node data
    if (position <= hierarchyData.Length - 4)
    {
        node.Level = hierarchyData.ToInt32(ref position);
    }

    nodes.Add(node);
}

// Build hierarchy tree
var rootNodes = nodes.Where(n => n.ParentId == Guid.Empty).ToList();
Console.WriteLine($"Found {rootNodes.Count} root nodes");

foreach (var root in rootNodes)
{
    PrintHierarchy(root, nodes, 0);
}

void PrintHierarchy(HierarchyNode node, List<HierarchyNode> allNodes, int depth)
{
    var indent = new string(' ', depth * 2);
    Console.WriteLine($"{indent}{node.Id} (Level {node.Level})");

    var children = allNodes.Where(n => n.ParentId == node.Id);
    foreach (var child in children)
    {
        PrintHierarchy(child, allNodes, depth + 1);
    }
}
```

### Distributed System Tracing
```csharp
byte[] traceData = GetDistributedTraceData();
var spans = new List<TraceSpan>();
var position = 0;

while (position <= traceData.Length - 48) // Minimum span data
{
    var span = new TraceSpan
    {
        TraceId = traceData.ToGuid(ref position),      // 16 bytes
        SpanId = traceData.ToGuid(ref position),       // 16 bytes
        ParentSpanId = traceData.ToGuid(ref position), // 16 bytes
        StartTime = traceData.ToDateTime(ref position), // 8 bytes
        Duration = traceData.ToTimeSpan(ref position)   // 8 bytes
    };

    // Optional service name
    if (position < traceData.Length)
    {
        byte nameLength = traceData.ToByte(ref position);
        if (position + nameLength <= traceData.Length)
        {
            span.ServiceName = traceData.ToUtf8String(ref position, nameLength);
        }
    }

    spans.Add(span);
}

// Group by trace ID
var traceGroups = spans.GroupBy(s => s.TraceId);
foreach (var traceGroup in traceGroups)
{
    Console.WriteLine($"Trace {traceGroup.Key}:");
    var orderedSpans = traceGroup.OrderBy(s => s.StartTime);

    foreach (var span in orderedSpans)
    {
        var parentInfo = span.ParentSpanId == Guid.Empty ? "root" : $"parent: {span.ParentSpanId}";
        Console.WriteLine($"  Span {span.SpanId} ({parentInfo})");
        Console.WriteLine($"    Service: {span.ServiceName}");
        Console.WriteLine($"    Duration: {span.Duration.TotalMilliseconds:F2} ms");
    }
}
```

### Database Relationship Mapping
```csharp
byte[] relationshipData = GetRelationshipData();
var relationships = new Dictionary<Guid, List<Guid>>();
var position = 0;

while (position <= relationshipData.Length - 32) // 16 + 16 bytes
{
    Guid fromId = relationshipData.ToGuid(ref position);
    Guid toId = relationshipData.ToGuid(ref position);

    if (!relationships.ContainsKey(fromId))
    {
        relationships[fromId] = new List<Guid>();
    }

    relationships[fromId].Add(toId);
}

// Analyze relationships
Console.WriteLine($"Relationship Analysis:");
Console.WriteLine($"Total entities: {relationships.Keys.Union(relationships.Values.SelectMany(v => v)).Distinct().Count()}");
Console.WriteLine($"Total relationships: {relationships.Values.Sum(v => v.Count)}");

// Find entities with most relationships
var topEntities = relationships
    .OrderByDescending(kvp => kvp.Value.Count)
    .Take(5);

Console.WriteLine($"Top connected entities:");
foreach (var entity in topEntities)
{
    Console.WriteLine($"  {entity.Key}: {entity.Value.Count} connections");
}
```

### GUID Validation and Analysis
```csharp
byte[] guidCollection = GetGuidCollection();
var analysis = new GuidAnalysis();
var position = 0;

while (position <= guidCollection.Length - 16)
{
    Guid guid = guidCollection.ToGuidOrDefault(ref position, Guid.Empty);

    if (guid == Guid.Empty)
    {
        analysis.InvalidCount++;
        continue;
    }

    analysis.ValidCount++;
    analysis.UniqueGuids.Add(guid);

    // Analyze GUID characteristics
    byte[] guidBytes = guid.ToByteArray();

    // Check if it's a sequential GUID (SQL Server style)
    bool isSequential = IsSequentialGuid(guidBytes);
    if (isSequential)
    {
        analysis.SequentialCount++;
    }

    // Check version (usually in byte 6)
    byte version = (byte)((guidBytes[6] & 0xF0) >> 4);
    analysis.VersionCounts[version]++;
}

Console.WriteLine($"GUID Analysis Results:");
Console.WriteLine($"  Valid GUIDs: {analysis.ValidCount}");
Console.WriteLine($"  Invalid GUIDs: {analysis.InvalidCount}");
Console.WriteLine($"  Unique GUIDs: {analysis.UniqueGuids.Count}");
Console.WriteLine($"  Duplicates: {analysis.ValidCount - analysis.UniqueGuids.Count}");
Console.WriteLine($"  Sequential GUIDs: {analysis.SequentialCount}");
Console.WriteLine($"  Version distribution:");

foreach (var versionCount in analysis.VersionCounts.OrderBy(kvp => kvp.Key))
{
    Console.WriteLine($"    Version {versionCount.Key}: {versionCount.Value}");
}

bool IsSequentialGuid(byte[] guidBytes)
{
    // Simple heuristic: check if last 6 bytes show increasing pattern
    // Real implementation would be more sophisticated
    for (int i = 10; i < 15; i++)
    {
        if (guidBytes[i] > guidBytes[i + 1])
        {
            return false;
        }
    }
    return true;
}

class GuidAnalysis
{
    public int ValidCount { get; set; }
    public int InvalidCount { get; set; }
    public int SequentialCount { get; set; }
    public HashSet<Guid> UniqueGuids { get; set; } = new();
    public Dictionary<byte, int> VersionCounts { get; set; } = new();
}
```

### Security Token Processing
```csharp
byte[] securityTokens = GetSecurityTokens();
var tokens = new List<SecurityToken>();
var position = 0;

while (position <= securityTokens.Length - 40) // Minimum token size
{
    var token = new SecurityToken
    {
        TokenId = securityTokens.ToGuid(ref position),        // 16 bytes
        SessionId = securityTokens.ToGuid(ref position),      // 16 bytes
        IssuedAt = securityTokens.ToDateTime(ref position),   // 8 bytes
        ExpiresAt = securityTokens.ToDateTime(ref position)   // 8 bytes
    };

    // Additional token data
    if (position + 8 <= securityTokens.Length)
    {
        token.UserId = securityTokens.ToInt64(ref position);
    }

    tokens.Add(token);
}

// Validate and analyze tokens
var now = DateTime.UtcNow;
var validTokens = tokens.Where(t => t.ExpiresAt > now).ToList();
var expiredTokens = tokens.Where(t => t.ExpiresAt <= now).ToList();

Console.WriteLine($"Security Token Analysis:");
Console.WriteLine($"  Total tokens: {tokens.Count}");
Console.WriteLine($"  Valid tokens: {validTokens.Count}");
Console.WriteLine($"  Expired tokens: {expiredTokens.Count}");

// Group by session
var sessionGroups = validTokens.GroupBy(t => t.SessionId);
Console.WriteLine($"  Active sessions: {sessionGroups.Count()}");

foreach (var session in sessionGroups)
{
    var sessionTokens = session.ToList();
    var earliestToken = sessionTokens.Min(t => t.IssuedAt);
    var latestExpiry = sessionTokens.Max(t => t.ExpiresAt);

    Console.WriteLine($"    Session {session.Key}:");
    Console.WriteLine($"      Tokens: {sessionTokens.Count}");
    Console.WriteLine($"      Started: {earliestToken}");
    Console.WriteLine($"      Expires: {latestExpiry}");
}

class SecurityToken
{
    public Guid TokenId { get; set; }
    public Guid SessionId { get; set; }
    public DateTime IssuedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public long UserId { get; set; }
}
```

## GUID Format and Structure

### Standard GUID Layout (16 bytes)
```csharp
// GUID structure: XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX
// Byte layout:     [0-3]   [4-5][6-7][8-9]  [10-15]
//                 Data1   Data2 Data3 Data4   Data4

byte[] guidBytes = {
    // Data1 (4 bytes) - Little endian
    0x78, 0x56, 0x34, 0x12,
    // Data2 (2 bytes) - Little endian
    0xBC, 0x9A,
    // Data3 (2 bytes) - Little endian
    0xF0, 0xDE,
    // Data4 (8 bytes) - Big endian
    0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0
};

Guid guid = guidBytes.ToGuid(0);
Console.WriteLine($"GUID: {guid}");
// Output: 12345678-9abc-def0-1234-56789abcdef0

// Note: First 8 bytes use little-endian, last 8 bytes use big-endian
// This is the standard Microsoft GUID byte order
```

## Performance Characteristics

| Operation | Size | Performance | Notes |
|-----------|------|-------------|-------|
| `ToGuid` | 16 bytes | Fast | Direct byte array constructor |
| `ToGuidOrDefault` | 16 bytes | Fast | Exception-free validation |

## Common Use Cases

1. **Database Primary Keys**: Unique record identifiers
2. **Session Management**: User session and authentication tokens
3. **Distributed Systems**: Request correlation and tracing IDs
4. **File Systems**: Unique file and directory identifiers
5. **Caching**: Cache key generation and management
6. **Message Queues**: Message and transaction identifiers
7. **APIs**: Request/response correlation tracking
8. **Security**: Token and certificate identifiers

## Best Practices

- **Use Guid.Empty** as a meaningful "null" value for optional GUIDs
- **Validate GUID uniqueness** in critical applications
- **Consider sequential GUIDs** for database performance (SQL Server)
- **Use correlation IDs** for distributed system tracing
- **Handle parsing failures gracefully** with OrDefault methods
- **Store GUIDs efficiently** in binary format rather than strings
- **Use appropriate GUID versions** (v1 for MAC-based, v4 for random)

## GUID Versions

- **Version 1**: Time-based with MAC address
- **Version 2**: DCE security version
- **Version 3**: Name-based using MD5
- **Version 4**: Random or pseudo-random
- **Version 5**: Name-based using SHA-1

## See Also

- <xref:Plugin.ByteArrays.ByteArrayExtensions> - For GUID string representations
- <xref:Plugin.ByteArrays.ByteArrayBuilder> - For constructing GUID byte arrays
- <xref:Plugin.ByteArrays.ByteArrayUtilities> - For GUID analysis and validation
- <xref:Plugin.ByteArrays.ObjectToByteArrayExtensions> - For GUID serialization
