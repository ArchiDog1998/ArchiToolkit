using ArchiToolkit.CppInteropGenerator.ViewModels.Pages;
using Wpf.Ui.Abstractions.Controls;

namespace ArchiToolkit.CppInteropGenerator.Views.Pages;

public partial class HeaderFilesPage : INavigableView<HeaderFilesViewModel>
{
    public HeaderFilesViewModel ViewModel { get; }

    public HeaderFilesPage(HeaderFilesViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;
        InitializeComponent();
    }
}