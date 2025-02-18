using ArchiToolkit.InterpolatedParser.Options;

namespace ArchiToolkit.InterpolatedParser.ParseItems;

/// <summary>
/// The parse item.
/// </summary>
public interface IParseItem
{
    /// <summary>
    /// Index of the regex.
    /// </summary>
    public int RegexIndex { get; }

    /// <summary>
    /// Your pre modification.
    /// </summary>
    public PreModifyOptions PreModification { get; }
}