using ArchiToolkit.Assertions.AssertionItems;
using ArchiToolkit.Assertions.Constraints;
using ArchiToolkit.Assertions.Resources;

namespace ArchiToolkit.Assertions.Assertions.Extensions;

/// <summary>
///     Nullable struct assertions.
/// </summary>
public static class NullableStructAssertionExtensions
{
    /// <summary>
    ///     Whether it have value.
    /// </summary>
    /// <param name="assertion"></param>
    /// <param name="assertionParams"></param>
    /// <typeparam name="TStruct"></typeparam>
    /// <returns></returns>
    public static AndWhichConstraint<TStruct?, TStruct> HaveValue<TStruct>(
        this ObjectAssertion<TStruct?> assertion,
        AssertionParams? assertionParams = null)
        where TStruct : struct
    {
        return assertion.AssertCheck(() => assertion.Subject!.Value, ".Value", //Let it throw the exception.
            assertion.Subject.HasValue, AssertionItemType.Null,
            AssertionLocalization.HaveValueAssertion, assertionParams);
    }
}