using System.Linq.Expressions;
using ArchiToolkit.Assertions.AssertionItems;
using ArchiToolkit.Assertions.Constraints;
using ArchiToolkit.Assertions.Resources;
using ArchiToolkit.Assertions.Utils;

namespace ArchiToolkit.Assertions.Assertions;

/// <summary>
/// The assertion for the collection
/// </summary>
/// <typeparam name="TValue"></typeparam>
/// <typeparam name="TItem"></typeparam>
public class CollectionAssertion<TValue, TItem> : ObjectAssertion<TValue> where TValue : IEnumerable<TItem>
{
    private protected override string ValueString => Value.GetItemsString();

    internal CollectionAssertion(TValue value, string valueName, AssertionType type) : base(value, valueName, type)
    {
    }

    #region ItemEquality

    /// <summary>
    /// Contains items
    /// </summary>
    /// <param name="expectedValue"></param>
    /// <param name="equalityComparer"></param>
    /// <param name="reasonFormat"></param>
    /// <param name="reasonArgs"></param>
    /// <returns></returns>
    public AndConstraint<CollectionAssertion<TValue, TItem>> Contain(TItem expectedValue,
        IEqualityComparer<TItem>? equalityComparer = null, string reasonFormat = "", params object?[] reasonArgs)
    {
        var comparer = equalityComparer ?? EqualityComparer<TItem>.Default;
        if (IsSucceed(Value.Contains(expectedValue, comparer), out var reverse)) return new AndConstraint<CollectionAssertion<TValue, TItem>>(this);

        var message = FormatString(AssertionLocalization.ContainAssertion,
            string.Format(reasonFormat, reasonArgs), reverse,
            new Argument("ExpectedValue", expectedValue));

        AddAssertionItem(AssertionItemType.ItemEquality, message);
        return new AndConstraint<CollectionAssertion<TValue, TItem>>(this);
    }

    /// <summary>
    /// Contains items
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="reasonFormat"></param>
    /// <param name="reasonArgs"></param>
    /// <returns></returns>
    public AndConstraint<CollectionAssertion<TValue, TItem>> Contain(Expression<Func<TItem, bool>> predicate,
        string reasonFormat = "", params object?[] reasonArgs)
    {
        var func = predicate.Compile();
        if (IsSucceed(Value.Any(func), out var reverse)) return new AndConstraint<CollectionAssertion<TValue, TItem>>(this);

        var message = FormatString(AssertionLocalization.ContaionExpressionAssesrtion,
            string.Format(reasonFormat, reasonArgs), reverse,
            new Argument("Expression", predicate.Body));

        AddAssertionItem(AssertionItemType.ItemEquality, message);
        return new AndConstraint<CollectionAssertion<TValue, TItem>>(this);
    }

    /// <summary>
    /// contain items
    /// </summary>
    /// <param name="expectedValues"></param>
    /// <param name="equalityComparer"></param>
    /// <param name="reasonFormat"></param>
    /// <param name="reasonArgs"></param>
    /// <returns></returns>
    public AndConstraint<CollectionAssertion<TValue, TItem>> Contain(IEnumerable<TItem> expectedValues,
        IEqualityComparer<TItem>? equalityComparer = null, string reasonFormat = "", params object?[] reasonArgs)
    {
        var comparer = equalityComparer ?? EqualityComparer<TItem>.Default;
        var values = expectedValues as TItem[] ?? expectedValues.ToArray();
        if (IsSucceed(values.Except(Value, comparer).Any(), out var reverse)) return new AndConstraint<CollectionAssertion<TValue, TItem>>(this);

        var message = FormatString(AssertionLocalization.ContainAssertion,
            string.Format(reasonFormat, reasonArgs), reverse,
            new Argument("ExpectedValues", values.GetItemsString()));

        AddAssertionItem(AssertionItemType.ItemEquality, message);
        return new AndConstraint<CollectionAssertion<TValue, TItem>>(this);
    }

    /// <summary>
    /// have the count.
    /// </summary>
    /// <param name="expectedCount"></param>
    /// <param name="reasonFormat"></param>
    /// <param name="reasonArgs"></param>
    /// <returns></returns>
    public AndConstraint<CollectionAssertion<TValue, TItem>> HaveCount(int expectedCount, string reasonFormat = "",
        params object?[] reasonArgs)
    {
        var actualCount = Value.Count();
        if (IsSucceed(actualCount == expectedCount, out var reverse)) return new AndConstraint<CollectionAssertion<TValue, TItem>>(this);

        var message = FormatString(AssertionLocalization.CountAssertion,
            string.Format(reasonFormat, reasonArgs), reverse,
            new Argument("ActualCount", actualCount),
            new Argument("ExpectedCount", expectedCount));

        AddAssertionItem(AssertionItemType.ItemCount, message);
        return new AndConstraint<CollectionAssertion<TValue, TItem>>(this);
    }

