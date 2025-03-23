using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using ArchiToolkit.RoslynHelper.Extensions;
using ArchiToolkit.RoslynHelper.Names;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ArchiToolkit.Grasshopper.SourceGenerator;

[Generator(LanguageNames.CSharp)]
[SuppressMessage("MicrosoftCodeAnalysisCorrectness", "RS1035:Do not use APIs banned for analyzers")]
public class DocumentObjectGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var attributeTypes =
            context.SyntaxProvider.ForAttributeWithMetadataName("ArchiToolkit.Grasshopper.DocObjAttribute",
                static (node, _) => node is BaseTypeDeclarationSyntax,
                static (context, _) => new TypeGenerator(context.TargetSymbol));

        var attributeMethods =
            context.SyntaxProvider.ForAttributeWithMetadataName("ArchiToolkit.Grasshopper.DocObjAttribute",
                static (node, _) => node is BaseMethodDeclarationSyntax method &&
                                    method.Modifiers.Any(m => m.IsKind(SyntaxKind.StaticKeyword)),
                static (context, _) => new MethodGenerator(context.TargetSymbol));

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

        var baseComponent = GetBaseComponent(assembly.GetAttributes())
                            ?? arg.Compilation.GetTypeByMetadataName("Grasshopper.Kernel.GH_Component");
        var baseCategory = GetBaseCategory(assembly.GetAttributes()) ?? assembly.Name;
        var baseSubcategory = GetBaseSubcategory(assembly.GetAttributes()) ?? assembly.Name;
        var baseAttribute = GetBaseAttribute(assembly.GetAttributes());

        var typeAndClasses = new Dictionary<string, string>();

        BasicGenerator.Translations.Clear();
        BasicGenerator.Icons.Clear();
        BasicGenerator.BaseCategory = baseCategory;
        BasicGenerator.BaseSubcategory = baseSubcategory;
        TypeGenerator.BaseGoo = arg.Compilation.GetTypeByMetadataName("Grasshopper.Kernel.Types.GH_Goo`1")!;
        foreach (var type in types)
        {
            type.GenerateSource(context);

            var key = type.Name.FullName;
            var className = "global::" + type.NameSpace + "." + type.RealClassName;
            if (!typeAndClasses.ContainsKey(key)) typeAndClasses.Add(key, className);
            key = className + ".Goo";
            if (!typeAndClasses.ContainsKey(key)) typeAndClasses.Add(key, className);
        }

        foreach (var item in GetAllParams(arg.Compilation.GlobalNamespace)
                     .OrderByDescending(i => i.Score))
        foreach (var k in item.Keys)
        {
            var key = k.GetName().FullName;
            if (typeAndClasses.ContainsKey(key)) continue;
            typeAndClasses.Add(key, item.Type.GetName().FullName);
        }

        MethodGenerator.NeedIdNames = methods
            .GroupBy(method => method.ClassName)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToArray();

        MethodParamItem.TypeDictionary = typeAndClasses;

        BasicGenerator.BaseAttribute = baseAttribute;
        MethodGenerator.GlobalBaseComponent = baseComponent!;

        foreach (var method in methods) method.GenerateSource(context);

        if (GetCsprojDirectory(assembly) is { } dir)
        {
            GenerateTranslations(dir.FullName);
            GenerateIcons(dir.FullName);
        }
    }

    private static void GenerateTranslations(string directory)
    {
        directory = Path.Combine(directory, "l10n");
        if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
        try
        {
            ResxManager.Generate(Path.Combine(directory, "ArchiToolkit.Resources.resx"), BasicGenerator.Translations);
        }
        catch
        {
            // ignored
        }
    }

    private static void GenerateIcons(string directory)
    {
        directory = Path.Combine(directory, "Icons");
        if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
        foreach (var file in Directory.EnumerateFiles(directory, "*.png"))
            try
            {
                var fileInfo = new FileInfo(file);
                if (fileInfo.Length != 120) continue;
                fileInfo.Delete();
            }
            catch
            {
                // ignored
            }

        var assembly = typeof(DocumentObjectGenerator).Assembly;
        foreach (var icon in BasicGenerator.Icons)
        {
            var iconType = icon[0];
            var fileName = Path.Combine(directory, string.Concat(icon.Substring(1), ".png"));
            if (File.Exists(fileName)) continue;
            try
            {
                using var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                switch (iconType)
                {
                    case 'P':
                        assembly.GetManifestResourceStream("ArchiToolkit.Grasshopper.SourceGenerator.Icons.Red.png")
                            ?.CopyTo(fileStream);
                        break;
                    case 'C':
                        assembly.GetManifestResourceStream("ArchiToolkit.Grasshopper.SourceGenerator.Icons.Blue.png")
                            ?.CopyTo(fileStream);
                        break;
                    default:
                        assembly.GetManifestResourceStream("ArchiToolkit.Grasshopper.SourceGenerator.Icons.White.png")
                            ?.CopyTo(fileStream);
                        break;
                }
            }
            catch
            {
                // ignored
            }
        }
    }

    private static DirectoryInfo? GetCsprojDirectory(IAssemblySymbol assembly)
    {
        var fileCs = assembly.Locations
            .Where(l => l.Kind is LocationKind.SourceFile)
            .Select(i => i.GetLineSpan().Path)
            .FirstOrDefault(File.Exists);

        if (string.IsNullOrEmpty(fileCs)) return null;

        var directory = new FileInfo(fileCs).Directory;

        while (directory is not null
               && !directory.EnumerateFiles("*.csproj").Any())
            directory = directory.Parent;

        return directory;
    }

    public static ITypeSymbol? GetBaseComponent(IEnumerable<AttributeData> attributes)
    {
        return (from type in attributes.Select(attribute => attribute.AttributeClass).OfType<INamedTypeSymbol>()
            where type.IsGenericType
            where type.ConstructUnboundGenericType().GetName().FullName is
                "global::ArchiToolkit.Grasshopper.BaseComponentAttribute<>"
            select type.TypeArguments[0]).FirstOrDefault();
    }

    public static string? GetBaseAttribute(IEnumerable<AttributeData> attributes)
    {
        return GetTypeAttribute(attributes, "global::ArchiToolkit.Grasshopper.ObjAttrAttribute<>")?.FullName;
    }

    public static TypeName? GetTypeAttribute(IEnumerable<AttributeData> attributes, string name)
    {
        foreach (var attr in attributes)
        {
            if (attr.AttributeClass is not { } attributeClass) continue;
            if (!attributeClass.IsGenericType) continue;
            if (attributeClass.TypeArguments.Length < 1) continue;
            if (attributeClass.ConstructUnboundGenericType().GetName().FullName != name) continue;
            return attributeClass.TypeArguments[0].GetName();
        }

        return null;
    }

    public static string? GetBaseCategory(IEnumerable<AttributeData> attributes)
    {
        return GetBaseCate(attributes, "global::ArchiToolkit.Grasshopper.CategoryAttribute");
    }

    public static string? GetBaseSubcategory(IEnumerable<AttributeData> attributes)
    {
        return GetBaseCate(attributes, "global::ArchiToolkit.Grasshopper.SubcategoryAttribute");
    }

    private static string? GetBaseCate(IEnumerable<AttributeData> attributes, string attributeFullname)
    {
        foreach (var attr in attributes)
        {
            if (attr.AttributeClass is not { } attributeClass) continue;
            if (attributeClass.GetName().FullName != attributeFullname) continue;
            if (attr.ConstructorArguments.Length is 0) continue;
            return attr.ConstructorArguments[0].Value?.ToString();
        }

        return null;
    }

    private static IEnumerable<GhParamItem> GetAllParams(INamespaceSymbol namespaceSymbol)
    {
        foreach (var type in GetTypesFromNamespace(namespaceSymbol))
        {
            if (type.IsAbstract) continue;
            if (type.IsGenericType) continue;
            if (type.DeclaredAccessibility is not Accessibility.Public) continue;
            if (!type.AllInterfaces.Any(i => i.GetName().FullName is "global::Grasshopper.Kernel.IGH_Param")) continue;
            if (type.Name.EndsWith("OBSOLETE", StringComparison.OrdinalIgnoreCase)) continue;
            yield return new GhParamItem(type);
        }
    }

    private static IEnumerable<INamedTypeSymbol> GetTypesFromNamespace(INamespaceSymbol namespaceSymbol)
    {
        return namespaceSymbol.GetTypeMembers()
            .SelectMany(type => (IEnumerable<INamedTypeSymbol>) [type, ..GetNestedTypes(type)])
            .Concat(namespaceSymbol.GetNamespaceMembers().SelectMany(GetTypesFromNamespace));
    }

    private static IEnumerable<INamedTypeSymbol> GetNestedTypes(INamedTypeSymbol typeSymbol)
    {
        return typeSymbol.GetTypeMembers()
            .SelectMany(nestTypes => (IEnumerable<INamedTypeSymbol>) [nestTypes, ..GetNestedTypes(nestTypes)]);
    }
}