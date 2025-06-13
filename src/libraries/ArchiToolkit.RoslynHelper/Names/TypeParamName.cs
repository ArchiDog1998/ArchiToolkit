using ArchiToolkit.RoslynHelper.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace ArchiToolkit.RoslynHelper.Names;

/// <summary>
/// </summary>
public class TypeParamName : BaseName<ITypeParameterSymbol>, ITypeParamName
{
    internal TypeParamName(ITypeParameterSymbol symbol) : base(symbol)
    {
    }

    /// <summary>
    ///     Prefix
    /// </summary>
    public string Prefix { get; set; } = string.Empty;

    /// <summary>
    /// </summary>
    public TypeParameterSyntax Syntax => TypeParameter(Identifier(SyntaxName));

    public string SyntaxName => Prefix + Symbol.Name;

    /// <summary>
    /// </summary>
    public TypeParameterConstraintClauseSyntax? ConstraintClause
    {
        get
        {
            var constraints = new List<TypeParameterConstraintSyntax>();

            if (Symbol.HasReferenceTypeConstraint)
                constraints.Add(ClassOrStructConstraint(SyntaxKind.ClassConstraint));

            if (Symbol.HasUnmanagedTypeConstraint)
                constraints.Add(TypeConstraint(IdentifierName("unmanaged")));
            else if (Symbol.HasValueTypeConstraint)
                constraints.Add(ClassOrStructConstraint(SyntaxKind.StructConstraint));

            if (Symbol.HasNotNullConstraint)
                constraints.Add(TypeConstraint(IdentifierName("notnull")));

            foreach (var constraintType in Symbol.ConstraintTypes)
                constraints.Add(TypeConstraint(ParseTypeName(constraintType.GetName().FullName)));

            if (Symbol.HasConstructorConstraint)
                constraints.Add(ConstructorConstraint());

            if (constraints.Count is 0) return null;

            return TypeParameterConstraintClause(
                IdentifierName(Prefix + Symbol.Name),
                SeparatedList(constraints)
            );
        }
    }
}