using System.Diagnostics;

namespace ArchiToolkit.Grasshopper;

/// <summary>
/// Optional input
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
[Conditional(Constant.KeepAttributes)]
public class OptionalAttribute : Attribute;