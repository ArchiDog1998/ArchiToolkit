namespace ArchiToolkit.InterpolatedParser.SourceGenerator;

partial class FormatGenerator
{
    private static ClassDeclarationSyntax? GetParserType(ITypeSymbol type, MetadataName metadataName,
        out ObjectCreationExpressionSyntax creation)
    {
        var className = "Parser_" + metadataName.HashName;

        var typeName = type.GetFullMetadataName(true);
        var basicClass = ClassDeclaration(className)
            .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword)))
            .WithParameterList(ParameterList());
        creation = ObjectCreationExpression(IdentifierName(className))
            .WithArgumentList(ArgumentList());

        //TODO: number parsing.

        if (HasStaticMethod(type, "Parse", "System.ReadOnlySpan<System.Char>", "System.IFormatProvider")
            && HasStaticMethod(type, "Parse", "System.ReadOnlySpan<System.Char>", "System.Globalization.NumberStyles", "System.IFormatProvider")
            && HasStaticMethod(type, "TryParse", "System.ReadOnlySpan<System.Char>", "System.IFormatProvider")
            && HasStaticMethod(type, "TryParse", "System.ReadOnlySpan<System.Char>", "System.Globalization.NumberStyles", "System.IFormatProvider"))
        {
            //return basicClass;
        }

        if (HasInterface(type, "System.ISpanParsable<TSelf>"))
        {
            return basicClass.WithBaseList(BaseList(
            [
                SimpleBaseType(
                    GenericName(Identifier("global::ArchiToolkit.InterpolatedParser.Parsers.SpanParseableParser"))
                        .WithTypeArgumentList(TypeArgumentList([IdentifierName(typeName)])))
            ]));
        }

        if (HasInterface(type, "System.IParsable<TSelf>"))
        {
            return basicClass.WithBaseList(BaseList(
            [
                SimpleBaseType(
                    GenericName(Identifier("global::ArchiToolkit.InterpolatedParser.Parsers.StringParseableParser"))
                        .WithTypeArgumentList(TypeArgumentList([IdentifierName(typeName)])))
            ]));
        }

        //TODO: reflection parsing.

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
                return !argumentsName
                    .Where((t, i) => method.Parameters[i].Type.GetFullMetadataName() != t).Any();
            });
    }
}