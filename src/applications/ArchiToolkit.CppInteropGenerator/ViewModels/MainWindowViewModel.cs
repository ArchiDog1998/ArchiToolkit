using System.ComponentModel;
using ArchiToolkit.CppInteropGenerator.Resources;
using ArchiToolkit.CppInteropGenerator.ViewModels.Pages;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace ArchiToolkit.CppInteropGenerator.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly DashboardViewModel _dashboardViewModel;
    private readonly HeaderFilesViewModel _headerFilesViewModel;
    private readonly INavigationService _navigationService;
    private readonly IReadOnlyCollection<IsReadyViewModel> _pages;
    private readonly ISnackbarService _snackbarService;

    public MainWindowViewModel(ISnackbarService snackbarService,
        INavigationService navigationService,
        DashboardViewModel dashboardViewModel,
        HeaderFilesViewModel headerFilesViewModel)
    {
        _snackbarService = snackbarService;
        _navigationService = navigationService;
        _dashboardViewModel = dashboardViewModel;
        _headerFilesViewModel = headerFilesViewModel;
        _pages = [dashboardViewModel, headerFilesViewModel];

        CheckPage();
        foreach (var page in _pages.Reverse()) page.PropertyChanged += PageOnPropertyChanged;

        return;

        void PageOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(IsReadyViewModel.IsReadyForConverting)) return;
            Application.Current.Dispatcher.Invoke(CheckPage);
        }

        void CheckPage()
        {
            foreach (var page in _pages)
            {
                if (page.IsReadyForConverting) continue;
                NeedEditPage = page;
                return;
            }

            NeedEditPage = null;
        }
    }

    public string ApplicationTitle { get; } = ApplicationLocalization.Tittle;

    public SymbolRegular MajorStepSymbol => NeedEditPage?.PageIcon ?? SymbolRegular.TriangleRight16;

    public string MajorStepName =>
        NeedEditPage is null ? ApplicationLocalization.ConvertButton : string.Format(ApplicationLocalization.MoveToButton, NeedEditPage.PageName);

    public string? MajorStepTooltip =>
        NeedEditPage is null ? null : string.Format(ApplicationLocalization.MoveToButtonDescription, NeedEditPage.PageName);


    [NotifyPropertyChangedFor(nameof(MajorStepSymbol))]
    [NotifyPropertyChangedFor(nameof(MajorStepSymbol))]
    [NotifyPropertyChangedFor(nameof(MajorStepName))]
    [ObservableProperty]
    public partial PageViewModel? NeedEditPage { get; set; } = null;

    public IReadOnlyCollection<object> MenuItems => [.._pages.Select(CreateByViewModel)];

    private static NavigationViewItem CreateByViewModel(PageViewModel pageViewModel)
    {
        return new NavigationViewItem
        {
            Content = pageViewModel.PageName,
            Icon = new SymbolIcon { Symbol = pageViewModel.PageIcon },
            TargetPageType = pageViewModel.PageType
        };
    }

    [RelayCommand]
    private Task NextMajorStepAsync() => Task.Run(() =>
    {
        if (NeedEditPage is not null)
        {
            Application.Current.Dispatcher.Invoke(() =>
                _navigationService.Navigate(NeedEditPage.PageType));
            return;
        }

        var succeedsCount = _headerFilesViewModel.ConvertItemViewModels
            .AsParallel().Count(i => i.Convert(_dashboardViewModel.OutputPath));

        Application.Current.Dispatcher.Invoke(() =>
        {
            _snackbarService.Show("Finished!",
                $"Succeed with {succeedsCount} items! {_headerFilesViewModel.ConvertItemViewModels.Count - succeedsCount} faileds.",
                ControlAppearance.Info, new SymbolIcon
                {
                    Symbol = SymbolRegular.Check24
                }, TimeSpan.FromSeconds(5));
        });
    });
}