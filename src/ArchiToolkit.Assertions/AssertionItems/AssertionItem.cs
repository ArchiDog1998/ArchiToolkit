using System.Diagnostics;

namespace ArchiToolkit.Assertions.AssertionItems;

/// <summary>
///     The assertion item
/// </summary>
/// <param name="Type"></param>
/// <param name="Message"></param>
/// <param name="StackTrace"></param>
/// <param name="Time"></param>
/// <param name="Tag"></param>
public readonly record struct AssertionItem(
    AssertionItemType Type,
    string Message,
    StackTrace StackTrace,
    DateTimeOffset Time,
    object? Tag);