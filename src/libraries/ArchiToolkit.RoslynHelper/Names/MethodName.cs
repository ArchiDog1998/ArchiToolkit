using System.Text;
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
    public ParameterName[] Parameters { get; }

    /// <summary>
    /// Return types.
    /// </summary>
    public TypeName ReturnType { get; }

    /// <summary>
    /// ContainingType
    /// </summary>
    public TypeName ContainingType { get; }

    internal MethodName(IMethodSymbol methodSymbol) : base(methodSymbol)
    {
        Parameters = methodSymbol.Parameters.GetNames().ToArray();
        ReturnType = methodSymbol.ReturnType.GetName();
        ContainingType = methodSymbol.ContainingType.GetName();
    }

    private protected override string GetSummaryName()
    {
        var builder = new StringBuilder(base.GetSummaryName());
        builder.Append('(').Append(string.Join(",", Parameters.Select(p =>
        {
            var type = ToSummary(p.Type.FullName);
            return p.Symbol.RefKind switch
            {
                RefKind.Ref => "ref " + type,
                RefKind.In => "in " + type,
                RefKind.Out => "out " + type,
                _ => type
            };
        }))).Append(')');
        return builder.ToString();
    }
}