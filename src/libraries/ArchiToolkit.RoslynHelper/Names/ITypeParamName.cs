using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ArchiToolkit.RoslynHelper.Names;

public interface ITypeParamName
{
    TypeParameterSyntax Syntax { get; }
    string SyntaxName { get; }
    TypeParameterConstraintClauseSyntax? ConstraintClause { get; }
}