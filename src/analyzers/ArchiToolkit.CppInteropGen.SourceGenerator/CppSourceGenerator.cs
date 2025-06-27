using System.Collections.Immutable;
using ArchiToolkit.RoslynHelper.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using static ArchiToolkit.RoslynHelper.Extensions.SyntaxExtensions;

namespace ArchiToolkit.CppInteropGen.SourceGenerator;

[Generator(LanguageNames.CSharp)]
public class CppSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var options = context.AnalyzerConfigOptionsProvider;

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

        context.RegisterSourceOutput(cppFiles.Collect().Combine(compilation.Combine(options)), Generate);
    }

    private static void Generate(SourceProductionContext content, (ImmutableArray<(string Path, SourceText? text)> Left, (Compilation compilation, AnalyzerConfigOptionsProvider option) Right) items)
    {
        var isInternal = items.Right.option.GlobalOptions.TryGetValue("build_property.CppInteropGen_Accessibility", out var value)
            && value.ToLower() is "internal";
        var compilation = items.Right.compilation;
        var assemblyName = compilation.AssemblyName;

        foreach (var (path, text) in items.Left)
        {
            if (text is null) continue;
            var fileName = Path.GetFileNameWithoutExtension(path);
            var className = fileName.Substring(0, fileName.Length - 2);

            var nameSpace = assemblyName?.EndsWith(".Wrapper") ?? false ? assemblyName : assemblyName + ".Wrapper";
            var node = NamespaceDeclaration(nameSpace)
                .WithMembers([new CppClassGenerator(text, className, isInternal).Generate()]);

            content.AddSource($"{className}.g.cs", node.NodeToString());
        }
    }
}