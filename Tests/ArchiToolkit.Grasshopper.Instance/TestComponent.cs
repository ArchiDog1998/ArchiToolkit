using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;

namespace ArchiToolkit.Grasshopper.Instance;

public sealed partial class TestComponent() : GH_Component(
    ArchiToolkitResources.Get("TestComponent.Name"),
    ArchiToolkitResources.Get("TestComponent.NickName"),
    ArchiToolkitResources.Get("TestComponent.Description"),
    ArchiToolkitResources.Get("Category.Name"),
    ArchiToolkitResources.Get("Subcategory.Name"))
{

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        // var p = (IGH_Param)Instances.ComponentServer.EmitObject(default);
        // pManager.AddIntegerParameter()

        {
            List<Arc> arcs = [new Arc()];
            var param = new Param_Arc();
            param.Hidden = true; // Hidden.
            param.SetPersistentData(arcs);
            pManager.AddParameter(param, "Input", "I", "Input", GH_ParamAccess.item);
        }
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
    }

    protected override void SolveInstance(IGH_DataAccess DA)
    {
    }

    public override GH_Exposure Exposure => (GH_Exposure)(1);

    protected override System.Drawing.Bitmap Icon => ArchiToolkitResources.GetIcon("Haha");
    public override Guid ComponentGuid => new ("66e13edd-d401-40c4-9e79-181db470fd8d");
}