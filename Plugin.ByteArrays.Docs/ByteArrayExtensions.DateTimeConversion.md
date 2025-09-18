# ByteArrayExtensions.DateTimeConversion

This documentation covers date and time conversion extension methods in `ByteArrayExtensions`. These methods handle DateTime, TimeSpan, DateTimeOffset, and Unix timestamp conversions with high precision and timezone support.

## Overview

DateTime conversion methods provide comprehensive temporal data handling:
- **DateTime** - Binary representation preserving full precision and kind
- **Unix timestamps** - Standard seconds-since-epoch format
- **TimeSpan** - Duration and time interval handling
- **DateTimeOffset** - Timezone-aware timestamps
- **High precision** - Tick-level accuracy for all operations

## DateTime Conversion

### ToDateTime
<xref:Plugin.ByteArrays.ByteArrayExtensions.ToDateTime*>

Converts 8 bytes to DateTime using .NET's binary representation, preserving full precision and DateTimeKind.

#### Usage Examples

```csharp
// Basic DateTime conversion
byte[] dateTimeData = GetDateTimeData();
var position = 0;

DateTime timestamp = dateTimeData.ToDateTime(ref position);
Console.WriteLine($"DateTime: {timestamp}");
Console.WriteLine($"Kind: {timestamp.Kind}"); // Local, Utc, or Unspecified
Console.WriteLine($"Position: {position}"); // 8

// File metadata timestamps
byte[] fileMetadata = GetFileMetadata();
position = 0;
DateTime createdTime = fileMetadata.ToDateTime(ref position);   // Creation time
DateTime modifiedTime = fileMetadata.ToDateTime(ref position);  // Last modified
DateTime accessedTime = fileMetadata.ToDateTime(ref position);  // Last accessed

Console.WriteLine($"File created: {createdTime}");
Console.WriteLine($"Last modified: {modifiedTime}");
Console.WriteLine($"Last accessed: {accessedTime}");

// Database record timestamps
byte[] recordData = GetDatabaseRecord();
position = 0;
int recordId = recordData.ToInt32(ref position);
DateTime insertTime = recordData.ToDateTime(ref position);
DateTime updateTime = recordData.ToDateTime(ref position);

// Log entry parsing
byte[] logEntry = GetLogEntry();
position = 0;
DateTime logTime = logEntry.ToDateTime(ref position);
int logLevel = logEntry.ToInt32(ref position);
string message = logEntry.ToUtf8String(ref position, 100);

Console.WriteLine($"[{logTime:yyyy-MM-dd HH:mm:ss.fff}] {message}");

// High precision timing
byte[] timingData = GetTimingData();
position = 0;
DateTime startTime = timingData.ToDateTime(ref position);
DateTime endTime = timingData.ToDateTime(ref position);

TimeSpan duration = endTime - startTime;
Console.WriteLine($"Operation took: {duration.TotalMilliseconds:F3} ms");
```

### ToDateTimeOrDefault
<xref:Plugin.ByteArrays.ByteArrayExtensions.ToDateTimeOrDefault*>

Safe DateTime conversion with default value fallback.

#### Usage Examples

```csharp
byte[] corruptedData = { 0x01, 0x02, 0x03 }; // Only 3 bytes, need 8
var position = 0;

// Safe conversion with default
DateTime defaultTime = new DateTime(2000, 1, 1);
DateTime parsed = corruptedData.ToDateTimeOrDefault(ref position, defaultTime);
Console.WriteLine($"Parsed: {parsed}"); // 2000-01-01 00:00:00
Console.WriteLine($"Position: {position}"); // 0 (not advanced on failure)

// Safe log parsing
byte[] unreliableLog = GetUnreliableLogData();
position = 0;
DateTime logTime = unreliableLog.ToDateTimeOrDefault(ref position, DateTime.Now);

if (logTime == DateTime.Now.Date) // Check if default was used
{
    Console.WriteLine("Warning: Could not parse log timestamp");
}
```

## Unix Timestamp Conversion

