// ReSharper disable RedundantUsingDirective
using System;
using System.Diagnostics;
using Grasshopper.Kernel;

namespace ArchiToolkit.Grasshopper;

/// <summary>
/// For specific the parent class.
/// </summary>
/// <typeparam name="T"></typeparam>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Assembly)]
[Conditional(Constant.KeepAttributes)]
internal class BaseComponentAttribute<T> : Attribute
    where T : IGH_Component;