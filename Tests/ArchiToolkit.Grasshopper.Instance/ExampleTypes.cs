using ArchiToolkit.Grasshopper;
using ArchiToolkit.Grasshopper.Instance;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;
[assembly: DocObj<AnotherType>(name: "What a type")]

namespace ArchiToolkit.Grasshopper.Instance;

public class AnotherType;

[TypeDesc("A Type", "An interesting Type")]
[Exposure(GH_Exposure.quarternary)]
[DocObj]
public class MyType : IGH_PreviewData, IGH_BakeAwareData
{
    public void DrawViewportWires(GH_PreviewWireArgs args)
    {
    }

    public void DrawViewportMeshes(GH_PreviewMeshArgs args)
    {
    }

    public BoundingBox ClippingBox => BoundingBox.Unset;
    public bool BakeGeometry(RhinoDoc doc, ObjectAttributes att, out Guid obj_guid)
    {
        obj_guid = Guid.Empty;
        return false;
    }
}