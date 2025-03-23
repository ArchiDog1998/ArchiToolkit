// ReSharper disable RedundantUsingDirective
using System;
using Grasshopper.Kernel;
#pragma warning disable CS9113 // Parameter is unread.

namespace ArchiToolkit.Grasshopper;

/// <summary>
/// Your custom exposure.
/// </summary>
/// <param name="exposure"></param>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method)]
internal class ExposureAttribute(GH_Exposure exposure) : Attribute;