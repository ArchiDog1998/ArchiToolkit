using System.Diagnostics;
#pragma warning disable CS9113 // Parameter is unread.

namespace ArchiToolkit.Grasshopper;

/// <summary>
/// The persistent data to set.
/// </summary>
/// <param name="propertyName"></param>
[AttributeUsage(AttributeTargets.Parameter)]
[Conditional(Constant.KeepAttributes)]
public class PersistentDataAttribute(string propertyName) : Attribute;

/// <summary>
/// <inheritdoc cref="PersistentDataAttribute"/>
/// </summary>
/// <param name="propertyName"><inheritdoc cref="PersistentDataAttribute"/></param>
/// <typeparam name="T"></typeparam>
[AttributeUsage(AttributeTargets.Parameter)]
[Conditional(Constant.KeepAttributes)]
public class PersistentDataAttribute<T>(string propertyName) : Attribute;