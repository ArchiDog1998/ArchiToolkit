#if NETCOREAPP
using ArchiToolkit.InterpolatedParser.Options;
using ArchiToolkit.InterpolatedParser.Parsers;

namespace ArchiToolkit.InterpolatedParser.ParseItems;

public class SpanParseItem<T>(in T value, int index, ISpanParser<T> parser, TrimType type)
    : ParseItem<T>(in value, index, type), ISpanParseItem
{
    public void Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        SetValue(parser.Parse(s, provider));
    }

    public bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        if (!parser.TryParse(s, provider, out var result)) return false;
        SetValue(result);
        return true;
    }
}
#endif