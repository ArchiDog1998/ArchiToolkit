using ArchiToolkit.InterpolatedParser.Parsers;

namespace ArchiToolkit.InterpolatedParser.ParseItems;

public class StringParseItem<T>(in T value, int index, IStringParser<T> parser)
    : ParseItem<T>(in value, index), IStringParseItem
{
    public void Parse(string s, IFormatProvider? provider)
    {
        SetValue(parser.Parse(s, provider));
    }

    public bool TryParse(string s, IFormatProvider? provider)
    {
        if (!parser.TryParse(s, provider, out var result)) return false;
        SetValue(result);
        return true;
    }
}