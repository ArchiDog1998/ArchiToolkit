using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using ArchiToolkit.Assertions.AssertionItems;
using ArchiToolkit.Assertions.Constraints;
using ArchiToolkit.Assertions.Execution;
using ArchiToolkit.Assertions.Resources;
using ArchiToolkit.Assertions.Utils;

namespace ArchiToolkit.Assertions.Assertions;

/// <summary>
///     Just the Assertion
/// </summary>
/// <typeparam name="TValue"></typeparam>
public class ObjectAssertion<TValue> : IAssertion
{
    private readonly List<AssertionItem> _items = [];
    private readonly AssertionScope _scope;

    private bool _reversed;

    internal ObjectAssertion(TValue value, string valueName, AssertionType type)
    {
        Value = value;
        ValueName = valueName;
        Type = type;
        CreatedTime = DateTimeOffset.Now;
        _scope = AssertionScope.Current;
        _scope.AddAssertion(this);
    }

    /// <summary>
    ///     The value itself
    /// </summary>
    public TValue Value { get; }

    /// <summary>
    ///     The value name
    /// </summary>
    public string ValueName { get; }

    private protected virtual string? ValueString => Value?.ToString();

    /// <inheritdoc />
    public IReadOnlyList<AssertionItem> Items => _items;

    /// <inheritdoc />
    public AssertionType Type { get; }

    /// <inheritdoc />
    public DateTimeOffset CreatedTime { get; }

    /// <summary>
    ///     Not
    /// </summary>
    /// <returns></returns>
    public ObjectAssertion<TValue> Not()
    {
        _reversed = !_reversed;
        return this;
    }

    #region Match

    /// <summary>
    ///     Match the method
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="reasonFormat"></param>
    /// <param name="reasonArgs"></param>
    /// <returns></returns>
    public AndConstraint<ObjectAssertion<TValue>> Match(Expression<Func<TValue, bool>> predicate,
        string reasonFormat = "", params object?[] reasonArgs)
    {
        if (IsSucceed(predicate.Compile()(Value), out var reverse))
            return new AndConstraint<ObjectAssertion<TValue>>(this);

        var message = FormatString(AssertionLocalization.MatchAssertion,
            string.Format(reasonFormat, reasonArgs), reverse,
            new Argument("Expression", predicate.Body));

        AddAssertionItem(AssertionItemType.Match, message);
        return new AndConstraint<ObjectAssertion<TValue>>(this);
    }

    #endregion

    #region Range

    /// <summary>
    ///     Should be in range.
    /// </summary>
    /// <param name="minimumValue"></param>
    /// <param name="maximumValue"></param>
    /// <param name="comparer"></param>
    /// <param name="reasonFormat"></param>
    /// <param name="reasonArgs"></param>
    /// <returns></returns>
    public AndConstraint<ObjectAssertion<TValue>> BeInRange(TValue minimumValue, TValue maximumValue,
        IComparer<TValue>? comparer = null,
        string reasonFormat = "", params object?[] reasonArgs)
    {
        var realComparer = comparer ?? Comparer<TValue>.Default;
        if (IsSucceed(realComparer.Compare(Value, minimumValue) >= 0 && realComparer.Compare(Value, maximumValue) <= 0,
                out var reverse))
            return new AndConstraint<ObjectAssertion<TValue>>(this);
        var message = FormatString(AssertionLocalization.RangeAssertion,
            string.Format(reasonFormat, reasonArgs), reverse,
            new Argument("MinimumValue", minimumValue),
            new Argument("MaximumValue", maximumValue));

        AddAssertionItem(AssertionItemType.Range, message);
        return new AndConstraint<ObjectAssertion<TValue>>(this);
    }

    #endregion

    #region Null

