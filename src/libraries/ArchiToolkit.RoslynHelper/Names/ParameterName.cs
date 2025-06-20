using ArchiToolkit.RoslynHelper.Extensions;
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

            if (Symbol.HasExplicitDefaultValue) //DefaultValue
            {
                var defaultExpression = CreateDefaultValueExpression(Symbol.Type, Symbol.ExplicitDefaultValue);
                if (defaultExpression != null)
                {
                    param = param.WithDefault(EqualsValueClause(defaultExpression));
                }
            }

            return param;
        }
    }

    private static ExpressionSyntax? CreateDefaultValueExpression(ITypeSymbol type, object? value)
    {
        if (value == null)
        {
            return LiteralExpression(SyntaxKind.NullLiteralExpression);
        }

        return type.SpecialType switch
        {
            SpecialType.System_Boolean => LiteralExpression((bool)value ? SyntaxKind.TrueLiteralExpression : SyntaxKind.FalseLiteralExpression),
            SpecialType.System_String => LiteralExpression(SyntaxKind.StringLiteralExpression, Literal((string)value)),
            SpecialType.System_Char => LiteralExpression(SyntaxKind.CharacterLiteralExpression, Literal((char)value)),
            SpecialType.System_Int32 => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal((int)value)),
            SpecialType.System_Double => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal((double)value)),
            SpecialType.System_Single => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal((float)value)),
            SpecialType.System_Decimal => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal((decimal)value)),
            SpecialType.System_Int64 => LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal((long)value)),
            _ => null
        };
    }
}