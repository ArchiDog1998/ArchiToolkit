using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ArchiToolkit.InterpolatedParser.SourceGenerator;

[Generator(LanguageNames.CSharp)]
public class InitGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var methodDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node is MethodDeclarationSyntax,
                static (context, _) => (MethodDeclarationSyntax)context.Node)
            .Collect();

        context.RegisterSourceOutput(methodDeclarations, Generate);
    }

    private static void Generate(SourceProductionContext context, ImmutableArray<MethodDeclarationSyntax> methods)
    {
        var assembly = typeof(InitGenerator).Assembly;
        foreach (var name in assembly.GetManifestResourceNames())
        {
            using var stream = assembly.GetManifestResourceStream(name);
            if (stream == null) continue;

            using var reader = new StreamReader(stream);

            var root = CSharpSyntaxTree.ParseText(reader.ReadToEnd(),
                    new CSharpParseOptions(preprocessorSymbols: ["NET7_0_OR_GREATER", "NETCOREAPP", "NETFRAMEWORK"]))
                .GetRoot();
            var updateRoot = new GeneratedRewriter(typeof(InitGenerator)).Visit(root);
            context.AddSource($"{name}.g.cs", updateRoot.NodeToString());
        }
    }
}