    /// <summary>
    ///     The item is type of.
    /// </summary>
    /// <param name="reasonFormat"></param>
    /// <param name="reasonArgs"></param>
    /// <returns></returns>
    public AndConstraint<ObjectAssertion<TValue>> BeNull(string reasonFormat = "", params object?[] reasonArgs)
    {
        if (IsSucceed(Value is null, out var reverse)) return new AndConstraint<ObjectAssertion<TValue>>(this);
        var message = FormatString(AssertionLocalization.NullAssertion,
            string.Format(reasonFormat, reasonArgs), reverse);

        AddAssertionItem(AssertionItemType.DataType, message);
        return new AndConstraint<ObjectAssertion<TValue>>(this);
    }

    #endregion

    private protected readonly record struct Argument(string Name, object? Value);

    #region Comparison

    /// <summary>
    ///     Should be greater.
    /// </summary>
    /// <param name="expectedValue"></param>
    /// <param name="comparer"></param>
    /// <param name="reasonFormat"></param>
    /// <param name="reasonArgs"></param>
    /// <returns></returns>
    public AndConstraint<ObjectAssertion<TValue>> BeGreaterThanOrEqualTo(TValue expectedValue,
        IComparer<TValue>? comparer = null,
        string reasonFormat = "", params object?[] reasonArgs)
    {
        var realComparer = comparer ?? Comparer<TValue>.Default;
        if (IsSucceed(realComparer.Compare(Value, expectedValue) >= 0, out var reverse))
            return new AndConstraint<ObjectAssertion<TValue>>(this);

        var message = FormatString(AssertionLocalization.GreaterOrEqualAssertion,
            string.Format(reasonFormat, reasonArgs), reverse,
            new Argument("ComparedValue", expectedValue));

        AddAssertionItem(AssertionItemType.Comparison, message);
        return new AndConstraint<ObjectAssertion<TValue>>(this);
    }

    /// <summary>
    ///     Should be greater.
    /// </summary>
    /// <param name="expectedValue"></param>
    /// <param name="comparer"></param>
    /// <param name="reasonFormat"></param>
    /// <param name="reasonArgs"></param>
    /// <returns></returns>
    public AndConstraint<ObjectAssertion<TValue>> BeGreaterThan(TValue expectedValue,
        IComparer<TValue>? comparer = null,
        string reasonFormat = "", params object?[] reasonArgs)
    {
        var realComparer = comparer ?? Comparer<TValue>.Default;
        if (IsSucceed(realComparer.Compare(Value, expectedValue) > 0, out var reverse))
            return new AndConstraint<ObjectAssertion<TValue>>(this);

        var message = FormatString(AssertionLocalization.GreaterAssertion,
            string.Format(reasonFormat, reasonArgs), reverse,
            new Argument("ComparedValue", expectedValue));

        AddAssertionItem(AssertionItemType.Comparison, message);
        return new AndConstraint<ObjectAssertion<TValue>>(this);
    }

    /// <summary>
    ///     Less or equal to
    /// </summary>
    /// <param name="expectedValue"></param>
    /// <param name="comparer"></param>
    /// <param name="reasonFormat"></param>
    /// <param name="reasonArgs"></param>
    /// <returns></returns>
    public AndConstraint<ObjectAssertion<TValue>> BeLessThanOrEqualTo(TValue expectedValue,
        IComparer<TValue>? comparer = null,
        string reasonFormat = "", params object?[] reasonArgs)
    {
        var realComparer = comparer ?? Comparer<TValue>.Default;
        if (IsSucceed(realComparer.Compare(Value, expectedValue) <= 0, out var reverse))
            return new AndConstraint<ObjectAssertion<TValue>>(this);

        var message = FormatString(AssertionLocalization.LessOrEqualAssertion,
            string.Format(reasonFormat, reasonArgs), reverse,
            new Argument("ComparedValue", expectedValue));

        AddAssertionItem(AssertionItemType.Comparison, message);
        return new AndConstraint<ObjectAssertion<TValue>>(this);
    }

