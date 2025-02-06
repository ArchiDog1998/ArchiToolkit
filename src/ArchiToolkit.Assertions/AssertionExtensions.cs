using System.Runtime.CompilerServices;
using ArchiToolkit.Assertions.Assertions;
using ArchiToolkit.Assertions.Constraints;

namespace ArchiToolkit.Assertions;

/// <summary>
///     Contains some assertions
/// </summary>
public static class AssertionExtensions
{
    private static void ThrowIfInvalid<T>(T value)
    {
        switch (value)
        {
            case IAssertion:
                throw new InvalidOperationException("You can't create an assertion by an assertion!");
            case IConstraint:
                throw new InvalidOperationException("You can't create an assertion by an constraint! Try to use its property!");
        }
    }

    /// <summary>
    ///     The must assertion
    /// </summary>
    /// <param name="value"></param>
    /// <param name="valueName"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static ObjectAssertion<T> Must<T>(this T value,
        [CallerArgumentExpression(nameof(value))]
        string valueName = "")
    {
        ThrowIfInvalid(value);
        return new ObjectAssertion<T>(value, valueName, AssertionType.Must);
    }

    /// <summary>
    ///     The things should be.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="valueName"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static ObjectAssertion<T> Should<T>(this T value,
        [CallerArgumentExpression(nameof(value))]
        string valueName = "")
    {
        ThrowIfInvalid(value);
        return new ObjectAssertion<T>(value, valueName, AssertionType.Should);
    }

    /// <summary>
    ///     The things could be.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="valueName"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static ObjectAssertion<T> Could<T>(this T value,
        [CallerArgumentExpression(nameof(value))]
        string valueName = "")
    {
        ThrowIfInvalid(value);
        return new ObjectAssertion<T>(value, valueName, AssertionType.Could);
    }
}