### ToDateTimeFromUnixTimestamp
<xref:Plugin.ByteArrays.ByteArrayExtensions.ToDateTimeFromUnixTimestamp*>

Converts 4-byte Unix timestamps (seconds since January 1, 1970 UTC) to DateTime.

#### Usage Examples

```csharp
// Basic Unix timestamp conversion
byte[] unixData = { 0x00, 0x00, 0x00, 0x60 }; // Example timestamp
var position = 0;

DateTime unixTime = unixData.ToDateTimeFromUnixTimestamp(ref position);
Console.WriteLine($"Unix time: {unixTime}");
Console.WriteLine($"Position: {position}"); // 4

// Web API timestamp parsing
byte[] apiResponse = GetApiResponse();
position = 0;
int statusCode = apiResponse.ToInt32(ref position);
DateTime responseTime = apiResponse.ToDateTimeFromUnixTimestamp(ref position);
string responseData = apiResponse.ToUtf8String(ref position);

Console.WriteLine($"API Response at {responseTime}: {statusCode}");

// Network protocol timestamps
byte[] networkPacket = ReceivePacket();
position = 0;
byte packetType = networkPacket.ToByte(ref position);
DateTime packetTime = networkPacket.ToDateTimeFromUnixTimestamp(ref position);
ushort packetId = networkPacket.ToUInt16(ref position);

// Database Unix timestamp fields
byte[] dbRecord = GetDatabaseRecord();
position = 0;
long userId = dbRecord.ToInt64(ref position);
DateTime createdAt = dbRecord.ToDateTimeFromUnixTimestamp(ref position);
DateTime updatedAt = dbRecord.ToDateTimeFromUnixTimestamp(ref position);

// Bulk timestamp processing
byte[] timestampArray = GetTimestampArray();
var timestamps = new List<DateTime>();
position = 0;

while (position <= timestampArray.Length - 4) // Ensure 4 bytes remain
{
    timestamps.Add(timestampArray.ToDateTimeFromUnixTimestamp(ref position));
}

Console.WriteLine($"Processed {timestamps.Count} timestamps");

// Time range validation
byte[] eventData = GetEventData();
position = 0;
DateTime eventStart = eventData.ToDateTimeFromUnixTimestamp(ref position);
DateTime eventEnd = eventData.ToDateTimeFromUnixTimestamp(ref position);

if (eventEnd < eventStart)
{
    throw new InvalidDataException("Event end time cannot be before start time");
}

TimeSpan eventDuration = eventEnd - eventStart;
Console.WriteLine($"Event duration: {eventDuration}");
```

## TimeSpan Conversion

### ToTimeSpan
<xref:Plugin.ByteArrays.ByteArrayExtensions.ToTimeSpan*>

Converts 8 bytes to TimeSpan using tick representation (100-nanosecond intervals).

#### Usage Examples

