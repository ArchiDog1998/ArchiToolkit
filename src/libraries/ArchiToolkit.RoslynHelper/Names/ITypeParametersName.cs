using Microsoft.CodeAnalysis;

namespace ArchiToolkit.RoslynHelper.Names;

/// <summary>
///
/// </summary>
public interface ITypeParametersName
{
    /// <summary>
    ///     Get the type parameters symbol
    /// </summary>
    ITypeParameterSymbol[] TypeParameters { get; }
}