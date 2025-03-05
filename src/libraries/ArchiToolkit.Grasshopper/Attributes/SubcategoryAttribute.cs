using System.Diagnostics;
#pragma warning disable CS9113 // Parameter is unread.

namespace ArchiToolkit.Grasshopper;

/// <summary>
/// The subcategory of it.
/// </summary>
/// <param name="name"></param>
/// <param name="recognizeName">for localization</param>
[Conditional(Constant.KeepAttributes)]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method)]
public class SubcategoryAttribute(string name, string recognizeName = "" ) : Attribute;