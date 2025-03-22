using System.Diagnostics;

namespace ArchiToolkit.Grasshopper;

/// <summary>
/// Hidden attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
[Conditional(Constant.KeepAttributes)]
public class HiddenAttribute : Attribute;