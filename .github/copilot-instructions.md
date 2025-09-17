# Plugin.ByteArrays - AI Coding Agent Instructions

## Project Overview
This is a .NET 9 library providing comprehensive byte array utilities for performance-critical applications. The library offers three main components:
- **ByteArrayBuilder**: Fluent builder for constructing byte arrays from various types
- **ByteArrayExtensions**: Extension methods for reading/manipulating byte arrays
- **ObjectToByteArrayExtensions**: Type-safe object-to-byte-array conversions

## Architecture Patterns

### Partial Class Design
The `ByteArrayExtensions` class is split across multiple files using `partial class`:
- `ByteArrayExtensions.cs` - Core utilities and pattern matching
- `ByteArrayExtensions.PrimitiveTypeConversion.cs` - Boolean, byte, char conversions
- `ByteArrayExtensions.IntegerConversion.cs` - Integer type conversions
- `ByteArrayExtensions.FloatingPointConversion.cs` - Float, double, Half conversions
- `ByteArrayExtensions.StringConversion.cs` - UTF-8, ASCII, hex, Base64 conversions
- `ByteArrayExtensions.ComplexTypeConversion.cs` - Enum, Version, complex types
- `ByteArrayExtensions.ArrayManipulation.cs` - Slicing, concatenation, trimming, XOR

### Method Overload Pattern
Every conversion method follows this pattern:
```csharp
// Main method with ref position (advances position)
public static T ToType(this byte[] array, ref int position)

// Convenience overload with fixed position
public static T ToType(this byte[] array, int position = 0)

// Safe variant that returns default instead of throwing
public static T ToTypeOrDefault(this byte[] array, ref int position, T defaultValue = default)
public static T ToTypeOrDefault(this byte[] array, int position = 0, T defaultValue = default)
```

### Core Conversion Infrastructure
All conversions use `ExecuteConversionToType<T>()` which:
1. Validates array bounds and position
2. Extracts required bytes using `Skip().Take()`
3. Calls `BitConverter` functions
4. Advances position by type size
5. Provides detailed error messages with array contents via `ToDebugString()`

## Testing Conventions

### Test Structure
- Tests use **xUnit** with **FluentAssertions** (v7.2.0 for licensing)
- Test classes mirror main project structure with 1:1 mapping: `{ClassName}Tests` (e.g., `ByteArrayBuilderTests`)
- Test methods use descriptive names: `Method_Scenario_ExpectedResult`
- Each partial class file has its own dedicated test file for focused testing and maintainability

### Test Patterns
```csharp
// Standard test with position tracking
var data = new byte[] {1, 2, 3, 4};
var p = 0;
data.ToInt32(ref p).Should().Be(expectedValue);
p.Should().Be(4); // Verify position advanced

// Error testing
Action act = () => data.ToInt32(ref p);
act.Should().Throw<ArgumentOutOfRangeException>();
```

## Build System

### Central Package Management
Uses `Directory.Packages.props` with `ManagePackageVersionsCentrally=true`. All package versions defined centrally.

### MSBuild Configuration
- `Directory.Build.props` - Read BEFORE project file (OS detection, CI/CD, excluded folders)
- `Directory.Build.targets` - Read AFTER project file (packaging, versioning, analyzers)
- Strict analyzer settings: `TreatWarningsAsErrors=true`, `AnalysisMode=AllEnabledByDefault`

### Project Structure Variables
Projects use these variables in `.csproj`:
- `$(Project_Name)` - Used for assembly name, package ID, root namespace
- `$(Project_Description)` - Package/assembly description
- `$(Project_Copyright)` - Author/owner information
- `$(Project_Tags)` - NuGet package tags
- `$(Project_Url)` - Repository URL

## Development Workflows

### Build Commands
```bash
# Restore and build
dotnet restore Plugin.ByteArrays.slnx
dotnet build Plugin.ByteArrays.slnx -c Release

# Run tests with coverage (80% minimum)
dotnet test Plugin.ByteArrays.Tests/Plugin.ByteArrays.Tests.csproj \
  --logger "trx" \
  -p:CollectCoverage=true \
  -p:CoverletOutputFormat=cobertura

# Generate packages
dotnet build -p:GeneratePackageOnBuild=true -p:Version_Full=1.0.0
```

### Documentation Generation
Uses **DocFX** via tool manifest:
```bash
dotnet tool install docfx  # If not in .config/dotnet-tools.json
dotnet docfx metadata Plugin.ByteArrays.Docs/docfx.json
dotnet docfx build Plugin.ByteArrays.Docs/docfx.json
```

## Key Implementation Details

### Performance Optimizations
- Uses `ReadOnlySpan<byte>` with `SequenceEqual()` for comparisons (cast explicitly)
- `ByteArrayBuilder` uses `MemoryStream` internally for efficient building
- Implements `IDisposable` and `IAsyncDisposable` for resource management

### Safety Features
- Extensive bounds checking with detailed error messages
- `OrDefault` methods never advance position on failure
- Null argument validation using `ArgumentNullException.ThrowIfNull()`
- Enum validation for undefined values

### String Conversion Specifics
- Hex strings: Support flexible input (spaces, colons, dashes, 0x prefixes)
- UTF-8/ASCII: Length parameter `-1` means "read to end"
- Position tracking: Always passed by reference and auto-incremented

## Error Handling Philosophy
- Throw meaningful exceptions with context (array contents, position, expected size)
- Provide `OrDefault` variants for non-throwing scenarios
- Use `ArgumentOutOfRangeException` for bounds issues
- Include array debug representation in error messages

## CI/CD Integration
- GitHub Actions with automatic versioning (major.minor.patch)
- Deploys to NuGet.org and GitHub Pages documentation
- 80% code coverage requirement enforced
- Supports multi-targeting build matrix

When extending this library, maintain the established patterns for consistency and follow the safety-first approach with comprehensive bounds checking.
