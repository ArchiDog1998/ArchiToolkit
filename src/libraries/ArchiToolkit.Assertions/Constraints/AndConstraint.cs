using ArchiToolkit.Assertions.Assertions;

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
    public object?[] FailureReturnValue { get; set; } = [];
}