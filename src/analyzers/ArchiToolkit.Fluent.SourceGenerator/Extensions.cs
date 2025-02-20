using ArchiToolkit.RoslynHelper.Extensions;
using ArchiToolkit.RoslynHelper.Names;

namespace ArchiToolkit.Fluent.SourceGenerator;

internal static class Extensions
{
    public static MethodDeclarationSyntax AddTypeParameters(this MethodDeclarationSyntax method,
        TypeName type)
    {
        var typeParameters = type.TypeParameters.ToArray();
        if (typeParameters.Length == 0) return method;
        method = method.WithTypeParameterList(TypeParameterList([..typeParameters.Select(t => t.Syntax)]));
        var constraints = typeParameters
            .Select(t => t.ConstraintClause).Where(i => i is not null).ToArray();
        if (constraints.Length == 0) return method;
        return method.WithConstraintClauses([..constraints!]);
    }
}