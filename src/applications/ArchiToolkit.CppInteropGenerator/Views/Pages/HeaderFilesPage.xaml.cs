using ArchiToolkit.CppInteropGenerator.ViewModels.Pages;
using Wpf.Ui.Abstractions.Controls;

namespace ArchiToolkit.CppInteropGenerator.Views.Pages;

public partial class HeaderFilesPage : INavigableView<HeaderFilesViewModel>
{
    public HeaderFilesPage(HeaderFilesViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;
        InitializeComponent();
    }

    public HeaderFilesViewModel ViewModel { get; }
}