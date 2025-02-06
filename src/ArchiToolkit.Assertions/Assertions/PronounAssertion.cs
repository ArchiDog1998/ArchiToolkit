namespace ArchiToolkit.Assertions.Assertions;

/// <summary>
/// The assertion it.
/// </summary>
/// <typeparam name="TAssertion"></typeparam>
public class PronounAssertion<TAssertion> where TAssertion : IAssertion
{
    private readonly TAssertion _assertion;
    internal PronounAssertion(TAssertion assertion)
    {
        _assertion = assertion;
    }

    /// <summary>
    /// Must thing.
    /// </summary>
    public TAssertion Must => (TAssertion)_assertion.Duplicate(AssertionType.Must);

    /// <summary>
    ///
    /// </summary>
    public TAssertion Should => (TAssertion)_assertion.Duplicate(AssertionType.Should);

    /// <summary>
    ///
    /// </summary>
    public TAssertion Could => (TAssertion)_assertion.Duplicate(AssertionType.Could);
}