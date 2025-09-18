# ByteArrayExtensions.FloatingPointConversion

This documentation covers floating-point type conversion extension methods in `ByteArrayExtensions`. These methods handle single-precision (float), double-precision (double), and half-precision (Half) floating-point numbers with IEEE 754 compliance.

## Overview

Floating-point conversion methods handle decimal numbers stored in binary format using IEEE 754 standard. All methods follow the established pattern:
- **Main methods** with `ref int position` that advance the position automatically
- **Convenience overloads** with fixed position (default 0)
- **Safe variants** (`OrDefault`) that return default values instead of throwing exceptions
- **2, 4, or 8 byte operations** depending on the precision level

## Double-Precision Conversion (64-bit)

### ToDouble
<xref:Plugin.ByteArrays.ByteArrayExtensions.ToDouble*>

Converts 8 bytes to a double-precision floating-point number with range approximately ±1.7 × 10^308.

#### Usage Examples

```csharp
// Basic double conversion
byte[] data = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x3F }; // 1.0 in IEEE 754
var position = 0;

double value = data.ToDouble(ref position); // 1.0
Console.WriteLine($"Position after conversion: {position}"); // 8

// Scientific notation values
byte[] scientificData = GetScientificData();
position = 0;
double largeNumber = scientificData.ToDouble(ref position);  // e.g., 1.23e+100
double smallNumber = scientificData.ToDouble(ref position);  // e.g., 4.56e-50

// Practical use: Precise calculations
byte[] coordinateData = GetGPSData();
position = 0;
double latitude = coordinateData.ToDouble(ref position);   // High precision coordinates
double longitude = coordinateData.ToDouble(ref position);
double altitude = coordinateData.ToDouble(ref position);

// Financial calculations requiring precision
byte[] financialData = GetFinancialData();
position = 0;
double accountBalance = financialData.ToDouble(ref position);
double interestRate = financialData.ToDouble(ref position);
double exchangeRate = financialData.ToDouble(ref position);

// Special values
byte[] specialValues = GetSpecialDoubleValues();
double positiveInfinity = specialValues.ToDouble(0);
double negativeInfinity = specialValues.ToDouble(8);
double notANumber = specialValues.ToDouble(16);

Console.WriteLine($"Is Infinity: {double.IsInfinity(positiveInfinity)}");
Console.WriteLine($"Is NaN: {double.IsNaN(notANumber)}");
```

### ToDoubleOrDefault
<xref:Plugin.ByteArrays.ByteArrayExtensions.ToDoubleOrDefault*>

Safe double conversion with default value fallback.

#### Usage Examples

```csharp
byte[] shortData = { 1, 2, 3 }; // Only 3 bytes, need 8
var position = 0;

double invalid = shortData.ToDoubleOrDefault(ref position); // 0.0 (default)
Console.WriteLine($"Position: {position}"); // 0 (not advanced)

// Custom default for missing data
double temperature = shortData.ToDoubleOrDefault(0, defaultValue: -273.15); // Absolute zero
```

## Single-Precision Conversion (32-bit)

### ToSingle
<xref:Plugin.ByteArrays.ByteArrayExtensions.ToSingle*>

Converts 4 bytes to a single-precision floating-point number with range approximately ±3.4 × 10^38.

#### Usage Examples

```csharp
// Basic float conversion
byte[] data = { 0x00, 0x00, 0x80, 0x3F }; // 1.0f in IEEE 754
var position = 0;

float value = data.ToSingle(ref position); // 1.0f
Console.WriteLine($"Position after conversion: {position}"); // 4

// Graphics and game development
byte[] vertexData = GetVertexData();
position = 0;
float x = vertexData.ToSingle(ref position);  // Vertex X coordinate
float y = vertexData.ToSingle(ref position);  // Vertex Y coordinate
float z = vertexData.ToSingle(ref position);  // Vertex Z coordinate
float u = vertexData.ToSingle(ref position);  // Texture U coordinate
float v = vertexData.ToSingle(ref position);  // Texture V coordinate

// Sensor readings with moderate precision
byte[] sensorData = GetSensorData();
position = 0;
float temperature = sensorData.ToSingle(ref position);  // -40°C to +85°C
float humidity = sensorData.ToSingle(ref position);     // 0% to 100%
float pressure = sensorData.ToSingle(ref position);     // Atmospheric pressure

// Audio processing
byte[] audioSamples = GetAudioSamples();
var samples = new List<float>();
position = 0;

while (position < audioSamples.Length - 3) // Ensure 4 bytes available
{
    float sample = audioSamples.ToSingle(ref position);
    samples.Add(sample);
}

// Performance optimization: float vs double
// Use float when:
// - Memory usage is critical
// - GPU processing (graphics)
// - Large arrays of values
// - Precision requirements are moderate
```

