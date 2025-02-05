using System.Diagnostics.CodeAnalysis;
using ArchiToolkit.Assertions.AssertionItems;
using ArchiToolkit.Assertions.Constraints;
using ArchiToolkit.Assertions.Resources;

namespace ArchiToolkit.Assertions.Assertions.Extensions;

/// <summary>
/// The extension for the enum
/// </summary>
public static class EnumAssertionExtensions
{
    /// <summary>
    /// Whether it has the flag.
    /// </summary>
    /// <param name="assertion"></param>
    /// <param name="flag"></param>
    /// <param name="reasonFormat"></param>
    /// <param name="reasonArgs"></param>
    /// <typeparam name="TEnum"></typeparam>
    /// <returns></returns>
    public static AndConstraint<ObjectAssertion<TEnum>> HaveFlag<TEnum>(this ObjectAssertion<TEnum> assertion,
        TEnum flag,
        [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string reasonFormat = "", params object?[] reasonArgs)
        where TEnum : Enum
    {
        return assertion.AssertCheck(assertion.Subject.HasFlag(flag), AssertionItemType.Flag,
            AssertionLocalization.FlagAssertion,
            [
                new Argument("Flag", flag)
            ],
            reasonFormat, reasonArgs);
    }

    /// <summary>
    /// Be defined.
    /// </summary>
    /// <param name="assertion"></param>
    /// <param name="reasonFormat"></param>
    /// <param name="reasonArgs"></param>
    /// <typeparam name="TEnum"></typeparam>
    /// <returns></returns>
    public static AndConstraint<ObjectAssertion<TEnum>> BeDefined<TEnum>(this ObjectAssertion<TEnum> assertion,
        [StringSyntax(StringSyntaxAttribute.CompositeFormat)] string reasonFormat = "", params object?[] reasonArgs)
        where TEnum : Enum
    {
        return assertion.AssertCheck(Enum.IsDefined(typeof(TEnum), assertion.Subject) , AssertionItemType.Defined,
            AssertionLocalization.EnumDefinedAssertion,
            [
            ],
            reasonFormat, reasonArgs);
    }
}