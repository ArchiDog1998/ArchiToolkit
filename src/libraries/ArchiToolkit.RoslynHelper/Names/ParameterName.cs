using ArchiToolkit.RoslynHelper.Extensions;
using Microsoft.CodeAnalysis;

namespace ArchiToolkit.RoslynHelper.Names;

/// <summary>
/// </summary>
public class ParameterName : BaseName<IParameterSymbol>
{
    internal ParameterName(IParameterSymbol symbol) : base(symbol)
    {
        IsIn = symbol.RefKind is RefKind.In or RefKind.Ref or RefKind.None;
        IsOut = symbol.RefKind is RefKind.Out or RefKind.Ref;
        Type = symbol.Type.GetName();
    }

    /// <summary>
    /// </summary>
    public bool IsIn { get; }

    /// <summary>
    /// </summary>
    public bool IsOut { get; }

    /// <summary>
    ///     The type.
    /// </summary>
    public TypeName Type { get; }
}