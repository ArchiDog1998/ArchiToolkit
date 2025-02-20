using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace ArchiToolkit.RoslynHelper;

/// <summary>
/// Get the type name.
/// </summary>
public readonly struct TypeName
{
    private readonly Lazy<string> _lazyFullName, _lazySafeName, _lazyHashName, _lazySummaryName;
    private readonly Lazy<ITypeParameterSymbol[]> _lazyTypeParameters;

    /// <summary>
    /// The full name
    /// </summary>
    public string FullName => _lazyFullName.Value;

    /// <summary>
    /// The safe name.
    /// </summary>
    public string SafeName => _lazySafeName.Value;

    /// <summary>
    /// The hash name.
    /// </summary>
    public string HashName => _lazyHashName.Value;

    /// <summary>
    /// Get the type parameters symbol
    /// </summary>
    public ITypeParameterSymbol[] TypeParameters => _lazyTypeParameters.Value;

    /// <summary>
    ///
    /// </summary>
    public IEnumerable<TypeParameterSyntax> GetParameterSyntaxes(string prefix = "")
        => TypeParameters.Select(p => TypeParameter(Identifier(prefix + p.Name)));

    /// <summary>
    ///
    /// </summary>
    public IEnumerable<TypeParameterConstraintClauseSyntax> GetParameterConstraintClauses(string prefix = "")
    {
        foreach (var symbol in TypeParameters)
        {
            if (GetConstraintClause(symbol, prefix) is { } clause) yield return clause;
        }
    }

    private static TypeParameterConstraintClauseSyntax? GetConstraintClause(ITypeParameterSymbol symbol, string prefix)
    {
        var constraints = new List<TypeParameterConstraintSyntax>();

        if (symbol.HasReferenceTypeConstraint)
            constraints.Add(ClassOrStructConstraint(SyntaxKind.ClassConstraint));

        if (symbol.HasValueTypeConstraint)
            constraints.Add(ClassOrStructConstraint(SyntaxKind.StructConstraint));

        if (symbol.HasUnmanagedTypeConstraint)
            constraints.Add(TypeConstraint(IdentifierName("unmanaged")));

        if (symbol.HasNotNullConstraint)
            constraints.Add(TypeConstraint(IdentifierName("notnull")));

        foreach (var constraintType in symbol.ConstraintTypes)
        {
            constraints.Add(TypeConstraint(ParseTypeName(constraintType.GetTypeName().FullName)));
        }

        if (symbol.HasConstructorConstraint)
            constraints.Add(ConstructorConstraint());

        if (constraints.Count is 0) return null;

        return TypeParameterConstraintClause(
            IdentifierName(prefix + symbol.Name),
            SeparatedList(constraints)
        );
    }

    /// <summary>
    /// If it has Type Parameters.
    /// </summary>
    public bool HasTypeParameters => TypeParameters.Length > 0;

    /// <summary>
    /// The summary name
    /// </summary>
    public string SummaryName => _lazySummaryName.Value;

    internal TypeName(ITypeSymbol typeSymbol, int count)
    {
        var lazy = _lazyFullName =
            new Lazy<string>(() => typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
        _lazySafeName = new Lazy<string>(() =>
        {
            var name = typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat
                .WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted));
            return Regex.Replace(name, @"[.\[\]<>,\s:]", "_");
        });
        _lazyHashName = new Lazy<string>(() => GetHashName(lazy.Value, count));
        _lazyTypeParameters = new Lazy<ITypeParameterSymbol[]>(() => GetTypeParameterSymbols(typeSymbol).ToArray());
        _lazySummaryName = new Lazy<string>(() => ToSummary(lazy.Value));
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

    private static string ToSummary(string typeName)
    {
        return Regex.Replace(typeName, "<[^>]+>", match =>
        {
            var count = match.Value.Split(',').Length;
            var replacement = "{" + string.Join(", ", Enumerable.Range(1, count).Select(i => $"T{i}")) + "}";
            return replacement;
        });
    }
}