    /// <summary>
    ///     Should be less than.
    /// </summary>
    /// <param name="expectedValue"></param>
    /// <param name="comparer"></param>
    /// <param name="reasonFormat"></param>
    /// <param name="reasonArgs"></param>
    /// <returns></returns>
    public AndConstraint<ObjectAssertion<TValue>> BeLessThan(TValue expectedValue, IComparer<TValue>? comparer = null,
        string reasonFormat = "", params object?[] reasonArgs)
    {
        var realComparer = comparer ?? Comparer<TValue>.Default;
        if (IsSucceed(realComparer.Compare(Value, expectedValue) < 0, out var reverse))
            return new AndConstraint<ObjectAssertion<TValue>>(this);

        var message = FormatString(AssertionLocalization.LessAssertion,
            string.Format(reasonFormat, reasonArgs), reverse,
            new Argument("ComparedValue", expectedValue));

        AddAssertionItem(AssertionItemType.Comparison, message);
        return new AndConstraint<ObjectAssertion<TValue>>(this);
    }

    #endregion

    #region Equality

    /// <summary>
    ///     The item should be.
    /// </summary>
    /// <param name="expectedValue"></param>
    /// <param name="equalityComparer"></param>
    /// <param name="reasonFormat"></param>
    /// <param name="reasonArgs"></param>
    /// <returns></returns>
    public AndConstraint<ObjectAssertion<TValue>> Be(TValue expectedValue,
        IComparer<TValue>? equalityComparer = null, string reasonFormat = "", params object?[] reasonArgs)
    {
        var realComparer = equalityComparer ?? Comparer<TValue>.Default;
        if (IsSucceed(realComparer.Compare(Value, expectedValue) == 0, out var reverse))
            return new AndConstraint<ObjectAssertion<TValue>>(this);

        var message = FormatString(AssertionLocalization.EqualityAssertion,
            string.Format(reasonFormat, reasonArgs), reverse,
            new Argument("ExpectedValue", expectedValue));

        AddAssertionItem(AssertionItemType.Equality, message);
        return new AndConstraint<ObjectAssertion<TValue>>(this);
    }

    /// <summary>
    ///     The item should be.
    /// </summary>
    /// <param name="expectedValue"></param>
    /// <param name="equalityComparer"></param>
    /// <param name="reasonFormat"></param>
    /// <param name="reasonArgs"></param>
    /// <returns></returns>
    [OverloadResolutionPriority(1)]
    public AndConstraint<ObjectAssertion<TValue>> Be(TValue expectedValue,
        IEqualityComparer<TValue>? equalityComparer = null, string reasonFormat = "", params object?[] reasonArgs)
    {
        var comparer = equalityComparer ?? EqualityComparer<TValue>.Default;
        if (IsSucceed(comparer.Equals(Value, expectedValue), out var reverse))
            return new AndConstraint<ObjectAssertion<TValue>>(this);

        var message = FormatString(AssertionLocalization.EqualityAssertion,
            string.Format(reasonFormat, reasonArgs), reverse,
            new Argument("ExpectedValue", expectedValue));

        AddAssertionItem(AssertionItemType.Equality, message);
        return new AndConstraint<ObjectAssertion<TValue>>(this);
    }

    /// <summary>
    ///     be one of.
    /// </summary>
    /// <param name="expectedValues"></param>
    /// <param name="equalityComparer"></param>
    /// <param name="reasonFormat"></param>
    /// <param name="reasonArgs"></param>
    /// <returns></returns>
    public AndConstraint<ObjectAssertion<TValue>> BeOneOf(IEnumerable<TValue> expectedValues,
        IEqualityComparer<TValue>? equalityComparer = null, string reasonFormat = "", params object?[] reasonArgs)
    {
        var comparer = equalityComparer ?? EqualityComparer<TValue>.Default;
        var values = expectedValues as TValue[] ?? expectedValues.ToArray();
        if (IsSucceed(values.Contains(Value, comparer), out var reverse))
            return new AndConstraint<ObjectAssertion<TValue>>(this);

        var message = FormatString(AssertionLocalization.OneOfAssertion,
            string.Format(reasonFormat, reasonArgs), reverse,
            new Argument("ExpectedValues", values.GetItemsString()));

        AddAssertionItem(AssertionItemType.Equality, message);
        return new AndConstraint<ObjectAssertion<TValue>>(this);
    }

