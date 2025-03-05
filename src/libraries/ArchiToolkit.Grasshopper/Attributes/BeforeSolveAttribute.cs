using System.Diagnostics;
using Grasshopper.Kernel;

#pragma warning disable CS9113 // Parameter is unread.

namespace ArchiToolkit.Grasshopper;

/// <summary>
/// The <see cref="GH_Component.BeforeSolveInstance"/> method to use, you can add the field by ref into it.
/// </summary>
/// <param name="methodName"></param>
[AttributeUsage(AttributeTargets.Method)]
[Conditional(Constant.KeepAttributes)]
public class BeforeSolveAttribute(string methodName) : Attribute;