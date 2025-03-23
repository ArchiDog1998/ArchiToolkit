// ReSharper disable RedundantUsingDirective
using System;

namespace ArchiToolkit.Grasshopper;

/// <summary>
/// Hidden attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
internal class HiddenAttribute : Attribute;