﻿using System.Diagnostics;
using Grasshopper.Kernel;

#pragma warning disable CS9113 // Parameter is unread.

namespace ArchiToolkit.Grasshopper;

/// <summary>
/// The <see cref="GH_Component.AfterSolveInstance"/> method to use, you can add the field by ref into it.
/// </summary>
/// <param name="methodName"></param>
[AttributeUsage(AttributeTargets.Method)]
[Conditional(Constant.KeepAttributes)]
public class AfterSolveAttribute(string methodName) : Attribute;

/// <summary>
/// <inheritdoc cref="AfterSolveAttribute"/>
/// </summary>
/// <param name="methodName"><inheritdoc cref="AfterSolveAttribute"/></param>
/// <typeparam name="T">The type to get the data from</typeparam>
[AttributeUsage(AttributeTargets.Method)]
[Conditional(Constant.KeepAttributes)]
public class AfterSolveAttribute<T>(string methodName) : Attribute;