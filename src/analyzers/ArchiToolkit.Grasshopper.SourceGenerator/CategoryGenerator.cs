using ArchiToolkit.RoslynHelper.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static ArchiToolkit.RoslynHelper.Extensions.SyntaxExtensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;


namespace ArchiToolkit.Grasshopper.SourceGenerator;

public static class CategoryGenerator
{
    public static void GenerateIcons(SourceProductionContext context, IAssemblySymbol assembly,
        IEnumerable<string> icons)
    {
        var addIcon = MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)), Identifier("LoadIcon"))
            .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.StaticKeyword)))
            .WithParameterList(ParameterList(
            [
                Parameter(Identifier("iconName")).WithType(PredefinedType(Token(SyntaxKind.StringKeyword)))
            ]))
            .WithAttributeLists([
                GeneratedCodeAttribute(typeof(CategoryGenerator))
                    .AddAttributes(NonUserCodeAttribute())
            ])
            .WithBody(Block(IfStatement(IsPatternExpression(InvocationExpression(
                            IdentifierName("global::ArchiToolkit.Grasshopper.ArchiToolkitResources.GetIcon"))
                        .WithArgumentList(ArgumentList(
                        [
                            Argument(BinaryExpression(SyntaxKind.AddExpression, BinaryExpression(
                                    SyntaxKind.AddExpression, LiteralExpression(SyntaxKind.StringLiteralExpression,
                                        Literal(assembly.Name + ".Icons.")),
                                    IdentifierName("iconName")),
                                LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(".png"))))
                        ])),
                    UnaryPattern(RecursivePattern().WithPropertyPatternClause(PropertyPatternClause())
                        .WithDesignation(SingleVariableDesignation(Identifier("icon"))))), ReturnStatement()),
                ExpressionStatement(
                    InvocationExpression(
                            IdentifierName("global::Grasshopper.Instances.ComponentServer.AddCategoryIcon"))
                        .WithArgumentList(
                            ArgumentList(
                            [
                                Argument(InvocationExpression(
                                        IdentifierName("global::ArchiToolkit.Grasshopper.ArchiToolkitResources.Get"))
                                    .WithArgumentList(ArgumentList([Argument(IdentifierName("iconName"))]))),
                                Argument(IdentifierName("icon"))
                            ])))));

        var loadMethod = MethodDeclaration(IdentifierName("global::Grasshopper.Kernel.GH_LoadingInstruction"),
                Identifier("PriorityLoad"))
            .WithAttributeLists([
                GeneratedCodeAttribute(typeof(CategoryGenerator))
                    .AddAttributes(NonUserCodeAttribute())
            ])
            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.OverrideKeyword)))
            .WithBody(Block((IEnumerable<StatementSyntax>)
            [
                ..icons.Select(AddIcon),
                ReturnStatement(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName("global::Grasshopper.Kernel.GH_LoadingInstruction"),
                    IdentifierName("Proceed")))
            ]));

        var node = NamespaceDeclaration(assembly.Name).WithMembers([
            ClassDeclaration("AssemblyPriorityCategoryIcons")
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.SealedKeyword)))
                .WithBaseList(BaseList([
                    SimpleBaseType(IdentifierName("global::Grasshopper.Kernel.GH_AssemblyPriority"))
                ]))
                .WithAttributeLists([
                    GeneratedCodeAttribute(typeof(CategoryGenerator)).AddAttributes(NonUserCodeAttribute())
                ])
                .WithMembers([loadMethod, addIcon])
        ]);

        context.AddSource("AssemblyPriority.CategoryIcons.g.cs", node.NodeToString());
    }

    private static StatementSyntax AddIcon(string icon)
    {
        return ExpressionStatement(InvocationExpression(IdentifierName("LoadIcon"))
            .WithArgumentList(ArgumentList([
                Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(icon)))
            ])));
    }
}