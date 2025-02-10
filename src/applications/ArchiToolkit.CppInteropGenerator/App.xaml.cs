using System.Windows.Threading;
using ArchiToolkit.CppInteropGenerator.Data;
using ArchiToolkit.CppInteropGenerator.Services;
using ArchiToolkit.CppInteropGenerator.ViewModels;
using ArchiToolkit.CppInteropGenerator.ViewModels.Pages;
using ArchiToolkit.CppInteropGenerator.Views;
using ArchiToolkit.CppInteropGenerator.Views.Pages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wpf.Ui;
using Wpf.Ui.Abstractions;

namespace ArchiToolkit.CppInteropGenerator;

public partial class App
{
    private static readonly IHost Host = Microsoft.Extensions.Hosting.Host
        .CreateDefaultBuilder()
        // .ConfigureAppConfiguration(builder =>
        // {
        //     builder
        //         .SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location) ?? string.Empty)
        //         .AddJsonFile("appsettings.json", true, true)
        //         .AddEnvironmentVariables();
        // })
        // .ConfigureLogging((context, builder) =>
        // {
        //     builder.AddConsole(options =>
        //     {
        //         options.FormatterName = ConsoleFormatterNames.Simple; // Change to "simple", "json", or a custom formatter
        //     });
        // })
        .ConfigureServices((_, services) =>
        {
            services.AddDbContext<AppDbContext>();

            services.AddSingleton<INavigationViewPageProvider, PageService>();
            services.AddSingleton<IThemeService, ThemeService>();
            services.AddSingleton<ISnackbarService, SnackbarService>();
            services.AddSingleton<INavigationService, NavigationService>();

            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainWindowViewModel>();

            services.AddSingleton<DashboardPage>();
            services.AddSingleton<DashboardViewModel>();
            services.AddSingleton<HeaderFilesPage>();
            services.AddSingleton<HeaderFilesViewModel>();
        }).Build();


    private void OnStartup(object sender, StartupEventArgs e)
    {
        Host.Start();

        var window = Host.Services.GetRequiredService<MainWindow>();
        window.ShowWindow();
        window.Navigate(typeof(DashboardPage));
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