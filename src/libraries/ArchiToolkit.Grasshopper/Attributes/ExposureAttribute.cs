// ReSharper disable RedundantUsingDirective
using System;
using System.Diagnostics;
using Grasshopper.Kernel;
#pragma warning disable CS9113 // Parameter is unread.

namespace ArchiToolkit.Grasshopper;

/// <summary>
/// Your custom exposure.
/// </summary>
/// <param name="exposure"></param>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method)]
[Conditional(Constant.KeepAttributes)]
public class ExposureAttribute(GH_Exposure exposure) : Attribute;