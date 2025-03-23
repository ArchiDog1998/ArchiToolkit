using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace ArchiToolkit.Grasshopper.Instance;

[Category("Cate")]
public static class ExampleComponents
{
    [Subcategory("SubTest")]
    [Exposure(GH_Exposure.quarternary)]
    [ObjNames("Name", "Nickname", "Description")]
    [DocObj("Your Name")]
    public static List<int> TestClass(
        IGH_Component component,
        IGH_DataAccess da,
        [Hidden,ObjNames("Input", "I", "An input")]ref Arc i,
        [ObjField(true)]ref Arc myData,
        [ParamType("abc"),  PersistentData<ExampleTypes>("Hello")]List<int> test,
        [Optional,ParamType<Param_Integer>, PersistentData("Hello")]ref Io<GH_Structure<GH_Integer>> tree,
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

partial class Component_TestClass
{
    private int _test;

    public override bool Write(GH_IWriter writer)
    {
        IoHelper.Write(writer, "_test", _test);
        return base.Write(writer);
    }
}