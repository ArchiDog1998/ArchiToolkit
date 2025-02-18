using System.Text;
using Microsoft.CodeAnalysis;

namespace ArchiToolkit.RoslynHelper;

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
    ///     All the Children in this type.
    /// </summary>
    /// <typeparam name="T">The node type</typeparam>
    /// <param name="node"></param>
    /// <param name="removedNodes">the nodes need to removed.</param>
    /// <returns></returns>
    public static IEnumerable<T> GetChildren<T>(this SyntaxNode node, params SyntaxNode[] removedNodes)
        where T : SyntaxNode
    {
        if (removedNodes.Contains(node)) return [];
        if (node is T result) return [result];
        return node.ChildNodes().SelectMany(n => n.GetChildren<T>(removedNodes));
    }

    /// <summary>
    ///     Get the first parent with the specific <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T">The node type</typeparam>
    /// <param name="node"></param>
    /// <returns></returns>
    [Obsolete("Try AncestorsAndSelf() or Ancestors()")]
    public static T? GetParent<T>(this SyntaxNode? node) where T : SyntaxNode
    {
        if (node == null) return null;
        if (node is T result) return result;
        return GetParent<T>(node.Parent);
    }

    /// <summary>
    /// Get the metadata name of the symbol
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="hashCount"></param>
    /// <returns></returns>
    public static MetadataName GetMetadataName(this ISymbol symbol, int hashCount = 16) =>
        new(symbol, hashCount);

    /// <summary>
    ///     Get the full symbol name.
    /// </summary>
    /// <param name="s"></param>
    /// <param name="hasGlobal"></param>
    /// <returns></returns>
    public static string GetFullMetadataName(this ISymbol? s, bool hasGlobal = false)
    {
        if (s is null or INamespaceSymbol) return string.Empty;

        while (s != null && s is not ITypeSymbol) s = s.ContainingSymbol;

        if (s == null) return string.Empty;

        var sb = new StringBuilder(s.GetTypeSymbolName());

        s = s.ContainingSymbol;
        while (!IsRootNamespace(s))
        {
            try
            {
                sb.Insert(0, s.OriginalDefinition.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat) + '.');
            }
            catch
            {
                break;
            }

            s = s.ContainingSymbol;
        }

        return hasGlobal ? "global::" + sb : sb.ToString();

        static bool IsRootNamespace(ISymbol symbol)
        {
            return symbol is INamespaceSymbol { IsGlobalNamespace: true };
        }
    }

    private static string GetTypeSymbolName(this ISymbol symbol)
    {
        if (symbol is IArrayTypeSymbol arrayTypeSymbol) //Array
            return arrayTypeSymbol.ElementType.GetFullMetadataName() + "[]";

        var str = symbol.MetadataName;
        if (symbol is not INamedTypeSymbol symbolType) return str; //Generic

        var strs = str.Split('`');
        if (strs.Length < 2) return str;
        str = strs[0];

        str += "<" + string.Join(", ", symbolType.TypeArguments.Select(p => p.GetFullMetadataName())) + ">";
        return str;
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