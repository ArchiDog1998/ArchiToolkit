﻿using System.Diagnostics;
using Grasshopper.Kernel;
// ReSharper disable UnusedTypeParameter

namespace ArchiToolkit.Grasshopper;

/// <summary>
/// Add your custom attribute of the target.
/// </summary>
/// <typeparam name="T"></typeparam>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Assembly)]
[Conditional(Constant.KeepAttributes)]
public class ObjAttrAttribute<T> : Attribute
    where T : IGH_Attributes;