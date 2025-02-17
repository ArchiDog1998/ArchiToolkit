using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ArchiToolkit.InterpolatedParser.SourceGenerator;

[Generator(LanguageNames.CSharp)]
public class InitGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var methodDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => node is MethodDeclarationSyntax,
                transform: static (context, _) => (MethodDeclarationSyntax)context.Node)
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
            context.AddSource($"{name}.g.cs", reader.ReadToEnd());
        }
    }
}