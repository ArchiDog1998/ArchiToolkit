using System.Diagnostics.CodeAnalysis;
using ArchiToolkit.Assertions.AssertionItems;

namespace ArchiToolkit.Assertions.Assertions;

/// <summary>
///     The Assertion
/// </summary>
public interface IAssertion
{
    /// <summary>
    ///     The assertion items.
    /// </summary>
    IReadOnlyList<AssertionItem> Items { get; }

    /// <summary>
    ///     Created time
    /// </summary>
    DateTimeOffset CreatedTime { get; }

    /// <summary>
    ///     Assertion Type
    /// </summary>
    AssertionType Type { get; }
}