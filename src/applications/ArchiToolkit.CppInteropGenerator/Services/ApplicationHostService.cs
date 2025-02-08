using ArchiToolkit.CppInteropGenerator.Views;
using Microsoft.Extensions.Hosting;
using Wpf.Ui;

namespace ArchiToolkit.CppInteropGenerator.Services;

/// <summary>
/// Managed host of the application.
/// </summary>
public class ApplicationHostService(IServiceProvider serviceProvider) : IHostedService
{
    private INavigationWindow? _navigationWindow;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (Application.Current.Windows.OfType<MainWindow>().Any()) return Task.CompletedTask;

        _navigationWindow = (
            serviceProvider.GetService(typeof(INavigationWindow)) as INavigationWindow
        )!;
        _navigationWindow.ShowWindow();

        _navigationWindow.Navigate(typeof(Views.Pages.DashboardPage));
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
}