using ArchiToolkit.ValidResults;
using FluentValidation;

namespace ArchiToolkit.Console;

[GenerateValidResult<ISpanFormattable>]
public partial class SpanFormatResult
{
}

[GenerateValidResult<DateTime>]
public partial class DateTimeResult
{
}

public class TestClass3 : TestClass2
{

}

public class TestClass2 : IDisposable
{
    public void AddThings()
    {

    }

public void Dispose()
    {
        // TODO release managed resources here
    }
}