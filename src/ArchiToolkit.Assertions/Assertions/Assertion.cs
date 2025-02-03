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
    /// The item is type of.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public AndConstraint<Assertion<TValue>> BeTypeOf<T>()
    {
        var message = FormatString(AssertionLocalizaion.TypeAssertion,
            new Argument("ValueType", Value?.GetType().GetFullName()),
            new Argument("ExpectedType", typeof(T).GetFullName()));

        AddAssertionItem(AssertionItemType.DataType, message, Value is T);
        return new AndConstraint<Assertion<TValue>>(this);
    }

    private string FormatString(string formatString, params Argument[] arguments)
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
        return string.Format(formatString, [..allArguments.Select(a => a.Value)]);
    }

    private void AddAssertionItem(AssertionItemType type, string message, bool succeed)
    {
        try
        {
            var passed = _reversed ? !succeed : succeed;
            if (passed) return;
            var item = new AssertionItem(type, message, new StackTrace(), DateTimeOffset.Now);
            _items.Add(item);
            _scope.PushAssertionItem(item);
        }
        finally
        {
            _reversed = false;
        }
    }
}