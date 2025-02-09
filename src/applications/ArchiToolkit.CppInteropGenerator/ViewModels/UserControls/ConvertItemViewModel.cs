using System.IO;
using ArchiToolkit.CppInteropGenerator.Models;
using ArchiToolkit.CppInteropGenerator.Resources;
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

    public string LeadingNameSpaceName => ApplicationLocalization.LeadingNameSpace;
    public string DllNameName => ApplicationLocalization.DllName;
    public string ConvertTypeName => ApplicationLocalization.ConvertType;

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
        ErrorMessage = new ConvertItem(FilePath, LeadingNameSpace, LibraryName, ConvertType).Convert();
        Status = string.IsNullOrEmpty(ErrorMessage) ? ConvertingStatus.Success : ConvertingStatus.Error;
        return true;
    }

    [RelayCommand]
    public void Remove()
    {
        parentViewModel.ConvertItemViewModels.Remove(this);
    }
}