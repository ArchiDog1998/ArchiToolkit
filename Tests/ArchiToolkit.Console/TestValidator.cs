using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using ArchiToolkit.ValidResults;
using FluentResults;
using FluentValidation;

namespace ArchiToolkit.Console;

public class TestItem
{
    public static TestItem operator +(TestItem a, TestItem b) => new();
}

public static class DateTimeResultExtensions2
{

}

[GenerateValidResultAttribute<DateTime>]
public partial class DateTimeResult
{


}