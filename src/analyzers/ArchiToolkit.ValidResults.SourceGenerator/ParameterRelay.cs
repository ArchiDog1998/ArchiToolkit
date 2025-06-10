using ArchiToolkit.RoslynHelper.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace ArchiToolkit.ValidResults.SourceGenerator;

public class ParameterRelay(ITypeSymbol type, string name, RefKind kind, ExpressionSyntax? defaultValue = null)
{
    public ITypeSymbol Type => type;
    public string Name => name;
    public RefKind Kind => kind;
    public ExpressionSyntax? DefaultValue => defaultValue;

    public ParameterRelay(IParameterSymbol symbol) : this(symbol.Type, symbol.Name, symbol.RefKind,
        GetDefaultValueExpression(symbol))
    {
    }

    private static ExpressionSyntax? GetDefaultValueExpression(IParameterSymbol parameter)
    {
        if (!parameter.HasExplicitDefaultValue)
            return null;

        var value = parameter.ExplicitDefaultValue;
        var type = parameter.Type;

        if (value == null)
            return LiteralExpression(SyntaxKind.NullLiteralExpression);

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
            {
                return ParseExpression(
                    $"{type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}.{enumMember.Name}");
            }
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


    public ExpressionStatementSyntax? AfterAssign()
    {
        if (Kind is not RefKind.Ref and not RefKind.Out) return null;
        var resultDataType = TypeHelper.GetResultDataType(Type);
        return ExpressionStatement(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
            IdentifierName(Name), ObjectCreationExpression(resultDataType)
                .WithArgumentList(ArgumentList(
                [
                    Argument(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                        IdentifierName("__result"), IdentifierName("Result"))),
                    Argument(
                        IdentifierName("_" + Name))
                ]))));
    }

    public LocalDeclarationStatementSyntax CreateLocalDeclarationStatement(bool isTracker)
    {
        if (Type.IsRefLikeType)
        {
            return LocalDeclarationStatement(VariableDeclaration(IdentifierName("var"))
                .WithVariables([
                    VariableDeclarator(Identifier("_" + Name))
                        .WithInitializer(EqualsValueClause(IdentifierName(Name)))
                ]));
        }

        return Kind switch
        {
            RefKind.Out => LocalDeclarationStatement(VariableDeclaration(IdentifierName(Type.GetName().FullName))
                .WithVariables([
                    VariableDeclarator(Identifier("_" + Name))
                        .WithInitializer(EqualsValueClause(PostfixUnaryExpression(
                            SyntaxKind.SuppressNullableWarningExpression,
                            LiteralExpression(SyntaxKind.DefaultLiteralExpression, Token(SyntaxKind.DefaultKeyword)))))
                ])),
            _ => LocalDeclarationStatement(VariableDeclaration(IdentifierName("var"))
                .WithVariables([
                    VariableDeclarator(Identifier("_" + Name))
                        .WithInitializer(EqualsValueClause(MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression, IdentifierName(Name),
                            IdentifierName(isTracker ? "Value.ValueOrDefault" : "ValueOrDefault"))))
                ]))
        };
    }

    public ArgumentSyntax GenerateArgument()
    {
        var argument = Argument(IdentifierName("_" + Name));
        if (Modifier is { } modifier)
        {
            argument = argument.WithRefOrOutKeyword(Token(modifier));
        }

        return argument;
    }

    public ParameterSyntax GenerateParameter(bool isTracker, string trackerName, ITypeSymbol? containingType)
    {
        var parameter = Parameter(Identifier(Name));

        if (Type.IsRefLikeType)
        {
            parameter = parameter.WithType(IdentifierName(Type.GetName().FullName));
        }
        else if (isTracker)
        {
            if (Type.Equals(containingType, SymbolEqualityComparer.Default))
            {
                parameter = parameter.WithType(IdentifierName(trackerName));
            }
            else
            {
                parameter = parameter.WithType(
                    GenericName(Identifier("global::ArchiToolkit.ValidResults.ResultTracker"))
                        .WithTypeArgumentList(TypeArgumentList(
                        [
                            GenericName(Identifier("global::ArchiToolkit.ValidResults.ValidResult"))
                                .WithTypeArgumentList(TypeArgumentList(
                                [
                                    IdentifierName(Type.GetName().FullName)
                                ]))
                        ])));
            }
        }
        else
        {
            parameter = parameter.WithType(GenericName(Identifier("global::ArchiToolkit.ValidResults.ValidResult"))
                .WithTypeArgumentList(TypeArgumentList(
                [
                    IdentifierName(Type.GetName().FullName)
                ])));
        }


        if (Modifier is { } modifier)
        {
            parameter = parameter.WithModifiers(TokenList(Token(modifier)));
        }

        if (DefaultValue is not null)
        {
            parameter = parameter.WithType(NullableType(parameter.Type!))
                .WithDefault(EqualsValueClause(LiteralExpression(SyntaxKind.NullLiteralExpression)));
        }

        return parameter;
    }

    public ExpressionStatementSyntax? CreateDefaultValue()
    {
        if (DefaultValue is null) return null;

        return ExpressionStatement(
            AssignmentExpression(
                SyntaxKind.CoalesceAssignmentExpression,
                IdentifierName(Name),
                CastExpression(
                    IdentifierName(type.GetName().FullName),
                    DefaultValue)));
    }


    private SyntaxKind? Modifier => Kind switch
    {
        RefKind.Ref => SyntaxKind.RefKeyword,
        RefKind.Out => SyntaxKind.OutKeyword,
        RefKind.In => SyntaxKind.InKeyword,
        _ => null,
    };

    public LocalDeclarationStatementSyntax GenerateReason(out string reasonName, bool isTracker)
    {
        reasonName = "_" + name + "Reasons";

        var argumentList = isTracker
            ? ArgumentList(
            [
                Argument(IdentifierName(name))
            ])
            : ArgumentList(
            [
                Argument(IdentifierName(name)),
                Argument(IdentifierName(name + "Name")),
                Argument(IdentifierName("_filePath")),
                Argument(IdentifierName("_fileLineNumber"))
            ]);

        return LocalDeclarationStatement(VariableDeclaration(IdentifierName("var"))
            .WithVariables([
                VariableDeclarator(Identifier(reasonName))
                    .WithInitializer(EqualsValueClause(InvocationExpression(MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName("global::ArchiToolkit.ValidResults.ValidResultsExtensions"),
                            GenericName(Identifier("GetReasons"))
                                .WithTypeArgumentList(TypeArgumentList(
                                [
                                    GenericName(Identifier("global::ArchiToolkit.ValidResults.ValidResult"))
                                        .WithTypeArgumentList(
                                            TypeArgumentList([IdentifierName(type.GetName().FullName)]))
                                ]))))
                        .WithArgumentList(argumentList)))
            ]));
    }
}