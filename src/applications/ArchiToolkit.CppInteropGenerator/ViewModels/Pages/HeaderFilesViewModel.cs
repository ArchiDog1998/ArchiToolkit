using System.Collections.ObjectModel;
using ArchiToolkit.CppInteropGenerator.ViewModels.UserControls;
using ArchiToolkit.CppInteropGenerator.Views.Pages;
using Wpf.Ui.Controls;

namespace ArchiToolkit.CppInteropGenerator.ViewModels.Pages;

public partial class HeaderFilesViewModel : IsReadyViewModel
{
    private readonly DashboardViewModel _dashboardViewModel;

    public HeaderFilesViewModel(DashboardViewModel dashboardViewModel)
    {
        _dashboardViewModel = dashboardViewModel;

        dashboardViewModel.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName != nameof(DashboardViewModel.IsDefaultNameSpaceNotEmpty)) return;
            ImportHeaderFilesCommand.NotifyCanExecuteChanged();
        };
    }

    public override bool IsReadyForConverting => ConvertItemViewModels.Any();
    public override Type PageType => typeof(HeaderFilesPage);
    public override string PageName => "Header Files";
    public override string PageDescription => "A lot of header files.";
    public override SymbolRegular PageIcon => SymbolRegular.DocumentHeader16;

    private bool HaveDefaultNamespace => !string.IsNullOrEmpty(_dashboardViewModel.DefaultNameSpace);

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsReadyForConverting))]
    public partial ObservableCollection<ConvertItemViewModel> ConvertItemViewModels { get; set; } = [new(), new()];

    [RelayCommand]
    private Task ClearAllHeaderFilesAsync()
    {
        return Task.Run(() => { ConvertItemViewModels.Clear(); });
    }

    [RelayCommand]
    private Task ClearAllFailedHeaderFilesAsync()
    {
        return Task.Run(() =>
        {
            var failedItems = ConvertItemViewModels.Where(i => !string.IsNullOrEmpty(i.ErrorMessage)).ToArray();

            foreach (var item in failedItems) ConvertItemViewModels.Remove(item);
        });
    }

    [RelayCommand(CanExecute = nameof(HaveDefaultNamespace))]
    private Task ImportHeaderFilesAsync()
    {
        return Task.Run(() => { });
    }
}