namespace ArchiToolkit.Assertions.Tests;

public class GeneralTests
{
    [Test]
    public async Task Test()
    {
        int a = 0;
        a.Must().BeTypeOf<double>();
    }
}