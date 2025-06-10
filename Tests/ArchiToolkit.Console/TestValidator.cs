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
    public static ValidResult<bool> TryFormat(
        ValidResult<DateTime> self,
        Span<char> destination, out ValidResult<int> charsWritten,
        ReadOnlySpan<char> format = default,
        ValidResult<IFormatProvider>? provider = null,
        [System.Runtime.CompilerServices.CallerArgumentExpression("self")] string selfName = "",
        [System.Runtime.CompilerServices.CallerArgumentExpression("provider")] string providerName = "",
        [System.Runtime.CompilerServices.CallerFilePath] string _filePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int _fileLineNumber = 0)
    {
        provider ??= ValidResult<IFormatProvider>.Data.Ok((IFormatProvider)null);
    }
}

[GenerateValidResult<TestClass2>]
public partial class TestClass2Result
{

}

public class TestClass2
{
    public static TestClass2 Create(bool a = false) => new TestClass2();
}