# Plugin.ByteArrays - AI Coding Agent Instructions

## Project Overview
This is a .NET 9 library providing comprehensive byte array utilities for performance-critical applications. The library offers multiple specialized components:
- **ByteArrayBuilder**: Fluent builder for constructing byte arrays from various types
- **ByteArrayExtensions**: Extension methods for reading/manipulating byte arrays
- **ByteArrayAsyncExtensions**: Asynchronous operations for file I/O and parallel processing
- **ByteArrayCompressionExtensions**: Compression and decompression utilities
- **ByteArrayUtilities**: Analysis, formatting, and performance measurement tools
- **ByteArrayProtocolExtensions**: Protocol parsing including TLV structures
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
- `ByteArrayExtensions.DateTimeConversion.cs` - DateTime, TimeSpan, DateTimeOffset, Unix timestamps
- `ByteArrayExtensions.NetworkConversion.cs` - IP addresses, endpoints, big-endian conversions
- `ByteArrayExtensions.GuidConversion.cs` - GUID conversion utilities

### Specialized Extension Classes
Each specialized area has its own dedicated class:
- `ByteArrayAsyncExtensions` - Async operations with cancellation support
- `ByteArrayCompressionExtensions` - GZip, Deflate, Brotli compression
- `ByteArrayUtilities` - Analysis, entropy calculation, performance measurement
- `ByteArrayProtocolExtensions` - TLV parsing and protocol structures

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

## Documentation Standards

### Documentation Architecture Principles

#### 1:1 Source-to-Documentation Mapping
- **Every source file must have a corresponding documentation file** in the docs directory
- **Naming convention**: `{SourceFileName}.md` (exact match with source file name)
- **Organizational structure**: Mirror the source code structure in documentation
- **No orphaned documentation**: Remove docs when source files are deleted

#### Dynamic Documentation Structure
The documentation automatically follows the project's source code organization:
- **Partial classes**: Each partial class file gets its own documentation file
- **Extension classes**: Each specialized extension class gets dedicated documentation
- **Utility classes**: Supporting classes receive comprehensive documentation
- **Index organization**: Categorize documentation by logical groupings that reflect source structure

### Universal Documentation Content Standards

#### Required Sections (All Documentation Files)
1. **Class/Module Overview** - Purpose, capabilities, and architectural role
2. **API Documentation** - Complete method coverage with:
   - Syntax examples with proper language formatting
   - Parameter descriptions and types
   - Return value descriptions
   - Realistic usage scenarios
3. **Practical Examples** - Real-world usage patterns and common implementations
4. **Advanced Usage Patterns** - Complex scenarios and integration examples
5. **Performance Characteristics** - Time/space complexity and optimization guidance
6. **Best Practices** - Usage guidelines and common pitfalls
7. **Cross-References** - Links to related components using documentation framework syntax

#### Documentation Writing Standards

##### Code Examples Requirements
```language
// Always provide complete, executable examples
// Include proper error handling demonstrations
// Show both basic and advanced usage patterns
// Use realistic data and scenarios
```

##### Performance Documentation Template
```markdown
### Performance Characteristics
- **Method**: Complexity description with specific metrics
- **Memory Usage**: Allocation patterns and optimization notes
- **Scalability**: Behavior with large datasets
```

##### Cross-Reference Standards
- Use documentation framework's linking syntax for automatic resolution
- Reference related components within the same module
- Link to complementary modules and utilities
- Include bidirectional references where logical

### IDE Integration Requirements

#### Documentation Tooling Integration
- **Task automation**: Integrate documentation generation with IDE task system
- **Build integration**: Include documentation in build/CI processes
- **Live preview**: Support real-time documentation preview during development
- **Template support**: Provide documentation templates for new components

### Documentation Maintenance Protocols

#### Adding New Components
1. **Create corresponding documentation file** using established naming convention
2. **Follow content standards** including all required sections
3. **Update index/navigation** to include new component in appropriate category
4. **Establish cross-references** to related existing components
5. **Validate completeness** against component's public API surface

#### Modifying Existing Components
1. **Update documentation synchronously** with code changes
2. **Refresh examples** to reflect new functionality or changed behavior
3. **Update performance notes** if implementation changes affect characteristics
4. **Maintain cross-reference accuracy** and add new relationships
5. **Preserve backward compatibility notes** for breaking changes

#### Quality Assurance Standards
1. **Executable Examples** - All code samples must compile and run successfully
2. **Practical Relevance** - Examples demonstrate real-world usage, not just syntax
3. **Error Scenario Coverage** - Document failure modes and exception handling
4. **Performance Guidance** - Include actionable optimization recommendations
5. **Consistent Formatting** - Maintain uniform style across all documentation

#### Index and Navigation Management
- **Logical categorization**: Group related components together
- **Clear descriptions**: Provide concise but informative component summaries
- **Hierarchical organization**: Reflect architectural relationships in navigation
- **Search optimization**: Use descriptive headings and consistent terminology

### Documentation Generation Workflow
1. **Source-driven updates**: Documentation changes triggered by source modifications
2. **Automated generation**: Integrate with build system for automatic updates
3. **Quality validation**: Automated checking for completeness and accuracy
4. **Deployment integration**: Include documentation in CI/CD pipeline
5. **Version synchronization**: Ensure documentation versioning matches source releases

### Cross-Reference Architecture
- **Automatic linking**: Use framework-specific syntax for type-safe references
- **Bidirectional relationships**: Link related components in both directions
- **Hierarchical navigation**: Reflect component dependencies and relationships
- **External references**: Include links to relevant standards, specifications, or external resources

This documentation architecture ensures maintainable, comprehensive documentation that evolves with the codebase while providing excellent developer experience through consistent structure and rich cross-references.

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
