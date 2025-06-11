using ArchiToolkit.RoslynHelper.Names;
using Microsoft.CodeAnalysis;

namespace ArchiToolkit.RoslynHelper.Extensions;

/// <summary>
///     Extensions for symbol
/// </summary>
public static class SymbolExtensions
{
    /// <summary>
    ///     Get the type name.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static TypeName GetName(this ITypeSymbol symbol)
    {
        return new TypeName(symbol);
    }

    /// <summary>
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static TypeParamName GetName(this ITypeParameterSymbol symbol)
    {
        return new TypeParamName(symbol);
    }

    /// <summary>
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static MethodName GetName(this IMethodSymbol symbol)
    {
        return new MethodName(symbol);
    }

    /// <summary>
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static ParameterName GetName(this IParameterSymbol symbol)
    {
        return new ParameterName(symbol);
    }

    /// <summary>
    /// </summary>
    /// <param name="symbols"></param>
    /// <returns></returns>
    public static IEnumerable<TypeParamName> GetNames(this IEnumerable<ITypeParameterSymbol> symbols)
    {
        return symbols.Select(symbol => symbol.GetName());
    }

    /// <summary>
    /// </summary>
    /// <param name="symbols"></param>
    /// <returns></returns>
    public static IEnumerable<ParameterName> GetNames(this IEnumerable<IParameterSymbol> symbols)
    {
        return symbols.Select(symbol => symbol.GetName());
    }
}