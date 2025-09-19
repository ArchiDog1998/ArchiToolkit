using System.Diagnostics;
using ArchiToolkit.Assertions.AssertionItems;
using ArchiToolkit.Assertions.Assertions;
using ArchiToolkit.Assertions.Constraints;

namespace ArchiToolkit.Assertions.Execution;

/// <summary>
///     The strategy to handle the exceptions.
///     <remarks>
///         It is recommended to use the attribute <see cref="DebuggerHiddenAttribute" /> to all the method that this
///         has.
///     </remarks>
/// </summary>
public interface IAssertionStrategy
{
    /// <summary>
    ///     handel the assertion.
    /// </summary>
    /// <param name="scope"></param>
    /// <param name="assertions"></param>
    /// <returns></returns>
    object? HandleFailure(AssertionScope scope, IReadOnlyList<IAssertion> assertions);

    /// <summary>
    ///     Handle the assertions items.
    /// </summary>
    /// <param name="scope"></param>
    /// <param name="assertionType"></param>
    /// <param name="assertion"></param>
    /// <param name="tag"></param>
    /// <param name="callerInfo"></param>
    /// <returns>This value will push to <see cref="IAndConstraint.FailureReturnValues" /></returns>
    object? HandleFailure(AssertionScope scope, AssertionType assertionType, AssertionItem assertion, object? tag,
        CallerInfo callerInfo);
}