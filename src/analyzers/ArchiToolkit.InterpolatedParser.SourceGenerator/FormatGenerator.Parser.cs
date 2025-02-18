namespace ArchiToolkit.InterpolatedParser.SourceGenerator;

partial class FormatGenerator
{
    private static ClassDeclarationSyntax? GetParserType(ITypeSymbol type, MetadataName metadataName,
        out ObjectCreationExpressionSyntax creation)
    {
        var className = "Parser_" + metadataName.HashName;

        //Parameter(Identifier("format")).WithType(NullableType(PredefinedType(Token(SyntaxKind.StringKeyword))))
        var basicClass = ClassDeclaration(className)
            .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword)))
            .WithParameterList(ParameterList());
        creation = ObjectCreationExpression(IdentifierName(className))
            .WithArgumentList(ArgumentList());

        //TODO: number parsing.

        if (HasInterface(type, "System.ISpanParsable<TSelf>"))
        {
            return basicClass.WithBaseList(BaseList(
            [
                SimpleBaseType(
                    GenericName(Identifier("global::ArchiToolkit.InterpolatedParser.Parsers.SpanParseableParser"))
                        .WithTypeArgumentList(TypeArgumentList([IdentifierName(type.GetFullMetadataName(true))])))
            ]));
        }

        if (HasInterface(type, "System.IParsable<TSelf>"))
        {
            return basicClass.WithBaseList(BaseList(
            [
                SimpleBaseType(
                    GenericName(Identifier("global::ArchiToolkit.InterpolatedParser.Parsers.StringParseableParser"))
                        .WithTypeArgumentList(TypeArgumentList([IdentifierName(type.GetFullMetadataName(true))])))
            ]));
        }

        //TODO: reflection parsing.

        return null;
    }
}