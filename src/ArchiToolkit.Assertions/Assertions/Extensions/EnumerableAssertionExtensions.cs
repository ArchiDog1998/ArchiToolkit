using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using ArchiToolkit.Assertions.AssertionItems;
using ArchiToolkit.Assertions.Constraints;
using ArchiToolkit.Assertions.Resources;

namespace ArchiToolkit.Assertions.Assertions.Extensions;

/// <summary>
/// For the enumerable things
/// </summary>
public static class EnumerableAssertionExtensions
{
    #region ItemEquality

    /// <summary>
    ///     Get if the item contain single.
    /// </summary>
    /// <param name="assertion"></param>
    /// <param name="predicate"></param>
    /// <param name="reasonFormat"></param>
    /// <param name="reasonArgs"></param>
    /// <returns></returns>
    public static AndWhichConstraint<TValue, TItem>
        ContainSingle<TValue, TItem>(this ObjectAssertion<TValue> assertion,
            Expression<Func<TItem, bool>> predicate,
            [StringSyntax(StringSyntaxAttribute.CompositeFormat)]
            string reasonFormat = "", params object?[] reasonArgs)
        where TValue : IEnumerable<TItem>
    {
        var func = predicate.Compile();
        var items = assertion.Subject.Where(func).ToArray();

        return assertion.AssertCheck(() => items.First(), items.Length is 1, AssertionItemType.ItemEquality,
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
    /// <param name="assertion"></param>
    /// <param name="expectedValue"></param>
    /// <param name="equalityComparer"></param>
    /// <param name="reasonFormat"></param>
    /// <param name="reasonArgs"></param>
    /// <returns></returns>
    public static AndConstraint<TValue> Contain<TValue, TItem>(this ObjectAssertion<TValue> assertion,
        TItem expectedValue,
        IEqualityComparer<TItem>? equalityComparer = null,
        [StringSyntax(StringSyntaxAttribute.CompositeFormat)]
        string reasonFormat = "", params object?[] reasonArgs)
        where TValue : IEnumerable<TItem>
    {
        var comparer = equalityComparer ?? EqualityComparer<TItem>.Default;

        return assertion.AssertCheck(assertion.Subject.Contains(expectedValue, comparer),
            AssertionItemType.ItemEquality,
            AssertionLocalization.ContainAssertion,
            [
                new Argument("ExpectedValue", expectedValue)
            ],
            reasonFormat, reasonArgs);
    }

    /// <summary>
    ///     Contains items
    /// </summary>
    /// <param name="assertion"></param>
    /// <param name="predicate"></param>
    /// <param name="reasonFormat"></param>
    /// <param name="reasonArgs"></param>
    /// <returns></returns>
    public static AndConstraint<TValue> Contain<TValue, TItem>(this ObjectAssertion<TValue> assertion,
        Expression<Func<TItem, bool>> predicate,
        [StringSyntax(StringSyntaxAttribute.CompositeFormat)]
        string reasonFormat = "", params object?[] reasonArgs)
        where TValue : IEnumerable<TItem>
    {
        var func = predicate.Compile();
        return assertion.AssertCheck(assertion.Subject.Any(func), AssertionItemType.ItemEquality,
            AssertionLocalization.ContainExpressionAssertion,
            [
                new Argument("Expression", predicate.Body)
            ],
            reasonFormat, reasonArgs);
    }

    /// <summary>
    ///     contain items
    /// </summary>
    /// <param name="assertion"></param>
    /// <param name="expectedValues"></param>
    /// <param name="equalityComparer"></param>
    /// <param name="reasonFormat"></param>
    /// <param name="reasonArgs"></param>
    /// <returns></returns>
    public static AndConstraint<TValue> Contain<TValue, TItem>(this ObjectAssertion<TValue> assertion,
        IEnumerable<TItem> expectedValues,
        IEqualityComparer<TItem>? equalityComparer = null,
        [StringSyntax(StringSyntaxAttribute.CompositeFormat)]
        string reasonFormat = "", params object?[] reasonArgs)
        where TValue : IEnumerable<TItem>
    {
        var comparer = equalityComparer ?? EqualityComparer<TItem>.Default;
        var values = expectedValues as TItem[] ?? expectedValues.ToArray();

        return assertion.AssertCheck(values.Except(assertion.Subject, comparer).Any(), AssertionItemType.ItemEquality,
            AssertionLocalization.ContainAssertion,
            [
                new Argument("ExpectedValues", values)
            ],
            reasonFormat, reasonArgs);
    }

    #endregion

    #region Item Count

    /// <summary>
    ///  be empty
    /// </summary>
    /// <param name="assertion"></param>
    /// <param name="reasonFormat"></param>
    /// <param name="reasonArgs"></param>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    public static AndConstraint<TValue> BeEmpty<TValue>(this ObjectAssertion<TValue> assertion,
        [StringSyntax(StringSyntaxAttribute.CompositeFormat)]
        string reasonFormat = "",
        params object?[] reasonArgs)
        where TValue : IEnumerable
    {
        return assertion.AssertCheck(GetCount(assertion.Subject) > 0, AssertionItemType.Empty,
            AssertionLocalization.EmptyAssertion,
            [
            ],
            reasonFormat, reasonArgs);
    }

    /// <summary>
    /// have the count.
    /// </summary>
    /// <param name="assertion"></param>
    /// <param name="expectedCount"></param>
    /// <param name="reasonFormat"></param>
    /// <param name="reasonArgs"></param>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    public static AndConstraint<TValue> HaveCount<TValue>(this ObjectAssertion<TValue> assertion, int expectedCount,
        [StringSyntax(StringSyntaxAttribute.CompositeFormat)]
        string reasonFormat = "",
        params object?[] reasonArgs)
        where TValue : IEnumerable
    {
        var actualCount = GetCount(assertion.Subject);


        return assertion.AssertCheck(actualCount == expectedCount, AssertionItemType.ItemCount,
            AssertionLocalization.CountAssertion,
            [
                new Argument("ActualCount", actualCount),
                new Argument("ExpectedCount", expectedCount)
            ],
            reasonFormat, reasonArgs);
    }

