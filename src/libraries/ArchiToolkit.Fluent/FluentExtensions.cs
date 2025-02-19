namespace ArchiToolkit.Fluent;

/// <summary>
/// Make the one as fluent.
/// </summary>
public static class FluentExtensions
{
    /// <summary>
    /// Make this value as a fluent one.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="type"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Fluent<T> AsFluent<T>(this T value, FluentType type = FluentType.Lazy)
    {
        return new Fluent<T>(value, type);
    }
}