## Half-Precision Conversion (16-bit)

### ToHalf
<xref:Plugin.ByteArrays.ByteArrayExtensions.ToHalf*>

Converts 2 bytes to a half-precision floating-point number with range approximately ±65,504.

#### Usage Examples

```csharp
// Basic Half conversion
byte[] data = { 0x00, 0x3C }; // 1.0 in half-precision
var position = 0;

Half value = data.ToHalf(ref position); // (Half)1.0
Console.WriteLine($"Position after conversion: {position}"); // 2

// Convert to other types
float asFloat = (float)value;
double asDouble = (double)value;

// Machine learning and AI (common use case)
byte[] neuralWeights = GetNeuralNetworkWeights();
position = 0;
var weights = new List<Half>();

while (position < neuralWeights.Length - 1) // Ensure 2 bytes available
{
    Half weight = neuralWeights.ToHalf(ref position);
    weights.Add(weight);
}

// Graphics programming (modern GPUs support half-precision)
byte[] colorData = GetHDRColorData();
position = 0;
Half red = colorData.ToHalf(ref position);
Half green = colorData.ToHalf(ref position);
Half blue = colorData.ToHalf(ref position);
Half alpha = colorData.ToHalf(ref position);

// Memory-efficient storage
byte[] compressedData = GetCompressedFloatData();
var values = new Half[1000];
position = 0;

for (int i = 0; i < values.Length && position < compressedData.Length - 1; i++)
{
    values[i] = compressedData.ToHalf(ref position);
}

// Range limitations (important to understand)
// Half precision limitations:
// - Limited range: ±65,504
// - Limited precision: ~3-4 significant digits
// - Suitable for: graphics, ML weights, compressed data
// - Not suitable for: precise calculations, large numbers
```

## Advanced Usage Patterns

### Mixed Precision Data Structures
```csharp
byte[] mixedData = GetMixedPrecisionData();
var position = 0;

// Protocol with mixed floating-point types
Half compressionRatio = mixedData.ToHalf(ref position);      // 2 bytes
float processingTime = mixedData.ToSingle(ref position);     // 4 bytes
double preciseResult = mixedData.ToDouble(ref position);     // 8 bytes

Console.WriteLine($"Total bytes processed: {position}"); // 14
```

### Special Value Handling
```csharp
byte[] specialData = GetDataWithSpecialValues();
var position = 0;

while (position < specialData.Length - 7) // Ensure 8 bytes for double
{
    double value = specialData.ToDouble(ref position);

    if (double.IsNaN(value))
    {
        Console.WriteLine("Invalid measurement detected");
        continue;
    }

    if (double.IsInfinity(value))
    {
        Console.WriteLine("Overflow detected");
        continue;
    }

    // Process valid value
    ProcessValidValue(value);
}
```

### Precision Comparison and Conversion
```csharp
byte[] precisionTest = GetPrecisionTestData();

// Same binary data interpreted at different precisions
Half halfValue = precisionTest.ToHalf(0);
float floatValue = precisionTest.ToSingle(0);
double doubleValue = precisionTest.ToDouble(0);

Console.WriteLine($"Half:   {halfValue}");
Console.WriteLine($"Float:  {floatValue}");
Console.WriteLine($"Double: {doubleValue}");

// Precision loss demonstration
double original = 1.23456789012345;
Half compressed = (Half)original;
double restored = (double)compressed;

Console.WriteLine($"Original: {original}");
Console.WriteLine($"Restored: {restored}");
Console.WriteLine($"Loss:     {Math.Abs(original - restored)}");
```

