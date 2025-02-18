using System.Globalization;

namespace ArchiToolkit.InterpolatedParser.Parsers;

/// <inheritdoc />
public abstract class Parser : IParser
{
    /// <inheritdoc />
    public abstract Type TargetType { get; }

    private static readonly Dictionary<char, NumberStyles> ShorthandMappings = new()
    {
        { 'C', NumberStyles.Currency },
        { 'N', NumberStyles.Number },
        { 'X', NumberStyles.HexNumber },
        { 'G', NumberStyles.Any },
        { 'D', NumberStyles.Integer },
        { 'E', NumberStyles.Float },
        { 'F', NumberStyles.Float },
        { 'P', NumberStyles.Number },
        { 'I', NumberStyles.Integer },
        { 'H', NumberStyles.HexNumber },
        { 'A', NumberStyles.Any }
    };

    /// <inheritdoc />
    public string? Format { get; set; }

    /// <summary>
    /// Get the number style by the format string.
    /// </summary>
    /// <param name="format"></param>
    /// <returns></returns>
    protected static NumberStyles GetStyle(string format)
    {
        if (Enum.TryParse(format, true, out NumberStyles parsedStyle)) return parsedStyle;

        NumberStyles result = 0;

        foreach (var ch in format)
            if (ShorthandMappings.TryGetValue(ch, out var style))
                result |= style;

        return result;
    }
}