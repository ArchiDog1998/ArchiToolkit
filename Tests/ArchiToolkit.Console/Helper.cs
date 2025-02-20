using ArchiToolkit.Fluent;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ArchiToolkit.Console;

public static class Helper
{
    public static string GetFulTypeName(this Type type)
    {
        var compilation = CSharpCompilation.Create("Temp")
            .AddReferences(MetadataReference.CreateFromFile(type.Assembly.Location))
            .AddSyntaxTrees(CSharpSyntaxTree.ParseText($"class C {{ {type.FullName} field; }}"));

        var tree = compilation.SyntaxTrees.First();
        var model = compilation.GetSemanticModel(tree);

        var fieldDeclaration = tree.GetRoot()
            .DescendantNodes()
            .OfType<FieldDeclarationSyntax>()
            .First();

        var variableType = fieldDeclaration.Declaration.Type;
        var typeSymbol = model.GetSymbolInfo(variableType).Symbol as ITypeSymbol;

        return typeSymbol?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) ?? "Unknown";
    }

    /// <summary>
    /// <inheritdoc cref="TestClass{T}"/>
    /// <inheritdoc cref="global::ArchiToolkit.Console.TestClass{TTTT}" />
    /// </summary>
    /// <typeparam name="TT"></typeparam>
    public static void Test<TT>() where TT : struct
    {
        var a =
            new TestClass<TT>().AsFluent()
                .WithData(default(TT))
                .Result;
    }
}