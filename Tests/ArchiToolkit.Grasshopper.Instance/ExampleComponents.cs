using Grasshopper.Kernel;

namespace ArchiToolkit.Grasshopper.Instance;

public static class ExampleComponents
{
    [BaseComponent<GH_Component>]
    [ObjNames("Name", "Nickname", "Description")]
    [DocObj]
    public static void TestClass(
        IGH_DataAccess da,
        [ObjNames("Input", "I", "An input")]int i,
        [ObjField(true)]ref int myData,
        Io<double> data)
    {
        if (data.HasGot)
        {
            var a = data.Value;
        }
        //var a = ArchiToolkit_Resources.example;
    }
}