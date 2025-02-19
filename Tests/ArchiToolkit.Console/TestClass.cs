using ArchiToolkit.InterpolatedParser;

namespace ArchiToolkit.Console;

public class TestClass
{
    public void TestMethod<T>(T obj) where T : IParsable<T>
    {
        "What a good thing".Parse($"What a good {obj}");
    }
}