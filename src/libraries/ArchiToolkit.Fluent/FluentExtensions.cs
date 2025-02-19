namespace ArchiToolkit.Fluent;

public static partial class FluentExtensions
{
    public static Fluent<T> AsFluentImmediate<T>(this T value)
    {
        return new (value, FluentType.Immediate);
    }

    public static Fluent<T> AsFluentLazy<T>(this T value)
    {
        return new (value, FluentType.Lazy);
    }
}