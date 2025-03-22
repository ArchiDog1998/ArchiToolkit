using System.Collections.Immutable;
using ArchiToolkit.RoslynHelper.Extensions;
using ArchiToolkit.RoslynHelper.Names;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static ArchiToolkit.RoslynHelper.Extensions.SyntaxExtensions;

namespace ArchiToolkit.Grasshopper.SourceGenerator;

[Flags]
public enum ParamType : byte
{
    Da = 1 << 0,
    Component = 1 << 1,
    Field = 1 << 2,
    In = 1 << 3,
    Out = 1 << 4,
}

public enum ParamAccess : byte
{
    Item,
    List,
    Tree,
}

public class MethodParamItem(
    string name,
    TypeName type,
    ParamType paramType,
    ImmutableArray<AttributeData> attributes)
{
    public readonly string Name = name;
    public readonly TypeName TypeName = type;
    public readonly ParamType Type = paramType;
    public readonly ParamAccess Access = GetParamAccess(type.Symbol);
    public readonly ImmutableArray<AttributeData> Attributes = attributes;
    public static Dictionary<string, string> TypeDictionary { get; set; } = [];

    protected string GetParamClassName(string typeName)
    {
        return TypeDictionary.TryGetValue(typeName, out var className)
            ? className
            : "global::Grasshopper.Kernel.Parameters.Param_GenericObject";
    }

    public string ParameterName => Type switch
    {
        ParamType.Component => "this",
        ParamType.Da => "DA",
        ParamType.Field => "_" + Name,
        _ => Name,
    };

    public MethodParamItem(ParameterName name) : this(name.Name, name.Type, GetParamType(name),
        name.Symbol.GetAttributes())
    {
    }

    public FieldDeclarationSyntax Field()
    {
        return FieldDeclaration(VariableDeclaration(IdentifierName(TypeName.FullName))
                .WithVariables([VariableDeclarator(Identifier(ParameterName))]))
            .WithAttributeLists([GeneratedCodeAttribute(typeof(MethodParamItem))])
            .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword)));
    }

    public BlockSyntax IoBlock()
    {
        return Block(BlockItems());

        IEnumerable<StatementSyntax> BlockItems()
        {
            yield return CreateParameter();

            yield return AddParameter();
        }

        LocalDeclarationStatementSyntax CreateParameter()
        {
            var guidAttribute = Attributes.FirstOrDefault(a => a.AttributeClass?.GetName().FullName
                is "global::ArchiToolkit.Grasshopper.ParamTypeAttribute");
            if (guidAttribute is not null
                && guidAttribute.ConstructorArguments.Length > 0
                && guidAttribute.ConstructorArguments[0].Value?.ToString() is { } guidString)
            {
                return LocalDeclarationStatement(VariableDeclaration(IdentifierName("dynamic"))
                    .WithVariables(
                    [
                        VariableDeclarator(Identifier("param")).WithInitializer(EqualsValueClause(InvocationExpression(
                                IdentifierName("global::Grasshopper.Instances.ComponentServer.EmitObject"))
                            .WithArgumentList(ArgumentList([
                                Argument(ImplicitObjectCreationExpression()
                                    .WithArgumentList(ArgumentList(
                                    [
                                        Argument(LiteralExpression(SyntaxKind.StringLiteralExpression,
                                            Literal(guidString)))
                                    ])))
                            ]))))
                    ]));
            }

            var typeAttribute = Attributes.FirstOrDefault(a =>
                                {
                                    var attr = a.AttributeClass;
                                    if (attr is null) return false;
                                    if (!attr.IsGenericType) return false;
                                    if (attr.TypeArguments.Length < 1) return true;
                                    if (attr.ConstructUnboundGenericType().GetName().FullName
                                        is not "global::ArchiToolkit.Grasshopper.ParamTypeAttribute<>")
                                        return false;
                                    return true;
                                })?.AttributeClass?.TypeArguments[0].GetName().FullName
                                ?? GetParamClassName(GetInnerType(TypeName.Symbol).GetName().FullName);
            return LocalDeclarationStatement(VariableDeclaration(IdentifierName("var"))
                .WithVariables([
                    VariableDeclarator(Identifier("param"))
                        .WithInitializer(EqualsValueClause(
                            ObjectCreationExpression(IdentifierName(typeAttribute))
                                .WithArgumentList(ArgumentList())))
                ]));
        }

        StatementSyntax AddParameter()
        {

            return ExpressionStatement(InvocationExpression(MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression, IdentifierName("pManager"),
                    IdentifierName("AddParameter")))
                .WithArgumentList(ArgumentList(
                    [
                        Argument(CastExpression(IdentifierName("global::Grasshopper.Kernel.IGH_Param"), IdentifierName(Identifier("param")))),
                        MethodGenerator.GetArgumentKeyedString(Name + ".Name"),
                        MethodGenerator.GetArgumentKeyedString(Name + ".Nickname"),
                        MethodGenerator.GetArgumentKeyedString(Name + ".Description"),
                        Argument(
                            MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                IdentifierName("global::Grasshopper.Kernel.GH_ParamAccess"),
                                IdentifierName(Access switch
                                {
                                    ParamAccess.List => "list",
                                    ParamAccess.Tree => "tree",
                                    _ => "item",
                                })))
                    ])));
        }
    }

    private static ITypeSymbol GetInnerType(ITypeSymbol type)
    {
        if (type is not INamedTypeSymbol namedType) return type;

        if (namedType.AllInterfaces.FirstOrDefault(i =>
            {
                if (!i.IsGenericType) return false;
                if (i.TypeArguments.Length is 0) return false;
                return i.ConstructUnboundGenericType().GetName().FullName
                    is "global::System.Collections.Generic.IList";
            })?.TypeArguments[0] is { } listItem)
        {
            return GetInnerType(listItem);
        }

        if (!namedType.IsGenericType) return type;
        if (namedType.TypeArguments.Length is 0) return type;
        switch (namedType.ConstructUnboundGenericType().GetName().FullName)
        {
            case "global::ArchiToolkit.Grasshopper.Io<>":
            case "global::Grasshopper.Kernel.Data.GH_Structure<>":
            case "global::Grasshopper.DataTree<>":
                var innerType = namedType.TypeArguments[0];
                return GetInnerType(innerType);

            default:
                return type;
        }
    }


    private static ParamType GetParamType(ParameterName name)
    {
        ParamType paramType = 0;
        var typeSymbol = name.Type.Symbol;
        if (name.Symbol.GetAttributes().Any(a => a.AttributeClass?.GetName().FullName
                is "global::ArchiToolkit.Grasshopper.ObjFieldAttribute"))
        {
            return ParamType.Field;
        }

        foreach (var type in typeSymbol.AllInterfaces.Append(typeSymbol))
        {
            switch (type.GetName().FullName)
            {
                case "global::Grasshopper.Kernel.IGH_Component":
                    return ParamType.Component;
                case "global::Grasshopper.Kernel.IGH_DataAccess":
                    return ParamType.Da;
            }
        }

        if (name.IsIn) paramType |= ParamType.In;
        if (name.IsOut) paramType |= ParamType.Out;
        return paramType;
    }

    private static ParamAccess GetParamAccess(ITypeSymbol typeSymbol)
    {
        if (typeSymbol is INamedTypeSymbol { IsGenericType: true, TypeArguments.Length: 1 } namedTypeSymbol
            && namedTypeSymbol.ConstructUnboundGenericType().GetName().FullName ==
            "global::ArchiToolkit.Grasshopper.Io<>")
        {
            typeSymbol = namedTypeSymbol.TypeArguments[0];
        }

        foreach (var type in typeSymbol.AllInterfaces.Append(typeSymbol))
        {
            switch (type.GetName().FullName)
            {
                case "global::Grasshopper.Kernel.Data.IGH_Structure":
                case "global::Grasshopper.Kernel.Data.IGH_DataTree":
                    return ParamAccess.Tree;
                case "global::System.Collections.IList":
                    return ParamAccess.List;
            }
        }

        return ParamAccess.Item;
    }
}