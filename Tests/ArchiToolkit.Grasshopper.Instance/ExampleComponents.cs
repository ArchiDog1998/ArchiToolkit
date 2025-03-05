using Grasshopper.Kernel;

namespace ArchiToolkit.Grasshopper.Instance;

public static class ExampleComponents
{
    [BaseComponent<GH_Component>]
    [ObjNames("Name", "Nickname", "Description")]
    [DocObj]
    public static void TestClass(
        [ObjNames("Input", "I", "An input")]int i,
        [ObjField(true)]ref int myData)
    {
        //var a = ArchiToolkit_Resources.example;
    }
}