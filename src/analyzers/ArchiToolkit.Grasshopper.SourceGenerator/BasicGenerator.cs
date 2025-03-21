using System.Security.Cryptography;
using System.Text;
using ArchiToolkit.RoslynHelper.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static ArchiToolkit.RoslynHelper.Extensions.SyntaxExtensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace ArchiToolkit.Grasshopper.SourceGenerator;

public abstract class BasicGenerator
{
    private readonly ISymbol _symbol;
    public IAssemblySymbol Assembly { get; set; } = null!;

    protected string NameSpace => _symbol.ContainingNamespace.ToString();
    protected abstract string IdName { get; }

    public abstract string ClassName { get; }
    public string DocObjName { get; }

    protected BasicGenerator(ISymbol symbol)
    {
        _symbol = symbol;
        var docObj = symbol.GetAttributes().First(a =>
            a.AttributeClass?.GetName().FullName == "global::ArchiToolkit.Grasshopper.DocObjAttribute");
        var keyName = docObj.ConstructorArguments.Length > 0 ? docObj.ConstructorArguments[0].Value?.ToString() : null;
        DocObjName = keyName ?? string.Empty;
    }

    public Guid Id => StringToGuid(IdName);

    private static Guid StringToGuid(string input)
    {
        using var sha1 = SHA1.Create();
        var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
        var guidBytes = new byte[16];
        Array.Copy(hash, guidBytes, 16);
        return new Guid(guidBytes); // Use the first 16 bytes
    }

    public sealed override string ToString() => IdName;


    protected abstract ClassDeclarationSyntax ModifyClass(ClassDeclarationSyntax classSyntax);

    public void GenerateSource(SourceProductionContext context)
    {
        var guidProperty = PropertyDeclaration(IdentifierName("global::System.Guid"), Identifier("ComponentGuid"))
            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.OverrideKeyword)))
            .WithExpressionBody(ArrowExpressionClause(ImplicitObjectCreationExpression().WithArgumentList(ArgumentList(
            [
                Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(Id.ToString("D"))))
            ]))))
            .WithSemicolonToken(
                Token(SyntaxKind.SemicolonToken));

        var classSyntax = ClassDeclaration(ClassName)
            .WithModifiers(
            [
                Token(SyntaxKind.PublicKeyword),
                Token(SyntaxKind.SealedKeyword),
                Token(SyntaxKind.PartialKeyword)
            ])
            .WithAttributeLists([
                GeneratedCodeAttribute(typeof(DocumentObjectGenerator)).AddAttributes(NonUserCodeAttribute())
            ])
            .WithMembers([guidProperty]);
        var item = NamespaceDeclaration(NameSpace)
            .WithMembers([ModifyClass(classSyntax)]);

        context.AddSource(Id.ToString("N") + ".g.cs", item.NodeToString());
    }
}