using ArchiToolkit.Grasshopper;
using Grasshopper.Kernel;

namespace ArchiToolkit.Whatever;
public class Test
{
    [DocObj]
    public static int Add(int x, int y = 5) => x + y;

    [DocObj]
    public static Task<int> AddAsync(int x, ref int y) => Task.FromResult(x + y);
}
