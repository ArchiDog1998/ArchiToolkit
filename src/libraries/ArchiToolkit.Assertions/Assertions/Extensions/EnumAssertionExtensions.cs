using ArchiToolkit.Assertions.AssertionItems;
using ArchiToolkit.Assertions.Constraints;
using ArchiToolkit.Assertions.Resources;

namespace ArchiToolkit.Assertions.Assertions.Extensions;

/// <summary>
///     The extension for the enum
/// </summary>
public static class EnumAssertionExtensions
{
    /// <summary>
    ///     Whether it has the flag.
    /// </summary>
    /// <param name="assertion"></param>
    /// <param name="flag"></param>
    /// <param name="assertionParams"></param>
    /// <typeparam name="TEnum"></typeparam>
    /// <returns></returns>
    public static AndConstraint<TEnum> HaveFlag<TEnum>(this ObjectAssertion<TEnum> assertion,
        TEnum flag,
        AssertionParams? assertionParams = null)
        where TEnum : Enum
    {
        return assertion.AssertCheck(assertion.Subject.HasFlag(flag), AssertionItemType.Flag,
            new AssertMessage(AssertionLocalization.FlagAssertion, new Argument("Flag", flag)),
            assertionParams);
    }

    /// <summary>
    ///     Be defined.
    /// </summary>
    /// <param name="assertion"></param>
    /// <param name="assertionParams"></param>
    /// <typeparam name="TEnum"></typeparam>
    /// <returns></returns>
    public static AndConstraint<TEnum> BeDefined<TEnum>(this ObjectAssertion<TEnum> assertion,
        AssertionParams? assertionParams = null)
        where TEnum : Enum
    {
        return assertion.AssertCheck(Enum.IsDefined(typeof(TEnum), assertion.Subject), AssertionItemType.Defined,
            new AssertMessage(AssertionLocalization.EnumDefinedAssertion),
            assertionParams);
    }
}