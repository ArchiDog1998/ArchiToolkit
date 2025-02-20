namespace ArchiToolkit.Fluent.Tests;

public class LoggingType<TAA, TBB> where TAA : class, new()
    where TBB : struct
{
    public List<string> Logs { get; } = [];

    public TAA DataA
    {
        get;
        set
        {
            Logs.Add(nameof(DataA));
            field = value;
        }
    }

    public TBB AMethod<T>(ref T type, out int sth) where T : unmanaged
    {
        sth = 1;
        Logs.Add(nameof(AMethod));
        return default;
    }
}