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
    public readonly ISymbol Symbol;

    protected string NameSpace => Symbol.ContainingNamespace.ToString();
    protected abstract string IdName { get; }

    protected abstract string ClassName { get; }

    public string RealClassName
    {
        get
        {
            var id = Id.ToString("N").Substring(0, 8);
            var className = ClassName + "_" + id;
            return IsObsolete ? className + "_OBSOLETE" : className;
        }
    }

    public string KeyName => string.IsNullOrEmpty(field) ? NameSpace + "." + RealClassName : field;

    public string BaseCategory { get; set; } = null!;
    public string BaseSubcategory { get; set; } = null!;
    public string? Category { get; set; }
    public string? Subcategory { get; set; }

    public string? Exposure { get; }
    public bool IsObsolete { get; }

    protected BasicGenerator(ISymbol symbol)
    {
        Symbol = symbol;
        var docObj = symbol.GetAttributes().First(a =>
            a.AttributeClass?.GetName().FullName == "global::ArchiToolkit.Grasshopper.DocObjAttribute");
        var keyName = docObj.ConstructorArguments.Length > 0 ? docObj.ConstructorArguments[0].Value?.ToString() : null;
        KeyName = keyName ?? string.Empty;

        if (symbol.GetAttributes().Any(a =>
                a.AttributeClass?.GetName().FullName == "global::System.ObsoleteAttribute"))
        {
            IsObsolete = true;
            Exposure = "-1";
        }
        else
        {
            IsObsolete = false;
            var exposure = symbol.GetAttributes().FirstOrDefault(a =>
                a.AttributeClass?.GetName().FullName == "global::ArchiToolkit.Grasshopper.ExposureAttribute");
            Exposure = exposure?.ConstructorArguments[0].Value?.ToString();
        }

        var s = symbol;
        while (s is not null)
        {
            var attributes = s.GetAttributes();
            Category ??= DocumentObjectGenerator.GetBaseCategory(attributes);
            Subcategory ??= DocumentObjectGenerator.GetBaseSubcategory(attributes);
            s = s.ContainingSymbol;
        }
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
            .WithAttributeLists([
                GeneratedCodeAttribute(typeof(DocumentObjectGenerator)).AddAttributes(NonUserCodeAttribute())
            ])
            .WithSemicolonToken(
                Token(SyntaxKind.SemicolonToken));

        var attributes = GeneratedCodeAttribute(typeof(DocumentObjectGenerator)).AddAttributes(NonUserCodeAttribute());
        if (IsObsolete)
        {
            attributes = attributes.AddAttributes(ObsoleteAttribute());
        }

        var iconProperty = PropertyDeclaration(IdentifierName("global::System.Drawing.Bitmap"), Identifier("Icon"))
            .WithModifiers([Token(SyntaxKind.ProtectedKeyword), Token(SyntaxKind.OverrideKeyword)])
            .WithExpressionBody(
                ArrowExpressionClause(
                    InvocationExpression(
                            IdentifierName("global::ArchiToolkit.Grasshopper.ArchiToolkitResources.GetIcon"))
                        .WithArgumentList(ArgumentList([
                            Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(KeyName + ".png")))
                        ]))))
            .WithAttributeLists([
                GeneratedCodeAttribute(typeof(DocumentObjectGenerator)).AddAttributes(NonUserCodeAttribute())
            ])
            .WithSemicolonToken(
                Token(SyntaxKind.SemicolonToken));

        var classSyntax = ClassDeclaration(RealClassName)
            .WithModifiers(
            [
                Token(SyntaxKind.PublicKeyword),
                Token(SyntaxKind.SealedKeyword),
                Token(SyntaxKind.PartialKeyword)
            ])
            .WithAttributeLists([attributes])
            .WithMembers([guidProperty, iconProperty]);

        if (Exposure is not null && int.TryParse(Exposure, out var exposure))
        {
            var exposureProperty = PropertyDeclaration(IdentifierName("global::Grasshopper.Kernel.GH_Exposure"),
                    Identifier("Exposure"))
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.OverrideKeyword)))
                .WithExpressionBody(ArrowExpressionClause(CastExpression(
                    IdentifierName("global::Grasshopper.Kernel.GH_Exposure"), ParenthesizedExpression(LiteralExpression(
                        SyntaxKind.NumericLiteralExpression, Literal(exposure))))))
                .WithAttributeLists([
                    GeneratedCodeAttribute(typeof(DocumentObjectGenerator)).AddAttributes(NonUserCodeAttribute())
                ])
                .WithSemicolonToken(
                    Token(SyntaxKind.SemicolonToken));
            classSyntax = classSyntax.AddMembers([exposureProperty]);
        }

        var item = NamespaceDeclaration(NameSpace)
            .WithMembers([ModifyClass(classSyntax)]);

        context.AddSource(Id.ToString("N") + ".g.cs", item.NodeToString());
    }
}