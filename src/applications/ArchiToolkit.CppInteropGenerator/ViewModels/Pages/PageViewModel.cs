using Wpf.Ui.Controls;

namespace ArchiToolkit.CppInteropGenerator.ViewModels.Pages;

public abstract class PageViewModel : ObservableObject
{
    public abstract string PageName { get; }
    public abstract string PageDescription { get; }
    public abstract SymbolRegular PageIcon { get; }
    public abstract Type PageType { get; }
}