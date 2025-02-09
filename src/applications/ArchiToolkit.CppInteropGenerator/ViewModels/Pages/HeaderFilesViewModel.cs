using System.Collections.ObjectModel;
using System.IO;
using ArchiToolkit.CppInteropGenerator.Resources;
using ArchiToolkit.CppInteropGenerator.ViewModels.UserControls;
using ArchiToolkit.CppInteropGenerator.Views.Pages;
using Wpf.Ui.Controls;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;

namespace ArchiToolkit.CppInteropGenerator.ViewModels.Pages;

public partial class HeaderFilesViewModel(DashboardViewModel dashboardViewModel) : IsReadyViewModel
{
    public override bool IsReadyForConverting => ConvertItemViewModels.Any();
    public override Type PageType => typeof(HeaderFilesPage);
    public override string PageName => ApplicationLocalization.HeaderFilePage;
    public override string PageDescription => ApplicationLocalization.HeaderFilePageDescription;
    public override SymbolRegular PageIcon => SymbolRegular.DocumentHeader16;

    public string ClearCard  => ApplicationLocalization.ClearCard;
    public string ClearAll  => ApplicationLocalization.ClearAllButtonDescription;
    public string ClearSucceed  => ApplicationLocalization.ClearSucceedButtonDescription;
    public string ClearFailed  => ApplicationLocalization.ClearFailedButtonDescription;
    public string ImportFiles  => ApplicationLocalization.ImportFilesButton;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsReadyForConverting))]
    public partial ObservableCollection<ConvertItemViewModel> ConvertItemViewModels { get; set; } = [];

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
            // ReSharper disable once LocalizableElement
            Filter = ApplicationLocalization.HFileName + " (*.h)|*.h",
            Multiselect = true,
            Title = ApplicationLocalization.SelectHFilesTittle,
        };

        if (openFileDialog.ShowDialog() is not DialogResult.OK) return;

        foreach (var fileName in openFileDialog.FileNames)
        {
            var libraryName = dashboardViewModel.DefaultDllName;
            if (string.IsNullOrEmpty(libraryName))
            {
                libraryName = Path.GetFileNameWithoutExtension(fileName);
            }

            foreach (var model in ConvertItemViewModels.Where(i => i.FilePath == fileName).ToArray())
            {
                ConvertItemViewModels.Remove(model);
            }

            ConvertItemViewModels.Add(new ConvertItemViewModel(fileName, dashboardViewModel.LeadingNameSpace, libraryName,
                dashboardViewModel.ConvertType, this));
            OnPropertyChanged(nameof(IsReadyForConverting));
        }
    }
}