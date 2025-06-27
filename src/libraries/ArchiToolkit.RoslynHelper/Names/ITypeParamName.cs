using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ArchiToolkit.RoslynHelper.Names;

/// <summary>
/// The type param name.
/// </summary>
public interface ITypeParamName
{
    /// <summary>
    /// Syntax
    /// </summary>
    TypeParameterSyntax Syntax { get; }

    /// <summary>
    /// Syntax name
    /// </summary>
    string SyntaxName { get; }

    /// <summary>
    /// THe Constraint clause.
    /// </summary>
    TypeParameterConstraintClauseSyntax? ConstraintClause { get; }
}