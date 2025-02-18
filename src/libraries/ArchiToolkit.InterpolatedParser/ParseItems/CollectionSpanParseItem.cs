#if NETCOREAPP
using ArchiToolkit.InterpolatedParser.Options;
using ArchiToolkit.InterpolatedParser.Parsers;

namespace ArchiToolkit.InterpolatedParser.ParseItems;

public class CollectionSpanParseItem<TCollection, TValue>(
    in TCollection value,
    int index,
    ISpanParser<TValue> parser,
    string separator,
    TrimType type)
    : ParseItem<TCollection>(in value, index, type), ISpanParseItem
    where TCollection : ICollection<TValue>, new()
{
    public void Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        var item = new TCollection();
        var separatorSpan = separator.AsSpan();
        while (true)
        {
            var index = s.IndexOf(separatorSpan);
            if (index < 0)
            {
                item.Add(parser.Parse(s, provider));
                break;
            }

            item.Add(parser.Parse(s[..index], provider));
            s = s[(index + separatorSpan.Length)..];
        }

        SetValue(item);
    }

    public bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        var item = new TCollection();
        var separatorSpan = separator.AsSpan();
        var result = true;
        while (true)
        {
            var index = s.IndexOf(separatorSpan);
            if (index < 0)
            {
                if (parser.TryParse(s, provider, out var resultValue))
                    item.Add(resultValue);
                else
                    result = false;
                break;
            }
            else
            {
                if (parser.TryParse(s[..index], provider, out var resultValue))
                    item.Add(resultValue);
                else
                    result = false;
            }

            s = s[(index + separatorSpan.Length)..];
        }

        SetValue(item);
        return result;
    }
}
#endif