```csharp
// Basic TimeSpan conversion
byte[] timeSpanData = GetTimeSpanData();
var position = 0;

TimeSpan duration = timeSpanData.ToTimeSpan(ref position);
Console.WriteLine($"Duration: {duration}");
Console.WriteLine($"Total milliseconds: {duration.TotalMilliseconds}");
Console.WriteLine($"Position: {position}"); // 8

// Media file duration
byte[] mediaMetadata = GetMediaMetadata();
position = 0;
string title = mediaMetadata.ToUtf8String(ref position, 50);
TimeSpan duration = mediaMetadata.ToTimeSpan(ref position);
long fileSize = mediaMetadata.ToInt64(ref position);

Console.WriteLine($"'{title}' - Duration: {duration:hh\\:mm\\:ss}");
Console.WriteLine($"File size: {fileSize:N0} bytes");

// Performance measurement data
byte[] perfData = GetPerformanceData();
position = 0;
int operationId = perfData.ToInt32(ref position);
TimeSpan executionTime = perfData.ToTimeSpan(ref position);
TimeSpan waitTime = perfData.ToTimeSpan(ref position);
TimeSpan totalTime = perfData.ToTimeSpan(ref position);

Console.WriteLine($"Operation {operationId}:");
Console.WriteLine($"  Execution: {executionTime.TotalMilliseconds:F2} ms");
Console.WriteLine($"  Wait: {waitTime.TotalMilliseconds:F2} ms");
Console.WriteLine($"  Total: {totalTime.TotalMilliseconds:F2} ms");

// Timer and scheduling data
byte[] timerData = GetTimerData();
position = 0;
TimeSpan interval = timerData.ToTimeSpan(ref position);    // Timer interval
TimeSpan elapsed = timerData.ToTimeSpan(ref position);     // Time elapsed
TimeSpan remaining = timerData.ToTimeSpan(ref position);   // Time remaining

Console.WriteLine($"Timer: {elapsed}/{interval} (remaining: {remaining})");

// High precision measurements
byte[] precisionData = GetPrecisionTiming();
position = 0;
TimeSpan cpuTime = precisionData.ToTimeSpan(ref position);
TimeSpan userTime = precisionData.ToTimeSpan(ref position);
TimeSpan kernelTime = precisionData.ToTimeSpan(ref position);

Console.WriteLine($"CPU Time: {cpuTime.Ticks} ticks ({cpuTime.TotalMicroseconds:F1} μs)");
Console.WriteLine($"User: {userTime.TotalMicroseconds:F1} μs, Kernel: {kernelTime.TotalMicroseconds:F1} μs");
```

## DateTimeOffset Conversion

### ToDateTimeOffset
<xref:Plugin.ByteArrays.ByteArrayExtensions.ToDateTimeOffset*>

Converts 16 bytes to DateTimeOffset (8 bytes DateTime + 8 bytes TimeSpan offset).

#### Usage Examples

```csharp
// Basic DateTimeOffset conversion
byte[] dateTimeOffsetData = GetDateTimeOffsetData();
var position = 0;

DateTimeOffset timestampWithTz = dateTimeOffsetData.ToDateTimeOffset(ref position);
Console.WriteLine($"Timestamp: {timestampWithTz}");
Console.WriteLine($"UTC: {timestampWithTz.UtcDateTime}");
Console.WriteLine($"Offset: {timestampWithTz.Offset}");
Console.WriteLine($"Position: {position}"); // 16

// International event scheduling
byte[] eventData = GetEventData();
position = 0;
int eventId = eventData.ToInt32(ref position);
DateTimeOffset eventStart = eventData.ToDateTimeOffset(ref position);
DateTimeOffset eventEnd = eventData.ToDateTimeOffset(ref position);
string eventTitle = eventData.ToUtf8String(ref position, 100);

Console.WriteLine($"Event: {eventTitle}");
Console.WriteLine($"Start: {eventStart} ({eventStart.Offset})");
Console.WriteLine($"End: {eventEnd} ({eventEnd.Offset})");
Console.WriteLine($"UTC Start: {eventStart.UtcDateTime}");

// Multi-timezone log correlation
byte[] distributedLog = GetDistributedLogEntry();
position = 0;
string serverId = distributedLog.ToUtf8String(ref position, 10);
DateTimeOffset logTimestamp = distributedLog.ToDateTimeOffset(ref position);
int logLevel = distributedLog.ToInt32(ref position);
string message = distributedLog.ToUtf8String(ref position, 200);

Console.WriteLine($"[{serverId}] [{logTimestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] {message}");
Console.WriteLine($"UTC equivalent: {logTimestamp.UtcDateTime:yyyy-MM-dd HH:mm:ss.fff}");

// Financial transaction timestamps (timezone critical)
byte[] transactionData = GetTransactionData();
position = 0;
long transactionId = transactionData.ToInt64(ref position);
DateTimeOffset transactionTime = transactionData.ToDateTimeOffset(ref position);
decimal amount = (decimal)transactionData.ToDouble(ref position);
string currency = transactionData.ToUtf8String(ref position, 3);

Console.WriteLine($"Transaction {transactionId}:");
Console.WriteLine($"Amount: {amount:C} {currency}");
Console.WriteLine($"Local time: {transactionTime}");
Console.WriteLine($"UTC time: {transactionTime.UtcDateTime}");

// Synchronization across time zones
byte[] syncData = GetSynchronizationData();
var syncPoints = new List<DateTimeOffset>();
position = 0;

while (position <= syncData.Length - 16) // Ensure 16 bytes remain
{
    syncPoints.Add(syncData.ToDateTimeOffset(ref position));
}

// Convert all to UTC for comparison
var utcSyncPoints = syncPoints.Select(dt => dt.UtcDateTime).OrderBy(dt => dt);
Console.WriteLine($"Synchronized events in UTC order:");
foreach (var utcTime in utcSyncPoints)
{
    Console.WriteLine($"  {utcTime:yyyy-MM-dd HH:mm:ss.fff} UTC");
}
```

