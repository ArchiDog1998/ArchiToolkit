using System.Runtime.CompilerServices;
using ArchiToolkit.Assertions.Assertions;

namespace ArchiToolkit.Assertions;

/// <summary>
/// Contains some assertions
/// </summary>
public static class AssertionExtensions
{
    private static void Foo()
    {
        int a = 10;
        a.Should().Not().BeTypeOf<double>();
    }

    /// <summary>
    /// The must assertion
    /// </summary>
    /// <param name="value"></param>
    /// <param name="valueName"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Assertion<T> Must<T>(this T value, [CallerArgumentExpression(nameof(value))] string valueName = "")
    {
        return new Assertion<T>(value, valueName, AssertionType.Must);
    }

    /// <summary>
    /// The things should be.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="valueName"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Assertion<T> Should<T>(this T value, [CallerArgumentExpression(nameof(value))] string valueName = "")
    {
        return new Assertion<T>(value, valueName, AssertionType.Should);
    }

    /// <summary>
    /// The things could be.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="valueName"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Assertion<T> Could<T>(this T value, [CallerArgumentExpression(nameof(value))] string valueName = "")
    {
        return new Assertion<T>(value, valueName, AssertionType.Could);
    }
}