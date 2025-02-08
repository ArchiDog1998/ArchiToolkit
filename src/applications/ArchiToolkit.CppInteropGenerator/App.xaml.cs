using System.IO;
using System.Reflection;
using System.Windows.Threading;
using ArchiToolkit.CppInteropGenerator.Services;
using ArchiToolkit.CppInteropGenerator.ViewModels;
using ArchiToolkit.CppInteropGenerator.ViewModels.Pages;
using ArchiToolkit.CppInteropGenerator.Views;
using ArchiToolkit.CppInteropGenerator.Views.Pages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wpf.Ui;
using Wpf.Ui.Abstractions;

namespace ArchiToolkit.CppInteropGenerator;

public partial class App
{
    private static readonly IHost Host = Microsoft.Extensions.Hosting.Host
        .CreateDefaultBuilder()
        .ConfigureAppConfiguration(c =>
        {
            c.SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location) ?? string.Empty);
        })
        .ConfigureServices((_, services) =>
        {
            services.AddHostedService<ApplicationHostService>();

            services.AddSingleton<INavigationViewPageProvider, PageService>();
            services.AddSingleton<IThemeService, ThemeService>();
            services.AddSingleton<ISnackbarService, SnackbarService>();
            services.AddSingleton<INavigationService, NavigationService>();

            services.AddSingleton<INavigationWindow, MainWindow>();
            services.AddSingleton<MainWindowViewModel>();

            services.AddSingleton<DashboardPage>();
            services.AddSingleton<DashboardViewModel>();
            services.AddSingleton<HeaderFilesPage>();
            services.AddSingleton<HeaderFilesViewModel>();
        }).Build();


    private void OnStartup(object sender, StartupEventArgs e)
    {
        Host.Start();
    }

    private async void OnExit(object sender, ExitEventArgs e)
    {
        try
        {
            await Host.StopAsync();
            Host.Dispose();
        }
        catch
        {
            // ignored
        }
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
    }
}