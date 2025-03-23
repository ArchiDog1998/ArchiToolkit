// ReSharper disable RedundantUsingDirective
using System;
using Grasshopper.Kernel.Types;

namespace ArchiToolkit.Grasshopper;

/// <summary>
/// For the case that this type is a geometric type.
/// Please use <see cref="DocObjAttribute"/> first.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
internal class BaseGooAttribute<T> : Attribute where T : IGH_Goo;