using System.Collections.Immutable;
using ArchiToolkit.RoslynHelper.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static ArchiToolkit.RoslynHelper.Extensions.SyntaxExtensions;


namespace ArchiToolkit.ValidResults.SourceGenerator;

public static class TypeHelper
{
    public static TypeSyntax GetResultDataType(ITypeSymbol type)
    {
        if (GetParentDataType(type) is { } dataType)
        {
            type = dataType;
        }

        if (type.GetName().FullName is "global::ArchiToolkit.ValidResults.ValidResult.Data")
            return IdentifierName(Identifier("global::ArchiToolkit.ValidResults.ValidResult.Data"));

        NameSyntax childType = type.SpecialType is SpecialType.System_Void
            ? IdentifierName(Identifier("global::ArchiToolkit.ValidResults.ValidResult"))
            : GenericName(
                    Identifier("global::ArchiToolkit.ValidResults.ValidResult"))
                .WithTypeArgumentList(TypeArgumentList(
                [
                    IdentifierName(type.GetName().FullName)
                ]));
        return QualifiedName(childType, IdentifierName("Data"));
    }


    public static TypeSyntax FindValidResultType(Dictionary<ISymbol?, INamedTypeSymbol> dictionary,
        ITypeSymbol target)
    {
        if (target.SpecialType is SpecialType.System_Void)
        {
            return IdentifierName(Identifier("global::ArchiToolkit.ValidResults.ValidResult"));
        }

        if (GetParentDataType(target) is { } dataType)
        {
            target = dataType;
        }

        if (target.GetName().FullName is "global::ArchiToolkit.ValidResults.ValidResult.Data")
            return IdentifierName(Identifier("global::ArchiToolkit.ValidResults.ValidResult"));

        var loopTarget = target.BaseType;
        while (loopTarget is not null)
        {
            if (dictionary.TryGetValue(loopTarget, out var symbol))
                return IdentifierName(symbol.GetName().FullName);
            loopTarget = loopTarget.BaseType;
        }

        return GenericName(
                Identifier("global::ArchiToolkit.ValidResults.ValidResult"))
            .WithTypeArgumentList(TypeArgumentList(
            [
                IdentifierName(target.GetName().FullName)
            ]));
    }

    private static ITypeSymbol? GetParentDataType(ITypeSymbol type)
    {
        if (type.ContainingType is not { } containingType) return null;
        var resultInterface = containingType.AllInterfaces.FirstOrDefault(i =>
        {
            if (!i.IsGenericType) return false;
            return i.ConstructedFrom.GetName().FullName is "global::ArchiToolkit.ValidResults.IValidResult<TValue>";
        });
        return resultInterface?.TypeArguments.FirstOrDefault();
    }

}