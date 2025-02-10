using ArchiToolkit.CppInteropGenerator.Data;

namespace ArchiToolkit.CppInteropGenerator.ViewModels.Pages;

public abstract class IsReadyViewModel(AppDbContext dbContext) : PageViewModel
{
    protected AppDbContext DbContext { get; } = dbContext;
    public abstract bool IsReadyForConverting { get; }
}