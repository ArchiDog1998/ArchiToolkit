using ArchiToolkit.RoslynHelper.Extensions;

namespace ArchiToolkit.InterpolatedParser.SourceGenerator;

partial class FormatGenerator
{
    private static readonly Dictionary<string, string> TypeToStringSyntax = new()
    {
        { "System.DateOnly", "DateOnlyFormat" },
        { typeof(DateTime).FullName!, "DateTimeFormat" },
        { typeof(Guid).FullName!, "GuidFormat" },
        { "System.TimeOnly", "TimeOnlyFormat" },
        { typeof(TimeSpan).FullName!, "TimeSpanFormat" },

        // Numeric types
        { typeof(byte).FullName!, "NumericFormat" },
        { typeof(sbyte).FullName!, "NumericFormat" },
        { typeof(short).FullName!, "NumericFormat" },
        { typeof(ushort).FullName!, "NumericFormat" },
        { typeof(int).FullName!, "NumericFormat" },
        { typeof(uint).FullName!, "NumericFormat" },
        { typeof(long).FullName!, "NumericFormat" },
        { typeof(ulong).FullName!, "NumericFormat" },
        { typeof(float).FullName!, "NumericFormat" },
        { typeof(double).FullName!, "NumericFormat" },
        { typeof(decimal).FullName!, "NumericFormat" }
    };

    private static StructDeclarationSyntax BasicStruct(string typeName, string methodName, ParseItem item,
        IImmutableDictionary<ITypeSymbol, ObjectCreationExpressionSyntax> creations)
    {
        var method1 = MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)), Identifier("AppendFormatted"))
            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
            .WithAttributeLists([MethodAttribute()])
            .WithParameterList(ParameterList([
                Parameter(Identifier("t")).WithModifiers(TokenList(Token(SyntaxKind.InKeyword)))
                    .WithType(IdentifierName(typeName)),
                AddStringSyntax(
                    Parameter(Identifier("format")).WithType(PredefinedType(Token(SyntaxKind.StringKeyword))), item),
                CallerNameParameter()
            ]))
            .WithExpressionBody(ArrowExpressionClause(InvocationExpression(IdentifierName(methodName))
                .WithArgumentList(ArgumentList(
                [
                    Argument(IdentifierName("t")),
                    Argument(IdentifierName("format")),
                    Argument(IdentifierName("callerName"))
                ]))))
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

        var method2 = MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)), Identifier("AppendFormatted"))
            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
            .WithAttributeLists([MethodAttribute()])
            .WithParameterList(ParameterList([
                Parameter(Identifier("t")).WithModifiers(TokenList(Token(SyntaxKind.InKeyword)))
                    .WithType(IdentifierName(typeName)),
                CallerNameParameter()
            ]))
            .WithExpressionBody(ArrowExpressionClause(InvocationExpression(IdentifierName(methodName))
                .WithArgumentList(ArgumentList(
                [
                    Argument(IdentifierName("t")),
                    Argument(LiteralExpression(SyntaxKind.NullLiteralExpression)),
                    Argument(IdentifierName("callerName"))
                ]))))
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

        var method3 = ModifyMethod(
            MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)), Identifier(methodName))
                .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword)))
                .WithAttributeLists([MethodAttribute()])
                .WithParameterList(ParameterList([
                    Parameter(Identifier("t")).WithModifiers(TokenList(Token(SyntaxKind.InKeyword)))
                        .WithType(IdentifierName(typeName)),
                    Parameter(Identifier("format"))
                        .WithType(NullableType(PredefinedType(Token(SyntaxKind.StringKeyword)))),
                    Parameter(Identifier("callerName")).WithType(PredefinedType(Token(SyntaxKind.StringKeyword)))
                ])), item, creations);

        return StructDeclaration("InterpolatedParseStringHandler")
            .WithModifiers(
                TokenList(Token(SyntaxKind.InternalKeyword), Token(SyntaxKind.PartialKeyword)))
            .WithMembers([method1, method2, method3]);
    }

    private static MethodDeclarationSyntax ModifyMethod(MethodDeclarationSyntax method, ParseItem item,
        IImmutableDictionary<ITypeSymbol, ObjectCreationExpressionSyntax> creations)
    {
        if (item.SubType is null)
            return method.WithExpressionBody(ArrowExpressionClause(
                    InvocationExpression(IdentifierName("AppendObject"))
                        .WithArgumentList(ArgumentList(
                            [
                                Argument(IdentifierName("t")),
                                Argument(IdentifierName("format")),
                                Argument(IdentifierName("callerName")),
                                Argument(GetArgument(item.Type))
                            ]
                        ))))
                .WithSemicolonToken(
                    Token(SyntaxKind.SemicolonToken));

        return method.WithExpressionBody(ArrowExpressionClause(
                InvocationExpression(GenericName(Identifier("AppendCollection"))
                        .WithTypeArgumentList(TypeArgumentList(
                        [
                            IdentifierName(item.Type.GetFullMetadataName(out _, true)),
                            IdentifierName(item.SubType.GetFullMetadataName(out _, true))
                        ])))
                    .WithArgumentList(ArgumentList(
                        [
                            Argument(IdentifierName("t")),
                            Argument(IdentifierName("format")),
                            Argument(IdentifierName("callerName")),
                            Argument(GetArgument(item.Type)),
                            Argument(GetArgument(item.SubType))
                        ]
                    ))))
            .WithSemicolonToken(
                Token(SyntaxKind.SemicolonToken));

        ExpressionSyntax GetArgument(ITypeSymbol type)
        {
            if (creations.TryGetValue(type, out var name)) return name;
            return LiteralExpression(SyntaxKind.NullLiteralExpression);
        }
    }

    private static ParameterSyntax CallerNameParameter()
    {
        return Parameter(Identifier("callerName"))
            .WithAttributeLists([
                AttributeList(SingletonSeparatedList(
                    Attribute(IdentifierName("global::System.Runtime.CompilerServices.CallerArgumentExpression"))
                        .WithArgumentList(AttributeArgumentList(
                            [AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal("t")))]))))
            ])
            .WithType(PredefinedType(Token(SyntaxKind.StringKeyword)))
            .WithDefault(EqualsValueClause(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(""))));
    }

    private static ParameterSyntax AddStringSyntax(ParameterSyntax argument, ParseItem item)
    {
        var name = GetStringSyntax(item);
        if (string.IsNullOrEmpty(name)) return argument;
        return argument.WithAttributeLists([
            AttributeList(
            [
                Attribute(IdentifierName("global::System.Diagnostics.CodeAnalysis.StringSyntax")).WithArgumentList(
                    AttributeArgumentList(
                    [
                        AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(name)))
                    ]))
            ])
        ]);
    }

    private static string GetStringSyntax(ParseItem item)
    {
        var name = GetStringSyntax(item.Type);
        if (!string.IsNullOrEmpty(name)) return name;
        if (item.SubType is { } type) return GetStringSyntax(type);
        return string.Empty;
    }

    private static string GetStringSyntax(ITypeSymbol type)
    {
        return TypeToStringSyntax.TryGetValue(type.GetFullMetadataName(out _), out var name) ? name : string.Empty;
    }
}