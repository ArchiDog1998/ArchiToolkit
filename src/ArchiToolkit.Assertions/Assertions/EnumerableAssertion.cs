using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using ArchiToolkit.Assertions.AssertionItems;
using ArchiToolkit.Assertions.Constraints;
using ArchiToolkit.Assertions.Resources;
using ArchiToolkit.Assertions.Utils;

namespace ArchiToolkit.Assertions.Assertions;

/// <summary>
///     The assertion for the collection
/// </summary>
/// <typeparam name="TValue"></typeparam>
/// <typeparam name="TItem"></typeparam>
public class EnumerableAssertion<TValue, TItem> : ObjectAssertion<TValue> where TValue : IEnumerable<TItem>
{
    internal EnumerableAssertion(TValue subject, string valueName, AssertionType type) : base(subject, valueName, type)
    {
    }

    internal AndWhichConstraint<EnumerableAssertion<TValue, TItem>, TItem> AssertCheck(Func<TItem> result, bool succeed, AssertionItemType assertionItemType,
        [StringSyntax(StringSyntaxAttribute.CompositeFormat)]
        string formatString, Argument[] additionalArguments,
        [StringSyntax(StringSyntaxAttribute.CompositeFormat)]
        string reasonFormat, object?[] reasonArgs)
    {
        return AssertCheck(new AndWhichConstraint<EnumerableAssertion<TValue, TItem>, TItem>(this, result), succeed, assertionItemType, formatString,
            additionalArguments, reasonFormat, reasonArgs);
    }

    internal new AndConstraint<EnumerableAssertion<TValue, TItem>> AssertCheck(bool succeed, AssertionItemType assertionItemType,
        [StringSyntax(StringSyntaxAttribute.CompositeFormat)]
        string formatString, Argument[] additionalArguments,
        [StringSyntax(StringSyntaxAttribute.CompositeFormat)]
        string reasonFormat, object?[] reasonArgs)
    {
        return AssertCheck(new AndConstraint<EnumerableAssertion<TValue, TItem>>(this), succeed, assertionItemType, formatString,
            additionalArguments, reasonFormat, reasonArgs);
    }

    /// <summary>
    /// Not
    /// </summary>
    /// <returns></returns>
    public new EnumerableAssertion<TValue, TItem> Not
    {
        get
        {
            ReverseNot();
            return this;
        }
    }

    #region ItemEquality

    /// <summary>
    ///     Get if the item contain single.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="reasonFormat"></param>
    /// <param name="reasonArgs"></param>
    /// <returns></returns>
    public AndWhichConstraint<EnumerableAssertion<TValue, TItem>, TItem>
        ContainSingle(Expression<Func<TItem, bool>> predicate, string reasonFormat = "", params object?[] reasonArgs)
    {
        var func = predicate.Compile();
        var items = Subject.Where(func).ToArray();

        return AssertCheck(() => items.First(), items.Length is 1, AssertionItemType.ItemEquality,
            AssertionLocalization.ContainSingleExpressionAssertion,
            [
                new Argument("MatchedCount", items.Length),
                new Argument("Expression", predicate.Body)
            ],
            reasonFormat, reasonArgs);
    }

    /// <summary>
    ///     Contains items
    /// </summary>
    /// <param name="expectedValue"></param>
    /// <param name="equalityComparer"></param>
    /// <param name="reasonFormat"></param>
    /// <param name="reasonArgs"></param>
    /// <returns></returns>
    public AndConstraint<EnumerableAssertion<TValue, TItem>> Contain(TItem expectedValue,
        IEqualityComparer<TItem>? equalityComparer = null, string reasonFormat = "", params object?[] reasonArgs)
    {
        var comparer = equalityComparer ?? EqualityComparer<TItem>.Default;

        return AssertCheck(Subject.Contains(expectedValue, comparer), AssertionItemType.ItemEquality,
            AssertionLocalization.ContainAssertion,
            [
                new Argument("ExpectedValue", expectedValue)
            ],
            reasonFormat, reasonArgs);
    }

    /// <summary>
    ///     Contains items
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="reasonFormat"></param>
    /// <param name="reasonArgs"></param>
    /// <returns></returns>
    public AndConstraint<EnumerableAssertion<TValue, TItem>> Contain(Expression<Func<TItem, bool>> predicate,
        string reasonFormat = "", params object?[] reasonArgs)
    {
        var func = predicate.Compile();
        return AssertCheck(Subject.Any(func), AssertionItemType.ItemEquality,
            AssertionLocalization.ContainExpressionAssertion,
            [
                new Argument("Expression", predicate.Body)
            ],
            reasonFormat, reasonArgs);
    }

