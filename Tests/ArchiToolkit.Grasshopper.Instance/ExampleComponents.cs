using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace ArchiToolkit.Grasshopper.Instance;

public enum Ex
{
    A, B, C, D, E
}

[Category("Cate")]
public static class ExampleComponents
{
    [Subcategory("SubTest")]
    [Exposure(GH_Exposure.quarternary)]
    [ObjNames("Name", "Nickname", "Description")]
    [DocObj("Your Name")]
    [return: ObjNames("Result", "r", "Some interesting result")]
    public static List<int> TestClass(
        IGH_Component component,
        IGH_DataAccess da,
        [ObjNames("OK", "B", "C" )]Ex e,
        [Hidden,ObjNames("Input", "I", "An input")]ref Arc i,
        [ObjField(true)]ref int myData,
        List<int> test,
        [Optional,ParamType<Param_Integer>]ref Io<GH_Structure<GH_Integer>> tree,
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