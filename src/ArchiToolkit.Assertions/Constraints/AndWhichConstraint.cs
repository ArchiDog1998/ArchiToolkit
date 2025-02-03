namespace ArchiToolkit.Assertions.Constraints;

/// <summary>
/// The constraint
/// </summary>
/// <typeparam name="TParentConstraint"></typeparam>
/// <typeparam name="TMatchedElement"></typeparam>
public class AndWhichConstraint<TParentConstraint, TMatchedElement> : AndConstraint<TParentConstraint>
{
    /// <summary>
    /// The which thing.
    /// </summary>
    public TMatchedElement Which { get; }

    internal AndWhichConstraint(TParentConstraint value, TMatchedElement item) : base(value)
    {
        Which = item;
    }
}