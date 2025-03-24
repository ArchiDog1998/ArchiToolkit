using ArchiToolkit.Grasshopper;
using ArchiToolkit.Grasshopper.Instance;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
[assembly: DocObj<AnotherType>(name: "What a type")]

namespace ArchiToolkit.Grasshopper.Instance;

public class AnotherType;

[TypeDesc("A Type", "An interesting Type")]
[Exposure(GH_Exposure.quarternary)]
[BaseGoo<GH_GeometricGoo<MyType>>]
[DocObj]
public class MyType
{
    
}

partial class Param_MyType
{
    partial class Goo
    {
        public override IGH_GeometricGoo DuplicateGeometry()
        {
            throw new NotImplementedException();
        }

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

        public override BoundingBox Boundingbox => default;
    }
}