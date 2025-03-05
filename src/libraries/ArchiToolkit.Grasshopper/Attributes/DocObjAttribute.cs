using System.Diagnostics;

#pragma warning disable CS9113 // Parameter is unread.

namespace ArchiToolkit.Grasshopper;

/// <summary>
///     The document objects.
/// </summary>
/// <param name="recognizeName"></param>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter)]
[Conditional(Constant.KeepGrasshopper)]
public class DocObjAttribute(string recognizeName = "") : Attribute;