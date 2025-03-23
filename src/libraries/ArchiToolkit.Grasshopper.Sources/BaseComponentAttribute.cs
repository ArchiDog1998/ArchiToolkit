// ReSharper disable RedundantUsingDirective
using System;
using Grasshopper.Kernel;

namespace ArchiToolkit.Grasshopper;

/// <summary>
/// For specific the parent class.
/// </summary>
/// <typeparam name="T"></typeparam>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Assembly)]
internal class BaseComponentAttribute<T> : Attribute
    where T : IGH_Component;