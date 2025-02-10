using System.IO;
using ArchiToolkit.CppInteropGenerator.Data;
using ArchiToolkit.CppInteropGenerator.Models;
using ArchiToolkit.CppInteropGenerator.Resources;
using ArchiToolkit.CppInteropGenerator.Views.Pages;
using Wpf.Ui.Controls;

namespace ArchiToolkit.CppInteropGenerator.ViewModels.Pages;

public partial class DashboardViewModel(AppDbContext dbContext) : IsReadyViewModel(dbContext)
{
    public static ConvertTypeModel[] AllConvertTypes { get; } =
        [..Enum.GetValues<ConvertType>().Select(t => new ConvertTypeModel(t))];

    public override bool IsReadyForConverting => IsDirectoryExists;

    public override string PageName => ApplicationLocalization.DashboardPage;
    public override string PageDescription => ApplicationLocalization.DashboardPageDescription;
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

    [ObservableProperty]
    public partial ConvertTypeModel ConvertType { get; set; } = new (Models.ConvertType.NativeLibrary);

    public ConvertTypeModel[] ConvertTypes => AllConvertTypes;

    [RelayCommand]
    private void OpenFolderDialog()
    {
        var folderPicker = new FolderBrowserDialog
        {
            SelectedPath = OutputPath,
        };

        if (folderPicker.ShowDialog() is not DialogResult.OK) return;
        OutputPath = folderPicker.SelectedPath;
    }
}