﻿namespace ArchiToolkit.InterpolatedParser.SourceGenerator;

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
            .SelectMany(i => (ITypeSymbol?[]) [i.Type, i.SubType])
            .ToImmutableHashSet(SymbolEqualityComparer.Default)
            .OfType<ITypeSymbol>()
            .Select(t =>
            {
                var result = Generate(context, t, out var className);
                return (result, t, className);
            })
            .Where(t => t is { result: true, t: not null })
            .ToImmutableDictionary<(bool, ITypeSymbol, ObjectCreationExpressionSyntax), ITypeSymbol, ObjectCreationExpressionSyntax>(
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

    private static bool Generate(SourceProductionContext context, ITypeSymbol type, out ObjectCreationExpressionSyntax creation)
    {
        var name = type.GetMetadataName();
        var classDeclaration = GetParserType(type, name, out creation);
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

    private static AttributeListSyntax MethodAttribute()
    {
        return GeneratedCodeAttribute(typeof(FormatGenerator)).AddAttributes(NonUserCodeAttribute());
    }

    private readonly struct ParseItem(ITypeSymbol type, ITypeSymbol? subType)
    {
        public ITypeSymbol Type => type;
        public ITypeSymbol? SubType => subType;
    }
}