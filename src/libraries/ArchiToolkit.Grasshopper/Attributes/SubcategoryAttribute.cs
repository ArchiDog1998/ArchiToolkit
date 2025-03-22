using System.Diagnostics;
#pragma warning disable CS9113 // Parameter is unread.

namespace ArchiToolkit.Grasshopper;

/// <summary>
/// The subcategory of it.
/// </summary>
/// <param name="recognizeName">for localization</param>
[Conditional(Constant.KeepAttributes)]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Assembly)]
public class SubcategoryAttribute(string recognizeName) : Attribute;