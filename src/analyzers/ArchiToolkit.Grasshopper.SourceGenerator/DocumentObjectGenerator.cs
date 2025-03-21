using System.Collections.Immutable;
using System.Text;
using ArchiToolkit.RoslynHelper.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ArchiToolkit.Grasshopper.SourceGenerator;

[Generator(LanguageNames.CSharp)]
public class DocumentObjectGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var attributeTypes =
            context.SyntaxProvider.ForAttributeWithMetadataName("ArchiToolkit.Grasshopper.DocObjAttribute",
                static (node, _) => node is BaseTypeDeclarationSyntax,
                static (context, token) => new TypeGenerator(context.TargetSymbol));

        var attributeMethods =
            context.SyntaxProvider.ForAttributeWithMetadataName("ArchiToolkit.Grasshopper.DocObjAttribute",
                static (node, _) => node is BaseMethodDeclarationSyntax method &&
                                    method.Modifiers.Any(m => m.IsKind(SyntaxKind.StaticKeyword)),
                static (context, token) => new MethodGenerator(context.TargetSymbol));

        var items = attributeTypes.Collect().Combine(attributeMethods.Collect()).Combine(context.CompilationProvider);
        context.RegisterSourceOutput(items, Generate);
    }

    private static void Generate(SourceProductionContext context,
        ((ImmutableArray<TypeGenerator> Types, ImmutableArray<MethodGenerator> Methods) Items,
            Compilation Compilation) arg)
    {
        var assembly = arg.Compilation.Assembly;
        var types = arg.Items.Types;
        var methods = arg.Items.Methods;

        var builder = new StringBuilder();
        var baseComponent = GetBaseComponent(assembly.GetAttributes()) ?? "global::Grasshopper.Kernel.GH_Component";
        builder.AppendLine(baseComponent);

        foreach (var type in types)
        {
            type.Assembly = assembly;
            builder.AppendLine(type.ToString());
        }

        foreach (var method in methods)
        {
            method.Assembly = assembly;
            method.GlobalBaseComponent = baseComponent;
            method.GenerateSource(context);
        }

        context.AddSource("Test.cs", builder.ToString());
    }

    public static string? GetBaseComponent(IEnumerable<AttributeData> attributes)
    {
        return (from type in attributes.Select(attribute => attribute.AttributeClass).OfType<INamedTypeSymbol>()
            where type.IsGenericType
            where type.ConstructUnboundGenericType().GetName().FullName is
                "global::ArchiToolkit.Grasshopper.BaseComponentAttribute<>"
            select type.TypeArguments[0].GetName().FullName).FirstOrDefault();
    }
}