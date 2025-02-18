namespace ArchiToolkit.InterpolatedParser.Options;

/// <summary>
/// The data type to parse it.
/// </summary>
public enum DataType : byte
{
    /// <summary>
    /// Just an object.
    /// </summary>
    Item,

    /// <summary>
    /// Make it parse as list as possible.
    /// </summary>
    List,
}