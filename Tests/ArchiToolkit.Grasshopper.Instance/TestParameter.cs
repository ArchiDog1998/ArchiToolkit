using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

namespace ArchiToolkit.Grasshopper.Instance;


public class MyClass;

public sealed partial class TestParameter : GH_PersistentParam<TestParameter.Goo>
{
    public sealed partial class Goo : GH_Goo<MyClass>
    {
        public Goo()
        {
        }

        public Goo(MyClass value) : base(value)
        {
        }

        public Goo(Goo other) : base(other.Value)
        {
        }

        public override IGH_Goo Duplicate() => new Goo(Value);

        public override string ToString() => Value.ToString();

        public override bool IsValid => true;

        public override string TypeName => "TypeName";

        public override string TypeDescription => "TypeDescription";
    }

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

    protected override GH_GetterResult Prompt_Singular(ref Goo value)
    {
        var result = GH_GetterResult.cancel;
        PromptSingular(ref value, ref result);
        return result;
    }

    partial void PromptSingular(ref Goo value, ref GH_GetterResult result);

    protected override GH_GetterResult Prompt_Plural(ref List<Goo> values)
    {
        var result = GH_GetterResult.cancel;
        PromptPlural(ref values, ref result);
        return result;
    }

    partial void PromptPlural(ref List<Goo> values, ref GH_GetterResult result);

    public override Guid ComponentGuid => new("67DFEAB2-E366-4461-949A-D2616DAA5C48");
}