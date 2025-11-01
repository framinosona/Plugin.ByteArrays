# GitHub Copilot Instructions for Plugin.ByteArrays

## Project Overview

**Plugin.ByteArrays** is a comprehensive .NET library providing utilities for working with byte arrays. It offers high-performance, safe, and easy-to-use extension methods for converting between byte arrays and various data types, including primitives, strings, network types, date/time, GUIDs, compression, and async operations.

**Key Features:**
- Type-safe conversions between byte arrays and 60+ data types
- Support for `byte[]`, `ReadOnlySpan<byte>`, and `ReadOnlyMemory<byte>`
- Builder pattern for constructing byte arrays
- Async operations with cancellation support
- Compression (GZip, Deflate, Brotli)
- Network protocol support (TLV, endianness)
- Extensive null-safety and bounds checking

## Technology Stack

- **Framework:** .NET 9.0
- **Language:** C# (latest version)
- **Testing:** xUnit with FluentAssertions
- **Package Management:** Central Package Management (Directory.Packages.props)
- **CI/CD:** GitHub Actions
- **Documentation:** DocFX (published to GitHub Pages)

## Project Structure

```
Plugin.ByteArrays/
├── .github/
│   ├── workflows/ci.yml          # CI/CD pipeline
│   └── copilot-instructions.md   # This file
├── Plugin.ByteArrays/            # Main library
│   ├── ByteArrayExtensions.*.cs  # Extension methods organized by category
│   ├── ReadOnlySpanExtensions.*.cs
│   ├── ByteArrayBuilder.cs       # Fluent builder
│   └── *.cs                      # Other utilities
├── Plugin.ByteArrays.Tests/      # Unit tests (616+ tests)
└── Plugin.ByteArrays.Docs/       # Documentation site
```

## Development Setup

### Prerequisites
- .NET SDK 9.0.305 or later (see `global.json`)
- IDE: Visual Studio, VS Code, or JetBrains Rider

### Build Commands
```bash
# Restore dependencies
dotnet restore Plugin.ByteArrays.slnx

# Build (Release configuration)
dotnet build Plugin.ByteArrays.slnx --configuration Release --no-restore

# Run tests
dotnet test Plugin.ByteArrays.Tests/Plugin.ByteArrays.Tests.csproj --configuration Release --no-build

# Run tests with coverage
dotnet test Plugin.ByteArrays.Tests/Plugin.ByteArrays.Tests.csproj --configuration Release \
  -p:CollectCoverage=true -p:CoverletOutputFormat=cobertura
```

## Code Organization Patterns

### Extension Method Organization
Extension methods are organized by:
1. **Type category** (e.g., `ByteArrayExtensions.IntegerConversion.cs`)
2. **Target type** (e.g., `ReadOnlySpanExtensions.StringConversion.cs`)

**Pattern:**
- `ByteArrayExtensions.*.cs` - Extensions for `byte[]`
- `ReadOnlySpanExtensions.*.cs` - Extensions for `ReadOnlySpan<byte>`
- `ReadOnlyMemoryExtensions.cs` - Extensions for `ReadOnlyMemory<byte>` (delegates to Span)

### Method Naming Conventions
- **Conversion:** `ToType(...)` - Throws on error
- **Safe Conversion:** `ToTypeOrDefault(...)` - Returns default value on error
- **Position Tracking:** Methods accept `ref int position` to automatically advance
- **Big-Endian:** Methods with `BigEndian` suffix for network byte order

**Examples:**
```csharp
int value = array.ToInt32(ref position);           // Advances position by 4
int value = array.ToInt32OrDefault(ref position);  // Safe version
int value = array.ToInt32BigEndian(ref position);  // Network byte order
```

## Coding Standards

### Naming Conventions (Enforced by .editorconfig)
- **Classes/Structs/Interfaces:** PascalCase
- **Methods:** PascalCase
- **Properties:** PascalCase
- **Private fields:** `_camelCase` (underscore prefix)
- **Constants:** PascalCase
- **Parameters/Local variables:** camelCase

### Style Guidelines
- **Line length:** Max 240 characters
- **Indentation:** 4 spaces for C#
- **End of line:** LF (Unix-style)
- **Var usage:** Prefer `var` for built-in types and when type is apparent
- **Braces:** Always required, even for single-line statements
- **Expression bodies:** Avoid for methods and constructors; allow for properties when single-line
- **Usings:** System directives first, separated from others

