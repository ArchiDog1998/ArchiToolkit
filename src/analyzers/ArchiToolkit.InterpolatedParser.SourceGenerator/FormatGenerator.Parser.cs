namespace ArchiToolkit.InterpolatedParser.SourceGenerator;

partial class FormatGenerator
{
    private static ClassDeclarationSyntax? GetParserType(ITypeSymbol type, MetadataName metadataName,
        out string className)
    {
        className = "Parser_" + metadataName.HashName;

        var basicClass = ClassDeclaration(className)
            .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword)));

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