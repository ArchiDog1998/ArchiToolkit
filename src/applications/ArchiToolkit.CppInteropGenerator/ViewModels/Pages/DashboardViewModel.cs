using System.IO;
using ArchiToolkit.CppInteropGenerator.Views.Pages;
using Wpf.Ui.Controls;

namespace ArchiToolkit.CppInteropGenerator.ViewModels.Pages;

public partial class DashboardViewModel : IsReadyViewModel
{
    public override bool IsReadyForConverting => IsDirectoryExists;

    public override string PageName => "Home";
    public override string PageDescription => "Just the home page. And what can this tool do for you.";
    public override SymbolRegular PageIcon => SymbolRegular.Home16;
    public override Type PageType => typeof(DashboardPage);

    public bool IsDirectoryExists => Directory.Exists(OutputPath);

    public Visibility WarningVisibility => IsDirectoryExists ? Visibility.Collapsed : Visibility.Visible;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsDirectoryExists))]
    [NotifyPropertyChangedFor(nameof(WarningVisibility))]
    [NotifyPropertyChangedFor(nameof(IsReadyForConverting))]
    public partial string OutputPath { get; set; } = string.Empty;

    [ObservableProperty] public partial string LeadingNameSpace { get; set; } = "Cpp";


    [ObservableProperty] public partial string DefaultDllName { get; set; } = string.Empty;

    [RelayCommand]
    private void OpenFolderDialog()
    {
        var folderPicker = new FolderBrowserDialog
        {
            SelectedPath = OutputPath,
        };

        if(folderPicker.ShowDialog() is not DialogResult.OK) return;
        OutputPath = folderPicker.SelectedPath;
    }
}