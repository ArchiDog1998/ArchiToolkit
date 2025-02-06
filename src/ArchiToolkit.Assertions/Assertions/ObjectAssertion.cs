using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
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
public sealed class ObjectAssertion<TValue> : IAssertion
{
    private readonly DateTimeOffset _createTime;
    private readonly List<AssertionItem> _items = [];
    private readonly AssertionScope _scope;
    private readonly AssertionType _type;
    private readonly bool _isValid;
    private bool _reversed;

    internal ObjectAssertion(TValue subject, string valueName, AssertionType type, bool isValid = true)
        : this(subject, valueName, type, DateTimeOffset.Now, AssertionScope.Current, isValid)
    {
    }

    private ObjectAssertion(TValue subject, string valueName, AssertionType type, DateTimeOffset createTime,
        AssertionScope scope, bool isValid)
    {
        Subject = subject;
        SubjectName = string.IsNullOrEmpty(valueName) ? "Unknown" : valueName;
        _type = type;
        _createTime = createTime;
        _scope = scope;
        _isValid = isValid;
        _scope.AddAssertion(this);
    }

    /// <summary>
    ///     The subject.
    /// </summary>
    public TValue Subject { get; }

    /// <summary>
    ///     The subject name
    /// </summary>
    public string SubjectName { get; }

    private string? ValueString => Subject?.GetObjectString();

    /// <summary>
    ///     Not
    /// </summary>
    /// <returns></returns>
    public ObjectAssertion<TValue> Not
    {
        get
        {
            _reversed = !_reversed;
            return this;
        }
    }

    /// <inheritdoc />
    IReadOnlyList<AssertionItem> IAssertion.Items => _items;

    /// <inheritdoc />
    AssertionType IAssertion.Type => _type;

    /// <inheritdoc />
    DateTimeOffset IAssertion.CreatedTime => _createTime;

    internal ObjectAssertion<TValue> Duplicate(AssertionType type)
    {
        return type == _type ? this : new ObjectAssertion<TValue>(Subject, SubjectName, type, _createTime, _scope, _isValid);
    }


    #region Match