    #endregion

    #region Type

    /// <summary>
    ///     The item is type of.
    /// </summary>
    /// <param name="reasonFormat"></param>
    /// <param name="reasonArgs"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public AndWhichConstraint<ObjectAssertion<TValue>, T?> BeAssignableTo<T>(string reasonFormat = "",
        params object?[] reasonArgs)
    {
        var typedValue = Value is T type ? type : default;

        if (IsSucceed(Value is T, out var reverse))
            return new AndWhichConstraint<ObjectAssertion<TValue>, T?>(this, typedValue);

        var message = FormatString(AssertionLocalization.AssignableAssertion,
            string.Format(reasonFormat, reasonArgs), reverse,
            new Argument("ValueType", Value?.GetType().GetFullName()),
            new Argument("ExpectedType", typeof(T).GetFullName()));

        AddAssertionItem(AssertionItemType.DataType, message);
        return new AndWhichConstraint<ObjectAssertion<TValue>, T?>(this, typedValue);
    }

    /// <summary>
    ///     Be type of
    /// </summary>
    /// <param name="reasonFormat"></param>
    /// <param name="reasonArgs"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public AndConstraint<ObjectAssertion<TValue>> BeTypeOf<T>(string reasonFormat = "", params object?[] reasonArgs)
    {
        return BeTypeOf(typeof(T), reasonFormat, reasonArgs);
    }

    /// <summary>
    /// </summary>
    /// <param name="expectedType"></param>
    /// <param name="reasonFormat"></param>
    /// <param name="reasonArgs"></param>
    /// <returns></returns>
    public AndConstraint<ObjectAssertion<TValue>> BeTypeOf(Type expectedType, string reasonFormat = "",
        params object?[] reasonArgs)
    {
        var subjectType = Value?.GetType();
        if (IsSucceed(expectedType.IsGenericTypeDefinition && (subjectType?.IsGenericType ?? false)
                ? subjectType.GetGenericTypeDefinition() == expectedType
                : subjectType == expectedType, out var reverse))
            return new AndConstraint<ObjectAssertion<TValue>>(this);

        var message = FormatString(AssertionLocalization.TypeAssertion,
            string.Format(reasonFormat, reasonArgs), reverse,
            new Argument("ValueType", Value?.GetType().GetFullName()),
            new Argument("ExpectedType", expectedType.GetFullName()));

        AddAssertionItem(AssertionItemType.DataType, message);
        return new AndConstraint<ObjectAssertion<TValue>>(this);
    }

    #endregion

    #region Assertion Helper Methods

    private protected string FormatString(string formatString, string reason, bool reverse, params Argument[] arguments)
    {
        Argument[] allArguments =
        [
            new(nameof(Value), ValueString),
            new(nameof(ValueName), ValueName),
            new(nameof(AssertionType), Type switch
            {
                AssertionType.Must => AssertionLocalization.Must,
                AssertionType.Should => AssertionLocalization.Should,
                AssertionType.Could => AssertionLocalization.Could,
                _ => "Unknown"
            }),
            new("Not", reverse ? AssertionLocalization.Not : string.Empty),
            ..arguments
        ];
        var index = 0;
        formatString = allArguments.Aggregate(formatString,
            (current, argument) => current.ReplacePlaceHolder(argument.Name, (index++).ToString()));
        var result = string.Format(formatString, [..allArguments.Select(a => a.Value)]);
        if (!string.IsNullOrWhiteSpace(reason)) result += $"\n{string.Format(AssertionLocalization.Reason, reason)}";

        return result;
    }

    private protected bool IsSucceed(bool succeed, out bool reverse)
    {
        try
        {
            reverse = _reversed;
            return _reversed ? !succeed : succeed;
        }
        finally
        {
            _reversed = false;
        }
    }

    private protected void AddAssertionItem(AssertionItemType type, string message)
    {
        var item = new AssertionItem(type, message, new StackTrace(2, true), DateTimeOffset.Now);
        _items.Add(item);
        _scope.PushAssertionItem(item, Type);
    }

    #endregion
}