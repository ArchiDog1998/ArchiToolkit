using ArchiToolkit.Grasshopper;
using ArchiToolkit.Grasshopper.Instance;
using Grasshopper;
using Grasshopper.Kernel;

//[assembly: BaseComponent<MyBaseComponent>]
[assembly: ObjAttr<CustomAttribute>]
[assembly: CategoryInfo("Cate", "Cat")]
[assembly: CategoryInfo("Cate", "OK", 'i')]
[assembly: CategoryInfo(null, "Short Name", 'S')]

public sealed class AssemblyPriority : GH_AssemblyPriority
{
    public override GH_LoadingInstruction PriorityLoad()
    {
        return GH_LoadingInstruction.Proceed;
    }

    private static void LoadInfo(string iconName)
    {
        var key = ArchiToolkitResources.Get(iconName);
        Instances.ComponentServer.AddCategoryShortName(key, ArchiToolkitResources.Get(iconName + ".ShortName"));
        Instances.ComponentServer.AddCategorySymbolName(key, ArchiToolkitResources.Get(iconName + ".SymbolName")[0]);
    }
}