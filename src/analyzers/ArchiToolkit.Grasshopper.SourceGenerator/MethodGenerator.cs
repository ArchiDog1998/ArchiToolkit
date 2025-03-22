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
    public static string[] NeedIdNames { get; set; } = [];

    public readonly MethodName Name;
    private readonly List<MethodParamItem> _parameters;

    protected override bool NeedId => NeedIdNames.Contains(ClassName);

    public MethodGenerator(ISymbol symbol) : base(symbol)
    {
        if (symbol is not IMethodSymbol methodSymbol)
            throw new ArgumentException("Symbol is not a type method symbol");
        Name = methodSymbol.GetName();
        var items = Name.Parameters.Select(p => new MethodParamItem(p));
        if (methodSymbol.ReturnType.SpecialType is not SpecialType.System_Void)
        {
            items = items.Append(new MethodParamItem("result", methodSymbol.ReturnType.GetName(), ParamType.Out,
                methodSymbol.GetReturnTypeAttributes()));
        }
        _parameters = items.ToList();
    }

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

    public string GlobalBaseComponent { get; set; } = null!;

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
            .WithBody(Block(_parameters.Where(p => p.Type.HasFlag(ParamType.In)).Select(p => p.IoBlock())));

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
            .WithBody(Block(_parameters.Where(p => p.Type.HasFlag(ParamType.Out)).Select(p => p.IoBlock())));

        return classSyntax.WithParameterList(ParameterList())
            .WithBaseList(BaseList(
            [
                PrimaryConstructorBaseType(IdentifierName(baseComponent))
                    .WithArgumentList(ArgumentList(
                    [
                        GetArgumentKeyedString(".Component.Name"),
                        GetArgumentKeyedString(".Component.Nickname"),
                        GetArgumentKeyedString(".Component.Description"),
                        GetArgumentRawString("Category." + (Category ?? BaseCategory)),
                        GetArgumentRawString("Subcategory." + (Subcategory ?? BaseSubcategory)),
                    ]))
            ]))
            .AddMembers(
            [
                .._parameters.Where(p => p.Type is ParamType.Field).Select(p => p.Field()),
                inputMethod,
                outputMethod,
            ]);
    }

    private static ArgumentSyntax GetArgumentRawString(string key)
    {
        return GetArgumentString(Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(key))));
    }

    public static ArgumentSyntax GetArgumentKeyedString(string key)
    {
        return GetArgumentString(Argument(BinaryExpression(SyntaxKind.AddExpression,
            IdentifierName("ResourceKey"), LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(key)))));
    }

    private static ArgumentSyntax GetArgumentString(ArgumentSyntax argument)
    {
        return Argument(InvocationExpression(
                IdentifierName("global::ArchiToolkit.Grasshopper.ArchiToolkitResources.Get"))
            .WithArgumentList(ArgumentList([argument])));
    }
}