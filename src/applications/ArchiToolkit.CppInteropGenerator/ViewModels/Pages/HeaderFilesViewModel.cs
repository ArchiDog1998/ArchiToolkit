using System.Collections.ObjectModel;
using ArchiToolkit.CppInteropGenerator.ViewModels.UserControls;
using ArchiToolkit.CppInteropGenerator.Views.Pages;
using Microsoft.Win32;
using Wpf.Ui.Controls;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;

namespace ArchiToolkit.CppInteropGenerator.ViewModels.Pages;

public partial class HeaderFilesViewModel : IsReadyViewModel
{
    public override bool IsReadyForConverting => ConvertItemViewModels.Any();
    public override Type PageType => typeof(HeaderFilesPage);
    public override string PageName => "Header Files";
    public override string PageDescription => "A lot of header files.";
    public override SymbolRegular PageIcon => SymbolRegular.DocumentHeader16;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsReadyForConverting))]
    public partial ObservableCollection<ConvertItemViewModel> ConvertItemViewModels { get; set; } = [new(), new()];

    public HeaderFilesViewModel(DashboardViewModel dashboardViewModel)
    {
    }

    [RelayCommand]
    private void ClearSucceedFiles()
    {
        ClearHeaderFiles(ConvertItemViewModel.ConvertingStatus.Success);
    }

    [RelayCommand]
    private void ClearFailedFiles()
    {
        ClearHeaderFiles(ConvertItemViewModel.ConvertingStatus.Error);
    }

    [RelayCommand]
    private void ClearAllFiles()
    {
        ConvertItemViewModels.Clear();
    }

    private void ClearHeaderFiles(ConvertItemViewModel.ConvertingStatus status)
    {
        var failedItems = ConvertItemViewModels.Where(i => i.Status == status).ToArray();
        foreach (var item in failedItems) ConvertItemViewModels.Remove(item);
    }

    [RelayCommand]
    private void ImportHeaderFiles()
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "Header Files (*.h)|*.h",  // Only allow .h files
            Multiselect = true,  // Allow multiple file selection
            Title = "Select Header Files"
        };

        if (openFileDialog.ShowDialog() is not DialogResult.OK) return;

        foreach (var fileName in openFileDialog.FileNames)
        {

        }
    }
}