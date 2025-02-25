using System.Text;
using Microsoft.CodeAnalysis;

namespace ArchiToolkit.RoslynHelper.Extensions;

/// <summary>
///     The default Roslyn Extensions.
/// </summary>
public static class RoslynExtensions
{
    /// <summary>
    ///     Get the base types and this type.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static IEnumerable<ITypeSymbol> GetBaseTypesAndThis(this ITypeSymbol type)
    {
        var current = type;
        while (current != null)
        {
            yield return current;
            current = current.BaseType;
        }
    }

    /// <summary>
    ///     Print a node to string.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public static string NodeToString(this SyntaxNode node)
    {
        using var stringWriter = new StringWriter();
        node.NormalizeWhitespace().WriteTo(stringWriter);
        return stringWriter.ToString();
    }
}