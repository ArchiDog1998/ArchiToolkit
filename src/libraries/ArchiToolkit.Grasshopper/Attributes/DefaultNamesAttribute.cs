using System.Diagnostics;
#pragma warning disable CS9113 // Parameter is unread.

namespace ArchiToolkit.Grasshopper;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter)]
[Conditional(Constant.KeepGrasshopper)]
public class DefaultNamesAttribute(string name, string nickName, string description) : Attribute;