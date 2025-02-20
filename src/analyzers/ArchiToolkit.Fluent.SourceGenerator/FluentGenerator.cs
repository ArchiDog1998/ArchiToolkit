using ArchiToolkit.RoslynHelper.Extensions;
using ArchiToolkit.RoslynHelper.Names;
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
        context.AddSource("_Major.g.cs", root.NodeToString());
    }

    private static bool Predicate(SyntaxNode node, CancellationToken token)
    {
        if (node is not InvocationExpressionSyntax invocation) return false;
        return invocation.ArgumentList.Arguments.Count >= 0;
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
                typeSymbol = typeSymbol.OriginalDefinition;
                if (typeSymbol.GetName().FullName is "global::ArchiToolkit.Fluent.FluentType"
                    or "global::ArchiToolkit.Fluent.Fluent<TTarget>") continue;
                yield return typeSymbol;
            }
        }
    }

    private static void Generate(SourceProductionContext context, ITypeSymbol type, IAssemblySymbol? assembly)
    {
        var name = type.GetName();
        var root = NamespaceDeclaration("ArchiToolkit.Fluent",
            $"For adding the fluent extensions of {name.FullName}").AddMembers(
            GetClass().AddMembers([
                ..type.GetMembers().SelectMany(member => GetMemberDeclarations(name, member, assembly))
            ]));

        context.AddSource($"{name.SafeName}.g.cs", root.NodeToString());
    }

    private static IEnumerable<MemberDeclarationSyntax> GetMemberDeclarations(TypeName typeName, ISymbol member,
        IAssemblySymbol? assembly)
    {
        if (member.IsStatic) yield break;
        switch (member)
        {
            case IPropertySymbol property:
            {
                var propType = property.Type.GetName().FullName;
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
                var fieldType = field.Type.GetName().FullName;
                var fieldName = field.Name;
                yield return SetPropertyDirect(typeName, fieldType, fieldName);
                yield return SetPropertyInvoke(typeName, fieldType, fieldName);
                break;
            }
            case IMethodSymbol method when CanAccess(method, assembly) && method.MethodKind == MethodKind.Ordinary:
                yield return Invoke(method.GetName());
                break;
        }
    }

    private static TypeSyntax? GetReturnType(MethodName method)
    {
        var tuples = method.Parameters.Where(p => p.IsOut)
            .Select(p => TupleElement(IdentifierName(p.Type.FullName)).WithIdentifier(Identifier(p.Name))).ToArray();
        var isVoid = method.ReturnType.Symbol.SpecialType == SpecialType.System_Void;

        if (tuples.Length is 0)
        {
            if (isVoid) return null;
            return IdentifierName(method.ReturnType.FullName);
        }
        else
        {
            if (isVoid) return TupleType([..tuples]);
            return TupleType([
                TupleElement(IdentifierName(method.ReturnType.FullName)).WithIdentifier(Identifier("result")),
                ..tuples
            ]);
        }
    }

    private static MethodDeclarationSyntax Invoke(MethodName method)
    {
        var returnType = GetReturnType(method);
        var inParameters = method.Parameters.Where(p => p.IsIn).ToArray();

        var types = new List<TypeSyntax>(2) { IdentifierName(method.ContainingType.FullName) };
        if (returnType is not null) types.Add(returnType);

        var invocation = InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                IdentifierName("data"),
                IdentifierName(method.Name)))
            .WithArgumentList(
                ArgumentList(
                [
                    ..method.Parameters.Select(n =>
                    {
                        return n.Symbol.RefKind switch
                        {
                            RefKind.Ref => Argument(IdentifierName(n.Name))
                                .WithRefOrOutKeyword(Token(SyntaxKind.RefKeyword)),
                            RefKind.Out => Argument(DeclarationExpression(IdentifierName("var"),
                                    SingleVariableDesignation(Identifier(n.Name))))
                                .WithRefOrOutKeyword(Token(SyntaxKind.OutKeyword)),
                            _ => Argument(IdentifierName(n.Name))
                        };
                    })
                ]));

        List<StatementSyntax> statements = [];
        if (returnType is null)
        {
            statements.Add(ExpressionStatement(invocation));
        }
        else
        {
            var names = method.Parameters.Where(p => p.IsOut).Select(i => i.Name).ToList();
            if (method.ReturnType.Symbol.SpecialType == SpecialType.System_Void)
            {
                statements.Add(ExpressionStatement(invocation));
            }
            else
            {
                statements.Add(LocalDeclarationStatement(VariableDeclaration(IdentifierName("var"))
                    .WithVariables([VariableDeclarator(Identifier("result")).WithInitializer(EqualsValueClause(invocation))])));
                names.Insert(0, "result");
            }
            statements.Add(ReturnStatement(TupleExpression([..names.Select(n => Argument(IdentifierName(n)))])));
        }
        var summary = method.ContainingType.SummaryName + "." + method.SummaryName;

        return MethodDeclaration(GenericName(Identifier("DoResult"))
                    .WithTypeArgumentList(TypeArgumentList([..types])),
                Identifier("Do" + method.Name))
            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)))
            .AddAttributes()
            .WithParameterList(
                ParameterList(
                [
                    Parameter(Identifier("fluent")).WithModifiers(TokenList(Token(SyntaxKind.ThisKeyword)))
                        .WithType(GenericName(Identifier(Fluent))
                            .WithTypeArgumentList(TypeArgumentList([IdentifierName(method.ContainingType.FullName)]))),
                    ..inParameters.Select(n =>
                        Parameter(Identifier(n.Name)).WithType(IdentifierName(n.Type.FullName)))
                ]))
            .WithBody(Block(ReturnStatement(InvocationExpression(MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression, IdentifierName("fluent"),
                        IdentifierName("InvokeMethod")))
                    .WithArgumentList(ArgumentList([Argument(IdentifierName("Invoke"))]))),
                LocalFunctionStatement(returnType ?? PredefinedType(Token(SyntaxKind.VoidKeyword)),
                        Identifier("Invoke"))
                    .WithParameterList(ParameterList([
                        Parameter(Identifier("data")).WithModifiers(
                                TokenList(Token(SyntaxKind.RefKeyword)))
                            .WithType(IdentifierName(method.ContainingType.FullName))
                    ]))
                    .WithBody(Block(statements))))
            .WithXmlComment(
                $$"""
                  /// <summary>
                  ///     Invoke the method <see cref="{{summary}}" /> in <see cref="{{method.ContainingType.SummaryName}}" />
                  ///     <para>
                  ///         <inheritdoc cref="{{summary}}" />
                  ///     </para>
                  /// </summary>
                  /// <param name="fluent">Self</param>
                  /// {{string.Join("\n/// ", inParameters.Select(p => $"<param name=\"{p.Name}\"><inheritdoc cref=\"{summary}\"/></param>"))}}
                  /// <returns>Self</returns>
                  """);

    }

    private const string Fluent = "global::ArchiToolkit.Fluent.Fluent",
        ModifyDelegate = "global::ArchiToolkit.Fluent.ModifyDelegate";

    private static MethodDeclarationSyntax SetPropertyDirect(TypeName typeName, string propertyType,
        string propertyName)
    {
        var parameter = Parameter(Identifier("value")).WithType(IdentifierName(propertyType));
        var statement = ExpressionStatement(
            AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName("data"),
                    IdentifierName(propertyName)),
                IdentifierName("value")));
        const string paramSummary = "<param name=\"value\">The value to input</param>";

        return SetProperty(typeName, propertyName, parameter, paramSummary, statement);
    }

    private static MethodDeclarationSyntax SetPropertyInvoke(TypeName typeName, string propertyType,
        string propertyName)
    {
        var parameter = Parameter(Identifier("modifyValue"))
            .WithType(GenericName(Identifier(ModifyDelegate)).WithTypeArgumentList(
                TypeArgumentList([IdentifierName(propertyType)])));

        var statement = ExpressionStatement(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
            MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName("data"),
                IdentifierName(propertyName)),
            InvocationExpression(IdentifierName("modifyValue"))
                .WithArgumentList(ArgumentList(
                [
                    Argument(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                        IdentifierName("data"), IdentifierName(propertyName)))
                ]))));

        const string paramSummary = "<param name=\"modifyValue\">The method to modify it</param>";

        return SetProperty(typeName, propertyName, parameter, paramSummary, statement);
    }

    private static MethodDeclarationSyntax SetProperty(TypeName typeName,
        string propertyName, ParameterSyntax parameter, string parameterSummary, params StatementSyntax[] statements)
    {
        return MethodDeclaration(GenericName(Identifier(Fluent))
                    .WithTypeArgumentList(TypeArgumentList([IdentifierName(typeName.FullName)])),
                Identifier("With" + propertyName))
            .WithModifiers(
                TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword)))
            .AddTypeParameters(typeName)
            .WithParameterList(ParameterList(
            [
                Parameter(Identifier("fluent")).WithModifiers(TokenList(Token(SyntaxKind.ThisKeyword)))
                    .WithType(GenericName(Identifier(Fluent)).WithTypeArgumentList(
                        TypeArgumentList(SingletonSeparatedList<TypeSyntax>(IdentifierName(typeName.FullName))))),
                parameter
            ]))
            .WithBody(Block(ReturnStatement(InvocationExpression(
                        MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName("fluent"),
                            IdentifierName("AddProperty")))
                    .WithArgumentList(ArgumentList([Argument(IdentifierName("Modify"))]))),
                LocalFunctionStatement(PredefinedType(Token(SyntaxKind.VoidKeyword)), Identifier("Modify"))
                    .WithParameterList(ParameterList(
                    [
                        Parameter(Identifier("data")).WithModifiers(TokenList(Token(SyntaxKind.RefKeyword)))
                            .WithType(IdentifierName(typeName.FullName))
                    ]))
                    .WithBody(Block(statements))))
            .AddAttributes()
            .WithXmlComment($$"""
                              /// <summary>
                              ///     Set the value <see cref="{{typeName.SummaryName}}.{{propertyName}}" /> in <see cref="{{typeName.SummaryName}}" />
                              ///     <para>
                              ///         <inheritdoc cref="{{typeName.SummaryName}}.{{propertyName}}" />
                              ///     </para>
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