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

    private int i = 0;
    protected override void BeforeSolveInstance()
    {
        TestBefore(ref i);
        base.BeforeSolveInstance();
    }

    private void TestBefore(ref int i)
    {

    }

    protected override void AfterSolveInstance()
    {
        base.AfterSolveInstance();
    }

    protected override void SolveInstance(IGH_DataAccess DA)
    {
    }

    protected override System.Drawing.Bitmap Icon => null!;
    public override Guid ComponentGuid => new ("66e13edd-d401-40c4-9e79-181db470fd8d");
}