using System.Diagnostics.CodeAnalysis;
using ArchiToolkit.Assertions.AssertionItems;
using ArchiToolkit.Assertions.Assertions;
using ArchiToolkit.Assertions.Constraints;
using ArchiToolkit.Assertions.Resources;

namespace ArchiToolkit.Assertions;

/// <summary>
/// Nullable struct assertions.
/// </summary>
public static class NullableStructAssertionExtensions
{
    /// <summary>
    /// Whether it have value.
    /// </summary>
    /// <param name="assertion"></param>
    /// <param name="reasonFormat"></param>
    /// <param name="reasonArgs"></param>
    /// <typeparam name="TStruct"></typeparam>
    /// <returns></returns>
    public static AndWhichConstraint<ObjectAssertion<TStruct?>, TStruct> HaveValue<TStruct>(this ObjectAssertion<TStruct?> assertion,
        [StringSyntax(StringSyntaxAttribute.CompositeFormat)]
        string reasonFormat = "", params object?[] reasonArgs)
        where TStruct : struct
    {
        return assertion.AssertCheck(() => assertion.Value!.Value, //Let it throw the exception.
            assertion.Value.HasValue, AssertionItemType.Null,
            AssertionLocalization.HaveValueAssertion, [],
            reasonFormat, reasonArgs);
    }
}