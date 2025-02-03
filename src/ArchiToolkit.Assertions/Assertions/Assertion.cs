using System.Diagnostics;
using ArchiToolkit.Assertions.AssertionItems;
using ArchiToolkit.Assertions.Constraints;
using ArchiToolkit.Assertions.Execution;
using ArchiToolkit.Assertions.Resources;
using ArchiToolkit.Assertions.Utils;

namespace ArchiToolkit.Assertions.Assertions;

/// <summary>
/// Just the Assertion
/// </summary>
/// <typeparam name="TValue"></typeparam>
public class Assertion<TValue> : IAssertion
{
    private readonly record struct Argument(string Name, object? Value);

    private bool _reversed;
    private readonly AssertionScope _scope;
    private readonly List<AssertionItem> _items = [];

    /// <inheritdoc />
    public IReadOnlyList<AssertionItem> Items => _items;

    /// <inheritdoc />
    public AssertionType Type { get; }

    /// <inheritdoc />
    public DateTimeOffset CreatedTime { get; }

    /// <summary>
    /// The value itself
    /// </summary>
    public TValue Value { get; }

    /// <summary>
    /// The value name
    /// </summary>
    public string ValueName { get; }

    internal Assertion(TValue value, string valueName, AssertionType type)
    {
        Value = value;
        ValueName = valueName;
        Type = type;
        CreatedTime = DateTimeOffset.Now;
        _scope = AssertionScope.Current;
        _scope.AddAssertion(this);
    }

    /// <summary>
    /// Not
    /// </summary>
    /// <returns></returns>
    public Assertion<TValue> Not()
    {
        _reversed = !_reversed;
        return this;
    }

    /// <summary>
    /// The item should be.
    /// </summary>
    /// <param name="expectValue"></param>
    /// <param name="equalityComparer"></param>
    /// <param name="reasonFormat"></param>
    /// <param name="reasonArgs"></param>
    /// <returns></returns>
    public AndConstraint<Assertion<TValue>> Be(TValue expectValue,
        IEqualityComparer<TValue>? equalityComparer = null, string reasonFormat = "", params object?[] reasonArgs)
    {
        var comparer = equalityComparer ?? EqualityComparer<TValue>.Default;
        if (IsSucceed(comparer.Equals(Value, expectValue))) return new AndConstraint<Assertion<TValue>>(this);

        var message = FormatString(AssertionLocalizaion.EqualityAssertion,
            string.Format(reasonFormat, reasonArgs),
            new Argument("ExpectValue", expectValue));

        AddAssertionItem(AssertionItemType.Equality, message);
        return new AndConstraint<Assertion<TValue>>(this);
    }

    /// <summary>
    /// The item is type of.
    /// </summary>
    /// <param name="reasonFormat"></param>
    /// <param name="reasonArgs"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public AndConstraint<Assertion<TValue>> BeTypeOf<T>(string reasonFormat = "", params object?[] reasonArgs)
    {
        if (IsSucceed(Value is T)) return new AndConstraint<Assertion<TValue>>(this);
        var message = FormatString(AssertionLocalizaion.TypeAssertion,
            string.Format(reasonFormat, reasonArgs),
            new Argument("ValueType", Value?.GetType().GetFullName()),
            new Argument("ExpectedType", typeof(T).GetFullName()));

        AddAssertionItem(AssertionItemType.DataType, message);
        return new AndConstraint<Assertion<TValue>>(this);
    }

    private string FormatString(string formatString, string reason, params Argument[] arguments)
    {
        Argument[] allArguments =
        [
            new(nameof(Value), Value),
            new(nameof(ValueName), ValueName),
            new(nameof(AssertionType), Type switch
            {
                AssertionType.Must => AssertionLocalizaion.Must,
                AssertionType.Should => AssertionLocalizaion.Should,
                AssertionType.Could => AssertionLocalizaion.Could,
                _ => "Unknown",
            }),
            ..arguments
        ];
        var index = 0;
        formatString = allArguments.Aggregate(formatString,
            (current, argument) => current.ReplacePlaceHolder(argument.Name, (index++).ToString()));
        var result = string.Format(formatString, [..allArguments.Select(a => a.Value)]);
        if (!string.IsNullOrWhiteSpace(reason))
        {
            result += $"\n{AssertionLocalizaion.Reason}{reason}";
        }

        return result;
    }

    private bool IsSucceed(bool succeed)
    {
        try
        {
            return _reversed ? !succeed : succeed;
        }
        finally
        {
            _reversed = false;
        }
    }

    private void AddAssertionItem(AssertionItemType type, string message)
    {
        var item = new AssertionItem(type, message, new StackTrace(2, true), DateTimeOffset.Now);
        _items.Add(item);
        _scope.PushAssertionItem(item, Type);
    }
}