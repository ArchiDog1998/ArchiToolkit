using System.Diagnostics;
#pragma warning disable CS9113 // Parameter is unread.

namespace ArchiToolkit.Grasshopper;

/// <summary>
/// The category of it.
/// </summary>
/// <param name="recognizeName"></param>
[Conditional(Constant.KeepAttributes)]
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method)]
public class CategoryAttribute(string recognizeName) : Attribute;