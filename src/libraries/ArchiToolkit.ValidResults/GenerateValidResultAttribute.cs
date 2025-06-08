using System.Diagnostics;

namespace ArchiToolkit.ValidResults;

[Conditional("GENERATE_VALID_RESULTS")]
[AttributeUsage(AttributeTargets.Class)]
public sealed class GenerateValidResultAttribute<T> : Attribute;