using System.IO;

namespace ArchiToolkit.CppInteropGenerator.ViewModels.Pages;

public partial class DashboardViewModel : ObservableObject
{
    public bool IsReadyForConverting => IsDirectoryExists;

    public bool IsDirectoryExists => Directory.Exists(OutputPath);

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsDirectoryExists))]
    [NotifyPropertyChangedFor(nameof(IsReadyForConverting))]
    public partial string OutputPath { get; set; } = string.Empty;
}