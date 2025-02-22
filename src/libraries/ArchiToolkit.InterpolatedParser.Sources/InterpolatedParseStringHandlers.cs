using System.Runtime.CompilerServices;
using ArchiToolkit.InterpolatedParser.Options;

namespace ArchiToolkit.InterpolatedParser;

[InterpolatedStringHandler]
internal sealed class ParseStringHandler : InterpolatedParseStringHandler
{
    public ParseStringHandler(int literalLength, int formattedCount, string input)
        : this(literalLength, formattedCount, input, new ParseOptions())
    {
    }

    public ParseStringHandler(int literalLength, int formattedCount, string input, ParseOptions options)
        : base(formattedCount, input, false, options)
    {
    }
}

[InterpolatedStringHandler]
internal sealed class TryParseStringHandler : InterpolatedParseStringHandler
{
    public TryParseStringHandler(int literalLength, int formattedCount, string input)
        : this(literalLength, formattedCount, input, new ParseOptions())
    {
    }

    public TryParseStringHandler(int literalLength, int formattedCount, string input, ParseOptions options)
        : base(formattedCount, input, true, options)
    {
    }
}