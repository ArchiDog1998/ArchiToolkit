﻿using System.Diagnostics;
using Grasshopper.Kernel;
#pragma warning disable CS9113 // Parameter is unread.

namespace ArchiToolkit.Grasshopper;

/// <summary>
/// Your custom exposure.
/// </summary>
/// <param name="exposure"></param>
[Conditional(Constant.KeepAttributes)]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method)]
public class ExposureAttribute(GH_Exposure exposure) : Attribute;