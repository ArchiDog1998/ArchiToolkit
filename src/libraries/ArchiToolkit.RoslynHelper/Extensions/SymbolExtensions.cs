using ArchiToolkit.RoslynHelper.Names;
using Microsoft.CodeAnalysis;

namespace ArchiToolkit.RoslynHelper.Extensions;

/// <summary>
/// Extensions for symbol
/// </summary>
public static class SymbolExtensions
{
    /// <summary>
    ///     Get the type name.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="hashCount"></param>
    /// <returns></returns>
    public static TypeName GetName(this ITypeSymbol symbol, int hashCount = 32) => new(symbol, hashCount);

    /// <summary>
    ///
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="prefix"></param>
    /// <returns></returns>
    public static TypeParamName GetName(this ITypeParameterSymbol symbol, string prefix = "") => new(symbol, prefix);

    /// <summary>
    ///
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static MethodName GetName(this IMethodSymbol symbol) => new(symbol);

    /// <summary>
    ///
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static ParameterName GetName(this IParameterSymbol symbol) => new(symbol);

    /// <summary>
    ///
    /// </summary>
    /// <param name="symbols"></param>
    /// <param name="prefix"></param>
    /// <returns></returns>
    public static IEnumerable<TypeParamName> GetNames(this IEnumerable<ITypeParameterSymbol> symbols,
        string prefix = "")
    {
        return symbols.Select(symbol => symbol.GetName(prefix));
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="symbols"></param>
    /// <param name="???"></param>
    /// <returns></returns>
    public static IEnumerable<ParameterName> GetNames(this IEnumerable<IParameterSymbol> symbols)
    {
        return symbols.Select(symbol => symbol.GetName());
    }
}