    /// <summary>
    ///     Match the method
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="reasonFormat"></param>
    /// <param name="reasonArgs"></param>
    /// <returns></returns>
    public AndConstraint<TValue> Match(Expression<Func<TValue, bool>> predicate,
        [StringSyntax(StringSyntaxAttribute.CompositeFormat)]
        string reasonFormat = "", params object?[] reasonArgs)
    {
        return AssertCheck(predicate.Compile()(Subject),
            AssertionItemType.Match, AssertionLocalization.MatchAssertion,
            [
                new Argument("Expression", predicate.Body)
            ],
            reasonFormat, reasonArgs);
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
    public AndConstraint<TValue> BeInRange(TValue minimumValue, TValue maximumValue,
        IComparer<TValue>? comparer = null,
        [StringSyntax(StringSyntaxAttribute.CompositeFormat)]
        string reasonFormat = "", params object?[] reasonArgs)
    {
        var realComparer = comparer ?? Comparer<TValue>.Default;

        return AssertCheck(
            realComparer.Compare(Subject, minimumValue) >= 0 && realComparer.Compare(Subject, maximumValue) <= 0,
            AssertionItemType.Range, AssertionLocalization.RangeAssertion,
            [
                new Argument("MinimumValue", minimumValue),
                new Argument("MaximumValue", maximumValue)
            ],
            reasonFormat, reasonArgs);
    }

    #endregion

    #region Null

    /// <summary>
    ///     The item is type of.
    /// </summary>
    /// <param name="reasonFormat"></param>
    /// <param name="reasonArgs"></param>
    /// <returns></returns>
    public AndConstraint<TValue> BeNull(
        [StringSyntax(StringSyntaxAttribute.CompositeFormat)]
        string reasonFormat = "", params object?[] reasonArgs)
    {
        return AssertCheck(Subject is null,
            AssertionItemType.Null, AssertionLocalization.NullAssertion,
            [
            ],
            reasonFormat, reasonArgs);
    }

    #endregion

    #region Comparison

    /// <summary>
    ///     Should be greater.
    /// </summary>
    /// <param name="comparedValue"></param>
    /// <param name="comparer"></param>
    /// <param name="reasonFormat"></param>
    /// <param name="reasonArgs"></param>
    /// <returns></returns>
    public AndConstraint<TValue> BeGreaterThanOrEqualTo(TValue comparedValue,
        IComparer<TValue>? comparer = null,
        [StringSyntax(StringSyntaxAttribute.CompositeFormat)]
        string reasonFormat = "", params object?[] reasonArgs)
    {
        var realComparer = comparer ?? Comparer<TValue>.Default;

        return AssertCheck(realComparer.Compare(Subject, comparedValue) >= 0,
            AssertionItemType.Comparison, AssertionLocalization.GreaterOrEqualAssertion,
            [
                new Argument("ComparedValue", comparedValue)
            ],
            reasonFormat, reasonArgs);
    }

    /// <summary>
    ///     Should be greater.
    /// </summary>
    /// <param name="comparedValue"></param>
    /// <param name="comparer"></param>
    /// <param name="reasonFormat"></param>
    /// <param name="reasonArgs"></param>
    /// <returns></returns>
    public AndConstraint<TValue> BeGreaterThan(TValue comparedValue,
        IComparer<TValue>? comparer = null,
        [StringSyntax(StringSyntaxAttribute.CompositeFormat)]
        string reasonFormat = "", params object?[] reasonArgs)
    {
        var realComparer = comparer ?? Comparer<TValue>.Default;

        return AssertCheck(realComparer.Compare(Subject, comparedValue) > 0,
            AssertionItemType.Comparison, AssertionLocalization.GreaterAssertion,
            [
                new Argument("ComparedValue", comparedValue)
            ],
            reasonFormat, reasonArgs);
    }

    /// <summary>
    ///     Less or equal to
    /// </summary>
    /// <param name="comparedValue"></param>
    /// <param name="comparer"></param>
    /// <param name="reasonFormat"></param>
    /// <param name="reasonArgs"></param>
    /// <returns></returns>
    public AndConstraint<TValue> BeLessThanOrEqualTo(TValue comparedValue,
        IComparer<TValue>? comparer = null,
        [StringSyntax(StringSyntaxAttribute.CompositeFormat)]
        string reasonFormat = "", params object?[] reasonArgs)
    {
        var realComparer = comparer ?? Comparer<TValue>.Default;

        return AssertCheck(realComparer.Compare(Subject, comparedValue) <= 0,
            AssertionItemType.Comparison, AssertionLocalization.LessOrEqualAssertion,
            [
                new Argument("ComparedValue", comparedValue)
            ],
            reasonFormat, reasonArgs);
    }

    /// <summary>
    ///     Should be less than.
    /// </summary>
    /// <param name="comparedValue"></param>
    /// <param name="comparer"></param>
    /// <param name="reasonFormat"></param>
    /// <param name="reasonArgs"></param>
    /// <returns></returns>
    public AndConstraint<TValue> BeLessThan(TValue comparedValue, IComparer<TValue>? comparer = null,
        [StringSyntax(StringSyntaxAttribute.CompositeFormat)]
        string reasonFormat = "", params object?[] reasonArgs)
    {
        var realComparer = comparer ?? Comparer<TValue>.Default;
        return AssertCheck(realComparer.Compare(Subject, comparedValue) < 0,
            AssertionItemType.Comparison, AssertionLocalization.LessAssertion,
            [
                new Argument("ComparedValue", comparedValue)
            ],
            reasonFormat, reasonArgs);
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
    public AndConstraint<TValue> Be(TValue expectedValue,
        IComparer<TValue>? equalityComparer = null,
        [StringSyntax(StringSyntaxAttribute.CompositeFormat)]
        string reasonFormat = "", params object?[] reasonArgs)
    {
        var realComparer = equalityComparer ?? Comparer<TValue>.Default;

        return AssertCheck(realComparer.Compare(Subject, expectedValue) == 0,
            AssertionItemType.Equality, AssertionLocalization.EqualityAssertion,
            [
                new Argument("ExpectedValue", expectedValue)
            ],
            reasonFormat, reasonArgs);
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
    public AndConstraint<TValue> Be(TValue expectedValue,
        IEqualityComparer<TValue>? equalityComparer = null,
        [StringSyntax(StringSyntaxAttribute.CompositeFormat)]
        string reasonFormat = "", params object?[] reasonArgs)
    {
        var comparer = equalityComparer ?? EqualityComparer<TValue>.Default;

        return AssertCheck(comparer.Equals(Subject, expectedValue),
            AssertionItemType.Equality, AssertionLocalization.EqualityAssertion,
            [
                new Argument("ExpectedValue", expectedValue)
            ],
            reasonFormat, reasonArgs);
    }

    /// <summary>
    ///     be one of.
    /// </summary>
    /// <param name="expectedValues"></param>
    /// <param name="equalityComparer"></param>
    /// <param name="reasonFormat"></param>
    /// <param name="reasonArgs"></param>
    /// <returns></returns>
    public AndConstraint<TValue> BeOneOf(IEnumerable<TValue> expectedValues,
        IEqualityComparer<TValue>? equalityComparer = null,
        [StringSyntax(StringSyntaxAttribute.CompositeFormat)]
        string reasonFormat = "", params object?[] reasonArgs)
    {
        var comparer = equalityComparer ?? EqualityComparer<TValue>.Default;
        var values = expectedValues as TValue[] ?? expectedValues.ToArray();

        return AssertCheck(values.Contains(Subject, comparer),
            AssertionItemType.Equality, AssertionLocalization.OneOfAssertion,
            [
                new Argument("ExpectedValues", values)
            ],
            reasonFormat, reasonArgs);
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
    public AndWhichConstraint<TValue, T?> BeAssignableTo<T>(string reasonFormat = "",
        params object?[] reasonArgs)
    {
        return AssertCheck(() => Subject is T type ? type : default, string.Empty,
            Subject is T, AssertionItemType.DataType, AssertionLocalization.AssignableAssertion,
            [
                new Argument("ValueType", Subject?.GetType().GetFullName()),
                new Argument("ExpectedType", typeof(T).GetFullName())
            ],
            reasonFormat, reasonArgs);
    }

    /// <summary>
    ///     Be type of
    /// </summary>
    /// <param name="reasonFormat"></param>
    /// <param name="reasonArgs"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public AndConstraint<TValue> BeTypeOf<T>(
        [StringSyntax(StringSyntaxAttribute.CompositeFormat)]
        string reasonFormat = "", params object?[] reasonArgs)
    {
        return BeTypeOf(typeof(T), reasonFormat, reasonArgs);
    }

    /// <summary>
    /// </summary>
    /// <param name="expectedType"></param>
    /// <param name="reasonFormat"></param>
    /// <param name="reasonArgs"></param>
    /// <returns></returns>
    public AndConstraint<TValue> BeTypeOf(Type expectedType, string reasonFormat = "",
        params object?[] reasonArgs)
    {
        var subjectType = Subject?.GetType();
        var succeed = expectedType.IsGenericTypeDefinition && (subjectType?.IsGenericType ?? false)
            ? subjectType.GetGenericTypeDefinition() == expectedType
            : subjectType == expectedType;

        return AssertCheck(succeed, AssertionItemType.DataType, AssertionLocalization.TypeAssertion,
            [
                new Argument("ValueType", Subject?.GetType().GetFullName()),
                new Argument("ExpectedType", expectedType.GetFullName())
            ],
            reasonFormat, reasonArgs);
    }

    #endregion

    #region Assertion Helper Methods

    /// <summary>
    /// </summary>
    /// <param name="succeed"></param>
    /// <param name="assertionItemType"></param>
    /// <param name="formatString"></param>
    /// <param name="additionalArguments"></param>
    /// <param name="reasonFormat"></param>
    /// <param name="reasonArgs"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public AndConstraint<TValue> AssertCheck(bool succeed, AssertionItemType assertionItemType,
        [StringSyntax(StringSyntaxAttribute.CompositeFormat)]
        string formatString, Argument[] additionalArguments,
        [StringSyntax(StringSyntaxAttribute.CompositeFormat)]
        string reasonFormat, object?[] reasonArgs)
    {
        return AssertCheck(new AndConstraint<TValue>(this), succeed, assertionItemType, formatString,
            additionalArguments, reasonFormat, reasonArgs);
    }

    /// <summary>
    ///     Just the check
    /// </summary>
    /// <param name="resultGetter"></param>
    /// <param name="suffix"></param>
    /// <param name="succeed"></param>
    /// <param name="assertionItemType"></param>
    /// <param name="formatString"></param>
    /// <param name="additionalArguments"></param>
    /// <param name="reasonFormat"></param>
    /// <param name="reasonArgs"></param>
    /// <typeparam name="TMatchedElement"></typeparam>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public AndWhichConstraint<TValue, TMatchedElement> AssertCheck<TMatchedElement>(
        Func<TMatchedElement> resultGetter, string suffix, bool succeed, AssertionItemType assertionItemType,
        [StringSyntax(StringSyntaxAttribute.CompositeFormat)]
        string formatString, Argument[] additionalArguments,
        [StringSyntax(StringSyntaxAttribute.CompositeFormat)]
        string reasonFormat, object?[] reasonArgs)
    {
        return AssertCheck(new AndWhichConstraint<TValue, TMatchedElement>(this, resultGetter, suffix), succeed,
            assertionItemType, formatString,
            additionalArguments, reasonFormat, reasonArgs);
    }

    private TResult AssertCheck<TResult>(TResult result, bool succeed, AssertionItemType assertionItemType,
        [StringSyntax(StringSyntaxAttribute.CompositeFormat)]
        string formatString, Argument[] additionalArguments,
        [StringSyntax(StringSyntaxAttribute.CompositeFormat)]
        string reasonFormat, object?[] reasonArgs)
    {
        if (IsSucceed(succeed, out var reverse)) return result;
        var message = FormatString(formatString, string.Format(reasonFormat, reasonArgs), reverse, additionalArguments);
        AddAssertionItem(assertionItemType, message);
        return result;
    }

    private string FormatString(string formatString, string reason, bool reverse, params Argument[] arguments)
    {
        Argument[] allArguments =
        [
            new(nameof(Subject), ValueString),
            new(nameof(SubjectName), SubjectName),
            new(nameof(AssertionType), _type switch
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
        var result = string.Format(formatString, [..allArguments.Select(a => a.Value.GetObjectString())]);
        if (!string.IsNullOrWhiteSpace(reason)) result += $"\n{string.Format(AssertionLocalization.Reason, reason)}";

        return result;
    }

    private bool IsSucceed(bool succeed, out bool reverse)
    {
        if (!_isValid)
        {
            reverse = false;
            return true;
        }
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

    private void AddAssertionItem(AssertionItemType type, string message)
    {
        var skipIndex = GetIndex(new StackTrace());
        var item = new AssertionItem(type, message, new StackTrace(skipIndex, true), DateTimeOffset.Now);
        _items.Add(item);
        _scope.PushAssertionItem(item, _type);
        return;

        static int GetIndex(StackTrace stackTrace)
        {
            var index = 0;
            foreach (var frame in stackTrace.GetFrames())
            {
                index++;

                if (frame.GetMethod() is not MethodInfo method) continue;
                if (method.ReturnType.GetInterfaces().All(i => i != typeof(IConstraint))) continue;
                if (method.DeclaringType?.Assembly == typeof(IAssertion).Assembly
                    && method.Name.Contains(nameof(ObjectAssertion<object>.AssertCheck))) continue;
                return index;
            }

            throw new InvalidOperationException("Failed to get the frame index!");
        }
    }

    #endregion
}