using ArchiToolkit.RoslynHelper.Extensions;

namespace ArchiToolkit.InterpolatedParser.SourceGenerator;

partial class FormatGenerator
{
    private static ClassDeclarationSyntax? GetParserType(ITypeSymbol type, MetadataName metadataName,
        out ObjectCreationExpressionSyntax creation)
    {
        var className = "Parser_" + metadataName.HashName;

        var typeName = type.GetFullMetadataName(out _, true);
        var typeNameSimple = type.GetFullMetadataName(out _);
        var basicClass = ClassDeclaration(className)
            .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword)))
            .WithParameterList(ParameterList());
        creation = ObjectCreationExpression(IdentifierName(className))
            .WithArgumentList(ArgumentList());

        //TODO: number parsing.

        if (HasStaticMethod(type, "Parse", "System.ReadOnlySpan<System.Char>", "System.IFormatProvider")
            && HasStaticMethod(type, "Parse", "System.ReadOnlySpan<System.Char>", "System.Globalization.NumberStyles",
                "System.IFormatProvider")
            && HasStaticMethod(type, "TryParse", "System.ReadOnlySpan<System.Char>", "System.IFormatProvider",
                typeNameSimple)
            && HasStaticMethod(type, "TryParse", "System.ReadOnlySpan<System.Char>",
                "System.Globalization.NumberStyles", "System.IFormatProvider", typeNameSimple))
            return NumberChange(
                BaseTypeChange(basicClass, "global::ArchiToolkit.InterpolatedParser.Parsers.SpanParser", typeName),
                typeName, "System.ReadOnlySpan<System.Char>");

        if (HasStaticMethod(type, "Parse", "System.String", "System.IFormatProvider")
            && HasStaticMethod(type, "Parse", "System.String", "System.Globalization.NumberStyles",
                "System.IFormatProvider")
            && HasStaticMethod(type, "TryParse", "System.String", "System.IFormatProvider",
                typeNameSimple)
            && HasStaticMethod(type, "TryParse", "System.String",
                "System.Globalization.NumberStyles", "System.IFormatProvider", typeNameSimple))
            return NumberChange(
                BaseTypeChange(basicClass, "global::ArchiToolkit.InterpolatedParser.Parsers.SpanParser", typeName),
                typeName, "System.String");

        if (HasInterface(type, "System.ISpanParsable<TSelf>"))
            return BaseTypeChange(basicClass, "global::ArchiToolkit.InterpolatedParser.Parsers.SpanParseableParser",
                typeName);

        if (HasStaticMethod(type, "Parse", "System.ReadOnlySpan<System.Char>", "System.IFormatProvider")
            && HasStaticMethod(type, "TryParse", "System.ReadOnlySpan<System.Char>", "System.IFormatProvider",
                typeNameSimple))
            return GeneralChange(
                BaseTypeChange(basicClass, "global::ArchiToolkit.InterpolatedParser.Parsers.SpanParser", typeName),
                typeName, "System.ReadOnlySpan<System.Char>");

        if (HasInterface(type, "System.IParsable<TSelf>"))
            return BaseTypeChange(basicClass, "global::ArchiToolkit.InterpolatedParser.Parsers.StringParseableParser",
                typeName);

        if (HasStaticMethod(type, "Parse", "System.String", "System.IFormatProvider")
            && HasStaticMethod(type, "TryParse", "System.String", "System.IFormatProvider",
                typeNameSimple))
            return GeneralChange(
                BaseTypeChange(basicClass, "global::ArchiToolkit.InterpolatedParser.Parsers.SpanParser", typeName),
                typeName, "System.String");
        return null;
    }

    private static bool HasStaticMethod(ITypeSymbol typeSymbol, string methodName, params string[] argumentsName)
    {
        return typeSymbol
            .GetMembers(methodName)
            .OfType<IMethodSymbol>()
            .Any(method =>
            {
                if (!method.IsStatic) return false;
                if (method.Parameters.Length != argumentsName.Length) return false;
                return !argumentsName
                    .Where((t, i) => method.Parameters[i].Type.GetFullMetadataName(out _) != t).Any();
            });
    }

    private static ClassDeclarationSyntax BaseTypeChange(ClassDeclarationSyntax declaration, string baseTypeName,
        string typeName)
    {
        return declaration.WithBaseList(BaseList(
        [
            SimpleBaseType(GenericName(Identifier(baseTypeName))
                .WithTypeArgumentList(TypeArgumentList([IdentifierName(typeName)])))
        ]));
    }

    private static MethodDeclarationSyntax ParseMethod(string typeName, string inputName)
    {
        return MethodDeclaration(IdentifierName(typeName), Identifier("Parse"))
            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.OverrideKeyword)))
            .WithAttributeLists([MethodAttribute()])
            .WithParameterList(
                ParameterList([
                    Parameter(Identifier("s")).WithType(IdentifierName(inputName)),
                    Parameter(Identifier("provider"))
                        .WithType(NullableType(IdentifierName("global::System.IFormatProvider")))
                ]));
    }

    private static MethodDeclarationSyntax TryParseMethod(string typeName, string inputName)
    {
        return MethodDeclaration(PredefinedType(Token(SyntaxKind.BoolKeyword)), Identifier("TryParse"))
            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.OverrideKeyword)))
            .WithAttributeLists([MethodAttribute()])
            .WithParameterList(ParameterList(
            [
                Parameter(Identifier("s")).WithType(IdentifierName(inputName)),
                Parameter(Identifier("provider"))
                    .WithType(NullableType(IdentifierName("global::System.IFormatProvider"))),
                Parameter(Identifier("result"))
                    .WithModifiers(TokenList(Token(SyntaxKind.OutKeyword)))
                    .WithType(IdentifierName(typeName))
            ]));
    }

    private static ClassDeclarationSyntax NumberChange(ClassDeclarationSyntax declaration,
        string typeName, string inputName)
    {
        var member1 = ParseMethod(typeName, inputName)
            .WithBody(Block(ReturnStatement(ConditionalExpression(IsPatternExpression(
                    IdentifierName("Format"),
                    ConstantPattern(LiteralExpression(SyntaxKind.NullLiteralExpression))),
                InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                        IdentifierName(typeName), IdentifierName("Parse")))
                    .WithArgumentList(ArgumentList(
                    [
                        Argument(IdentifierName("s")),
                        Argument(IdentifierName("provider"))
                    ])),
                InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                        IdentifierName(typeName), IdentifierName("Parse")))
                    .WithArgumentList(ArgumentList(
                    [
                        Argument(IdentifierName("s")),
                        Argument(IdentifierName("NumberStyle")),
                        Argument(IdentifierName("provider"))
                    ]))))));

        var method2 = TryParseMethod(typeName, inputName)
            .WithBody(Block(ReturnStatement(ConditionalExpression(IsPatternExpression(IdentifierName("Format"),
                    ConstantPattern(LiteralExpression(SyntaxKind.NullLiteralExpression))),
                InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                        IdentifierName(typeName), IdentifierName("TryParse")))
                    .WithArgumentList(ArgumentList(
                    [
                        Argument(IdentifierName("s")),
                        Argument(IdentifierName("provider")),
                        Argument(IdentifierName("result")).WithRefOrOutKeyword(Token(SyntaxKind.OutKeyword))
                    ])),
                InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                        IdentifierName(typeName), IdentifierName("TryParse")))
                    .WithArgumentList(
                        ArgumentList(
                        [
                            Argument(IdentifierName("s")),
                            Argument(IdentifierName("NumberStyle")),
                            Argument(IdentifierName("provider")),
                            Argument(IdentifierName("result")).WithRefOrOutKeyword(Token(SyntaxKind.OutKeyword))
                        ]))))));
        return declaration.WithMembers([member1, method2]);
    }

    private static ClassDeclarationSyntax GeneralChange(ClassDeclarationSyntax declaration,
        string typeName, string inputName)
    {
        var member1 = ParseMethod(typeName, inputName)
            .WithBody(Block(ReturnStatement(
                InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                        IdentifierName(typeName), IdentifierName("Parse")))
                    .WithArgumentList(ArgumentList(
                    [
                        Argument(IdentifierName("s")),
                        Argument(IdentifierName("provider"))
                    ])))));

        var method2 = TryParseMethod(typeName, inputName)
            .WithBody(Block(ReturnStatement(
                InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                        IdentifierName(typeName), IdentifierName("TryParse")))
                    .WithArgumentList(ArgumentList(
                    [
                        Argument(IdentifierName("s")),
                        Argument(IdentifierName("provider")),
                        Argument(IdentifierName("result")).WithRefOrOutKeyword(Token(SyntaxKind.OutKeyword))
                    ])))));
        return declaration.WithMembers([member1, method2]);
    }
}