using System.Diagnostics;
#pragma warning disable CS9113 // Parameter is unread.

namespace ArchiToolkit.Grasshopper;

/// <summary>
/// if the input data is a field.
/// </summary>
/// <param name="saveToFile">if it should be saved into the file</param>
[AttributeUsage(AttributeTargets.Parameter)]
[Conditional(Constant.KeepAttributes)]
public class ObjFieldAttribute(bool saveToFile = false) : Attribute;