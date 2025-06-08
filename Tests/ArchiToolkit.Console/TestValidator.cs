using ArchiToolkit.ValidResults;
using FluentValidation;

namespace ArchiToolkit.Console;

[GenerateValidResultAttribute<DateTime>]
public partial class DateTimeResult
{
}

public class TestClass2 : IDisposable
{
    public void Dispose()
    {
        // TODO release managed resources here
    }
}