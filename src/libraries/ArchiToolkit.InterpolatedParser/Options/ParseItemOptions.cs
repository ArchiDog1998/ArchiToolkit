using ArchiToolkit.InterpolatedParser.Parsers;

namespace ArchiToolkit.InterpolatedParser.Options;

public readonly record struct ParseItemOptions(string ParameterName)
{
    public static readonly ParseItemOptions Default = new();

    public delegate string ToStringDelegate(object? value, string? format);

    public ParseType Type { get; init; } = ParseType.List;

    public IParser? Parser { get; init; }

    public ToStringDelegate? CustomToString { get; init; }
}