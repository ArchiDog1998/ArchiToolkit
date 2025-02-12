using ArchiToolkit.Assertions.Assertions;
using ArchiToolkit.Assertions.Execution;

namespace ArchiToolkit.Assertions.Constraints;

/// <summary>
///     Just the And Constraint
/// </summary>
/// <typeparam name="TValue"></typeparam>
public class AndConstraint<TValue> : IAndConstraint
{
    internal AndConstraint(ObjectAssertion<TValue> assertion)
    {
        And = assertion;
    }

    /// <summary>
    ///     And things.
    /// </summary>
    public ObjectAssertion<TValue> And { get; }

    /// <summary>
    ///     And it.
    /// </summary>
    public PronounConstraint<TValue> AndIt => new(And);

    /// <inheritdoc />
    public IDictionary<IAssertionStrategy, object>? FailureReturnValues { get; set; }
}