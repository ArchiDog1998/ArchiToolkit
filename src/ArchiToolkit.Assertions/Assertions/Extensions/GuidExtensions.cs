using System.Diagnostics.CodeAnalysis;
using ArchiToolkit.Assertions.AssertionItems;
using ArchiToolkit.Assertions.Constraints;
using ArchiToolkit.Assertions.Resources;

namespace ArchiToolkit.Assertions.Assertions.Extensions;

/// <summary>
///     Extension for the guid
/// </summary>
public static class GuidExtensions
{
    /// <summary>
    ///     Empty/
    /// </summary>
    /// <param name="assertion"></param>
    /// <param name="reasonFormat"></param>
    /// <param name="reasonArgs"></param>
    /// <returns></returns>
    public static AndConstraint<Guid> BeEmpty(this ObjectAssertion<Guid> assertion,
        [StringSyntax(StringSyntaxAttribute.CompositeFormat)]
        string reasonFormat = "", params object?[] reasonArgs)
    {
        return assertion.AssertCheck(assertion.Subject == Guid.Empty, AssertionItemType.Empty,
            AssertionLocalization.EmptyAssertion,
            [
            ],
            reasonFormat, reasonArgs);
    }
}