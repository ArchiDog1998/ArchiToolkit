namespace ArchiToolkit.Assertions.Constraints;

/// <summary>
///     The constraint
/// </summary>
/// <typeparam name="TAssertion"></typeparam>
/// <typeparam name="TMatchedElement"></typeparam>
public class AndWhichConstraint<TAssertion, TMatchedElement> : AndConstraint<TAssertion>
{
    private readonly Lazy<TMatchedElement> _itemGetter;
    internal AndWhichConstraint(TAssertion value, Func<TMatchedElement> itemGetter) : base(value)
    {
        _itemGetter = new Lazy<TMatchedElement>(itemGetter);
    }

    /// <summary>
    ///     The which thing.
    /// </summary>
    public TMatchedElement Which => _itemGetter.Value;
}