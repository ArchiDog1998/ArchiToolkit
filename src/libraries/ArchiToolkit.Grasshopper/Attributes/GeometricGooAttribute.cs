﻿using System.Diagnostics;

namespace ArchiToolkit.Grasshopper;

/// <summary>
/// For the case that this type is a geometric type.
/// Please use <see cref="DocObjAttribute"/> first.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
[Conditional(Constant.KeepAttributes)]
public class GeometricGooAttribute : Attribute;