    /// <summary>
    ///     contain items
    /// </summary>
    /// <param name="expectedValues"></param>
    /// <param name="equalityComparer"></param>
    /// <param name="reasonFormat"></param>
    /// <param name="reasonArgs"></param>
    /// <returns></returns>
    public AndConstraint<EnumerableAssertion<TValue, TItem>> Contain(IEnumerable<TItem> expectedValues,
        IEqualityComparer<TItem>? equalityComparer = null, string reasonFormat = "", params object?[] reasonArgs)
    {
        var comparer = equalityComparer ?? EqualityComparer<TItem>.Default;
        var values = expectedValues as TItem[] ?? expectedValues.ToArray();

        return AssertCheck(values.Except(Subject, comparer).Any(), AssertionItemType.ItemEquality,
            AssertionLocalization.ContainAssertion,
            [
                new Argument("ExpectedValues", values)
            ],
            reasonFormat, reasonArgs);
    }

    #endregion

    #region Item Count

    /// <summary>
    ///     have the count.
    /// </summary>
    /// <param name="expectedCount"></param>
    /// <param name="reasonFormat"></param>
    /// <param name="reasonArgs"></param>
    /// <returns></returns>
    public AndConstraint<EnumerableAssertion<TValue, TItem>> HaveCount(int expectedCount, string reasonFormat = "",
        params object?[] reasonArgs)
    {
        var actualCount = Subject.Count();

        return AssertCheck(actualCount == expectedCount, AssertionItemType.ItemCount,
            AssertionLocalization.CountAssertion,
            [
                new Argument("ActualCount", actualCount),
                new Argument("ExpectedCount", expectedCount)
            ],
            reasonFormat, reasonArgs);
    }

    /// <summary>
    ///     greater than.
    /// </summary>
    /// <param name="expectedCount"></param>
    /// <param name="reasonFormat"></param>
    /// <param name="reasonArgs"></param>
    /// <returns></returns>
    public AndConstraint<EnumerableAssertion<TValue, TItem>> HaveCountGreaterThan(int expectedCount,
        string reasonFormat = "",
        params object?[] reasonArgs)
    {
        var actualCount = Subject.Count();
        return AssertCheck(actualCount > expectedCount, AssertionItemType.ItemCount,
            AssertionLocalization.CountGreaterAssertion,
            [
                new Argument("ActualCount", actualCount),
                new Argument("ExpectedCount", expectedCount)
            ],
            reasonFormat, reasonArgs);
    }

    /// <summary>
    /// </summary>
    /// <param name="expectedCount"></param>
    /// <param name="reasonFormat"></param>
    /// <param name="reasonArgs"></param>
    /// <returns></returns>
    public AndConstraint<EnumerableAssertion<TValue, TItem>> HaveCountGreaterThanOrEqualTo(int expectedCount,
        string reasonFormat = "",
        params object?[] reasonArgs)
    {
        var actualCount = Subject.Count();
        return AssertCheck(actualCount >= expectedCount, AssertionItemType.ItemCount,
            AssertionLocalization.CountGreaterOrEqualAssertion,
            [
                new Argument("ActualCount", actualCount),
                new Argument("ExpectedCount", expectedCount)
            ],
            reasonFormat, reasonArgs);
    }

    /// <summary>
    ///     The count less than.
    /// </summary>
    /// <param name="expectedCount"></param>
    /// <param name="reasonFormat"></param>
    /// <param name="reasonArgs"></param>
    /// <returns></returns>
    public AndConstraint<EnumerableAssertion<TValue, TItem>> HaveCountLessThan(int expectedCount,
        string reasonFormat = "",
        params object?[] reasonArgs)
    {
        var actualCount = Subject.Count();
        return AssertCheck(actualCount < expectedCount, AssertionItemType.ItemCount,
            AssertionLocalization.CountLessAssertion,
            [
                new Argument("ActualCount", actualCount),
                new Argument("ExpectedCount", expectedCount)
            ],
            reasonFormat, reasonArgs);
    }

    /// <summary>
    ///     The count less than or equal to.
    /// </summary>
    /// <param name="expectedCount"></param>
    /// <param name="reasonFormat"></param>
    /// <param name="reasonArgs"></param>
    /// <returns></returns>
    public AndConstraint<EnumerableAssertion<TValue, TItem>> HaveCountLessThanOrEqualTo(int expectedCount,
        string reasonFormat = "",
        params object?[] reasonArgs)
    {
        var actualCount = Subject.Count();
        return AssertCheck(actualCount <= expectedCount, AssertionItemType.ItemCount,
            AssertionLocalization.CountLessOrEqualAssertion,
            [
                new Argument("ActualCount", actualCount),
                new Argument("ExpectedCount", expectedCount)
            ],
            reasonFormat, reasonArgs);
    }

    #endregion
}