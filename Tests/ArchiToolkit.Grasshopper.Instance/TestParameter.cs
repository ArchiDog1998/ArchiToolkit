using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

namespace ArchiToolkit.Grasshopper.Instance;

public class MyClass
{

}

public partial class TestParameter : GH_PersistentParam<GH_Goo<MyClass>>
{

    public TestParameter() : this("", "", "", "", "")
    {

    }
    public TestParameter(GH_InstanceDescription nTag) : base(nTag)
    {
    }

    public TestParameter(string name, string nickname, string description, string category, string subcategory)
        : base(name, nickname, description, category, subcategory)
    {
    }

    protected override GH_GetterResult Prompt_Singular(ref GH_Goo<MyClass> value)
    {
        var result = GH_GetterResult.cancel;
        PromptSingular(ref value,  ref result);
        return result;
    }

    partial void PromptSingular(ref GH_Goo<MyClass> value, ref GH_GetterResult result);

    protected override GH_GetterResult Prompt_Plural(ref List<GH_Goo<MyClass>> values)
    {
        var result = GH_GetterResult.cancel;
        PromptPlural(ref values,  ref result);
        return result;
    }
    partial void PromptPlural(ref List<GH_Goo<MyClass>> values, ref GH_GetterResult result);

    protected override Bitmap Icon => null!;

    public override Guid ComponentGuid => new ("67DFEAB2-E366-4461-949A-D2616DAA5C48");

    public override GH_Exposure Exposure => GH_Exposure.hidden;
}