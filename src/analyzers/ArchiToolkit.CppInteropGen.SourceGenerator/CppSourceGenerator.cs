using System.Collections.Immutable;
using ArchiToolkit.RoslynHelper.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using static ArchiToolkit.RoslynHelper.Extensions.SyntaxExtensions;

namespace ArchiToolkit.CppInteropGen.SourceGenerator;

[Generator(LanguageNames.CSharp)]
public class CppSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var cppFiles = context.AdditionalTextsProvider
            .Where(file => file.Path.EndsWith(".cpp")
                           || file.Path.EndsWith(".h")
                           || file.Path.EndsWith(".hxx")
                           || file.Path.EndsWith(".cxx")
            )
            .Select((file, cancellationToken) =>
            {
                var text = file.GetText(cancellationToken);
                return (file.Path, text);
            });

        var compilation = context.CompilationProvider;

        context.RegisterSourceOutput(cppFiles.Collect().Combine(compilation), Generate);
    }

    private static void Generate(SourceProductionContext content,
        (ImmutableArray<(string Path, SourceText? text)> Left, Compilation Right) items)
    {
        var compilation = items.Right;
        var assemblyName = compilation.AssemblyName;

        foreach (var (path, text) in items.Left)
        {
            if (text is null) continue;
            var fileName = Path.GetFileNameWithoutExtension(path);
            var className = fileName.Substring(0, fileName.Length - 2);

            var node = NamespaceDeclaration(assemblyName + ".Wrapper")
                .WithMembers([new CppClassGenerator(text, className).Generate()]);

            content.AddSource($"{className}.g.cs", node.NodeToString());
        }
    }
}