using System.Text;
using ArchiToolkit.RoslynHelper.Extensions;
using ArchiToolkit.RoslynHelper.Names;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static ArchiToolkit.RoslynHelper.Extensions.SyntaxExtensions;

namespace ArchiToolkit.Grasshopper.SourceGenerator;

public class MethodGenerator : BasicGenerator
{
    private readonly List<MethodParamItem> _parameters;

    public readonly MethodName Name;

    public MethodGenerator(ISymbol symbol) : base(symbol)
    {
        if (symbol is not IMethodSymbol methodSymbol)
            throw new ArgumentException("Symbol is not a type method symbol");
        Name = methodSymbol.GetName();
        var owner = Name.ContainingType;
        var items = Name.Parameters.Select(p => new MethodParamItem(this, p, owner));
        if (methodSymbol.ReturnType.SpecialType is not SpecialType.System_Void)
            items = items.Append(new MethodParamItem(this, "result", methodSymbol.ReturnType.GetName(), ParamType.Out,
                owner,
                methodSymbol.GetReturnTypeAttributes()));

        _parameters = items.ToList();
    }

    public static string[] NeedIdNames { get; set; } = [];

    protected override bool NeedId => NeedIdNames.Contains(ClassName);

    protected override string IdName
    {
        get
        {
            var builder = new StringBuilder();
            var sig = Name.Signature;
            builder.Append(sig.ContainingType.GetName().FullName);
            builder.Append('.');
            builder.Append(sig.MethodName);
            builder.Append('.');
            builder.Append(sig.TypeArgumentsCount);
            builder.Append('(');
            builder.Append(string.Join(", ", sig.ParameterTypes.Select(type => type.GetName().FullName)));
            builder.Append(')');
            return builder.ToString();
        }
    }

    public override string ClassName => "Component_" + Name.Name;
    protected override char IconType => 'C';

    public static ITypeSymbol GlobalBaseComponent { get; set; } = null!;