### Documentation Requirements
- **XML documentation:** Required for all public APIs
- **Format:** Use `<summary>`, `<param>`, `<returns>`, `<exception>` tags
- **Description:** Be concise and accurate
- **Examples:** Include in `<example>` tags for complex methods

**Example:**
```csharp
/// <summary>
///     Converts the first four bytes of the array to a 32-bit signed integer.
///     Advances the position by 4 bytes.
/// </summary>
/// <param name="array">The byte array to process. Cannot be null.</param>
/// <param name="position">The current position in the array. Will be advanced by 4.</param>
/// <returns>The converted 32-bit signed integer.</returns>
/// <exception cref="ArgumentNullException">Thrown if array is null.</exception>
/// <exception cref="ArgumentOutOfRangeException">Thrown if there are not enough bytes.</exception>
public static int ToInt32(this byte[] array, ref int position)
```

## Testing Guidelines

### Testing Framework
- **Framework:** xUnit
- **Assertions:** FluentAssertions (v7.2.0 - open source version)
- **Coverage:** Coverlet (target: high coverage)

### Test Organization
- **File naming:** `{ClassName}Tests.cs` (matches source file)
- **Test methods:** Use descriptive names following pattern: `MethodName_Scenario_ExpectedBehavior`
- **Categories:** Organize tests by feature/scenario

### Test Patterns
```csharp
[Fact]
public void ToInt32_WithValidData_ReturnsCorrectValue()
{
    // Arrange
    var array = new byte[] { 0x01, 0x00, 0x00, 0x00 };
    var position = 0;
    
    // Act
    var result = array.ToInt32(ref position);
    
    // Assert
    result.Should().Be(1);
    position.Should().Be(4);
}

[Theory]
[InlineData(new byte[] { 0x00 }, "insufficient bytes")]
public void ToInt32_WithInsufficientBytes_ThrowsException(byte[] array, string reason)
{
    // Arrange
    var position = 0;
    
    // Act & Assert
    var action = () => array.ToInt32(ref position);
    action.Should().Throw<ArgumentOutOfRangeException>();
}
```

### Testing Best Practices
- **Test both success and failure paths**
- **Test boundary conditions** (null, empty, exact size, too small)
- **Test position advancement** for methods with `ref position`
- **Use theory/inline data** for multiple similar test cases
- **Avoid test interdependencies** - each test should be independent

## Performance Considerations

### Span<T> vs Arrays
- **Prefer `ReadOnlySpan<byte>`** for performance-critical code (no allocations)
- **Use `byte[]`** when you need to store/return data
- **Use `ReadOnlyMemory<byte>`** for async scenarios or when you need to capture a slice

### Allocation Avoidance
- Avoid unnecessary allocations in hot paths
- Use `stackalloc` for small temporary buffers
- Reuse buffers when possible with `ArrayPool<T>`

### Best Practices
```csharp
// Good: No allocations
public static int ToInt32(this ReadOnlySpan<byte> span, ref int position)
{
    // Direct span manipulation
}

// Good: Reuse buffer
var pool = ArrayPool<byte>.Shared;
var buffer = pool.Rent(size);
try
{
    // Use buffer
}
finally
{
    pool.Return(buffer);
}
```

## Common Patterns

### Builder Pattern (ByteArrayBuilder)
```csharp
var bytes = new ByteArrayBuilder()
    .Append(42)
    .Append("Hello")
    .Append(DateTime.Now)
    .ToArray();
```

### Safe Conversion with OrDefault
```csharp
// Instead of try-catch, use OrDefault variants
var value = array.ToInt32OrDefault(ref position, defaultValue: -1);
```

### Position Tracking
```csharp
// Automatically advance position through a buffer
var position = 0;
var id = buffer.ToInt32(ref position);      // position now = 4
var name = buffer.ToStringUtf8(ref position, length: 20); // position now = 24
var timestamp = buffer.ToDateTime(ref position); // position now = 32
```

## Anti-Patterns to Avoid

### ❌ Don't Do This
```csharp
// Manual array slicing (inefficient)
var subset = array.Skip(offset).Take(length).ToArray();
var value = BitConverter.ToInt32(subset, 0);

// Ignoring position parameter
var value = array.ToInt32(0); // Position doesn't advance

// Manual byte manipulation
var value = array[0] | (array[1] << 8) | (array[2] << 16) | (array[3] << 24);
```

