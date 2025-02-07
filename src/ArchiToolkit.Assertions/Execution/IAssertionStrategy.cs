using ArchiToolkit.Assertions.AssertionItems;
using ArchiToolkit.Assertions.Assertions;
using ArchiToolkit.Assertions.Constraints;

namespace ArchiToolkit.Assertions.Execution;

/// <summary>
///     The strategy to handle the exceptions
/// </summary>
public interface IAssertionStrategy
{
    /// <summary>
    ///     handel the assertion.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="assertions"></param>
    /// <returns></returns>
    object? HandleFailure(string context, IReadOnlyList<IAssertion> assertions);

    /// <summary>
    ///     Handle the assertions items.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="assertionType"></param>
    /// <param name="assertion"></param>
    /// <param name="tag"></param>
    /// <returns>This value will push to <see cref="IAndConstraint.FailureReturnValue"/></returns>
    object? HandleFailure(string context, AssertionType assertionType, AssertionItem assertion, object? tag);
}