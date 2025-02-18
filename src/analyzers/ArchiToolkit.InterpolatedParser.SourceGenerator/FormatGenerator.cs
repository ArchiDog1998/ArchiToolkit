namespace ArchiToolkit.InterpolatedParser.SourceGenerator;

[Generator(LanguageNames.CSharp)]
public partial class FormatGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var methodInvocations = context.SyntaxProvider
            .CreateSyntaxProvider(Predicate, TransForm).Collect();

        context.RegisterSourceOutput(methodInvocations, Generate);
    }

    private static bool Predicate(SyntaxNode node, CancellationToken token)
    {
        if (node is not InvocationExpressionSyntax invocation) return false;
        return invocation.ArgumentList.Arguments.Count > 0;
    }

    private IEnumerable<TypeInfo> TransForm(GeneratorSyntaxContext context, CancellationToken token)
    {
        var model = context.SemanticModel;
        if (context.Node is not InvocationExpressionSyntax invocation) yield break;
        if (model.GetSymbolInfo(invocation).Symbol is not IMethodSymbol symbol) yield break;
        if (symbol.ContainingType.GetFullMetadataName() is not
            "ArchiToolkit.InterpolatedParser.InterpolatedParserExtensions") yield break;

        foreach (var arg in invocation.ArgumentList.Arguments)
        {
            if (arg.Expression is not InterpolatedStringExpressionSyntax interpolatedString) continue;
            foreach (var content in interpolatedString.Contents)
            {
                if (content is not InterpolationSyntax interpolation) continue;
                yield return model.GetTypeInfo(interpolation.Expression);
            }
        }
    }

    private static void Generate(SourceProductionContext context, ImmutableArray<IEnumerable<TypeInfo>> typeInfos)
    {
        ParseItem[] items =
        [
            ..
            from typeSymbol in typeInfos.SelectMany(infos => infos).ToImmutableHashSet()
                .Select(typeInfo => typeInfo.Type).OfType<ITypeSymbol>()
            let typeValue = GetCollectionElementType(typeSymbol)
            select new ParseItem(typeSymbol, typeValue)
        ];
        Generate(context, items);
        return;

        static ITypeSymbol? GetCollectionElementType(ITypeSymbol typeSymbol)
        {
            var iCollectionInterface = typeSymbol.AllInterfaces
                .FirstOrDefault(i => i.OriginalDefinition.ToString() == "System.Collections.Generic.ICollection<T>");
            return iCollectionInterface?.TypeArguments.FirstOrDefault();
        }
    }

    private static bool HasInterface(ITypeSymbol typeSymbol, string interfaceName)
    {
        return typeSymbol.AllInterfaces.Any(i => i.OriginalDefinition.ToString() == interfaceName);
    }

    private static void Generate(SourceProductionContext context, ParseItem[] items)
    {
        var validTypes = items
            .SelectMany(i => (ITypeSymbol?[]) [i.Type, i.SubType]).OfType<ITypeSymbol>()
            .Select(t =>
            {
                var result = Generate(context, t, out var className);
                return (result, t, className);
            })
            .Where(t => t is { result: true, t: not null })
            .ToImmutableDictionary<(bool, ITypeSymbol, string), ITypeSymbol, string>(
                i => i.Item2,
                i => i.Item3,
                SymbolEqualityComparer.Default);

        foreach (var item in items)
        {
            var metadataName = item.Type.GetMetadataName();
            var methodName = "Add_" + metadataName.HashName;
            var root = NamespaceDeclaration("ArchiToolkit.InterpolatedParser",
                    $"For adding the formatted type of {metadataName.TypeName}")
                .AddMembers(BasicStruct(metadataName.TypeName, methodName, item, validTypes));
            context.AddSource($"Format.{metadataName.SafeName}.g.cs", root.NodeToString());
        }
    }

    private static bool Generate(SourceProductionContext context, ITypeSymbol type, out string className)
    {
        var name = type.GetMetadataName();

        //context.AddSource($"Test.{name.SafeName}.g.cs",     string.Join("\n", type.AllInterfaces.Select(i => i.OriginalDefinition)));

        var classDeclaration = GetParserType(type, name, out className);
        if (classDeclaration == null) return false;

        var root = NamespaceDeclaration("ArchiToolkit.InterpolatedParser",
                $"For adding the formatted type of {name.TypeName}")
            .AddMembers(StructDeclaration("InterpolatedParseStringHandler")
                .WithModifiers(
                    TokenList(Token(SyntaxKind.InternalKeyword), Token(SyntaxKind.PartialKeyword)))
                .AddMembers(classDeclaration.WithAttributeLists([
                    GeneratedCodeAttribute(typeof(FormatGenerator)).AddAttributes(NonUserCodeAttribute())
                ]))
            );

        context.AddSource($"Parser.{name.SafeName}.g.cs", root.NodeToString());
        return true;
    }

    private static StructDeclarationSyntax BasicStruct(string typeName, string methodName, ParseItem item,
        IImmutableDictionary<ITypeSymbol, string> classNames)
    {
        var method1 = MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)), Identifier("AppendFormatted"))
            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
            .WithAttributeLists([MethodAttribute()])
            .WithParameterList(ParameterList([
                Parameter(Identifier("t")).WithModifiers(TokenList(Token(SyntaxKind.InKeyword)))
                    .WithType(IdentifierName(typeName)),
                Parameter(Identifier("format")).WithType(PredefinedType(Token(SyntaxKind.StringKeyword))),
                CallerNameParameter()
            ]))
            .WithExpressionBody(ArrowExpressionClause(InvocationExpression(IdentifierName(methodName))
                .WithArgumentList(ArgumentList(
                [
                    Argument(IdentifierName("t")).WithRefOrOutKeyword(Token(SyntaxKind.InKeyword)),
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
                    Argument(IdentifierName("t")).WithRefOrOutKeyword(Token(SyntaxKind.InKeyword)),
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
                ])), item, classNames);

        return StructDeclaration("InterpolatedParseStringHandler")
            .WithModifiers(
                TokenList(Token(SyntaxKind.InternalKeyword), Token(SyntaxKind.PartialKeyword)))
            .WithMembers([method1, method2, method3]);
    }

    private static MethodDeclarationSyntax ModifyMethod(MethodDeclarationSyntax method, ParseItem item,
        IImmutableDictionary<ITypeSymbol, string> classNames)
    {
        if (item.SubType is null)
            return method.WithExpressionBody(ArrowExpressionClause(
                    InvocationExpression(IdentifierName("AppendObject"))
                        .WithArgumentList(ArgumentList(
                            [
                                Argument(IdentifierName("t")).WithRefOrOutKeyword(Token(SyntaxKind.InKeyword)),
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
                            IdentifierName(item.Type.GetFullMetadataName(true)),
                            IdentifierName(item.SubType.GetFullMetadataName(true))
                        ])))
                    .WithArgumentList(ArgumentList(
                        [
                            Argument(IdentifierName("t")).WithRefOrOutKeyword(Token(SyntaxKind.InKeyword)),
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
            if (classNames.TryGetValue(type, out var name))
                return ObjectCreationExpression(IdentifierName(name))
                    .WithArgumentList(ArgumentList());

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

    private static AttributeListSyntax MethodAttribute()
    {
        return GeneratedCodeAttribute(typeof(FormatGenerator)).AddAttributes(NonUserCodeAttribute(),
            Attribute(IdentifierName("global::System.Runtime.CompilerServices.MethodImpl"))
                .WithArgumentList(AttributeArgumentList(
                [
                    AttributeArgument(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                        IdentifierName("global::System.Runtime.CompilerServices.MethodImplOptions"),
                        IdentifierName("AggressiveInlining")))
                ])));
    }

    private readonly struct ParseItem(ITypeSymbol type, ITypeSymbol? subType)
    {
        public ITypeSymbol Type => type;
        public ITypeSymbol? SubType => subType;
    }
}