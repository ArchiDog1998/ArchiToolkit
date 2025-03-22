using System.Text;
using ArchiToolkit.RoslynHelper.Extensions;
using ArchiToolkit.RoslynHelper.Names;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace ArchiToolkit.Grasshopper.SourceGenerator;

public class MethodGenerator : BasicGenerator
{
    private readonly MethodName _name;

    public MethodGenerator(ISymbol symbol): base(symbol)
    {
        if (symbol is not IMethodSymbol methodSymbol)
            throw new ArgumentException("Symbol is not a type method symbol");
        _name = methodSymbol.GetName();
    }

    protected override string IdName
    {
        get
        {
            var builder = new StringBuilder();
            var sig = _name.Signature;
            builder.Append(sig.ContainingType.GetName().FullName);
            builder.Append('.');
            builder.Append(sig.MethodName);
            builder.Append('.');
            builder.Append(sig.TypeArgumentsCount);
            builder.Append('(');
            builder.Append( string.Join(", ", sig.ParameterTypes.Select(type => type.GetName().FullName)));
            builder.Append(')');
            return builder.ToString();
        }
    }

    protected override string ClassName => "Component_" + _name.Name;

    public string GlobalBaseComponent { get; set; } = null!;

    protected override ClassDeclarationSyntax ModifyClass(ClassDeclarationSyntax classSyntax)
    {
        var baseComponent = DocumentObjectGenerator.GetBaseComponent(_name.Symbol.GetAttributes()) ?? GlobalBaseComponent;
        var keyName = KeyName;
        var keyComponent = keyName + ".Component";
        return classSyntax.WithParameterList(ParameterList())
            .WithBaseList(BaseList(
            [
                PrimaryConstructorBaseType(IdentifierName(baseComponent))
                    .WithArgumentList(ArgumentList(
                        [
                            GetArgumentString(keyComponent + ".Name"),
                            GetArgumentString(keyComponent + ".Nickname"),
                            GetArgumentString(keyComponent + ".Description"),
                            GetArgumentString("Category." + (Category ?? BaseCategory)),
                            GetArgumentString("Subcategory." + (Subcategory ?? BaseSubcategory)),
                        ]))
            ]));
    }

    private static ArgumentSyntax GetArgumentString(string key)
    {
        return Argument(InvocationExpression(
                    IdentifierName("global::ArchiToolkit.Grasshopper.ArchiToolkitResources.Get"))
                .WithArgumentList(ArgumentList([Argument(LiteralExpression(SyntaxKind.StringLiteralExpression,Literal(key)))])));
    }
}