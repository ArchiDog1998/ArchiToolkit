// ReSharper disable RedundantUsingDirective
using System;
#pragma warning disable CS9113 // Parameter is unread.

namespace ArchiToolkit.Grasshopper;

/// <summary>
/// The obj names to make.
/// </summary>
/// <param name="name"></param>
/// <param name="nickName"></param>
/// <param name="description"></param>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
internal class ObjNamesAttribute(string name, string nickName, string description) : Attribute;