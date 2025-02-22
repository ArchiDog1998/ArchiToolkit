namespace ArchiToolkit.Fluent.Tests;

public class LazyTest
{
    public async Task Lazy<T>(Func<T, Task> checkBefore, Func<T, Task> checkAfter, Func<Fluent<T>, Fluent<T>> doWithFluent) where T : class, new()
    {
        var obj = new T();
        await checkBefore(obj);

        var fluent = doWithFluent(obj.AsFluent(FluentType.Lazy));
        await checkBefore(obj);
        _ = fluent.Result;
        await checkAfter(obj);
    }
}