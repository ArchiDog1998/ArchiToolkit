using Microsoft.CodeAnalysis;

namespace ArchiToolkit.RoslynHelper;

public readonly struct MethodSignature(IMethodSymbol methodSymbol) : IEquatable<MethodSignature>
{
    public string MethodName { get; } = methodSymbol.Name;

    public ITypeSymbol ContainingType { get; } = methodSymbol.IsExtensionMethod
        ? methodSymbol.Parameters[0].Type.OriginalDefinition
        : methodSymbol.ContainingType.OriginalDefinition;

    public ITypeSymbol[] ParameterTypes { get; } = methodSymbol.Parameters
        .Skip(methodSymbol.IsExtensionMethod ? 1 : 0)
        .Select(p => p.Type.OriginalDefinition)
        .ToArray();

    public RefKind[] RefKinds { get; } = methodSymbol.Parameters
        .Skip(methodSymbol.IsExtensionMethod ? 1 : 0)
        .Select(i => i.RefKind)
        .ToArray();

    public int TypeArgumentsCount { get; } =
        methodSymbol.TypeArguments.Length + methodSymbol.ContainingType.TypeArguments.Length;

    public bool Equals(MethodSignature other)
    {
        if (!MethodName.Equals(other.MethodName)) return false;
        if (!TypeArgumentsCount.Equals(other.TypeArgumentsCount)) return false;
        if (!ContainingType.Equals(other.ContainingType, SymbolEqualityComparer.Default)) return false;
        if (!ParameterTypes.Length.Equals(other.ParameterTypes.Length)) return false;
        for (var i = 0; i < ParameterTypes.Length; i++)
        {
            var thisType = ParameterTypes[i];
            var otherType = other.ParameterTypes[i];
            if (thisType.TypeKind == TypeKind.TypeParameter
                && otherType.TypeKind == TypeKind.TypeParameter)
                continue;

            if (RefKinds[i] != other.RefKinds[i]) return false;
            if (!thisType.Equals(otherType, SymbolEqualityComparer.Default)) return false;
        }

        return true;
    }

    public override bool Equals(object? obj)
    {
        return obj is MethodSignature other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = MethodName.GetHashCode();
            hashCode = (hashCode * 397) ^ ParameterTypes.Length.GetHashCode();
            hashCode = (hashCode * 397) ^ TypeArgumentsCount.GetHashCode();
            return hashCode;
        }
    }
}