using ArchiToolkit.Assertions.AssertionItems;
using ArchiToolkit.Assertions.Assertions;

namespace ArchiToolkit.Assertions.Execution;

/// <summary>
///
/// </summary>
public class AssertionScope : IDisposable
{
    private readonly AssertionScope? _parent;
    private readonly IAssertionStrategy _strategy;
    private readonly string _context;
    private readonly List<IAssertion> _assertions = [];

    /// <summary>
    /// The current scope
    /// </summary>
    public static AssertionScope Current
    {
        get => CurrentScope.Value ?? new AssertionScope(AssertionService.DefaultPushStrategy, string.Empty);
        set => CurrentScope.Value = value;
    }

    private static readonly AsyncLocal<AssertionScope?> CurrentScope = new();

    /// <summary>
    ///
    /// </summary>
    /// <param name="context"></param>
    public AssertionScope(string context = "")
        : this(AssertionService.DefaultScopeStrategy, context)
    {
    }

    /// <summary>
    /// Add the assertion.
    /// </summary>
    /// <param name="strategy"></param>
    /// <param name="context"></param>
    public AssertionScope(IAssertionStrategy strategy, string context= "")
    {
        _strategy = strategy;
        _parent = CurrentScope.Value;
        _context = context;

        CurrentScope.Value = this;
    }

    internal void AddAssertion(IAssertion assertion)
    {
        _assertions.Add(assertion);
    }

    internal void PushAssertionItem(AssertionItem assertionItem, AssertionType assertionType)
    {
        _strategy.HandleFailure(_context, assertionType, assertionItem);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_parent is not null)
        {
            CurrentScope.Value = _parent;
        }

        GC.SuppressFinalize(this);

        _strategy.HandleFailure(_context, _assertions);
    }
}