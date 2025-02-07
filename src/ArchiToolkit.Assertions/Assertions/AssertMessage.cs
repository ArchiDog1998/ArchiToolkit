using System.Diagnostics.CodeAnalysis;

namespace ArchiToolkit.Assertions.Assertions;

/// <summary>
/// </summary>
/// <param name="Format"></param>
/// <param name="AdditionalArguments"></param>
public readonly record struct AssertMessage(
    [StringSyntax(StringSyntaxAttribute.CompositeFormat)]
    string Format,
    params Argument[] AdditionalArguments)
{
    /// <summary>
    ///     convert by the default string.
    /// </summary>
    /// <param name="reason"></param>
    /// <returns></returns>
    public static implicit operator AssertMessage(string reason)
    {
        return new AssertMessage(reason);
    }
}