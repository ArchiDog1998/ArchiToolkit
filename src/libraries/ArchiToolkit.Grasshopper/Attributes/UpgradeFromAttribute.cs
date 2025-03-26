﻿using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Grasshopper.Kernel;
#pragma warning disable CS9113 // Parameter is unread.

namespace ArchiToolkit.Grasshopper;

/// <summary>
/// Generate a <see cref="IGH_UpgradeObject"/> for you to upgrade to this <see cref="IGH_Component"/>.
/// </summary>
/// <remarks>
/// <para><b>⚠ WARNING:</b> Please use <see cref="DocObjAttribute"/> first.</para>
/// </remarks>
/// <param name="oldComponentGuid"></param>
/// <param name="year">The <see cref="DateTime.Year"/> of the <see cref="IGH_UpgradeObject.Version"/></param>
/// <param name="month">The <see cref="DateTime.Month"/> of the <see cref="IGH_UpgradeObject.Version"/></param>
/// <param name="day">The <see cref="DateTime.Day"/> of the <see cref="IGH_UpgradeObject.Version"/></param>
/// <param name="hour">The <see cref="DateTime.Hour"/> of the <see cref="IGH_UpgradeObject.Version"/></param>
/// <param name="minute">The <see cref="DateTime.Minute"/> of the <see cref="IGH_UpgradeObject.Version"/></param>
/// <param name="second">The <see cref="DateTime.Second"/> of the <see cref="IGH_UpgradeObject.Version"/></param>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
[Conditional(Constant.KeepAttributes)]
public class UpgradeFromAttribute([StringSyntax(StringSyntaxAttribute.GuidFormat)]string oldComponentGuid,
    int year, int month, int day, int hour = 0, int minute = 0, int second = 0) : Attribute;