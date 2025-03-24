using ArchiToolkit.RoslynHelper.Extensions;
using ArchiToolkit.RoslynHelper.Names;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static ArchiToolkit.RoslynHelper.Extensions.SyntaxExtensions;

namespace ArchiToolkit.Grasshopper.SourceGenerator;

public class TypeGenerator : BasicGenerator
{
    public string ObjTypeName, ObjTypeDescription;
    public readonly TypeName Name;

    public TypeGenerator(ISymbol symbol) : base(symbol)
    {
        if (symbol is not ITypeSymbol typeSymbol)
            throw new ArgumentException("Symbol is not a type symbol");
        Name = typeSymbol.GetName();
        ObjTypeName = ObjTypeDescription = Name.Name;
        GetObjNames(Symbol.GetAttributes(), ref ObjTypeName, ref ObjTypeDescription);
    }

    private static void GetObjNames(IEnumerable<AttributeData> attributes, ref string name, ref string description)
    {
        if (attributes
                .FirstOrDefault(a => a.AttributeClass?.GetName().FullName
                    is "global::ArchiToolkit.Grasshopper.TypeDescAttribute") is not
            { ConstructorArguments.Length: 2 } attr) return;

        var relay = attr.ConstructorArguments[0].Value?.ToString();
        if (!string.IsNullOrEmpty(relay)) name = relay!;
        relay = attr.ConstructorArguments[1].Value?.ToString();
        if (!string.IsNullOrEmpty(relay)) description = relay!;
    }

    protected override string IdName => Name.FullName;

    public override string ClassName => "Param_" + Name.Name;
    protected override char IconType => 'P';

    public static ITypeSymbol BaseGoo { get; set; } = null!;

