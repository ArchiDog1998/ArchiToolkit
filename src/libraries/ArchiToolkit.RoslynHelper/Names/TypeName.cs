using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;

namespace ArchiToolkit.RoslynHelper.Names;

/// <summary>
///     Get the type name.
/// </summary>
public class TypeName : TypeParametersName<ITypeSymbol>
{
    private readonly Lazy<string> _lazySafeName, _lazyHashName, _lazySummaryName;

    /// <summary>
    ///     The safe name.
    /// </summary>
    public string SafeName => _lazySafeName.Value;

    /// <summary>
    ///     The hash name.
    /// </summary>
    public string HashName => _lazyHashName.Value;


    /// <summary>
    ///     The summary name
    /// </summary>
    public string SummaryName => _lazySummaryName.Value;

    internal TypeName(ITypeSymbol typeSymbol, int count) : base(typeSymbol)
    {
        _lazySafeName = new Lazy<string>(() =>
        {
            var name = typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat
                .WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted));
            return Regex.Replace(name, @"[.\[\]<>,\s:]", "_");
        });
        _lazyHashName = new Lazy<string>(() => GetHashName(FullName, count));
        _lazySummaryName = new Lazy<string>(() => ToSummary(FullName));
    }

    private static string GetHashName(string input, int count)
    {
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_1234567890";
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
        return string.Concat(hashBytes.Take(count).Select(b => chars[b % chars.Length]));
    }

    private static string ToSummary(string typeName)
    {
        return Regex.Replace(typeName, "<[^>]+>", match =>
        {
            var count = match.Value.Split(',').Length;
            var replacement = "{" + string.Join(", ", Enumerable.Range(1, count).Select(i => $"T{i}")) + "}";
            return replacement;
        });
    }

    private protected override IEnumerable<ITypeParameterSymbol> GetTypeParameters(ITypeSymbol symbol)
    {
        return GetTypeParameterSymbols(symbol);

        static IEnumerable<ITypeParameterSymbol> GetTypeParameterSymbols(ITypeSymbol symbol)
        {
            if (symbol is ITypeParameterSymbol typeParameterSymbol) yield return typeParameterSymbol;
            if (symbol is not INamedTypeSymbol namedTypeSymbol) yield break;
            foreach (var typeParameter in namedTypeSymbol.TypeArguments.SelectMany(GetTypeParameterSymbols))
                yield return typeParameter;
        }
    }
}