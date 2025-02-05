namespace ArchiToolkit.Assertions.Constraints;

/// <summary>
///     Just the And Constraint
/// </summary>
/// <typeparam name="TAssertion"></typeparam>
public class AndConstraint<TAssertion>
{
    internal AndConstraint(TAssertion value)
    {
        And = value;
    }

    /// <summary>
    ///     And things.
    /// </summary>
    public TAssertion And { get; }
}