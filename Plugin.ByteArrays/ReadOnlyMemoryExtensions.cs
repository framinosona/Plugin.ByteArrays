namespace Plugin.ByteArrays;

/// <summary>
///     Functions for working with ReadOnlyMemory&lt;byte&gt; providing memory-safe operations for reading and manipulating byte data.
/// </summary>
public static class ReadOnlyMemoryExtensions
{
    #region Core Utilities

    /// <summary>
    ///     Converts the ReadOnlyMemory&lt;byte&gt; into a readable string for debugging purposes.
    ///     Each byte is represented as a decimal value, separated by commas.
    /// </summary>
    /// <param name="memory">The ReadOnlyMemory&lt;byte&gt; to process.</param>
    /// <returns>A string representing the memory as comma-separated decimal numbers.</returns>
    public static string ToDebugString(this ReadOnlyMemory<byte> memory)
    {
        return memory.Span.ToDebugString();
    }

    /// <summary>
    ///     Converts the given ReadOnlyMemory&lt;byte&gt; to its hexadecimal string representation for debugging.
    ///     Each byte is represented as a two-digit hex value, separated by commas.
    /// </summary>
    /// <param name="memory">The ReadOnlyMemory&lt;byte&gt; to process.</param>
    /// <returns>A string representing the memory as comma-separated hex numbers.</returns>
    public static string ToHexDebugString(this ReadOnlyMemory<byte> memory)
    {
        return memory.Span.ToHexDebugString();
    }

    #endregion

    #region Pattern Matching and Search

    /// <summary>
    ///     Checks if a ReadOnlyMemory&lt;byte&gt; starts with a specific pattern.
    /// </summary>
    /// <param name="memory">The memory to check.</param>
    /// <param name="pattern">The pattern to look for.</param>
    /// <returns>True if the memory starts with the pattern, false otherwise.</returns>
    public static bool StartsWith(this ReadOnlyMemory<byte> memory, ReadOnlySpan<byte> pattern)
    {
        return memory.Span.StartsWith(pattern);
    }

    /// <summary>
    ///     Determines whether the ReadOnlyMemory&lt;byte&gt; ends with the specified pattern.
    /// </summary>
    /// <param name="memory">The memory to check.</param>
    /// <param name="pattern">The pattern to check for at the end of the memory.</param>
    /// <returns>True if the memory ends with the specified pattern; otherwise, false.</returns>
    public static bool EndsWith(this ReadOnlyMemory<byte> memory, ReadOnlySpan<byte> pattern)
    {
        return memory.Span.EndsWith(pattern);
    }

    /// <summary>
    ///     Finds the first occurrence of a pattern in a ReadOnlyMemory&lt;byte&gt;.
    /// </summary>
    /// <param name="memory">The memory to search in.</param>
    /// <param name="pattern">The pattern to search for.</param>
    /// <returns>The index of the first occurrence, or -1 if not found.</returns>
    public static int IndexOf(this ReadOnlyMemory<byte> memory, ReadOnlySpan<byte> pattern)
    {
        return memory.Span.IndexOf(pattern);
    }

    #endregion

    #region Comparison and Equality

    /// <summary>
    ///     Checks if two ReadOnlyMemory&lt;byte&gt; instances are identical in content and length.
    /// </summary>
    /// <param name="memory1">First ReadOnlyMemory&lt;byte&gt;.</param>
    /// <param name="memory2">Second ReadOnlyMemory&lt;byte&gt;.</param>
    /// <returns>True if memory regions are identical; otherwise, false.</returns>
    public static bool IsIdenticalTo(this ReadOnlyMemory<byte> memory1, ReadOnlyMemory<byte> memory2)
    {
        return memory1.Span.IsIdenticalTo(memory2.Span);
    }

    #endregion
}
