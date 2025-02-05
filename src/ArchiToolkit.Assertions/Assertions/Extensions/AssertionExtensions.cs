using System.Runtime.CompilerServices;

namespace ArchiToolkit.Assertions.Assertions.Extensions;

/// <summary>
///     Contains some assertions
/// </summary>
public static class AssertionExtensions
{
    #region Object

    /// <summary>
    ///     The must assertion
    /// </summary>
    /// <param name="value"></param>
    /// <param name="valueName"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    [OverloadResolutionPriority(-1)]
    public static ObjectAssertion<T> Must<T>(this T value,
        [CallerArgumentExpression(nameof(value))] string valueName = "")
    {
        return new ObjectAssertion<T>(value, valueName, AssertionType.Must);
    }

    /// <summary>
    ///     The things should be.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="valueName"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    [OverloadResolutionPriority(-1)]
    public static ObjectAssertion<T> Should<T>(this T value,
        [CallerArgumentExpression(nameof(value))] string valueName = "")
    {
        return new ObjectAssertion<T>(value, valueName, AssertionType.Should);
    }

    /// <summary>
    ///     The things could be.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="valueName"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    [OverloadResolutionPriority(-1)]
    public static ObjectAssertion<T> Could<T>(this T value,
        [CallerArgumentExpression(nameof(value))] string valueName = "")
    {
        return new ObjectAssertion<T>(value, valueName, AssertionType.Could);
    }

    #endregion

    #region Collection

    /// <summary>
    ///     The must assertion
    /// </summary>
    /// <param name="collection"></param>
    /// <param name="valueName"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static EnumerableAssertion<IEnumerable<T>, T> Must<T>(this IEnumerable<T> collection,
        [CallerArgumentExpression(nameof(collection))] string valueName = "")
    {
        return new EnumerableAssertion<IEnumerable<T>, T>(collection, valueName, AssertionType.Must);
    }

    /// <summary>
    ///     The things should be.
    /// </summary>
    /// <param name="collection"></param>
    /// <param name="valueName"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static EnumerableAssertion<IEnumerable<T>, T> Should<T>(this IEnumerable<T> collection,
        [CallerArgumentExpression(nameof(collection))] string valueName = "")
    {
        return new EnumerableAssertion<IEnumerable<T>, T>(collection, valueName, AssertionType.Should);
    }

    /// <summary>
    ///     The things could be.
    /// </summary>
    /// <param name="collection"></param>
    /// <param name="valueName"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static EnumerableAssertion<IEnumerable<T>, T> Could<T>(this IEnumerable<T> collection,
        [CallerArgumentExpression(nameof(collection))] string valueName = "")
    {
        return new EnumerableAssertion<IEnumerable<T>, T>(collection, valueName, AssertionType.Could);
    }

    #endregion
}