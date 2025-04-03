using ArchiToolkit.Grasshopper;
using Grasshopper.Kernel;

namespace ArchiToolkit.Whatever;
public class Test
{
    [DocObj]
    public static int Add(int x, int y = 5) => x + y;

    private const string stringArg = "stringArgValue";

    private string _testString = "What a field".Loc();

    [DocObj]
    public static Task<int> AddAsync(int x, int y, out string name)
    {
        "What Hell".Loc();
        name = "Localization String".Loc("Optional Key");
        var b = stringArg.Loc(stringArg);
        return Task.FromResult(x + y);
    }
}