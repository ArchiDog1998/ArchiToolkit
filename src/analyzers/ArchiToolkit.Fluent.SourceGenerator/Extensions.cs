namespace ArchiToolkit.Fluent.SourceGenerator;

internal static class Extensions
{
    public static MethodDeclarationSyntax AddTypeParameters(this MethodDeclarationSyntax method,
        TypeName type)
    {
        var typeParameters = type.GetParameterSyntaxes().ToArray();
        if (typeParameters.Length == 0) return method;
        method = method.WithTypeParameterList(TypeParameterList([..typeParameters]));
        var constraints =type.GetParameterConstraintClauses().ToArray();
        if (constraints.Length == 0) return method;
        return method.WithConstraintClauses([..constraints]);
    }
}