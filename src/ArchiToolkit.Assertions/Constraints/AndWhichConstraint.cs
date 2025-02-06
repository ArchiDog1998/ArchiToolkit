using ArchiToolkit.Assertions.Assertions;

namespace ArchiToolkit.Assertions.Constraints;

/// <summary>
///     The constraint
/// </summary>
/// <typeparam name="TValue"></typeparam>
/// <typeparam name="TMatchedElement"></typeparam>
public class AndWhichConstraint<TValue, TMatchedElement> : AndConstraint<TValue>
{
    private readonly Lazy<TMatchedElement> _itemGetter;

    internal AndWhichConstraint(ObjectAssertion<TValue> value, Func<TMatchedElement> itemGetter) : base(value)
    {
        _itemGetter = new Lazy<TMatchedElement>(itemGetter);
    }

    /// <summary>
    ///     The which thing.
    /// </summary>
    public TMatchedElement Which => _itemGetter.Value;
}