### Performance-Optimized Reading
```csharp
byte[] largeFloatArray = GetLargeFloatArray();
var floatList = new List<float>(largeFloatArray.Length / 4);
var position = 0;

// Efficient bulk reading
while (position <= largeFloatArray.Length - 4) // Ensure 4 bytes remain
{
    floatList.Add(largeFloatArray.ToSingle(ref position));
}

// Alternative: unsafe code for maximum performance (advanced)
unsafe
{
    fixed (byte* ptr = largeFloatArray)
    {
        float* floatPtr = (float*)ptr;
        int count = largeFloatArray.Length / 4;

        for (int i = 0; i < count; i++)
        {
            floatList.Add(floatPtr[i]);
        }
    }
}
```

### Error Handling and Validation
```csharp
byte[] unreliableData = GetUnreliableFloatData();
var validValues = new List<double>();
var position = 0;

while (position < unreliableData.Length)
{
    double value = unreliableData.ToDoubleOrDefault(ref position, defaultValue: double.NaN);

    if (!double.IsNaN(value) && !double.IsInfinity(value))
    {
        // Additional validation
        if (value >= -1000.0 && value <= 1000.0) // Expected range
        {
            validValues.Add(value);
        }
    }
}

Console.WriteLine($"Extracted {validValues.Count} valid values");
```

## IEEE 754 Format Details

### Binary Representation
```csharp
// Understanding the binary format
double value = 1.0;
byte[] bytes = BitConverter.GetBytes(value);

Console.WriteLine($"Double 1.0 as bytes: {string.Join(", ", bytes.Select(b => $"0x{b:X2}"))}");
// Output: 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xF0, 0x3F

// Sign bit, exponent, and mantissa
var bits = BitConverter.DoubleToInt64Bits(value);
var sign = (bits >> 63) & 1;
var exponent = (bits >> 52) & 0x7FF;
var mantissa = bits & 0xFFFFFFFFFFFFF;

Console.WriteLine($"Sign: {sign}, Exponent: {exponent}, Mantissa: {mantissa}");
```

## Performance Characteristics

| Type | Size | Precision | Performance | Use Cases |
|------|------|-----------|-------------|----------|
| `Half` | 2 bytes | ~3-4 digits | Fastest | Graphics, ML, compression |
| `float` | 4 bytes | ~6-7 digits | Fast | Games, sensors, audio |
| `double` | 8 bytes | ~15-16 digits | Moderate | Science, finance, precision |

## Common Use Cases

1. **Scientific Computing**: High-precision calculations requiring double precision
2. **Graphics Programming**: Vertex data, colors, transforms using float/Half
3. **Machine Learning**: Neural network weights often stored as Half precision
4. **Audio Processing**: Sample data typically stored as float
5. **Game Development**: Position, rotation, scale data using float
6. **Financial Systems**: Currency and interest calculations using double
7. **Sensor Data**: Temperature, pressure readings using float
8. **Geographic Data**: GPS coordinates requiring double precision

## Best Practices

- **Use `double`** for: Financial calculations, scientific computing, GPS coordinates
- **Use `float`** for: Graphics, audio, sensors, general-purpose decimal values
- **Use `Half`** for: Memory-constrained scenarios, ML models, compressed storage
- **Validate special values**: Always check for NaN and Infinity in critical applications
- **Consider endianness**: Ensure byte order matches your data source
- **Handle precision loss**: Be aware of rounding errors in floating-point arithmetic

## See Also

- <xref:Plugin.ByteArrays.ByteArrayExtensions> - For whole number conversions and basic type conversions
- <xref:Plugin.ByteArrays.ByteArrayBuilder> - For constructing byte arrays from floating-point values
- <xref:Plugin.ByteArrays.ByteArrayUtilities> - For analysis and measurement tools
