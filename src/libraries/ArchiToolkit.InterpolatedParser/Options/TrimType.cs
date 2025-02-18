namespace ArchiToolkit.InterpolatedParser.Options;

/// <summary>
///     Trim Type
/// </summary>
public enum TrimType : byte
{
    /// <summary>
    ///     Nothing
    /// </summary>
    None,

    /// <summary>
    ///     Trim
    /// </summary>
    Trim,

    /// <summary>
    ///     Trim at start
    /// </summary>
    TrimStart,

    /// <summary>
    ///     Trim at end
    /// </summary>
    TrimEnd,
}