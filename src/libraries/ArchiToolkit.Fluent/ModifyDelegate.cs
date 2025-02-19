namespace ArchiToolkit.Fluent;

/// <summary>
/// The delegate to modify the value
/// </summary>
/// <typeparam name="T"></typeparam>
public delegate T ModifyDelegate<T>(in T input);