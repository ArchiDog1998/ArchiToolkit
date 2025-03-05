using System.Diagnostics;
#pragma warning disable CS9113 // Parameter is unread.

namespace ArchiToolkit.Grasshopper;

/// <summary>
/// Add the icon to it, which must be in the resource.
/// </summary>
/// <param name="resourceFullPath"></param>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Struct)]
[Conditional(Constant.KeepAttributes)]
public class IconAttribute(string resourceFullPath) : Attribute;