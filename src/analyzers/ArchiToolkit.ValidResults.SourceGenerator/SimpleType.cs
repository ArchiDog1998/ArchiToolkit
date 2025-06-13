using ArchiToolkit.RoslynHelper.Extensions;
using Microsoft.CodeAnalysis;

namespace ArchiToolkit.ValidResults.SourceGenerator;

public class SimpleType(string fullName, string name, string nameSpace)
{
    public string FullName => fullName;
    public string Name => name;
    public string NameSpace => nameSpace;

    public SimpleType(ITypeSymbol  type) : this(type.GetName().FullName, type.Name, type.ContainingNamespace.ToDisplayString())
    {

    }
}