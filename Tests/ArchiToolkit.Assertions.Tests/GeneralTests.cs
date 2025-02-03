using ArchiToolkit.Assertions.Execution;

namespace ArchiToolkit.Assertions.Tests;

public class GeneralTests
{
    [Test]
    public async Task TestMethodHere()
    {
        int a = 0;
        //a.Must().BeTypeOf<double>("Nice reason.");

        using (new AssertionScope("你好"))
        {
            a.Must().BeTypeOf<double>("Bad reason.");
        }
    }
}