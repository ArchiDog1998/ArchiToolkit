namespace ArchiToolkit.CppInteropGenerator.ViewModels.UserControls;

public partial class ConvertItemViewModel : ObservableObject
{
    [ObservableProperty] public partial string ErrorMessage { get; set; } = string.Empty;

    public bool Convert(string outputFolder)
    {
        return true;
    }
}