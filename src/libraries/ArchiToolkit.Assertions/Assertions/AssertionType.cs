namespace ArchiToolkit.Assertions.Assertions;

/// <summary>
///     The type of the assertion.
/// </summary>
public enum AssertionType : byte
{
    /// <summary>
    ///     Must way.
    /// </summary>
    Must,

    /// <summary>
    ///     Should way.
    /// </summary>
    Should,

    /// <summary>
    ///     Could way.
    /// </summary>
    Could
}