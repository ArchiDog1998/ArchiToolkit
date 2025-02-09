﻿using ArchiToolkit.Assertions.AssertionItems;
using ArchiToolkit.Assertions.Assertions;

namespace ArchiToolkit.Assertions.Execution;

/// <summary>
/// </summary>
public class AssertionScope : IDisposable
{
    private static readonly AsyncLocal<AssertionScope?> CurrentScope = new();
    private readonly List<IAssertion> _assertions = [];
    private readonly string _context;
    private readonly AssertionScope? _parent;
    private readonly IAssertionStrategy _strategy;

    /// <summary>
    /// </summary>
    /// <param name="context"></param>
    public AssertionScope(string context = "")
        : this(AssertionService.DefaultScopeStrategy, context)
    {
    }

    /// <summary>
    ///     Add the assertion.
    /// </summary>
    /// <param name="strategy"></param>
    /// <param name="context"></param>
    public AssertionScope(IAssertionStrategy strategy, string context = "")
    {
        _strategy = strategy;
        _parent = CurrentScope.Value;
        _context = context;

        CurrentScope.Value = this;
    }

    /// <summary>
    ///     The current scope
    /// </summary>
    public static AssertionScope Current
    {
        get => CurrentScope.Value ?? new AssertionScope(AssertionService.DefaultPushStrategy, string.Empty);
        set => CurrentScope.Value = value;
    }


    private bool _handledFailure;
    /// <inheritdoc />
    public void Dispose()
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
        return _strategy.HandleFailure(_context, _assertions);
    }

    internal void AddAssertion(IAssertion assertion)
    {
        _assertions.Add(assertion);
    }

    internal object? PushAssertionItem(AssertionItem assertionItem, AssertionType assertionType, object? tag)
    {
        return _strategy.HandleFailure(_context, assertionType, assertionItem, tag);
    }
}