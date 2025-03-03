namespace ArchiToolkit.Fluent.SourceGenerator;

public readonly struct MethodSignature(IMethodSymbol methodSymbol) : IEquatable<MethodSignature>
{
    private readonly string _methodName = methodSymbol.Name;

    private readonly ITypeSymbol[] _parameterTypes = methodSymbol.Parameters
        .Skip(methodSymbol.IsExtensionMethod ? 1 : 0)
        .Select(p => p.Type.OriginalDefinition)
        .ToArray();

    private readonly int _typeArgumentsCount =
        methodSymbol.TypeArguments.Length + methodSymbol.ContainingType.TypeArguments.Length;


    public bool Equals(MethodSignature other)
    {
        if (!_methodName.Equals(other._methodName)) return false;
        if (!_typeArgumentsCount.Equals(other._typeArgumentsCount)) return false;
        if (!_parameterTypes.Length.Equals(_parameterTypes.Length)) return false;
        for (var i = 0; i < _parameterTypes.Length; i++)
        {
            var thisType = _parameterTypes[i];
            var otherType = _parameterTypes[i];
            if (thisType.TypeKind == TypeKind.TypeParameter
                && otherType.TypeKind == TypeKind.TypeParameter)
            {
                continue;
            }
            if(!thisType.Equals(otherType, SymbolEqualityComparer.Default)) return false;
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
            var hashCode = _methodName.GetHashCode();
            hashCode = (hashCode * 397) ^ _parameterTypes.Length.GetHashCode();
            hashCode = (hashCode * 397) ^ _typeArgumentsCount.GetHashCode();
            return hashCode;
        }
    }
}