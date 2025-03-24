// ReSharper disable RedundantUsingDirective
using System;
using System.Diagnostics;
using Grasshopper.Kernel;
// ReSharper disable UnusedTypeParameter
#pragma warning disable CS9113 // Parameter is unread.

namespace ArchiToolkit.Grasshopper;

/// <summary>
/// You can't set <see cref="PersistentDataAttribute"/> when you are using this.
/// </summary>
/// <param name="id"></param>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
[Conditional(Constant.KeepAttributes)]
public class ParamTypeAttribute(string id): Attribute;

/// <summary>
/// You can specify the param here.
/// </summary>
/// <typeparam name="T"></typeparam>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
[Conditional(Constant.KeepAttributes)]
public class ParamTypeAttribute<T>: Attribute where T: IGH_Param;