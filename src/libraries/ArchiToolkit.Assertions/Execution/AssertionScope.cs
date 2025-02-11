using ArchiToolkit.Assertions.AssertionItems;
using ArchiToolkit.Assertions.Assertions;

namespace ArchiToolkit.Assertions.Execution;

/// <summary>
/// </summary>
public class AssertionScope : IDisposable
{
    private static readonly AsyncLocal<AssertionScope?> CurrentScope = new();
    private readonly List<IAssertion> _assertions = [];
    private readonly AssertionScope? _parent;
    private readonly IAssertionStrategy _strategy;

    /// <summary>
    /// The Context.
    /// </summary>
    public string Context { get; }

    /// <summary>
    /// The tag to show.
    /// </summary>
    public object? Tag { get; }

    /// <summary>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="tag"></param>
    public AssertionScope(string context = "", object? tag = null)
        : this(AssertionService.DefaultScopeStrategy, context, tag)
    {
    }

    /// <summary>
    ///     Add the assertion.
    /// </summary>
    /// <param name="strategy"></param>
    /// <param name="context"></param>
    /// <param name="tag"></param>
    public AssertionScope(IAssertionStrategy strategy, string context = "", object? tag = null)
    {
        _strategy = strategy;
        _parent = CurrentScope.Value;
        Context = context;
        Tag = tag;

        CurrentScope.Value = this;
    }

    internal static AssertionScope Current
    {
        get => CurrentScope.Value ?? new AssertionScope(AssertionService.DefaultPushStrategy, string.Empty);
        set => CurrentScope.Value = value;
    }


    private bool _handledFailure;
    /// <inheritdoc />
    void IDisposable.Dispose()
    {
        if (!_handledFailure)
        {
            HandleFailure();
        }

        if (_parent is not null) CurrentScope.Value = _parent;
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Manually handle failure
    /// </summary>
    /// <returns></returns>
    public object? HandleFailure()
    {
        _handledFailure = true;
        return _strategy.HandleFailure(this, _assertions);
    }

    internal void AddAssertion(IAssertion assertion)
    {
        _assertions.Add(assertion);
    }

    internal object? PushAssertionItem(AssertionItem assertionItem, AssertionType assertionType, object? tag)
    {
        return _strategy.HandleFailure(this, assertionType, assertionItem, tag);
    }
}