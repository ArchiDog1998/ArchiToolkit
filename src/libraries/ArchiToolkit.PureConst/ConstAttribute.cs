﻿using System.Diagnostics;

namespace ArchiToolkit.PureConst;

/// <summary>
///     The const parameter or the method
/// </summary>
[Conditional("KEEP_CONST")]
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method)]
public sealed class ConstAttribute : Attribute;