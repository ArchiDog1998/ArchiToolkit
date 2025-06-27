﻿using ArchiToolkit.RoslynHelper.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

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

    /// <summary>
    /// The parameter syntax.
    /// </summary>
    public ParameterSyntax ParameterSyntax
    {
        get
        {
            var param = Parameter(Identifier(Name)).WithType(IdentifierName(Type.FullName));
            switch (Symbol.RefKind)
            {
                case RefKind.Ref:
                    param = param.WithModifiers(TokenList(Token(SyntaxKind.RefKeyword)));
                    break;
                case RefKind.Out:
                    param = param.WithModifiers(TokenList(Token(SyntaxKind.OutKeyword)));
                    break;
                case RefKind.In:
                    param = param.WithModifiers(TokenList(Token(SyntaxKind.InKeyword)));
                    break;
                case RefKind.None:
                case RefKind.RefReadOnlyParameter:
                default:
                    break;
            }

            if (DefaultValueExpression is {} defaultExpression)
            {
                param = param.WithDefault(EqualsValueClause(defaultExpression));
            }

            return param;
        }
    }

    public ExpressionSyntax? DefaultValueExpression => GetDefaultValueExpression(Symbol);


    private static ExpressionSyntax? GetDefaultValueExpression(IParameterSymbol parameter)
    {
        if (!parameter.HasExplicitDefaultValue)
            return null;

        var value = parameter.ExplicitDefaultValue;
        var type = parameter.Type;

        if (value == null)
            return type.IsReferenceType
                ? LiteralExpression(SyntaxKind.NullLiteralExpression)
                : LiteralExpression(SyntaxKind.DefaultLiteralExpression);

        switch (type.SpecialType)
        {
            case SpecialType.System_String:
                return LiteralExpression(
                    SyntaxKind.StringLiteralExpression,
                    Literal((string)value));
            case SpecialType.System_Char:
                return LiteralExpression(
                    SyntaxKind.CharacterLiteralExpression,
                    Literal((char)value));
            case SpecialType.System_Boolean:
                return LiteralExpression((bool)value
                    ? SyntaxKind.TrueLiteralExpression
                    : SyntaxKind.FalseLiteralExpression);
        }

        if (type is INamedTypeSymbol { EnumUnderlyingType: not null } && value is IConvertible)
        {
            var enumMember = type.GetMembers()
                .OfType<IFieldSymbol>()
                .FirstOrDefault(f => f.HasConstantValue && Equals(f.ConstantValue, value));

            if (enumMember != null)
                return ParseExpression(
                    $"{type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}.{enumMember.Name}");
        }

        return value switch
        {
            int intValue => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(intValue)),
            double doubleValue => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(doubleValue)),
            float floatValue => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(floatValue)),
            long longValue => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(longValue)),
            byte byteValue => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(byteValue)),
            short shortValue => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(shortValue)),
            decimal decimalValue => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(decimalValue)),
            _ => ParseExpression(value.ToString())
        };
    }
}