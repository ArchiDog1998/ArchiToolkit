using System.Collections.Immutable;
using ArchiToolkit.RoslynHelper.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static ArchiToolkit.RoslynHelper.Extensions.SyntaxExtensions;

namespace ArchiToolkit.ValidResults.SourceGenerator;

[Generator(LanguageNames.CSharp)]
public sealed class ValidResultsGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classes = context.SyntaxProvider.CreateSyntaxProvider(
            static (s, _) => s is BaseTypeDeclarationSyntax,
            static (c, token) =>
                ModelExtensions.GetDeclaredSymbol(c.SemanticModel, (BaseTypeDeclarationSyntax)c.Node, token));

        context.RegisterSourceOutput(classes.Collect(), Generate);
    }

    private static void Generate(SourceProductionContext context, ImmutableArray<ISymbol?> sources)
    {
        var dictionary = GetClassesSymbols(sources);
        foreach (var pair in dictionary)
        {
            if (pair.Key is not INamedTypeSymbol data) continue;
            GenerateItem(context, dictionary, pair.Value, data);
        }
    }

    private static void GenerateItem(SourceProductionContext context, Dictionary<ISymbol?, INamedTypeSymbol> dictionary,
        INamedTypeSymbol target, INamedTypeSymbol data)
    {
        var members = data
            .GetMembers()
            .Where(p => p.DeclaredAccessibility is Accessibility.Public)
            .ToArray();

        if (members.OfType<IPropertySymbol>().Any(m => m.Name is "Value")) return;
        if (members.OfType<IFieldSymbol>().Any(m => m.Name is "Value")) return;

        var baseType = TypeHelper.FindValidResultType(dictionary, data, out var addDisposed);
        var trackerName = target.Name + "Tracker";

        IEnumerable<BaseTypeSyntax> baseTypes = [SimpleBaseType(baseType)];
        if (addDisposed)
        {
            baseTypes = baseTypes.Append(SimpleBaseType(IdentifierName("global::System.IDisposable")));
        }

        IEnumerable<MemberDeclarationSyntax> disposeableMembers = addDisposed
            ?
            [
                MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)), Identifier("Dispose"))
                    .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                    .WithAttributeLists([
                        GeneratedCodeAttribute(typeof(ValidResultsGenerator)).AddAttributes(NonUserCodeAttribute())
                    ])
                    .WithBody(Block(
                        ExpressionStatement(ConditionalAccessExpression(IdentifierName("ValueOrDefault"),
                            InvocationExpression(MemberBindingExpression(IdentifierName("Dispose")))))))
            ]
            : [];

        var node = NamespaceDeclaration(target.ContainingNamespace.ToDisplayString())
            .WithMembers(
            [
                ClassDeclaration(target.Name).WithModifiers(TokenList(Token(SyntaxKind.PartialKeyword)))
                    .WithBaseList(BaseList([..baseTypes]))
                    .WithMembers([
                        ..CreatorMembers(target.Name, data),
                        ..GenerateMembers(members, dictionary, trackerName),
                        ..disposeableMembers,
                    ]),
                ClassDeclaration(target.Name + "Extensions")
                    .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)))
                    .WithAttributeLists([GeneratedCodeAttribute(typeof(ValidResultsGenerator))])
                    .WithMembers([
                        GenerateCreateTracker(target),
                        GenerateCreateTracker(data),
                        MethodDeclaration(IdentifierName(target.Name), Identifier("ToValidResult"))
                            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)))
                            .WithAttributeLists([
                                GeneratedCodeAttribute(typeof(ValidResultsGenerator))
                                    .AddAttributes(NonUserCodeAttribute(), PureAttribute())
                            ])
                            .WithParameterList(ParameterList(
                            [
                                Parameter(Identifier("value"))
                                    .WithModifiers(TokenList(Token(SyntaxKind.ThisKeyword)))
                                    .WithType(IdentifierName(data.GetName().FullName))
                            ]))
                            .WithExpressionBody(ArrowExpressionClause(IdentifierName("value")))
                            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                        ..GenerateStaticMembers(members, dictionary, trackerName),
                    ]),
                ClassDeclaration(trackerName)
                    .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                    .WithAttributeLists([GeneratedCodeAttribute(typeof(ValidResultsGenerator))])
                    .WithBaseList(BaseList([
                        SimpleBaseType(
                            GenericName(Identifier("global::ArchiToolkit.ValidResults.ResultTracker"))
                                .WithTypeArgumentList(TypeArgumentList([baseType])))
                    ]))
                    .WithMembers([
                        ConstructorDeclaration(Identifier(trackerName))
                            .WithModifiers(TokenList(Token(SyntaxKind.InternalKeyword)))
                            .WithAttributeLists([
                                GeneratedCodeAttribute(typeof(ValidResultsGenerator))
                                    .AddAttributes(NonUserCodeAttribute())
                            ])
                            .WithParameterList(ParameterList(
                            [
                                Parameter(Identifier("value")).WithType(baseType),
                                Parameter(Identifier("callerInfo"))
                                    .WithType(PredefinedType(Token(SyntaxKind.StringKeyword)))
                            ]))
                            .WithInitializer(ConstructorInitializer(SyntaxKind.BaseConstructorInitializer,
                                ArgumentList(
                                [
                                    Argument(IdentifierName("value")),
                                    Argument(IdentifierName("callerInfo"))
                                ])))
                            .WithBody(Block()),
                        ..GenerateStaticOperatorMembers(members, dictionary, trackerName),
                    ])
            ]);
        context.AddSource(target.Name + ".g.cs", node.NodeToString());
        return;

        MethodDeclarationSyntax GenerateCreateTracker(ITypeSymbol type)
        {
            return MethodDeclaration(IdentifierName(trackerName),
                    Identifier("Track"))
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)))
                .WithAttributeLists([
                    GeneratedCodeAttribute(typeof(ValidResultsGenerator))
                        .AddAttributes(NonUserCodeAttribute(), PureAttribute())
                ])
                .WithParameterList(ParameterList([
                    Parameter(Identifier("value"))
                        .WithType(
                            IdentifierName(type.GetName().FullName)),
                    ..MethodParametersHelper.GenerateCallersSyntax(["value"])
                ]))
                .WithBody(Block(ReturnStatement(ObjectCreationExpression(
                        IdentifierName(trackerName))
                    .WithArgumentList(ArgumentList(
                    [
                        Argument(IdentifierName("value")),
                        Argument(InvocationExpression(MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                IdentifierName("global::ArchiToolkit.ValidResults.ValidResultsExtensions"),
                                IdentifierName("GetCallerInfo")))
                            .WithArgumentList(ArgumentList(
                            [
                                Argument(IdentifierName("valueName")),
                                Argument(IdentifierName("_filePath")),
                                Argument(IdentifierName("_fileLineNumber"))
                            ])))
                    ])))
                ));
        }
    }

    private static IEnumerable<MemberDeclarationSyntax> GenerateStaticOperatorMembers(
        IReadOnlyCollection<ISymbol> members,
        Dictionary<ISymbol?, INamedTypeSymbol> dictionary, string trackerName)
    {
        foreach (var method in members
                     .OfType<IMethodSymbol>()
                     .Where(p => !p.ReturnType.IsRefLikeType)
                     .Where(p => p.IsStatic)
                     .Where(p => p.MethodKind is MethodKind.UserDefinedOperator))
        {
            yield return StaticOrdinaryMethod(method, dictionary, MethodParametersHelper.MethodType.Operator,
                trackerName);
        }
    }

    private static IEnumerable<MemberDeclarationSyntax> GenerateStaticMembers(IReadOnlyCollection<ISymbol> members,
        Dictionary<ISymbol?, INamedTypeSymbol> dictionary, string trackerName)
    {
        foreach (var method in members
                     .OfType<IMethodSymbol>()
                     .Where(p => !p.ReturnType.IsRefLikeType)
                     .Where(p => !p.IsStatic))
        {
            if (method.MethodKind is MethodKind.Ordinary)
            {
                yield return OrdinaryMethod(method, dictionary, trackerName);
            }
        }
    }

    private static IEnumerable<MemberDeclarationSyntax> GenerateMembers(IReadOnlyCollection<ISymbol> members,
        Dictionary<ISymbol?, INamedTypeSymbol> dictionary, string trackerName)
    {
        foreach (var property in members
                     .OfType<IPropertySymbol>()
                     .Where(p => !p.IsStatic)
                     .Where(p => !p.Type.IsRefLikeType))
        {
            yield return GenerateProperty(property.Type, property.Name, property.ContainingType,
                dictionary, property.GetMethod is not null, property.SetMethod is not null);
        }

        foreach (var field in members
                     .OfType<IFieldSymbol>()
                     .Where(p => !p.IsStatic)
                     .Where(p => !p.Type.IsRefLikeType))
        {
            yield return GenerateProperty(field.Type, field.Name, field.ContainingType,
                dictionary, true, true);
        }

        foreach (var method in members
                     .OfType<IMethodSymbol>()
                     .Where(p => !p.ReturnType.IsRefLikeType)
                     .Where(p => p.IsStatic))
        {
            if (method.MethodKind is MethodKind.Ordinary)
            {
                yield return StaticOrdinaryMethod(method, dictionary, MethodParametersHelper.MethodType.Static,
                    trackerName);
            }
        }
    }

    private static MemberDeclarationSyntax OrdinaryMethod(IMethodSymbol method,
        Dictionary<ISymbol?, INamedTypeSymbol> dictionary, string trackerName)
    {
        return MethodParametersHelper.GenerateMethodByParameters(method,
        [
            new ParameterRelay(method.ContainingType, "self", RefKind.None),
            ..method.Parameters.Select(p => new ParameterRelay(p))
        ], dictionary, MethodParametersHelper.MethodType.Instance, trackerName);
    }

    private static MemberDeclarationSyntax StaticOrdinaryMethod(IMethodSymbol method,
        Dictionary<ISymbol?, INamedTypeSymbol> dictionary, MethodParametersHelper.MethodType type, string trackerName)
    {
        return MethodParametersHelper.GenerateMethodByParameters(method,
            [..method.Parameters.Select(p => new ParameterRelay(p))],
            dictionary, type, trackerName);
    }

    private static PropertyDeclarationSyntax GenerateProperty(ITypeSymbol propertyType, string propertyName,
        ITypeSymbol declarationType,
        Dictionary<ISymbol?, INamedTypeSymbol> dictionary, bool hasGetter, bool hasSetter)
    {
        List<AccessorDeclarationSyntax> accessors = new(2);

        if (hasGetter)
        {
            accessors.Add(AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                .WithExpressionBody(ArrowExpressionClause(InvocationExpression(
                        IdentifierName("GetProperty"))
                    .WithArgumentList(ArgumentList(
                    [
                        Argument(ParenthesizedLambdaExpression()
                            .WithExpressionBody(MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression, IdentifierName("Value"),
                                IdentifierName(propertyName))))
                    ]))))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)));
        }

        if (hasSetter)
        {
            accessors.Add(AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                .WithExpressionBody(ArrowExpressionClause(InvocationExpression(
                        IdentifierName("SetProperty"))
                    .WithArgumentList(ArgumentList(
                    [
                        Argument(IdentifierName("value")),
                        Argument(SimpleLambdaExpression(Parameter(Identifier("v")))
                            .WithExpressionBody(AssignmentExpression(
                                SyntaxKind.SimpleAssignmentExpression,
                                MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName("Value"),
                                    IdentifierName(propertyName)), IdentifierName("v"))))
                    ]))))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)));
        }

        return PropertyDeclaration(TypeHelper.FindValidResultType(dictionary, propertyType, out _), Identifier(propertyName))
            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
            .WithAttributeLists([
                GeneratedCodeAttribute(typeof(ValidResultsGenerator)).AddAttributes(NonUserCodeAttribute())
            ])
            .WithXmlComment(
                $"/// <inheritdoc cref=\"{declarationType.GetName().SummaryName}.{propertyName}\"/>")
            .WithAccessorList(AccessorList([..accessors]));
    }

    private static IEnumerable<MemberDeclarationSyntax> CreatorMembers(string className, INamedTypeSymbol dataType)
    {
        yield return ConstructorDeclaration(Identifier(className))
            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
            .WithAttributeLists([
                GeneratedCodeAttribute(typeof(ValidResultsGenerator)).AddAttributes(NonUserCodeAttribute())
            ])
            .WithParameterList(ParameterList(
            [
                Parameter(Identifier("data")).WithType(IdentifierName("Data"))
            ]))
            .WithInitializer(ConstructorInitializer(SyntaxKind.BaseConstructorInitializer,
                ArgumentList([
                    Argument(IdentifierName("data"))
                ])))
            .WithBody(Block());

        yield return ConversionOperatorDeclaration(Token(SyntaxKind.ImplicitKeyword),
                IdentifierName(className))
            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)))
            .WithAttributeLists([
                GeneratedCodeAttribute(typeof(ValidResultsGenerator))
                    .AddAttributes(NonUserCodeAttribute(), PureAttribute())
            ])
            .WithParameterList(ParameterList(
            [
                Parameter(Identifier("data"))
                    .WithType(IdentifierName("Data"))
            ]))
            .WithExpressionBody(ArrowExpressionClause(ImplicitObjectCreationExpression()
                .WithArgumentList(ArgumentList(
                [
                    Argument(IdentifierName("data"))
                ]))))
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

        yield return ConversionOperatorDeclaration(Token(SyntaxKind.ImplicitKeyword),
                IdentifierName(className))
            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)))
            .WithAttributeLists([
                GeneratedCodeAttribute(typeof(ValidResultsGenerator))
                    .AddAttributes(NonUserCodeAttribute(), PureAttribute())
            ])
            .WithParameterList(ParameterList(
            [
                Parameter(Identifier("value")).WithType(IdentifierName(dataType.GetName().FullName))
            ]))
            .WithExpressionBody(ArrowExpressionClause(InvocationExpression(
                    MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                        IdentifierName("Data"), IdentifierName("Ok")))
                .WithArgumentList(ArgumentList(
                [
                    Argument(IdentifierName("value"))
                ]))))
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
    }

    private static Dictionary<ISymbol?, INamedTypeSymbol> GetClassesSymbols(IEnumerable<ISymbol?> symbols)
    {
        return symbols
            .OfType<INamedTypeSymbol>()
            .Select(s => (s, s
                .GetAttributes()
                .Select(attribute => attribute.AttributeClass)
                .OfType<INamedTypeSymbol>()
                .Where(symbol => symbol.IsGenericType)
                .Where(symbol => symbol.ConstructedFrom.GetName().FullName
                    is "global::ArchiToolkit.ValidResults.GenerateValidResultAttribute<T>")
                .Select(symbol => symbol.TypeArguments.FirstOrDefault())
                .OfType<INamedTypeSymbol>()
                .FirstOrDefault()))
            .Where(i => i.Item2 is not null)
            .ToDictionary(i => i.Item2, i => i.s, SymbolEqualityComparer.Default);
    }
}