using ArchiToolkit.Assertions.Assertions;

namespace ArchiToolkit.Assertions.Constraints;

/// <summary>
///     Just the And Constraint
/// </summary>
/// <typeparam name="TAssertion"></typeparam>
public class AndConstraint<TAssertion> : IConstraint where TAssertion : IAssertion
{
    internal AndConstraint(TAssertion value)
    {
        And = value;
    }

    /// <summary>
    ///     And things.
    /// </summary>
    public TAssertion And { get; }

    /// <summary>
    /// And it.
    /// </summary>
    public PronounAssertion<TAssertion> AndIt => new(And);
}