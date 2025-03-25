using ArchiToolkit.Grasshopper;
using ArchiToolkit.Grasshopper.Instance;
using Grasshopper;
using Grasshopper.Kernel;

//[assembly: BaseComponent<MyBaseComponent>]
[assembly: Category("MyTestCategory")]
[assembly: ObjAttr<CustomAttribute>]

public partial class AssemblyPriority : GH_AssemblyPriority
{
    public override GH_LoadingInstruction PriorityLoad()
    {
        LoadIcon("Yes");
        return GH_LoadingInstruction.Proceed;
    }

    private static void LoadIcon(string iconName)
    {
        if (ArchiToolkitResources.GetIcon("ABC" + iconName + ".png") is not { } icon) return;
        Instances.ComponentServer.AddCategoryIcon(ArchiToolkitResources.Get(iconName), icon);
    }
}