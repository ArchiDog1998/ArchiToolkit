using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;

namespace ArchiToolkit.Grasshopper.Instance;

public static class ExampleComponents
{
    [Subcategory("SubTest")]
    [Category("Cate")]
    [Exposure(GH_Exposure.quarternary)]
    [ObjNames("Name", "Nickname", "Description")]
    [DocObj("Your Name")]
    public static List<int> TestClass(
        IGH_Component component,
        IGH_DataAccess da,
        [ObjNames("Input", "I", "An input")]ref int i,
        [ObjField(true)]ref int myData,
        [ParamType("abc")]List<int> test,
        [ParamType<Param_Integer>]ref Io<GH_Structure<GH_Integer>> tree,
        Io<double> data)
    {
        if (data.HasGot)
        {
            var a = data.Value;
        }

        return [];
        //var a = ArchiToolkit_Resources.example;
    }
}