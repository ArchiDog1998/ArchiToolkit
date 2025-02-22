using Microsoft.CodeAnalysis;

namespace ArchiToolkit.RoslynHelper.Names;

/// <inheritdoc />
public abstract class BaseName<T> : IName<T> where T : ISymbol
{
    private readonly Lazy<string> _lazyFullName, _lazySummaryName;

    /// <inheritdoc />
    public T Symbol { get; }

    /// <inheritdoc />
    public string Name => Symbol.Name;

    /// <inheritdoc />
    public string FullName => _lazyFullName.Value;

    /// <inheritdoc />
    public string SummaryName => _lazySummaryName.Value;


    private protected BaseName(T symbol)
    {
        Symbol = symbol;
        _lazyFullName = new Lazy<string>(() => symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
        _lazySummaryName = new Lazy<string>(GetSummaryName);
    }

    private protected virtual string GetSummaryName()
    {
        return ToSummary(Symbol.OriginalDefinition
            .ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
    }

    private protected static string ToSummary(string name) => name.Replace('<', '{').Replace('>', '}');
}