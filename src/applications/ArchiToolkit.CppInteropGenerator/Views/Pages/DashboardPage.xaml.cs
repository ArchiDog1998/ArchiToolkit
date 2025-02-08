using ArchiToolkit.CppInteropGenerator.ViewModels.Pages;
using Wpf.Ui.Abstractions.Controls;

namespace ArchiToolkit.CppInteropGenerator.Views.Pages;

public partial class DashboardPage : INavigableView<DashboardViewModel>
{
    public DashboardPage(DashboardViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;
        InitializeComponent();
    }

    public DashboardViewModel ViewModel { get; }
}