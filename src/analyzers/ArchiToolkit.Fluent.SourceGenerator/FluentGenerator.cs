using System.Reflection;
using System.Text.RegularExpressions;
using TypeInfo = Microsoft.CodeAnalysis.TypeInfo;

namespace ArchiToolkit.Fluent.SourceGenerator;

[Generator(LanguageNames.CSharp)]
public class FluentGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(InitializationOutput);
        var methodInvocations = context.SyntaxProvider
            .CreateSyntaxProvider(Predicate, TransForm).Collect();

        context.RegisterSourceOutput(methodInvocations, Generate);
    }

    private static void InitializationOutput(IncrementalGeneratorPostInitializationContext context)
    {
        var root = NamespaceDeclaration("ArchiToolkit.Fluent")
            .AddMembers(GetClass().WithAttributeLists(
            [
                GeneratedCodeAttribute(typeof(FluentGenerator)).AddAttributes(NonUserCodeAttribute())
            ]));
        context.AddSource($"Major.g.cs", root.NodeToString());
    }

    private static bool Predicate(SyntaxNode node, CancellationToken token)
    {
        if (node is not InvocationExpressionSyntax invocation) return false;
        return invocation.ArgumentList.Arguments.Count > 0;
    }

    private static (IAssemblySymbol?, IEnumerable<TypeInfo>) TransForm(GeneratorSyntaxContext context,
        CancellationToken token)
    {
        var items = TransForm(context);
        var node = context.Node.Ancestors().OfType<BaseTypeDeclarationSyntax>().FirstOrDefault();
        if (node is null) return (null, items);
        return (context.SemanticModel.GetDeclaredSymbol(node, token)?.ContainingAssembly, items);
    }

    private static IEnumerable<TypeInfo> TransForm(GeneratorSyntaxContext context)
    {
        var model = context.SemanticModel;
        if (context.Node is not InvocationExpressionSyntax invocation) yield break;
        if (model.GetSymbolInfo(invocation).Symbol is not IMethodSymbol symbol) yield break;
        if (symbol.ContainingType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) is not
            "global::ArchiToolkit.Fluent.FluentExtensions") yield break;

        if (invocation.Expression is MemberAccessExpressionSyntax memberAccess)
        {
            yield return model.GetTypeInfo(memberAccess.Expression);
        }

        foreach (var arg in invocation.ArgumentList.Arguments)
        {
            yield return model.GetTypeInfo(arg.Expression);
        }
    }

    private static void Generate(SourceProductionContext context,
        ImmutableArray<(IAssemblySymbol?, IEnumerable<TypeInfo>)> types)
    {
        var assemblies = types.Select(i => i.Item1).FirstOrDefault(i => i is not null);
        foreach (var type in GetTypes(types).ToImmutableHashSet(SymbolEqualityComparer.Default).OfType<ITypeSymbol>())
        {
            Generate(context, type, assemblies);
        }

        return;

        static IEnumerable<ITypeSymbol> GetTypes(ImmutableArray<(IAssemblySymbol?, IEnumerable<TypeInfo>)> types)
        {
            foreach (var type in types.SelectMany(tps => tps.Item2))
            {
                if (type.Type is not { } typeSymbol) continue;
                if (typeSymbol.GetTypeName().FullName is "global::ArchiToolkit.Fluent.FluentType") continue;
                yield return typeSymbol;
            }
        }
    }

    private static void Generate(SourceProductionContext context, ITypeSymbol type, IAssemblySymbol? assembly)
    {
        var name = type.GetTypeName();
        var root = NamespaceDeclaration("ArchiToolkit.Fluent",
            $"For adding the fluent extensions of {name.FullName}").AddMembers(
            GetClass().AddMembers([
                ..type.GetMembers().SelectMany(member => GetMemberDeclarations(name.FullName, member, assembly))
            ]));

        context.AddSource($"Fluent.{name.SafeName}.g.cs", root.NodeToString());
    }

    private static IEnumerable<MemberDeclarationSyntax> GetMemberDeclarations(string typeName, ISymbol member,
        IAssemblySymbol? assembly)
    {
        if (member.IsStatic) yield break;
        switch (member)
        {
            case IPropertySymbol property:
            {
                var propType = property.Type.GetTypeName().FullName;
                var propName = property.Name;
                if (CanAccess(property.SetMethod, assembly))
                {
                    yield return SetPropertyDirect(typeName, propType, propName);
                    if (CanAccess(property.GetMethod, assembly))
                    {
                        yield return SetPropertyInvoke(typeName, propType, propName);
                    }
                }

                break;
            }
            case IFieldSymbol field when CanAccess(field, assembly):
            {
                var fieldType = field.Type.GetTypeName().FullName;
                var fieldName = field.Name;
                yield return SetPropertyDirect(typeName, fieldType, fieldName);
                yield return SetPropertyInvoke(typeName, fieldType, fieldName);
                break;
            }
            case IMethodSymbol method:
                break;
        }
    }

    private const string Fluent = "global::ArchiToolkit.Fluent.Fluent",
        ModifyDelegate = "global::ArchiToolkit.Fluent.ModifyDelegate";

    private static MethodDeclarationSyntax SetPropertyDirect(string typeName, string propertyType, string propertyName)
    {
        return MethodDeclaration(GenericName(Identifier(Fluent))
                    .WithTypeArgumentList(TypeArgumentList([IdentifierName(typeName)])),
                Identifier("With" + propertyName))
            .WithModifiers(
                TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)))
            .WithParameterList(ParameterList(
            [
                Parameter(Identifier("fluent")).WithModifiers(TokenList(Token(SyntaxKind.ThisKeyword)))
                    .WithType(GenericName(Identifier(Fluent))
                        .WithTypeArgumentList(
                            TypeArgumentList(SingletonSeparatedList<TypeSyntax>(IdentifierName(typeName))))),
                Parameter(Identifier("value")).WithType(IdentifierName(propertyType))
            ]))
            .WithBody(Block(ReturnStatement(InvocationExpression(
                        MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName("fluent"),
                            IdentifierName("AddProperty")))
                    .WithArgumentList(ArgumentList([Argument(IdentifierName("Modify"))]))),
                LocalFunctionStatement(PredefinedType(Token(SyntaxKind.VoidKeyword)), Identifier("Modify"))
                    .WithParameterList(ParameterList(
                    [
                        Parameter(Identifier("data")).WithModifiers(TokenList(Token(SyntaxKind.RefKeyword)))
                            .WithType(IdentifierName(typeName))
                    ]))
                    .WithBody(Block(ExpressionStatement(
                        AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                            MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName("data"),
                                IdentifierName(propertyName)),
                            IdentifierName("value")))))))
            .WithAttributeLists(
            [
                GeneratedCodeAttribute(typeof(FluentGenerator)).AddAttributes(NonUserCodeAttribute())
            ])
            .WithXmlComment(
                $$"""
                  /// <summary>
                  ///     Set the value <see cref="{{typeName}}.{{propertyName}}" /> in <see cref="{{typeName}}" />
                  ///     <para>
                  ///         <inheritdoc cref="{{typeName}}.{{propertyName}}" />
                  ///     </para>
                  /// </summary>
                  /// <param name="fluent">Self</param>
                  /// <param name="value">The value to input</param>
                  /// <returns>Self</returns>
                  """);
    }

    private static MethodDeclarationSyntax SetPropertyInvoke(string typeName, string propertyType, string propertyName)
    {
        return MethodDeclaration(GenericName(Identifier(Fluent))
                .WithTypeArgumentList(TypeArgumentList([IdentifierName(typeName)])), Identifier("With" + propertyName))
            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)))
            .WithParameterList(ParameterList(
            [
                Parameter(Identifier("fluent")).WithModifiers(TokenList(Token(SyntaxKind.ThisKeyword)))
                    .WithType(GenericName(Identifier(Fluent))
                        .WithTypeArgumentList(TypeArgumentList([IdentifierName(typeName)]))),
                Parameter(Identifier("modifyValue"))
                    .WithType(GenericName(Identifier(ModifyDelegate)).WithTypeArgumentList(
                        TypeArgumentList([IdentifierName(propertyType)])))
            ]))
            .WithBody(Block(ReturnStatement(InvocationExpression(
                        MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName("fluent"),
                            IdentifierName("AddProperty")))
                    .WithArgumentList(ArgumentList([Argument(IdentifierName("Modify"))]))),
                LocalFunctionStatement(PredefinedType(Token(SyntaxKind.VoidKeyword)), Identifier("Modify"))
                    .WithParameterList(ParameterList(
                    [
                        Parameter(Identifier("data"))
                            .WithModifiers(TokenList(Token(SyntaxKind.RefKeyword)))
                            .WithType(IdentifierName(typeName))
                    ]))
                    .WithBody(Block(ExpressionStatement(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                        MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName("data"),
                            IdentifierName(propertyName)),
                        InvocationExpression(IdentifierName("modifyValue"))
                            .WithArgumentList(ArgumentList(
                            [
                                Argument(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName("data"), IdentifierName(propertyName)))
                            ]))))))))
            .WithAttributeLists(
            [
                GeneratedCodeAttribute(typeof(FluentGenerator)).AddAttributes(NonUserCodeAttribute())
            ])
            .WithXmlComment($$"""
                            /// <summary>
                            ///     <inheritdoc cref="With{{propertyName}}(ArchiToolkit.Fluent.Fluent{{{typeName}}},{{propertyType}})" />
                            /// </summary>
                            /// <param name="fluent">Self</param>
                            /// <param name="modifyValue">The method to modify it</param>
                            /// <returns>Self</returns>
                            """);
    }

    private static bool CanAccess(ISymbol? symbol, IAssemblySymbol? assembly)
    {
        if (symbol is null) return false;
        var access = symbol.DeclaredAccessibility;
        if (access == Accessibility.Public) return true;
        if (assembly is not null && symbol.ContainingAssembly.Equals(assembly, SymbolEqualityComparer.Default))
        {
            if (access is Accessibility.Internal or Accessibility.ProtectedOrInternal) return true;
        }

        return false;
    }

    private static ClassDeclarationSyntax GetClass()
    {
        return ClassDeclaration("FluentObjectsExtensions")
            .WithModifiers(TokenList(
                Token(SyntaxKind.InternalKeyword), Token(SyntaxKind.StaticKeyword), Token(SyntaxKind.PartialKeyword)));
    }
}