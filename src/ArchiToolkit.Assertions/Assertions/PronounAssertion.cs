namespace ArchiToolkit.Assertions.Assertions;

/// <summary>
///     The assertion it.
/// </summary>
/// <typeparam name="TValue"></typeparam>
public class PronounAssertion<TValue>
{
    private readonly ObjectAssertion<TValue> _assertion;

    internal PronounAssertion(ObjectAssertion<TValue> assertion)
    {
        _assertion = assertion;
    }

    /// <summary>
    ///     Must thing.
    /// </summary>
    public ObjectAssertion<TValue> Must => _assertion.Duplicate(AssertionType.Must);

    /// <summary>
    /// </summary>
    public ObjectAssertion<TValue> Should => _assertion.Duplicate(AssertionType.Should);

    /// <summary>
    /// </summary>
    public ObjectAssertion<TValue> Could => _assertion.Duplicate(AssertionType.Could);
}