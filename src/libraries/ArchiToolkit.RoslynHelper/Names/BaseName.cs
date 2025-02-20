using Microsoft.CodeAnalysis;

namespace ArchiToolkit.RoslynHelper.Names;

/// <inheritdoc />
public abstract class BaseName<T> : IName<T> where T : ISymbol
{
    private readonly Lazy<string> _lazyFullName;

    /// <inheritdoc />
    public T Symbol { get; }

    /// <inheritdoc />
    public string Name => Symbol.Name;

    /// <inheritdoc />
    public string FullName => _lazyFullName.Value;

    private protected BaseName(T symbol)
    {
        Symbol = symbol;
        _lazyFullName = new Lazy<string>(() => symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
    }
}