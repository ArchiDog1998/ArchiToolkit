using ArchiToolkit.RoslynHelper.Extensions;
using ArchiToolkit.RoslynHelper.Names;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ArchiToolkit.Grasshopper.SourceGenerator;

public class TypeGenerator : BasicGenerator
{
    public readonly TypeName Name;
    protected override string IdName => Name.FullName;

    protected override string ClassName => "Param_" + Name.Name;
    public string RealGooName => ToRealName("GH_" + Name.Name);

    protected override ClassDeclarationSyntax ModifyClass(ClassDeclarationSyntax classSyntax)
    {
        return classSyntax;
    }

    public TypeGenerator(ISymbol symbol) : base(symbol)
    {
        if (symbol is not ITypeSymbol typeSymbol)
            throw new ArgumentException("Symbol is not a type symbol");
        Name = typeSymbol.GetName();
    }
}