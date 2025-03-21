using Grasshopper.Kernel;

namespace ArchiToolkit.Grasshopper.Instance;

public abstract class MyBaseComponent(
    string name,
    string nickname,
    string description,
    string category,
    string subCategory)
    : GH_Component(name, nickname, description, category, subCategory);