    /// <summary>
    /// greater than.
    /// </summary>
    /// <param name="expectedCount"></param>
    /// <param name="reasonFormat"></param>
    /// <param name="reasonArgs"></param>
    /// <returns></returns>
    public AndConstraint<CollectionAssertion<TValue, TItem>> HaveCountGreaterThan(int expectedCount, string reasonFormat = "",
        params object?[] reasonArgs)
    {
        var actualCount = Value.Count();
        if (IsSucceed(actualCount > expectedCount, out var reverse)) return new AndConstraint<CollectionAssertion<TValue, TItem>>(this);

        var message = FormatString(AssertionLocalization.CountGreaterAssertion,
            string.Format(reasonFormat, reasonArgs), reverse,
            new Argument("ActualCount", actualCount),
            new Argument("ExpectedCount", expectedCount));

        AddAssertionItem(AssertionItemType.ItemCount, message);
        return new AndConstraint<CollectionAssertion<TValue, TItem>>(this);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="expectedCount"></param>
    /// <param name="reasonFormat"></param>
    /// <param name="reasonArgs"></param>
    /// <returns></returns>
    public AndConstraint<CollectionAssertion<TValue, TItem>> HaveCountGreaterThanOrEqualTo(int expectedCount, string reasonFormat = "",
        params object?[] reasonArgs)
    {
        var actualCount = Value.Count();
        if (IsSucceed(actualCount >= expectedCount, out var reverse)) return new AndConstraint<CollectionAssertion<TValue, TItem>>(this);

        var message = FormatString(AssertionLocalization.CountGreaterOrEqualAssertion,
            string.Format(reasonFormat, reasonArgs), reverse,
            new Argument("ActualCount", actualCount),
            new Argument("ExpectedCount", expectedCount));

        AddAssertionItem(AssertionItemType.ItemCount, message);
        return new AndConstraint<CollectionAssertion<TValue, TItem>>(this);
    }

    /// <summary>
    /// The count less than.
    /// </summary>
    /// <param name="expectedCount"></param>
    /// <param name="reasonFormat"></param>
    /// <param name="reasonArgs"></param>
    /// <returns></returns>
    public AndConstraint<CollectionAssertion<TValue, TItem>> HaveCountLessThan(int expectedCount, string reasonFormat = "",
        params object?[] reasonArgs)
    {
        var actualCount = Value.Count();
        if (IsSucceed(actualCount < expectedCount, out var reverse)) return new AndConstraint<CollectionAssertion<TValue, TItem>>(this);

        var message = FormatString(AssertionLocalization.CountLessAssertion,
            string.Format(reasonFormat, reasonArgs), reverse,
            new Argument("ActualCount", actualCount),
            new Argument("ExpectedCount", expectedCount));

        AddAssertionItem(AssertionItemType.ItemCount, message);
        return new AndConstraint<CollectionAssertion<TValue, TItem>>(this);
    }

    /// <summary>
    /// The count less than or equal to.
    /// </summary>
    /// <param name="expectedCount"></param>
    /// <param name="reasonFormat"></param>
    /// <param name="reasonArgs"></param>
    /// <returns></returns>
    public AndConstraint<CollectionAssertion<TValue, TItem>> HaveCountLessThanOrEqualTo(int expectedCount, string reasonFormat = "",
        params object?[] reasonArgs)
    {
        var actualCount = Value.Count();
        if (IsSucceed(actualCount <= expectedCount, out var reverse)) return new AndConstraint<CollectionAssertion<TValue, TItem>>(this);

        var message = FormatString(AssertionLocalization.CountLessOrEqualAssertion,
            string.Format(reasonFormat, reasonArgs), reverse,
            new Argument("ActualCount", actualCount),
            new Argument("ExpectedCount", expectedCount));

        AddAssertionItem(AssertionItemType.ItemCount, message);
        return new AndConstraint<CollectionAssertion<TValue, TItem>>(this);
    }
    #endregion
}