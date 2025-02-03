namespace ArchiToolkit.Assertions.Constraints;

/// <summary>
/// Just the And Constratint
/// </summary>
/// <typeparam name="T"></typeparam>
public class AndConstraint<T>
{
    /// <summary>
    /// And things.
    /// </summary>
    public T And { get; }
    internal AndConstraint(T value)
    {
        And = value;
    }
}