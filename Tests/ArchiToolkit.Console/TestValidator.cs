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

[GenerateValidResult<TestClass2>]
public partial class TestClass2Result
{

}

public class TestClass2
{
    public static TestClass2 Create() => new TestClass2();
}