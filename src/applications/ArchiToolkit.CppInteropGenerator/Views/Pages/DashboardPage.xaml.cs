using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using ArchiToolkit.CppInteropGenerator.ViewModels.Pages;
using Wpf.Ui.Abstractions.Controls;
using TextBox = Wpf.Ui.Controls.TextBox;

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

    private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is not TextBox textBox) return;
        if (ViewModel.IsDirectoryExists == Directory.Exists(textBox.Text)) return;
        ViewModel.OutputPath = textBox.Text;
    }
}