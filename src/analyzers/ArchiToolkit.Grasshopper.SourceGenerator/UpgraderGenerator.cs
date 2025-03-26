using ArchiToolkit.RoslynHelper.Extensions;
using ArchiToolkit.RoslynHelper.Names;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static ArchiToolkit.RoslynHelper.Extensions.SyntaxExtensions;

namespace ArchiToolkit.Grasshopper.SourceGenerator;

public class UpgraderGenerator(string nameSpace, string className, TypeName toType, Guid guid, DateTime time)
{
    public static UpgraderGenerator? Create(MethodGenerator generator)
    {
        if (GetAttribute(generator.Symbol.GetAttributes()) is not {ConstructorArguments.Length:6 } attr) return null;
        var type = attr.AttributeClass!.TypeArguments[0].GetName();
        var year =attr.ConstructorArguments[0].Value as int? ?? 2025;
        var month =attr.ConstructorArguments[1].Value as int? ?? 1;
        var day =attr.ConstructorArguments[2].Value as int? ?? 1;
        var hour =attr.ConstructorArguments[3].Value as int? ?? 0;
        var minute =attr.ConstructorArguments[4].Value as int? ?? 0;
        var second =attr.ConstructorArguments[5].Value as int? ?? 0;
        return new (generator.NameSpace, generator.RealClassName, type, generator.Id, new DateTime(year, month, day, hour, minute, second));
    }


    public void GenerateSource(SourceProductionContext context)
    {
        var item = NamespaceDeclaration(nameSpace)
            .WithMembers([Generate(className, toType, guid, time)]);
        context.AddSource(nameSpace + "." + className + ".Upgrader.g.cs", item.NodeToString());
    }

    private static AttributeData? GetAttribute(IEnumerable<AttributeData> attributes)
    {
        foreach (var attr in attributes)
        {
            if (attr.AttributeClass is not { } attributeClass) continue;
            if (!attributeClass.IsGenericType) continue;
            if (attributeClass.TypeArguments.Length < 1) continue;
            if (attributeClass.ConstructUnboundGenericType().GetName().FullName
                is not "global::ArchiToolkit.Grasshopper.UpgradeToAttribute<>") continue;
            return attr;
        }

        return null;
    }
    private static ClassDeclarationSyntax Generate(string className,TypeName toType, Guid guid, DateTime time)
    {
        var id = guid.ToString("D");

        return ClassDeclaration("Upgrader_" + className)
            .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.SealedKeyword)))
            .WithParameterList(ParameterList())
            .WithBaseList(BaseList(
            [
                PrimaryConstructorBaseType(GenericName(Identifier("ComponentUpgrader"))
                        .WithTypeArgumentList(TypeArgumentList([IdentifierName(toType.FullName)])))
                    .WithArgumentList(ArgumentList(
                    [
                        Argument(ImplicitObjectCreationExpression().WithArgumentList(ArgumentList(
                        [
                            Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(id)))
                        ]))),
                        Argument(ImplicitObjectCreationExpression().WithArgumentList(ArgumentList(
                        [
                            Argument(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(time.Year))),
                            Argument(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(time.Month))),
                            Argument(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(time.Day))),
                            Argument(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(time.Hour))),
                            Argument(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(time.Minute))),
                            Argument(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(time.Second)))
                        ])))
                    ]))
            ]))
            .WithAttributeLists([
                GeneratedCodeAttribute(typeof(UpgraderGenerator)).AddAttributes(NonUserCodeAttribute())
            ]);
    }
}