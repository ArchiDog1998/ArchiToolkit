using ArchiToolkit.Fluent;
using ArchiToolkit.PureConst;

namespace ArchiToolkit.Console;

/// <summary>
/// Test summary.
/// </summary>
[FluentApi(typeof(int))]
public static class TestClass
{
    public static void AddIt(this int i)
    {
        i += 1;
    }
}
