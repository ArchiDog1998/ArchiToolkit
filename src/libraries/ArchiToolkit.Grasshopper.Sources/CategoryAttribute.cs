// ReSharper disable RedundantUsingDirective
using System;
#pragma warning disable CS9113 // Parameter is unread.

namespace ArchiToolkit.Grasshopper;

/// <summary>
/// The category of it.
/// </summary>
/// <param name="recognizeName"></param>
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method)]
internal class CategoryAttribute(string recognizeName) : Attribute;