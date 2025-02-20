using ArchiToolkit.RoslynHelper.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace ArchiToolkit.RoslynHelper.Names;

/// <summary>
///
/// </summary>
public class TypeParamName: BaseName<ITypeSymbol>
{
    private readonly ITypeParameterSymbol _typeParameter;
    private readonly string _prefix;

    /// <summary>
    ///
    /// </summary>
    public TypeParameterSyntax Syntax => TypeParameter(Identifier(_prefix + _typeParameter.Name));

    /// <summary>
    ///
    /// </summary>
    public TypeParameterConstraintClauseSyntax? ConstraintClause
    {
        get
        {
            var constraints = new List<TypeParameterConstraintSyntax>();

            if (_typeParameter.HasReferenceTypeConstraint)
                constraints.Add(ClassOrStructConstraint(SyntaxKind.ClassConstraint));

            if (_typeParameter.HasValueTypeConstraint)
                constraints.Add(ClassOrStructConstraint(SyntaxKind.StructConstraint));

            if (_typeParameter.HasUnmanagedTypeConstraint)
                constraints.Add(TypeConstraint(IdentifierName("unmanaged")));

            if (_typeParameter.HasNotNullConstraint)
                constraints.Add(TypeConstraint(IdentifierName("notnull")));

            foreach (var constraintType in _typeParameter.ConstraintTypes)
                constraints.Add(TypeConstraint(ParseTypeName(constraintType.GetName().FullName)));

            if (_typeParameter.HasConstructorConstraint)
                constraints.Add(ConstructorConstraint());

            if (constraints.Count is 0) return null;

            return TypeParameterConstraintClause(
                IdentifierName(_prefix + _typeParameter.Name),
                SeparatedList(constraints)
            );
        }
    }


    internal TypeParamName(ITypeParameterSymbol symbol, string prefix) : base(symbol)
    {
        _typeParameter = symbol;
        _prefix = prefix;
    }
}