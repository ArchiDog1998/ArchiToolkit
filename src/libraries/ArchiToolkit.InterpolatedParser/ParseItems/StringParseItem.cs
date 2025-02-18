using ArchiToolkit.InterpolatedParser.Options;
using ArchiToolkit.InterpolatedParser.Parsers;

namespace ArchiToolkit.InterpolatedParser.ParseItems;

/// <summary>
/// Parse by the string.
/// </summary>
/// <param name="value"></param>
/// <param name="index"></param>
/// <param name="parser"></param>
/// <param name="preModify"></param>
/// <typeparam name="T"></typeparam>
public sealed class StringParseItem<T>(in T value, int index, IStringParser<T> parser, PreModifyOptions preModify)
    : ParseItem<T>(in value, index, preModify), IStringParseItem
{
    /// <inheritdoc />
    public void Parse(string s, IFormatProvider? provider)
    {
        SetValue(parser.Parse(s, provider));
    }

    /// <inheritdoc />
    public bool TryParse(string s, IFormatProvider? provider)
    {
        if (!parser.TryParse(s, provider, out var result)) return false;
        SetValue(result);
        return true;
    }
}