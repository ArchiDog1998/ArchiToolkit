namespace ArchiToolkit.Grasshopper.Instance;

public class Test
{
    [UpgradeFrom("498A54B0-DA31-4C9F-905D-6FEA011DBFD5", 2025, 3, 1)]
    [UpgradeTo<Component_TestClass>(2025, 3, 26)]
    [DocObj]
    public static int OldAdd(int x, int y) => x + y;

    [DocObj]
    public static int Add(int x, int y) => x + y;
}