    protected override ClassDeclarationSyntax ModifyClass(ClassDeclarationSyntax classSyntax)
    {
        var baseComponent = DocumentObjectGenerator.GetBaseComponent(Name.Symbol.GetAttributes()) ??
                            GlobalBaseComponent;

        var inputMethod = MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)),
                Identifier("RegisterInputParams"))
            .WithModifiers(TokenList(Token(SyntaxKind.ProtectedKeyword), Token(SyntaxKind.OverrideKeyword)))
            .WithParameterList(ParameterList([
                Parameter(Identifier("pManager"))
                    .WithType(IdentifierName("GH_InputParamManager"))
            ]))
            .WithAttributeLists([
                GeneratedCodeAttribute(typeof(MethodGenerator)).AddAttributes(NonUserCodeAttribute())
            ])
            .WithBody(Block(_parameters.Where(p => p.Type.HasFlag(ParamType.In)).Select(p => p.IoBlock(true))));

        var outputMethod = MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)),
                Identifier("RegisterOutputParams"))
            .WithModifiers(TokenList(Token(SyntaxKind.ProtectedKeyword), Token(SyntaxKind.OverrideKeyword)))
            .WithParameterList(ParameterList([
                Parameter(Identifier("pManager"))
                    .WithType(IdentifierName("GH_OutputParamManager"))
            ]))
            .WithAttributeLists([
                GeneratedCodeAttribute(typeof(MethodGenerator)).AddAttributes(NonUserCodeAttribute())
            ])
            .WithBody(Block(_parameters.Where(p => p.Type.HasFlag(ParamType.Out)).Select(p => p.IoBlock(false))));

        var invocation = InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                IdentifierName(Name.ContainingType.FullName), IdentifierName(Name.Name)))
            .WithArgumentList(
                ArgumentList(
                [
                    .._parameters.Where(i => i.Parameter is not null)
                        .Select(i =>
                        {
                            var arg = Argument(IdentifierName(i.ParameterName));
                            return i.Parameter?.Symbol.RefKind switch
                            {
                                RefKind.Ref => arg.WithRefOrOutKeyword(Token(SyntaxKind.RefKeyword)),
                                RefKind.In => arg.WithRefOrOutKeyword(Token(SyntaxKind.InKeyword)),
                                RefKind.Out => Argument(DeclarationExpression(IdentifierName("var"),
                                        SingleVariableDesignation(Identifier(i.ParameterName))))
                                    .WithRefOrOutKeyword(Token(SyntaxKind.OutKeyword)),
                                _ => arg
                            };
                        })
                ]));

        var computeMethod =
            MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)), Identifier("SolveInstance"))
                .WithModifiers(TokenList(Token(SyntaxKind.ProtectedKeyword), Token(SyntaxKind.OverrideKeyword)))
                .WithParameterList(
                    ParameterList(
                    [
                        Parameter(Identifier("DA"))
                            .WithType(IdentifierName("global::Grasshopper.Kernel.IGH_DataAccess"))
                    ]))
                .WithAttributeLists([
                    GeneratedCodeAttribute(typeof(MethodGenerator)).AddAttributes(NonUserCodeAttribute())
                ])
                .WithBody(Block((IEnumerable<StatementSyntax>)
                [
                    .._parameters.Where(p => p.Type.HasFlag(ParamType.In)).Select((p, i) => p.GetData(i)),
                    _parameters.Any(p => p.Parameter is null)
                        ? LocalDeclarationStatement(VariableDeclaration(IdentifierName("var"))
                            .WithVariables([
                                VariableDeclarator(Identifier("result")).WithInitializer(EqualsValueClause(invocation))
                            ]))
                        : ExpressionStatement(invocation),
                    .._parameters.Where(p => p.Type.HasFlag(ParamType.Out)).Select((p, i) => p.SetData(i))
                ]));

        var readMethod = MethodDeclaration(PredefinedType(Token(SyntaxKind.BoolKeyword)), Identifier("Read"))
            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.OverrideKeyword)))
            .WithParameterList(ParameterList([
                Parameter(Identifier("reader")).WithType(
                    IdentifierName("global::GH_IO.Serialization.GH_IReader"))
            ]))
            .WithAttributeLists([
                GeneratedCodeAttribute(typeof(MethodGenerator)).AddAttributes(NonUserCodeAttribute())
            ])
            .WithBody(Block((IEnumerable<StatementSyntax>)
            [
                .._parameters
                    .Where(p => p.Type is ParamType.Field && p.Io)
                    .Select(p => p.ReadData()),
                ReturnStatement(InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                        BaseExpression(), IdentifierName("Read")))
                    .WithArgumentList(ArgumentList([Argument(IdentifierName("reader"))])))
            ]));

        var writeMethod = MethodDeclaration(PredefinedType(Token(SyntaxKind.BoolKeyword)), Identifier("Write"))
            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.OverrideKeyword)))
            .WithParameterList(ParameterList([
                Parameter(Identifier("writer")).WithType(
                    IdentifierName("global::GH_IO.Serialization.GH_IWriter"))
            ]))
            .WithAttributeLists([
                GeneratedCodeAttribute(typeof(MethodGenerator)).AddAttributes(NonUserCodeAttribute())
            ])
            .WithBody(Block((IEnumerable<StatementSyntax>)
            [
                .._parameters
                    .Where(p => p.Type is ParamType.Field && p.Io)
                    .Select(p => p.WriteData()),
                ReturnStatement(InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                        BaseExpression(), IdentifierName("Write")))
                    .WithArgumentList(ArgumentList([Argument(IdentifierName("writer"))])))
            ]));

        var addedMembers = baseComponent
            .GetBaseTypesAndThis()
            .SelectMany(t => t.GetMembers())
            .Where(m => m.DeclaredAccessibility is not Accessibility.Private and not Accessibility.ProtectedAndInternal)
            .Select(i => i.Name)
            .ToArray();

        string name = Symbol.Name, nickname = Symbol.Name, description = Symbol.Name;
        DocumentObjectGenerator.GetObjNames(Name.Symbol.GetAttributes(), ref name, ref nickname, ref description);

        classSyntax = classSyntax.WithParameterList(ParameterList())
            .WithBaseList(BaseList(
            [
                PrimaryConstructorBaseType(IdentifierName(baseComponent.GetName().FullName))
                    .WithArgumentList(ArgumentList(
                    [
                        Argument(GetArgumentKeyedString(".Component.Name", name)),
                        Argument(GetArgumentKeyedString(".Component.Nickname", nickname)),
                        Argument(GetArgumentKeyedString(".Component.Description", description)),
                        Argument(GetArgumentRawString("Category." + (Category ?? BaseCategory), (Category ?? BaseCategory))),
                        Argument(GetArgumentRawString("Subcategory." + (Subcategory ?? BaseSubcategory), (Subcategory ?? BaseSubcategory)))
                    ]))
            ]))
            .AddMembers(
            [
                .._parameters
                    .Where(p => p.Type is ParamType.Field)
                    .Where(p => !addedMembers.Contains(p.Name))
                    .Select(p => p.Field()),
                inputMethod,
                outputMethod,
                computeMethod,
                readMethod,
                writeMethod
            ]);


        if ((DocumentObjectGenerator.GetBaseAttribute(Name.Symbol.GetAttributes())
             ?? BaseAttribute) is { } attributeName)
            classSyntax = classSyntax.AddMembers(MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)),
                    Identifier("CreateAttributes"))
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.OverrideKeyword)))
                .WithAttributeLists([
                    GeneratedCodeAttribute(typeof(MethodGenerator)).AddAttributes(NonUserCodeAttribute())
                ])
                .WithBody(Block(ExpressionStatement(AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression, IdentifierName("m_attributes"),
                    ObjectCreationExpression(IdentifierName(attributeName))
                        .WithArgumentList(ArgumentList([Argument(ThisExpression())])))))));

        return classSyntax;
    }
}