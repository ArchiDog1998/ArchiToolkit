namespace ArchiToolkit.Fluent.Tests;


public class BasicType<TAa, TBb> where TAa : class, new()
    where TBb : struct
{
    public string Name { get; private set; } = string.Empty;
    public TAa DataA { get; set; } = new();

    public TBb AMethod<T>(ref T type, out int sth) where T : unmanaged
    {
        Name = typeof(T).Name;
        sth = 1;
        return default;
    }
}