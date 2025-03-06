using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace ArchiToolkit.Grasshopper.Instance;

partial class GooG
{
    public override BoundingBox GetBoundingBox(Transform xform)
    {
        throw new NotImplementedException();
    }

    public override IGH_GeometricGoo Transform(Transform xform)
    {
        throw new NotImplementedException();
    }

    public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
    {
        throw new NotImplementedException();
    }
}

public sealed partial class GooG : GH_GeometricGoo<MyClass>
{
    public GooG()
    {
    }

    public GooG(MyClass value) : base(value)
    {
    }

    public GooG(Goo other) : base(other.Value)
    {
    }

    public override BoundingBox Boundingbox => default;

    public override IGH_GeometricGoo DuplicateGeometry() => new GooG(Value);

    public override string ToString() => Value.ToString();

    public override string TypeName => "Name";

    public override string TypeDescription => "Description";
}

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

    public override string TypeName => "Name";

    public override string TypeDescription => "Description";
}

public class MyClass
{
}

public sealed partial class TestParameter : GH_PersistentParam<GH_Goo<MyClass>>
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
        PromptSingular(ref value, ref result);
        return result;
    }

    partial void PromptSingular(ref GH_Goo<MyClass> value, ref GH_GetterResult result);

    protected override GH_GetterResult Prompt_Plural(ref List<GH_Goo<MyClass>> values)
    {
        var result = GH_GetterResult.cancel;
        PromptPlural(ref values, ref result);
        return result;
    }

    partial void PromptPlural(ref List<GH_Goo<MyClass>> values, ref GH_GetterResult result);

    protected override Bitmap Icon => null!;

    public override Guid ComponentGuid => new("67DFEAB2-E366-4461-949A-D2616DAA5C48");

    public override GH_Exposure Exposure => GH_Exposure.hidden;

    private static Bitmap GetIcon(string resourceName)
    {
        using var stream = typeof(TestParameter).Assembly.GetManifestResourceStream(resourceName);
        return stream is null ? null! : new Bitmap(stream);
    }
}