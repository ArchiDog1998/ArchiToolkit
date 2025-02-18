namespace ArchiToolkit.InterpolatedParser.SourceGenerator;

partial class FormatGenerator
{
    private static ClassDeclarationSyntax? GetParserType(ITypeSymbol type, MetadataName metadataName,
        out ObjectCreationExpressionSyntax creation)
    {
        var className = "Parser_" + metadataName.HashName;

        //Parameter(Identifier("format")).WithType(NullableType(PredefinedType(Token(SyntaxKind.StringKeyword))))
        var basicClass = ClassDeclaration(className)
            .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword)));
        var basicCreation = ObjectCreationExpression(IdentifierName(className));

        //TODO: number parsing.

        if (HasInterface(type, "System.ISpanParsable<TSelf>"))
        {
            creation = basicCreation.WithArgumentList(ArgumentList());
            return basicClass.WithParameterList(ParameterList()).WithBaseList(BaseList(
            [
                SimpleBaseType(
                    GenericName(Identifier("global::ArchiToolkit.InterpolatedParser.Parsers.SpanParseableParser"))
                        .WithTypeArgumentList(TypeArgumentList([IdentifierName(type.GetFullMetadataName(true))])))
            ]));
        }

        if (HasInterface(type, "System.IParsable<TSelf>"))
        {
            creation = basicCreation.WithArgumentList(ArgumentList());
            return basicClass.WithParameterList(ParameterList()).WithBaseList(BaseList(
            [
                SimpleBaseType(
                    GenericName(Identifier("global::ArchiToolkit.InterpolatedParser.Parsers.StringParseableParser"))
                        .WithTypeArgumentList(TypeArgumentList([IdentifierName(type.GetFullMetadataName(true))])))
            ]));
        }

        //TODO: reflection parsing.

        creation = basicCreation.WithArgumentList(ArgumentList());
        return null;
    }
}