#if NETCOREAPP
using ArchiToolkit.InterpolatedParser.Options;
using ArchiToolkit.InterpolatedParser.Parsers;

namespace ArchiToolkit.InterpolatedParser.ParseItems;

/// <summary>
/// Parse by the span
/// </summary>
/// <param name="value"></param>
/// <param name="index"></param>
/// <param name="parser"></param>
/// <param name="preModify"></param>
/// <typeparam name="T"></typeparam>
public sealed class SpanParseItem<T>(in T value, int index, ISpanParser<T> parser, PreModifyOptions preModify)
    : ParseItem<T>(in value, index, preModify), ISpanParseItem
{
    /// <inheritdoc />
    public void Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        SetValue(parser.Parse(s, provider));
    }

    /// <inheritdoc />
    public bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        if (!parser.TryParse(s, provider, out var result)) return false;
        SetValue(result);
        return true;
    }
}
#endif