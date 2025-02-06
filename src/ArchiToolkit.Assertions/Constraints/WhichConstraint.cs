using ArchiToolkit.Assertions.Assertions;

namespace ArchiToolkit.Assertions.Constraints;

/// <summary>
/// </summary>
public class WhichConstraint<TValue> : IConstraint
{
    private readonly string _name;
    private readonly TValue _value;

    internal WhichConstraint(TValue value, string name)
    {
        _value = value;
        _name = name;
    }

    /// <summary>
    /// </summary>
    public ObjectAssertion<TValue> Must => new(_value, _name, AssertionType.Must);

    /// <summary>
    /// </summary>
    public ObjectAssertion<TValue> Should => new(_value, _name, AssertionType.Should);

    /// <summary>
    /// </summary>
    public ObjectAssertion<TValue> Could => new(_value, _name, AssertionType.Could);
}