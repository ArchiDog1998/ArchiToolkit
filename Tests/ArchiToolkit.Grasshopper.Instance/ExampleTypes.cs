using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace ArchiToolkit.Grasshopper.Instance;

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