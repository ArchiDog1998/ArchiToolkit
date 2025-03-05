using System.Diagnostics;

#pragma warning disable CS9113 // Parameter is unread.

namespace ArchiToolkit.Grasshopper;

/// <summary>
///     The document objects.
/// </summary>
/// <param name="recognizeName">for localization</param>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.ReturnValue
| AttributeTargets.Class | AttributeTargets.Struct)]
[Conditional(Constant.KeepAttributes)]
public class DocObjAttribute(string recognizeName = "") : Attribute;