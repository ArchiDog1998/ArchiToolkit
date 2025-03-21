using ArchiToolkit.RoslynHelper.Extensions;
using ArchiToolkit.RoslynHelper.Names;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ArchiToolkit.Grasshopper.SourceGenerator;

public class TypeGenerator : BasicGenerator
{
    private readonly TypeName _name;
    protected override string IdName => _name.FullName;

    public override string ClassName => "Param_" + _name.Name;

    protected override ClassDeclarationSyntax ModifyClass(ClassDeclarationSyntax classSyntax)
    {
        return classSyntax;
    }

    public TypeGenerator(ISymbol symbol) : base(symbol)
    {
        if (symbol is not ITypeSymbol typeSymbol)
            throw new ArgumentException("Symbol is not a type symbol");
        _name = typeSymbol.GetName();
    }
}