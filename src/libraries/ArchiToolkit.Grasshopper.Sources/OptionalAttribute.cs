// ReSharper disable RedundantUsingDirective
using System;
using System.Diagnostics;

namespace ArchiToolkit.Grasshopper;

/// <summary>
/// Optional input
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
[Conditional(Constant.KeepAttributes)]
internal class OptionalAttribute : Attribute;