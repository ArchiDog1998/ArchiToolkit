using ArchiToolkit.ValidResults;
[assembly:GenerateValidResult(typeof(List<>),  "TestName")]
[assembly:GenerateValidResult(typeof(List<int>),  "TestName2")]
[assembly:GenerateValidResult(typeof(Dictionary<,>), "ListResult")]
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
    public int Value { get; } = 0;
    public static TestClass2 Create(bool a = false)
    {
        return new TestClass2();
    }

    public List<double> TryOne()
    {
        return [];
    }

    public static TestClass2 operator ~(TestClass2 a)
    {
        return a;
    }
}

public static class TestClass2Extensions
{
    public static void Ext(this TestClass2 self, bool a = false)
    {
    }
}