using Grasshopper.Kernel;

namespace ArchiToolkit.Grasshopper.Instance;

public sealed class TestComponent() : GH_Component("", "", "", "", "")
{
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
    }

    protected override void SolveInstance(IGH_DataAccess DA)
    {
    }

    protected override System.Drawing.Bitmap Icon => null!;
    public override Guid ComponentGuid => new ("66e13edd-d401-40c4-9e79-181db470fd8d");
}