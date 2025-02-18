using ArchiToolkit.InterpolatedParser.Parsers;

namespace ArchiToolkit.InterpolatedParser.Options;

public readonly record struct ParseItemOptions(string ParameterName)
{
    public static readonly ParseItemOptions Default = new();

    public delegate string ToStringDelegate(object? value, string? format);

    public ParseType ParseType { get; init; } = ParseType.Out;
    public DataType DataType { get; init; } = DataType.List;
    public IParser? Parser { get; init; }
    public string Separator { get; init; } = ",";

    public ToStringDelegate? CustomToString { get; init; }

    public string FormatToString(object? value, string? format)
    {
        if (CustomToString is { } toString)
            return toString(value, format);
        if (value is IFormattable formattable)
            return formattable.ToString(format, null); //TODO: Shall we send a format provider?
        return value?.ToString() ?? "<null>";
    }
}