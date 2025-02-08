namespace ArchiToolkit.CppInteropGenerator.ViewModels.Pages;

public abstract class IsReadyViewModel : PageViewModel
{
    public abstract bool IsReadyForConverting { get; }
}