## Advanced Usage Patterns

### Mixed DateTime Types in Protocols
```csharp
byte[] protocolMessage = ReceiveMessage();
var position = 0;

// Parse message with different time representations
byte messageType = protocolMessage.ToByte(ref position);
DateTime createdAt = protocolMessage.ToDateTime(ref position);              // .NET binary (8 bytes)
DateTime expiresAt = protocolMessage.ToDateTimeFromUnixTimestamp(ref position); // Unix (4 bytes)
TimeSpan validFor = protocolMessage.ToTimeSpan(ref position);              // Duration (8 bytes)
DateTimeOffset scheduledAt = protocolMessage.ToDateTimeOffset(ref position);   // With timezone (16 bytes)

Console.WriteLine($"Message created: {createdAt}");
Console.WriteLine($"Expires: {expiresAt}");
Console.WriteLine($"Valid for: {validFor}");
Console.WriteLine($"Scheduled: {scheduledAt}");
```

### Time Range Validation
```csharp
byte[] timeRangeData = GetTimeRangeData();
var position = 0;

DateTimeOffset rangeStart = timeRangeData.ToDateTimeOffset(ref position);
DateTimeOffset rangeEnd = timeRangeData.ToDateTimeOffset(ref position);
DateTimeOffset queryTime = timeRangeData.ToDateTimeOffset(ref position);

// Validate time range
if (rangeEnd <= rangeStart)
{
    throw new ArgumentException("Invalid time range: end must be after start");
}

// Check if query time is within range
bool isInRange = queryTime >= rangeStart && queryTime <= rangeEnd;
Console.WriteLine($"Query time {queryTime} is {(isInRange ? "within" : "outside")} range [{rangeStart} - {rangeEnd}]");

// Calculate relative position in range
if (isInRange)
{
    var totalDuration = rangeEnd - rangeStart;
    var elapsedDuration = queryTime - rangeStart;
    double percentComplete = (elapsedDuration.TotalMilliseconds / totalDuration.TotalMilliseconds) * 100;
    Console.WriteLine($"Progress: {percentComplete:F1}% through the time range");
}
```

### Performance Timing Analysis
```csharp
byte[] performanceLog = GetPerformanceLog();
var measurements = new List<PerformanceMeasurement>();
var position = 0;

while (position <= performanceLog.Length - 24) // 8 + 8 + 8 bytes per measurement
{
    var measurement = new PerformanceMeasurement
    {
        StartTime = performanceLog.ToDateTime(ref position),
        Duration = performanceLog.ToTimeSpan(ref position),
        CpuTime = performanceLog.ToTimeSpan(ref position)
    };
    measurements.Add(measurement);
}

// Analyze performance data
var totalDuration = measurements.Sum(m => m.Duration.TotalMilliseconds);
var totalCpuTime = measurements.Sum(m => m.CpuTime.TotalMilliseconds);
var avgDuration = measurements.Average(m => m.Duration.TotalMilliseconds);
var maxDuration = measurements.Max(m => m.Duration.TotalMilliseconds);

Console.WriteLine($"Performance Analysis:");
Console.WriteLine($"  Total operations: {measurements.Count}");
Console.WriteLine($"  Total duration: {totalDuration:F2} ms");
Console.WriteLine($"  Total CPU time: {totalCpuTime:F2} ms");
Console.WriteLine($"  Average duration: {avgDuration:F2} ms");
Console.WriteLine($"  Max duration: {maxDuration:F2} ms");
Console.WriteLine($"  CPU efficiency: {(totalCpuTime / totalDuration * 100):F1}%");
```

