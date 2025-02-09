using System.IO;
using ArchiToolkit.CppInteropGenerator.Models;
using ArchiToolkit.CppInteropGenerator.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace ArchiToolkit.CppInteropGenerator.ViewModels.UserControls;

public partial class ConvertItemViewModel(
    string filePath,
    string leadingNameSpace,
    string libraryName,
    ConvertTypeModel type,
    HeaderFilesViewModel parentViewModel)
    : ObservableObject
{
    public enum ConvertingStatus : byte
    {
        WaitingForConverting,
        Converting,
        Success,
        Error
    }

    public ConvertTypeModel[] ConvertTypes => DashboardViewModel.AllConvertTypes;
    public string FilePath { get; } = filePath;
    public string Tittle { get; } = Path.GetFileName(filePath);

    [NotifyPropertyChangedFor(nameof(Information))]
    [ObservableProperty]
    public partial string LeadingNameSpace { get; set; } = leadingNameSpace;

    [NotifyPropertyChangedFor(nameof(Information))]
    [ObservableProperty]
    public partial string LibraryName { get; set; } = libraryName;

    public SymbolRegular StatusSymbol => Status switch
    {
        ConvertingStatus.WaitingForConverting => SymbolRegular.Timer16,
        ConvertingStatus.Converting => SymbolRegular.ArrowSync16,
        ConvertingStatus.Success => SymbolRegular.Checkmark16,
        ConvertingStatus.Error => SymbolRegular.Warning16,
        _ => SymbolRegular.Question16
    };

    public string Information
    {
        get
        {
            var information = string.Join(", ", LibraryName, LeadingNameSpace, ConvertType);
            return string.IsNullOrEmpty(ErrorMessage) ? information : $"[{ErrorMessage}] {information}";
        }
    }

    [NotifyPropertyChangedFor(nameof(StatusSymbol))]
    [ObservableProperty]
    public partial ConvertingStatus Status { get; set; } = ConvertingStatus.WaitingForConverting;

    [NotifyPropertyChangedFor(nameof(Information))]
    [ObservableProperty]
    public partial string ErrorMessage { get; set; } = string.Empty;

    [NotifyPropertyChangedFor(nameof(Information))]
    [ObservableProperty]
    public partial ConvertTypeModel ConvertType { get; set; } = type;

    public bool Convert(string outputFolder)
    {
        ErrorMessage = string.Empty;
        Status = ConvertingStatus.Converting;
        Task.Delay(500).Wait();
        Status = ConvertingStatus.Success;
        return true;
    }

    [RelayCommand]
    public void Remove()
    {
        parentViewModel.ConvertItemViewModels.Remove(this);
    }
}