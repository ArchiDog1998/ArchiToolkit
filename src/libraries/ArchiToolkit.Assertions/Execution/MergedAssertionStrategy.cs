using ArchiToolkit.Assertions.AssertionItems;
using ArchiToolkit.Assertions.Assertions;

namespace ArchiToolkit.Assertions.Execution;

internal class MergedAssertionStrategy(params IAssertionStrategy[] strategies)
{
    public object?[] HandleFailure(AssertionScope scope, IReadOnlyList<IAssertion> assertions)
    {
        return strategies.Select(strategy => strategy.HandleFailure(scope, assertions)).ToArray();
    }

    public object?[] HandleFailure(AssertionScope scope, AssertionType assertionType, AssertionItem assertion,
        object? tag)
    {
        return strategies.Select(strategy => strategy.HandleFailure(scope, assertionType, assertion, tag)).ToArray();
    }
}