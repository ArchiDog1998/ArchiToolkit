namespace ArchiToolkit.CppInteropGenerator.ViewModels.UserControls;

public partial class ConvertItemViewModel : ObservableObject
{
    public enum ConvertingStatus : byte
    {
        WaitingForConverting,
        Converting,
        Success,
        Error,
    }

    [ObservableProperty] public partial ConvertingStatus Status { get; set; } = ConvertingStatus.WaitingForConverting;

    [ObservableProperty] public partial string ErrorMessage { get; set; } = string.Empty;

    public bool Convert(string outputFolder)
    {
        Status = ConvertingStatus.Converting;
        Task.Delay(500).Wait();
        Status = ConvertingStatus.Success;
        return true;
    }
}