### ✅ Do This Instead
```csharp
// Use Span for slicing (zero-copy)
var value = array.AsSpan(offset, length).ToInt32();

// Track position with ref
var position = 0;
var value = array.ToInt32(ref position); // Advances automatically

// Use provided extension methods
var value = array.ToInt32(ref position);
```

## Dependencies

### Production Dependencies
- `Microsoft.Extensions.Logging.Abstractions` (9.0.0) - For logging support

### Development Dependencies
- `xunit` (2.9.3) - Testing framework
- `xunit.runner.visualstudio` (3.1.5) - Visual Studio test adapter
- `FluentAssertions` (7.2.0) - Fluent assertion library (open source v7)
- `coverlet.collector` (6.0.4) - Code coverage collector
- `coverlet.msbuild` (6.0.4) - Code coverage MSBuild integration
- `Microsoft.NET.Test.Sdk` (18.0.0) - .NET Test SDK
- `JetBrains.Annotations` (2025.2.0) - JetBrains code annotations
- `Microsoft.CodeAnalysis.NetAnalyzers` (9.0.0) - Static analysis
- `Microsoft.SourceLink.GitHub` (8.0.0) - Source link support

## CI/CD Pipeline

The GitHub Actions workflow (`ci.yml`) performs:
1. **Versioning** - Automatic semantic versioning
2. **Build** - Restore, build with Release configuration
3. **Test** - Run all tests with coverage
4. **Package** - Create NuGet packages
5. **Documentation** - Build and deploy DocFX documentation to GitHub Pages
6. **Publish** - Publish to NuGet.org (on main branch)
7. **Release** - Create GitHub release with auto-generated notes

### Triggers
- Push to `main`/`master` branches
- Pull requests to `main`/`master`
- Manual workflow dispatch

## Contributing Guidelines

When contributing code:
1. **Follow existing patterns** - Match the organization and style of existing code
2. **Add tests** - All new functionality must have comprehensive tests
3. **Update documentation** - Add XML docs and update README if needed
4. **Keep changes minimal** - Focus on one feature/fix per PR
5. **Performance matters** - Consider allocation and performance impact
6. **Null safety** - Handle null inputs appropriately
7. **Bounds checking** - Always validate array/span boundaries

## Common Tasks

### Adding a New Conversion Type

1. **Add extension method** to appropriate file (e.g., `ByteArrayExtensions.IntegerConversion.cs`)
2. **Add ReadOnlySpan variant** in corresponding Span file
3. **Add ReadOnlyMemory support** (usually delegates to Span)
4. **Include both throwing and OrDefault variants**
5. **Add XML documentation**
6. **Create comprehensive tests**
7. **Update README table** with new type support

### Adding a New Feature Category

1. **Create new partial class file** (e.g., `ByteArrayExtensions.NewFeature.cs`)
2. **Follow naming pattern** `{ClassName}.{Category}.cs`
3. **Add corresponding test file** `{ClassName}.NewFeatureTests.cs`
4. **Document in README** under Features section

## Release Process

Releases are automated via GitHub Actions:
1. Merge PR to `main` branch
2. CI pipeline runs automatically
3. Version is calculated from commits (semantic versioning)
4. NuGet package is published
5. GitHub release is created with auto-generated notes
6. Documentation is deployed to GitHub Pages

## Useful Resources

- **Repository:** https://github.com/framinosona/Plugin.ByteArrays
- **NuGet Package:** https://www.nuget.org/packages/Plugin.ByteArrays
- **Documentation:** https://framinosona.github.io/Plugin.ByteArrays/
- **.NET Docs:** https://docs.microsoft.com/dotnet/
- **Span<T> Guide:** https://docs.microsoft.com/dotnet/api/system.span-1

## Troubleshooting

### Build Issues
- **Ensure .NET 9.0.305+ is installed** - Check with `dotnet --version`
- **Clean and restore** - Run `dotnet clean && dotnet restore`
- **Check global.json** - SDK version must match or be compatible

### Test Failures
- **Run tests in Release** - Some tests may behave differently in Debug mode
- **Check test output** - Use `dotnet test --logger "console;verbosity=detailed"`
- **Coverage issues** - Ensure coverlet packages are restored

### Common Errors
- **CS0246** - Missing package reference or using directive
- **CS8600/CS8602** - Null reference warnings (enable nullable context)
- **IDE0008** - Use `var` instead of explicit type
