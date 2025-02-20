namespace ArchiToolkit.Fluent.Tests;

public class BasicFluentTest
{
    public async Task BasicTest()
    {
        var obj = new LoggingType<Random, int>();
        obj.AsFluent();
    }
}