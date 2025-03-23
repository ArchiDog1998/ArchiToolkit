// ReSharper disable RedundantUsingDirective
using System;
using System.Diagnostics;

namespace ArchiToolkit.Grasshopper;

/// <summary>
/// Hidden attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
[Conditional(Constant.KeepAttributes)]
internal class HiddenAttribute : Attribute;