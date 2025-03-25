using ArchiToolkit.Grasshopper;
using ArchiToolkit.Grasshopper.Instance;
using Grasshopper;
using Grasshopper.Kernel;

//[assembly: BaseComponent<MyBaseComponent>]
[assembly: Category("MyTestCategory")]
[assembly: ObjAttr<CustomAttribute>]

// public partial class AssemblyPriority : GH_AssemblyPriority
// {
//     public override GH_LoadingInstruction PriorityLoad()
//     {
//         Instances.ComponentServer.AddCategoryIcon(ArchiToolkitResources.Get("Yes"), ArchiToolkitResources.GetIcon("Yes.png"));
//         return GH_LoadingInstruction.Proceed;
//     }
// }