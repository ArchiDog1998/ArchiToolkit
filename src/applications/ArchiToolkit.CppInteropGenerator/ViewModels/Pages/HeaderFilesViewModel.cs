using System.Collections.ObjectModel;
using ArchiToolkit.CppInteropGenerator.ViewModels.UserControls;

namespace ArchiToolkit.CppInteropGenerator.ViewModels.Pages;

public partial class HeaderFilesViewModel : ObservableObject
{
    public bool IsReadyForConverting => true;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsReadyForConverting))]
    public partial ObservableCollection<ConvertItemViewModel> ConvertItemViewModels { get; set; } = [new(), new()];

    [RelayCommand]
    private async Task ClearAllHeaderFilesAsync()
    {

    }

    [RelayCommand]
    private async Task ImportHeaderFilesAsync()
    {

    }
}