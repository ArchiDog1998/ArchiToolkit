using System.IO;
using ArchiToolkit.CppInteropGenerator.Views.Pages;
using Wpf.Ui.Controls;

namespace ArchiToolkit.CppInteropGenerator.ViewModels.Pages;

public partial class DashboardViewModel : IsReadyViewModel
{
    public override bool IsReadyForConverting => IsDirectoryExists && IsDefaultNameSpaceNotEmpty;

    public override string PageName => "Home";
    public override string PageDescription => "Just the home page. And what can this tool do for you.";
    public override SymbolRegular PageIcon => SymbolRegular.Home16;

    public override Type PageType => typeof(DashboardPage);

    public bool IsDirectoryExists => Directory.Exists(OutputPath);

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsDirectoryExists))]
    [NotifyPropertyChangedFor(nameof(IsReadyForConverting))]
    public partial string OutputPath { get; set; } = string.Empty;

    public bool IsDefaultNameSpaceNotEmpty => !string.IsNullOrWhiteSpace(DefaultNameSpace);

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsDefaultNameSpaceNotEmpty))]
    [NotifyPropertyChangedFor(nameof(IsReadyForConverting))]
    public partial string DefaultNameSpace { get; set; } = string.Empty;

    [ObservableProperty] public partial string DefaultDllName { get; set; } = string.Empty;
}