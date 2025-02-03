using ArchiToolkit.Assertions.AssertionItems;
using ArchiToolkit.Assertions.Assertions;

namespace ArchiToolkit.Assertions.Execution;

/// <summary>
/// The strategy to handle the exceptions
/// </summary>
public interface IAssertionStrategy
{
    /// <summary>
    /// handel the assertion.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="assertions"></param>
    void HandleFailure(string context, IReadOnlyList<IAssertion> assertions);

    /// <summary>
    /// Handle the assertions items.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="assertionType"></param>
    /// <param name="assertion"></param>
    void HandleFailure(string context, AssertionType assertionType, AssertionItem assertion);
}