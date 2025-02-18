namespace ArchiToolkit.InterpolatedParser.Parsers;

/// <summary>
/// This should be one of <see cref="ISpanParser{T}"/> or <see cref="IStringParser{T}"/>
/// </summary>
public interface IParser
{
    /// <summary>
    /// The format string
    /// </summary>
    public string? Format { get; set; }
}