namespace ArchiToolkit.Console;

/// <summary>
/// Test summary.
/// </summary>
/// <typeparam name="T"></typeparam>
public class TestClass<T> where T :  struct
{
    /// <summary>
    /// What the hell.
    /// </summary>
    public T Data { get; set; } = default;
}