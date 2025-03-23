// ReSharper disable RedundantUsingDirective
using System;

#pragma warning disable CS9113 // Parameter is unread.

namespace ArchiToolkit.Grasshopper;

/// <summary>
///     The document objects.
/// </summary>
/// <param name="recognizeName">for localization</param>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Struct)]
internal class DocObjAttribute(string recognizeName = "") : Attribute;