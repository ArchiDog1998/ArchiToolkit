using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;

namespace ArchiToolkit.RoslynHelper;

public readonly struct TypeName
{
    private readonly Lazy<string> _lazyFullName, _lazySaveName, _lazyHashName;
    private readonly Lazy<ITypeParameterSymbol[]> _lazyTypeParameters;
    public string FullName => _lazyFullName.Value;

    public string SafeName => _lazySaveName.Value;

    public string HashName => _lazyHashName.Value;

    public ITypeParameterSymbol[] TypeParameters => _lazyTypeParameters.Value;

    public bool HasTypeParameters => TypeParameters.Length > 0;

    internal TypeName(ITypeSymbol typeSymbol, int count)
    {
        var lazy = _lazyFullName =
            new Lazy<string>(() => typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
        _lazySaveName = new Lazy<string>(() => Regex.Replace(lazy.Value, @"[.\[\]<>,\s]", "_"));
        _lazyHashName = new Lazy<string>(() => GetHashName(lazy.Value, count));
        _lazyTypeParameters = new Lazy<ITypeParameterSymbol[]>(() => GetTypeParameterSymbols(typeSymbol).ToArray());
    }

    private static IEnumerable<ITypeParameterSymbol> GetTypeParameterSymbols(ITypeSymbol symbol)
    {
        if (symbol is ITypeParameterSymbol typeParameterSymbol) yield return typeParameterSymbol;
        if (symbol is not INamedTypeSymbol namedTypeSymbol) yield break;
        foreach (var typeParameter in namedTypeSymbol.TypeArguments.SelectMany(GetTypeParameterSymbols))
        {
            yield return typeParameter;
        }
    }

    private static string GetHashName(string input, int count)
    {
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_1234567890";
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
        return string.Concat(hashBytes.Take(count).Select(b => chars[b % chars.Length]));
    }
}