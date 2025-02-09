using ArchiToolkit.CppInteropGenerator.ViewModels;
using Wpf.Ui;
using Wpf.Ui.Abstractions;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace ArchiToolkit.CppInteropGenerator.Views;

public partial class MainWindow : INavigationWindow
{
    public MainWindow(MainWindowViewModel viewModel,
        INavigationViewPageProvider pageService,
        INavigationService navigationService,
        ISnackbarService snackbarService)
    {
        ViewModel = viewModel;
        DataContext = this;
        SystemThemeWatcher.Watch(this);

        InitializeComponent();

        SetPageService(pageService);
        navigationService.SetNavigationControl(RootNavigation);
        snackbarService.SetSnackbarPresenter(SnackbarPresenter);
    }

    public MainWindowViewModel ViewModel { get; }

    public INavigationView GetNavigation()
    {
        return RootNavigation;
    }

    public bool Navigate(Type pageType)
    {
        return RootNavigation.Navigate(pageType);
    }

    public void SetServiceProvider(IServiceProvider serviceProvider)
    {
    }

    public void SetPageService(INavigationViewPageProvider navigationViewPageProvider)
    {
        RootNavigation.SetPageProviderService(navigationViewPageProvider);
    }

    public void ShowWindow()
    {
        Show();
    }

    public void CloseWindow()
    {
        Close();
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        Application.Current.Shutdown();
    }
}