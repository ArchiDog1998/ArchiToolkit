using System.Diagnostics;
#pragma warning disable CS9113 // Parameter is unread.

namespace ArchiToolkit.Grasshopper;

/// <summary>
/// The Parameter type name and type description.
/// </summary>
/// <param name="typeName"></param>
/// <param name="typeDesc"></param>
[AttributeUsage( AttributeTargets.Class | AttributeTargets.Struct)]
[Conditional(Constant.KeepAttributes)]
public class TypeDescAttribute(string typeName = "", string typeDesc = "") : Attribute;