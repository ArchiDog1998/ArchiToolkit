using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Special;
using Rhino.Geometry;

namespace ArchiToolkit.Grasshopper.Instance;

public sealed partial class TestComponent() : GH_Component(
    ArchiToolkitResources.Get(ResourceKey + "TestComponent.Name"),
    ArchiToolkitResources.Get("TestComponent.NickName"),
    ArchiToolkitResources.Get("TestComponent.Description"),
    ArchiToolkitResources.Get("Category.Name"),
    ArchiToolkitResources.Get("Subcategory.Name"))
{
    private const string ResourceKey = "Input";
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        {
            var param = new Param_Arc();
            // param.Hidden = true; // Hidden.
            // param.SetPersistentData([]);
            pManager.AddParameter(param, "Input", "I", "Input", GH_ParamAccess.item);
        }
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        {
            dynamic param = Instances.ComponentServer.EmitObject(new("abc"));
            // param.Hidden = true; // Hidden.
            // param.SetPersistentData([]);
            pManager.AddParameter((IGH_Param)param, "Input", "I", "Input", GH_ParamAccess.item);
        }
    }

    protected override void SolveInstance(IGH_DataAccess DA)
    {
    }

    public override GH_Exposure Exposure => (GH_Exposure)(1);

    protected override System.Drawing.Bitmap Icon => ArchiToolkitResources.GetIcon("Haha");
    public override Guid ComponentGuid => new ("66e13edd-d401-40c4-9e79-181db470fd8d");
}