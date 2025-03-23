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

    protected virtual bool NeedId => false;
    public string NameSpace => Symbol.ContainingNamespace.ToString();
    protected abstract string IdName { get; }

    public abstract string ClassName { get; }

    public string RealClassName => ToRealName(ClassName);

    protected abstract char IconType { get; }

    public string KeyName => string.IsNullOrEmpty(field) ? NameSpace + "." + ToRealNameNoTags(ClassName) : field;

    public static string BaseCategory { get; set; } = null!;
    public static string BaseSubcategory { get; set; } = null!;
    public static string? BaseAttribute { get; set; }
    public string? Category { get; set; }
    public string? Subcategory { get; set; }

    public string? Exposure { get; }
    public bool IsObsolete { get; }

    public Guid Id => StringToGuid(IdName);

    internal static List<string> Icons { get; } = [];
    internal static Dictionary<string, string> Translations { get; } = [];

    private string ToRealNameNoTags(string className)
    {
        return NeedId ? className + "_" + Id.ToString("N").Substring(0, 8) : className;
    }

    protected string ToRealName(string name)
    {
        name = ToRealNameNoTags(name);
        return IsObsolete ? name + "_OBSOLETE" : name;
    }

    private static Guid StringToGuid(string input)
    {
        using var sha1 = SHA1.Create();
        var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
        var guidBytes = new byte[16];
        Array.Copy(hash, guidBytes, 16);
        return new Guid(guidBytes); // Use the first 16 bytes
    }

    public sealed override string ToString()
    {
        return IdName;
    }

    protected abstract ClassDeclarationSyntax ModifyClass(ClassDeclarationSyntax classSyntax);

    public void GenerateSource(SourceProductionContext context)
    {
        var keyField = FieldDeclaration(
                VariableDeclaration(PredefinedType(Token(SyntaxKind.StringKeyword))).WithVariables(
                [
                    VariableDeclarator(Identifier("ResourceKey")).WithInitializer(
                        EqualsValueClause(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(KeyName))))
                ]))
            .WithAttributeLists([GeneratedCodeAttribute(typeof(BasicGenerator))])
            .WithModifiers(
                TokenList(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.ConstKeyword)));

        var guidProperty = PropertyDeclaration(IdentifierName("global::System.Guid"), Identifier("ComponentGuid"))
            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.OverrideKeyword)))
            .WithExpressionBody(ArrowExpressionClause(ImplicitObjectCreationExpression().WithArgumentList(ArgumentList(
            [
                Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(Id.ToString("D"))))
            ]))))
            .WithAttributeLists([
                GeneratedCodeAttribute(typeof(DocumentObjectGenerator)).AddAttributes(NonUserCodeAttribute())
            ])
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

        var attributes = GeneratedCodeAttribute(typeof(BasicGenerator)).AddAttributes(NonUserCodeAttribute());
        if (IsObsolete) attributes = attributes.AddAttributes(ObsoleteAttribute());

        var iconProperty = PropertyDeclaration(IdentifierName("global::System.Drawing.Bitmap"), Identifier("Icon"))
            .WithModifiers([Token(SyntaxKind.ProtectedKeyword), Token(SyntaxKind.OverrideKeyword)])
            .WithExpressionBody(ArrowExpressionClause(InvocationExpression(
                    IdentifierName("global::ArchiToolkit.Grasshopper.ArchiToolkitResources.GetIcon"))
                .WithArgumentList(ArgumentList([
                    Argument(BinaryExpression(SyntaxKind.AddExpression,
                        BinaryExpression(SyntaxKind.AddExpression, LiteralExpression(SyntaxKind.StringLiteralExpression,
                                Literal(Symbol.ContainingAssembly.Name + ".Icons.")),
                            IdentifierName("ResourceKey")),
                        LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(".png"))))
                ]))))
            .WithAttributeLists([
                GeneratedCodeAttribute(typeof(BasicGenerator)).AddAttributes(NonUserCodeAttribute())
            ])
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

        var classSyntax = ClassDeclaration(RealClassName)
            .WithModifiers(
            [
                Token(SyntaxKind.PublicKeyword),
                Token(SyntaxKind.SealedKeyword),
                Token(SyntaxKind.PartialKeyword)
            ])
            .WithAttributeLists([attributes])
            .WithMembers([keyField, guidProperty, iconProperty]);

        if (Exposure is not null && int.TryParse(Exposure, out var exposure))
        {
            var exposureProperty = PropertyDeclaration(IdentifierName("global::Grasshopper.Kernel.GH_Exposure"),
                    Identifier("Exposure"))
                .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.OverrideKeyword)))
                .WithExpressionBody(ArrowExpressionClause(CastExpression(
                    IdentifierName("global::Grasshopper.Kernel.GH_Exposure"), ParenthesizedExpression(LiteralExpression(
                        SyntaxKind.NumericLiteralExpression, Literal(exposure))))))
                .WithAttributeLists([
                    GeneratedCodeAttribute(typeof(BasicGenerator)).AddAttributes(NonUserCodeAttribute())
                ])
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

            classSyntax = classSyntax.AddMembers(exposureProperty);
        }

        var item = NamespaceDeclaration(NameSpace)
            .WithMembers([ModifyClass(classSyntax)]);

        context.AddSource(RealClassName + ".g.cs", item.NodeToString());
        Icons.Add(IconType + KeyName);
    }

    public static InvocationExpressionSyntax GetArgumentRawString(string key, string value)
    {
        Translations[key] = value;
        return GetArgumentString(Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(key))));
    }

    public InvocationExpressionSyntax GetArgumentKeyedString(string key, string value)
    {
        Translations[KeyName + key] = value;
        return GetArgumentString(Argument(BinaryExpression(SyntaxKind.AddExpression,
            IdentifierName("ResourceKey"), LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(key)))));
    }

    private static InvocationExpressionSyntax GetArgumentString(ArgumentSyntax argument)
    {
        return InvocationExpression(
                IdentifierName("global::ArchiToolkit.Grasshopper.ArchiToolkitResources.Get"))
            .WithArgumentList(ArgumentList([argument]));
    }
}