using System.Diagnostics.CodeAnalysis;
using ArchiToolkit.InterpolatedParser.Parsers;

namespace ArchiToolkit.InterpolatedParser.Options;

public readonly record struct ParseOptions()
{
    private readonly Dictionary<string, ParseItemOptions> _options = [];
    public ParseItemOptions[] ParameterOptions
    {
        init
        {
            _options = value.ToDictionary(o => o.ParameterName);
        }
    }

    public IParser[] Parsers { get; init; } = [];

    public ParseItemOptions this[string optionName]
        => _options.TryGetValue(optionName, out var options) ? options :ParseItemOptions.Default;

    public static implicit operator ParseOptions(ParseItemOptions[] options) => new()
    {
        ParameterOptions = options,
    };

    public static implicit operator ParseOptions(IParser[] parsers) => new()
    {
        Parsers = parsers,
    };
}