// ReSharper disable RedundantUsingDirective
using System;
using System.Diagnostics;

#pragma warning disable CS9113 // Parameter is unread.

namespace ArchiToolkit.Grasshopper;

/// <summary>
/// if the input data is a field. the data must be <see langword="ref"/>.
/// </summary>
/// <param name="saveToFile">if it should be saved into the file</param>
[AttributeUsage(AttributeTargets.Parameter)]
[Conditional(Constant.KeepAttributes)]
internal class ObjFieldAttribute(bool saveToFile = false) : Attribute;