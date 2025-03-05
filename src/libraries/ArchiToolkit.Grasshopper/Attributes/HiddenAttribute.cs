using System.Diagnostics;

namespace ArchiToolkit.Grasshopper;

/// <summary>
/// Hidden attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
[Conditional(Constant.KeepAttributes)]
public class HiddenAttribute : Attribute;