### Timezone Conversion and Scheduling
```csharp
byte[] schedulingData = GetSchedulingData();
var position = 0;

// Parse scheduled events with different timezones
var events = new List<ScheduledEvent>();
while (position <= schedulingData.Length - 20) // 16 + 4 bytes minimum
{
    var eventTime = schedulingData.ToDateTimeOffset(ref position);
    var eventDuration = TimeSpan.FromMinutes(schedulingData.ToInt32(ref position));

    events.Add(new ScheduledEvent
    {
        StartTime = eventTime,
        EndTime = eventTime.Add(eventDuration),
        Duration = eventDuration
    });
}

// Convert to local timezone for display
var localTimeZone = TimeZoneInfo.Local;
Console.WriteLine($"Schedule (converted to {localTimeZone.DisplayName}):");

foreach (var evt in events.OrderBy(e => e.StartTime.UtcDateTime))
{
    var localStart = TimeZoneInfo.ConvertTime(evt.StartTime, localTimeZone);
    var localEnd = TimeZoneInfo.ConvertTime(evt.EndTime, localTimeZone);

    Console.WriteLine($"  {localStart:yyyy-MM-dd HH:mm} - {localEnd:HH:mm} ({evt.Duration})");
    Console.WriteLine($"    Original: {evt.StartTime} ({evt.StartTime.Offset})");
}
```

## Performance Characteristics

| Operation | Size | Performance | Notes |
|-----------|------|-------------|-------|
| `ToDateTime` | 8 bytes | Fast | Direct binary conversion |
| `ToDateTimeFromUnixTimestamp` | 4 bytes | Fast | Unix epoch calculation |
| `ToTimeSpan` | 8 bytes | Fast | Tick-based conversion |
| `ToDateTimeOffset` | 16 bytes | Moderate | Two conversions + validation |

## Precision and Accuracy

- **DateTime**: 100-nanosecond precision (tick level)
- **Unix timestamps**: 1-second precision
- **TimeSpan**: 100-nanosecond precision (tick level)
- **DateTimeOffset**: 100-nanosecond precision + timezone offset

## Common Use Cases

1. **Log File Analysis**: Parsing timestamps from binary log formats
2. **Database Records**: Reading temporal fields from binary data
3. **Network Protocols**: Handling timestamps in binary messages
4. **File System Metadata**: Processing file creation/modification times
5. **Performance Monitoring**: Measuring execution times and intervals
6. **Scheduling Systems**: Managing timezone-aware appointments
7. **Financial Systems**: Precise transaction timestamps
8. **Distributed Systems**: Cross-timezone event correlation

## Best Practices

- **Use DateTimeOffset** for timezone-aware applications
- **Use Unix timestamps** for cross-platform compatibility
- **Use TimeSpan** for durations and intervals
- **Validate time ranges** to catch data corruption
- **Consider precision requirements** when choosing timestamp formats
- **Handle timezone conversions** carefully in international applications
- **Use OrDefault variants** for robust parsing of potentially corrupted data

## See Also

- <xref:Plugin.ByteArrays.ByteArrayExtensions> - For Unix timestamp integers
- <xref:Plugin.ByteArrays.ByteArrayBuilder> - For constructing temporal byte arrays
- <xref:Plugin.ByteArrays.ByteArrayUtilities> - For temporal data analysis
- <xref:Plugin.ByteArrays.ObjectToByteArrayExtensions> - For DateTime serialization
