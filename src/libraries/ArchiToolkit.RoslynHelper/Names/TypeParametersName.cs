using Microsoft.CodeAnalysis;

namespace ArchiToolkit.RoslynHelper.Names;

/// <summary>
/// For the one has type parameters.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class TypeParametersName<T> : BaseName<T>, ITypeParametersName
    where T : ISymbol
{
    private readonly Lazy<ITypeParameterSymbol[]> _lazyTypeParameters;

    /// <inheritdoc />
    public ITypeParameterSymbol[] TypeParameters => _lazyTypeParameters.Value;

    private protected TypeParametersName(T symbol) : base(symbol)
    {
        _lazyTypeParameters = new Lazy<ITypeParameterSymbol[]>(() => GetTypeParameters(symbol).ToArray());
    }

    private protected abstract IEnumerable<ITypeParameterSymbol> GetTypeParameters(T symbol);
}