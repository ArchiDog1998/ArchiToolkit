using System.Collections.ObjectModel;
using ArchiToolkit.CppInteropGenerator.Resources;
using ArchiToolkit.CppInteropGenerator.ViewModels.Pages;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace ArchiToolkit.CppInteropGenerator.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly ISnackbarService _snackbarService;
    private readonly DashboardViewModel _dashboardViewModel;
    private readonly HeaderFilesViewModel _headerFilesViewModel;

    public string ApplicationTitle { get; } = ApplicationLocalization.Tittle;

    [ObservableProperty]
    public partial ObservableCollection<object> MenuItems { get; set; } =
    [
        new NavigationViewItem()
        {
            Content = "Home",
            Icon = new SymbolIcon { Symbol = SymbolRegular.Home24 },
            TargetPageType = typeof(Views.Pages.DashboardPage)
        },
        new NavigationViewItem()
        {
            Content = "Header Files",
            Icon = new SymbolIcon { Symbol = SymbolRegular.DocumentHeader16 },
            TargetPageType = typeof(Views.Pages.HeaderFilesPage)
        }
    ];

    public MainWindowViewModel(ISnackbarService snackbarService,
        DashboardViewModel dashboardViewModel,
        HeaderFilesViewModel headerFilesViewModel)
    {
        _snackbarService = snackbarService;
        _dashboardViewModel = dashboardViewModel;
        _headerFilesViewModel = headerFilesViewModel;

        dashboardViewModel.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName != nameof(DashboardViewModel.IsReadyForConverting)) return;
            ConvertTheFilesCommand.NotifyCanExecuteChanged();
        };
        headerFilesViewModel.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName != nameof(HeaderFilesViewModel.IsReadyForConverting)) return;
            ConvertTheFilesCommand.NotifyCanExecuteChanged();
        };
    }

    private bool CanConvert()
    {
        return _dashboardViewModel.IsReadyForConverting && _headerFilesViewModel.IsReadyForConverting;
    }

    [RelayCommand(CanExecute = nameof(CanConvert))]
    private async Task ConvertTheFilesAsync()
    {
        await Task.Delay(2000);
        _snackbarService.Show("Finished!", "We make it!", ControlAppearance.Info, new SymbolIcon()
        {
            Symbol = SymbolRegular.Check24
        }, TimeSpan.FromSeconds(5));
    }
}