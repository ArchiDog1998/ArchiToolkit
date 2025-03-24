// ReSharper disable RedundantUsingDirective
using System;
using System.Diagnostics;

#pragma warning disable CS9113 // Parameter is unread.

namespace ArchiToolkit.Grasshopper;

/// <summary>
/// The subcategory of it.
/// </summary>
/// <param name="recognizeName">for localization</param>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Assembly)]
[Conditional(Constant.KeepAttributes)]
public class SubcategoryAttribute(string recognizeName) : Attribute;