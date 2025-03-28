using ArchiToolkit.Grasshopper;

namespace ArchiToolkit.Whatever;

public class Test
{
    [DocObj]
    public static int Add(int x, int y = 5) => x + y;
}