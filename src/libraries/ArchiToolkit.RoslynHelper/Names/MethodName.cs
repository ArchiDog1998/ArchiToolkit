using ArchiToolkit.RoslynHelper.Extensions;
using Microsoft.CodeAnalysis;

namespace ArchiToolkit.RoslynHelper.Names;

/// <summary>
///
/// </summary>
public class MethodName : TypeParametersName<IMethodSymbol>
{

    private protected override IEnumerable<ITypeParameterSymbol> GetTypeParameters(IMethodSymbol symbol)
    {
        return symbol.TypeParameters;
    }

    /// <summary>
    ///
    /// </summary>
    public IParameterSymbol[] Parameters { get; }

    /// <summary>
    /// Return types.
    /// </summary>
    public TypeName ReturnType { get; }

    internal MethodName(IMethodSymbol methodSymbol) : base(methodSymbol)
    {
        Parameters = methodSymbol.Parameters.ToArray();
        ReturnType = methodSymbol.ReturnType.GetName();
    }
}