    protected override ClassDeclarationSyntax ModifyClass(ClassDeclarationSyntax classSyntax)
    {
        classSyntax = classSyntax.WithBaseList(
            BaseList([
                SimpleBaseType(GenericName(Identifier("global::Grasshopper.Kernel.GH_PersistentParam"))
                    .WithTypeArgumentList(TypeArgumentList([
                        QualifiedName(
                            IdentifierName(RealClassName), IdentifierName("Goo"))
                    ])))
            ]));
        var basicGoo = (INamedTypeSymbol)(DocumentObjectGenerator.GetTypeAttribute(Name.Symbol.GetAttributes(),
                                              "global::ArchiToolkit.Grasshopper.BaseGooAttribute<>")?.Symbol
                                          ?? ((INamedTypeSymbol)BaseGoo).Construct(Name.Symbol));

        var gooClass = ClassDeclaration("Goo").WithModifiers(
                TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.SealedKeyword),
                    Token(SyntaxKind.PartialKeyword)))
            .WithBaseList(BaseList([
                SimpleBaseType(
                    IdentifierName(basicGoo.GetName().FullName))
            ]))
            .WithAttributeLists([
                GeneratedCodeAttribute(typeof(TypeGenerator)).AddAttributes(NonUserCodeAttribute())
            ])
            .WithMembers(
            [
                ConstructorDeclaration(Identifier("Goo"))
                    .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                    .WithAttributeLists([
                        GeneratedCodeAttribute(typeof(TypeGenerator)).AddAttributes(NonUserCodeAttribute())
                    ])
                    .WithBody(Block()),

                ConstructorDeclaration(Identifier("Goo"))
                    .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                    .WithParameterList(ParameterList(
                    [
                        Parameter(Identifier("value")).WithType(IdentifierName(Name.FullName))
                    ]))
                    .WithInitializer(ConstructorInitializer(SyntaxKind.BaseConstructorInitializer,
                        ArgumentList([Argument(IdentifierName("value"))])))
                    .WithAttributeLists([
                        GeneratedCodeAttribute(typeof(TypeGenerator)).AddAttributes(NonUserCodeAttribute())
                    ])
                    .WithBody(Block()),

                ConstructorDeclaration(Identifier("Goo"))
                    .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                    .WithParameterList(ParameterList([Parameter(Identifier("other")).WithType(IdentifierName("Goo"))]))
                    .WithInitializer(ConstructorInitializer(SyntaxKind.BaseConstructorInitializer,
                        ArgumentList(
                        [
                            Argument(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                IdentifierName("other"), IdentifierName("Value")))
                        ])))
                    .WithAttributeLists([
                        GeneratedCodeAttribute(typeof(TypeGenerator)).AddAttributes(NonUserCodeAttribute())
                    ])
                    .WithBody(Block()),

                MethodDeclaration(IdentifierName("global::Grasshopper.Kernel.Types.IGH_Goo"), Identifier("Duplicate"))
                    .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.OverrideKeyword)))
                    .WithExpressionBody(ArrowExpressionClause(ObjectCreationExpression(IdentifierName("Goo"))
                        .WithArgumentList(ArgumentList([Argument(IdentifierName("Value"))]))))
                    .WithAttributeLists([
                        GeneratedCodeAttribute(typeof(TypeGenerator)).AddAttributes(NonUserCodeAttribute())
                    ])
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),

                MethodDeclaration(PredefinedType(Token(SyntaxKind.StringKeyword)), Identifier("ToString"))
                    .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.OverrideKeyword)))
                    .WithExpressionBody(ArrowExpressionClause(BinaryExpression(SyntaxKind.CoalesceExpression,
                        ConditionalAccessExpression(IdentifierName("Value"),
                            InvocationExpression(MemberBindingExpression(IdentifierName("ToString")))),
                        LiteralExpression(SyntaxKind.StringLiteralExpression, Literal("<null>")))))
                    .WithAttributeLists([
                        GeneratedCodeAttribute(typeof(TypeGenerator)).AddAttributes(NonUserCodeAttribute())
                    ])
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),

                PropertyDeclaration(PredefinedType(Token(SyntaxKind.BoolKeyword)), Identifier("IsValid"))
                    .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.OverrideKeyword)))
                    .WithExpressionBody(ArrowExpressionClause(LiteralExpression(SyntaxKind.TrueLiteralExpression)))
                    .WithAttributeLists([
                        GeneratedCodeAttribute(typeof(TypeGenerator)).AddAttributes(NonUserCodeAttribute())
                    ])
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),

                PropertyDeclaration(PredefinedType(Token(SyntaxKind.StringKeyword)), Identifier("TypeName"))
                    .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.OverrideKeyword)))
                    .WithExpressionBody(ArrowExpressionClause(GetArgumentKeyedString(".TypeName", ObjTypeName)))
                    .WithAttributeLists([
                        GeneratedCodeAttribute(typeof(TypeGenerator)).AddAttributes(NonUserCodeAttribute())
                    ])
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),

                PropertyDeclaration(PredefinedType(Token(SyntaxKind.StringKeyword)), Identifier("TypeDescription"))
                    .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.OverrideKeyword)))
                    .WithExpressionBody(ArrowExpressionClause(GetArgumentKeyedString(".TypeDescription", ObjTypeDescription)))
                    .WithAttributeLists([
                        GeneratedCodeAttribute(typeof(TypeGenerator)).AddAttributes(NonUserCodeAttribute())
                    ])
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),

                MethodDeclaration(PredefinedType(Token(SyntaxKind.BoolKeyword)), Identifier("CastFrom"))
                    .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.OverrideKeyword)))
                    .WithParameterList(ParameterList([
                        Parameter(Identifier("source"))
                            .WithType(PredefinedType(Token(SyntaxKind.ObjectKeyword)))
                    ]))
                    .WithAttributeLists([
                        GeneratedCodeAttribute(typeof(TypeGenerator)).AddAttributes(NonUserCodeAttribute())
                    ])
                    .WithBody(Block(IfStatement(InvocationExpression(
                                    MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                        IdentifierName("global::ArchiToolkit.Grasshopper.ActiveObjectHelper"),
                                        GenericName(Identifier("CastFrom"))
                                            .WithTypeArgumentList(TypeArgumentList([IdentifierName(Name.FullName)]))))
                                .WithArgumentList(ArgumentList(
                                [
                                    Argument(IdentifierName("source")),
                                    Argument(DeclarationExpression(IdentifierName("var"),
                                            SingleVariableDesignation(Identifier("value"))))
                                        .WithRefOrOutKeyword(Token(SyntaxKind.OutKeyword))
                                ])),
                            Block(ExpressionStatement(
                                    AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                                        IdentifierName("Value"), IdentifierName("value"))),
                                ReturnStatement(LiteralExpression(SyntaxKind.TrueLiteralExpression))))
                        .WithElse(ElseClause(Block(ReturnStatement(InvocationExpression(
                                MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, BaseExpression(),
                                    IdentifierName("CastFrom")))
                            .WithArgumentList(ArgumentList([Argument(IdentifierName("source"))])))))))),

                MethodDeclaration(PredefinedType(Token(SyntaxKind.BoolKeyword)), Identifier("CastTo"))
                    .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.OverrideKeyword)))
                    .WithTypeParameterList(TypeParameterList([TypeParameter(Identifier("Q"))]))
                    .WithParameterList(ParameterList([
                        Parameter(Identifier("target"))
                            .WithModifiers(TokenList(Token(SyntaxKind.RefKeyword)))
                            .WithType(IdentifierName("Q"))
                    ]))
                    .WithAttributeLists([
                        GeneratedCodeAttribute(typeof(TypeGenerator)).AddAttributes(NonUserCodeAttribute())
                    ])
                    .WithBody(Block(ReturnStatement(BinaryExpression(SyntaxKind.LogicalOrExpression,
                        InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                IdentifierName("global::ArchiToolkit.Grasshopper.ActiveObjectHelper"),
                                IdentifierName("CastTo")))
                            .WithArgumentList(ArgumentList(
                            [
                                Argument(IdentifierName("Value")),
                                Argument(IdentifierName("target")).WithRefOrOutKeyword(Token(SyntaxKind.RefKeyword))
                            ])),
                        InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                BaseExpression(), IdentifierName("CastTo")))
                            .WithArgumentList(ArgumentList([
                                Argument(IdentifierName("target")).WithRefOrOutKeyword(Token(SyntaxKind.RefKeyword))
                            ]))))))
            ]);

        classSyntax = classSyntax.AddMembers(gooClass,
            ConstructorDeclaration(Identifier(RealClassName)).WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                .WithInitializer(ConstructorInitializer(SyntaxKind.ThisConstructorInitializer,
                    ArgumentList(
                    [
                        Argument(GetArgumentKeyedString(".Name", ObjName)),
                        Argument(GetArgumentKeyedString(".Nickname", ObjNickname)),
                        Argument(GetArgumentKeyedString(".Description", ObjDescription)),
                        Argument(GetArgumentRawString("Category." + (Category ?? BaseCategory),
                            Category ?? BaseCategory)),
                        Argument(GetArgumentRawString("Subcategory." + (Subcategory ?? "Parameter"),
                            Subcategory ?? "Parameter"))
                    ])))
                .WithAttributeLists([
                    GeneratedCodeAttribute(typeof(TypeGenerator)).AddAttributes(NonUserCodeAttribute())
                ])
                .WithBody(Block()),
            ConstructorDeclaration(Identifier(RealClassName)).WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                .WithParameterList(ParameterList([
                    Parameter(Identifier("nTag"))
                        .WithType(IdentifierName("global::Grasshopper.Kernel.GH_InstanceDescription"))
                ]))
                .WithInitializer(ConstructorInitializer(SyntaxKind.BaseConstructorInitializer,
                    ArgumentList([Argument(IdentifierName("nTag"))])))
                .WithAttributeLists([
                    GeneratedCodeAttribute(typeof(TypeGenerator)).AddAttributes(NonUserCodeAttribute())
                ])
                .WithBody(Block()),
            ConstructorDeclaration(Identifier(RealClassName)).WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                .WithParameterList(ParameterList(
                [
                    Parameter(Identifier("name"))
                        .WithType(PredefinedType(Token(SyntaxKind.StringKeyword))),
                    Parameter(Identifier("nickname"))
                        .WithType(PredefinedType(Token(SyntaxKind.StringKeyword))),
                    Parameter(Identifier("description"))
                        .WithType(PredefinedType(Token(SyntaxKind.StringKeyword))),
                    Parameter(Identifier("category"))
                        .WithType(PredefinedType(Token(SyntaxKind.StringKeyword))),
                    Parameter(Identifier("subcategory"))
                        .WithType(PredefinedType(Token(SyntaxKind.StringKeyword)))
                ]))
                .WithInitializer(ConstructorInitializer(SyntaxKind.BaseConstructorInitializer,
                    ArgumentList(
                    [
                        Argument(IdentifierName("name")),
                        Argument(IdentifierName("nickname")),
                        Argument(IdentifierName("description")),
                        Argument(IdentifierName("category")),
                        Argument(IdentifierName("subcategory"))
                    ])))
                .WithAttributeLists([
                    GeneratedCodeAttribute(typeof(TypeGenerator)).AddAttributes(NonUserCodeAttribute())
                ])
                .WithBody(Block()),
            MethodDeclaration(IdentifierName("global::Grasshopper.Kernel.GH_GetterResult"),
                    Identifier("Prompt_Singular"))
                .WithModifiers(TokenList(Token(SyntaxKind.ProtectedKeyword), Token(SyntaxKind.OverrideKeyword)))
                .WithParameterList(ParameterList(
                [
                    Parameter(Identifier("value")).WithModifiers(TokenList(Token(SyntaxKind.RefKeyword)))
                        .WithType(IdentifierName("Goo"))
                ]))
                .WithAttributeLists([
                    GeneratedCodeAttribute(typeof(TypeGenerator)).AddAttributes(NonUserCodeAttribute())
                ])
                .WithBody(Block(
                    LocalDeclarationStatement(VariableDeclaration(IdentifierName("var"))
                        .WithVariables(
                        [
                            VariableDeclarator(Identifier("result"))
                                .WithInitializer(EqualsValueClause(MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    IdentifierName("global::Grasshopper.Kernel.GH_GetterResult"),
                                    IdentifierName("cancel"))))
                        ])),
                    ExpressionStatement(InvocationExpression(IdentifierName("PromptSingular"))
                        .WithArgumentList(ArgumentList(
                        [
                            Argument(IdentifierName("value")).WithRefOrOutKeyword(Token(SyntaxKind.RefKeyword)),
                            Argument(IdentifierName("result")).WithRefOrOutKeyword(Token(SyntaxKind.RefKeyword))
                        ]))),
                    ReturnStatement(IdentifierName("result")))),
            MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)), Identifier("PromptSingular"))
                .WithModifiers(TokenList(Token(SyntaxKind.PartialKeyword)))
                .WithParameterList(ParameterList(
                [
                    Parameter(Identifier("value"))
                        .WithModifiers(TokenList(Token(SyntaxKind.RefKeyword))).WithType(IdentifierName("Goo")),
                    Parameter(Identifier("result")).WithModifiers(TokenList(Token(SyntaxKind.RefKeyword)))
                        .WithType(IdentifierName("global::Grasshopper.Kernel.GH_GetterResult"))
                ]))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
            MethodDeclaration(IdentifierName("global::Grasshopper.Kernel.GH_GetterResult"), Identifier("Prompt_Plural"))
                .WithModifiers(TokenList(Token(SyntaxKind.ProtectedKeyword), Token(SyntaxKind.OverrideKeyword)))
                .WithParameterList(
                    ParameterList(
                    [
                        Parameter(Identifier("values")).WithModifiers(TokenList(Token(SyntaxKind.RefKeyword)))
                            .WithType(GenericName(Identifier("global::System.Collections.Generic.List"))
                                .WithTypeArgumentList(TypeArgumentList([IdentifierName("Goo")])))
                    ]))
                .WithAttributeLists([
                    GeneratedCodeAttribute(typeof(TypeGenerator)).AddAttributes(NonUserCodeAttribute())
                ])
                .WithBody(Block(
                    LocalDeclarationStatement(VariableDeclaration(IdentifierName("var"))
                        .WithVariables(
                        [
                            VariableDeclarator(Identifier("result")).WithInitializer(EqualsValueClause(
                                MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                    IdentifierName("global::Grasshopper.Kernel.GH_GetterResult"),
                                    IdentifierName("cancel"))))
                        ])),
                    ExpressionStatement(InvocationExpression(IdentifierName("PromptPlural"))
                        .WithArgumentList(
                            ArgumentList(
                            [
                                Argument(IdentifierName("values")).WithRefOrOutKeyword(Token(SyntaxKind.RefKeyword)),
                                Argument(IdentifierName("result")).WithRefOrOutKeyword(Token(SyntaxKind.RefKeyword))
                            ]))),
                    ReturnStatement(IdentifierName("result")))),
            MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)), Identifier("PromptPlural"))
                .WithModifiers(TokenList(Token(SyntaxKind.PartialKeyword)))
                .WithParameterList(ParameterList(
                [
                    Parameter(Identifier("values")).WithModifiers(TokenList(Token(SyntaxKind.RefKeyword)))
                        .WithType(GenericName(Identifier("global::System.Collections.Generic.List"))
                            .WithTypeArgumentList(TypeArgumentList([IdentifierName("Goo")]))),
                    Parameter(Identifier("result")).WithModifiers(TokenList(Token(SyntaxKind.RefKeyword)))
                        .WithType(IdentifierName("global::Grasshopper.Kernel.GH_GetterResult"))
                ]))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
        );
        if (DocumentObjectGenerator.GetBaseAttribute(Name.Symbol.GetAttributes()) is { } attributeName)
            classSyntax = classSyntax.AddMembers(MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)),
                    Identifier("CreateAttributes"))
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.OverrideKeyword)))
                .WithAttributeLists([
                    GeneratedCodeAttribute(typeof(TypeGenerator)).AddAttributes(NonUserCodeAttribute())
                ])
                .WithBody(Block(ExpressionStatement(AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression, IdentifierName("m_attributes"),
                    ObjectCreationExpression(IdentifierName(attributeName))
                        .WithArgumentList(ArgumentList([Argument(ThisExpression())])))))));

        return classSyntax;
    }
}