    /// <summary>
    /// greater than.
    /// </summary>
    /// <param name="assertion"></param>
    /// <param name="expectedCount"></param>
    /// <param name="reasonFormat"></param>
    /// <param name="reasonArgs"></param>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    public static AndConstraint<TValue> HaveCountGreaterThan<TValue>(this ObjectAssertion<TValue> assertion,
        int expectedCount,
        [StringSyntax(StringSyntaxAttribute.CompositeFormat)]
        string reasonFormat = "",
        params object?[] reasonArgs)
        where TValue : IEnumerable
    {
        var actualCount = GetCount(assertion.Subject);

        return assertion.AssertCheck(actualCount > expectedCount, AssertionItemType.ItemCount,
            AssertionLocalization.CountGreaterAssertion,
            [
                new Argument("ActualCount", actualCount),
                new Argument("ExpectedCount", expectedCount)
            ],
            reasonFormat, reasonArgs);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="assertion"></param>
    /// <param name="expectedCount"></param>
    /// <param name="reasonFormat"></param>
    /// <param name="reasonArgs"></param>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    public static AndConstraint<TValue> HaveCountGreaterThanOrEqualTo<TValue>(this ObjectAssertion<TValue> assertion,
        int expectedCount,
        [StringSyntax(StringSyntaxAttribute.CompositeFormat)]
        string reasonFormat = "",
        params object?[] reasonArgs)
        where TValue : IEnumerable

    {
        var actualCount = GetCount(assertion.Subject);

        return assertion.AssertCheck(actualCount >= expectedCount, AssertionItemType.ItemCount,
            AssertionLocalization.CountGreaterOrEqualAssertion,
            [
                new Argument("ActualCount", actualCount),
                new Argument("ExpectedCount", expectedCount)
            ],
            reasonFormat, reasonArgs);
    }

    /// <summary>
    /// The count less than.
    /// </summary>
    /// <param name="assertion"></param>
    /// <param name="expectedCount"></param>
    /// <param name="reasonFormat"></param>
    /// <param name="reasonArgs"></param>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    public static AndConstraint<TValue> HaveCountLessThan<TValue>(this ObjectAssertion<TValue> assertion,
        int expectedCount,
        [StringSyntax(StringSyntaxAttribute.CompositeFormat)]
        string reasonFormat = "",
        params object?[] reasonArgs)
        where TValue : IEnumerable

    {
        var actualCount = GetCount(assertion.Subject);

        return assertion.AssertCheck(actualCount < expectedCount, AssertionItemType.ItemCount,
            AssertionLocalization.CountLessAssertion,
            [
                new Argument("ActualCount", actualCount),
                new Argument("ExpectedCount", expectedCount)
            ],
            reasonFormat, reasonArgs);
    }

    /// <summary>
    /// The count less than or equal to.
    /// </summary>
    /// <param name="assertion"></param>
    /// <param name="expectedCount"></param>
    /// <param name="reasonFormat"></param>
    /// <param name="reasonArgs"></param>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    public static AndConstraint<TValue> HaveCountLessThanOrEqualTo<TValue>(this ObjectAssertion<TValue> assertion,
        int expectedCount,
        [StringSyntax(StringSyntaxAttribute.CompositeFormat)]
        string reasonFormat = "", params object?[] reasonArgs)
        where TValue : IEnumerable
    {
        var actualCount = GetCount(assertion.Subject);
        return assertion.AssertCheck(actualCount <= expectedCount, AssertionItemType.ItemCount,
            AssertionLocalization.CountLessOrEqualAssertion,
            [
                new Argument("ActualCount", actualCount),
                new Argument("ExpectedCount", expectedCount)
            ],
            reasonFormat, reasonArgs);
    }

    private static int GetCount(IEnumerable enumerable)
    {
        return enumerable switch
        {
            ICollection collection => collection.Count,
            string str => str.Length,
            _ => enumerable.Cast<object?>().Count()
        };
    }

    #endregion
}