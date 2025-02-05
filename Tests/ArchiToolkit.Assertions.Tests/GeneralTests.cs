using ArchiToolkit.Assertions.AssertionItems;
using ArchiToolkit.Assertions.Assertions;
using ArchiToolkit.Assertions.Execution;
using TUnit.Core;

namespace ArchiToolkit.Assertions.Tests;

public class GeneralTests
{
    [Test]
    public async Task TestMethodHere()
    {
        List<int> a = [];
        int? b = null;
        var must = a.Must();


        new List<int>().Must();
        using (new AssertionScope("你好"))
        {
            a.Must();
            a.Must().BeAssignableTo<double>("Bad reason.");
        }
    }
}