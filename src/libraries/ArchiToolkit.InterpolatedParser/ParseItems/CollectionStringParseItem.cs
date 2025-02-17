using ArchiToolkit.InterpolatedParser.Parsers;

namespace ArchiToolkit.InterpolatedParser.ParseItems;

public class CollectionStringParseItem<TCollection, TValue>(in TCollection value, int index, IStringParser<TValue> parser, string separator)
    : ParseItem<TCollection>(in value, index), IStringParseItem
    where TCollection : ICollection<TValue>, new()
{
    public void Parse(string s, IFormatProvider? provider)
    {
        var item = new TCollection();
        while (true)
        {
            var index = s.IndexOf(separator, StringComparison.CurrentCulture);
            if (index < 0)
            {
                item.Add(parser.Parse(s, provider));
                break;
            }

            item.Add(parser.Parse(s[..index], provider));
            s = s[(index + separator.Length)..];
        }
        SetValue(item);
    }

    public bool TryParse(string s, IFormatProvider? provider)
    {
        var item = new TCollection();
        var result = true;
        while (true)
        {
            var index = s.IndexOf(separator, StringComparison.CurrentCulture);
            if (index < 0)
            {
                if (parser.TryParse(s, provider, out var resultValue))
                {
                    item.Add(resultValue);
                }
                else
                {
                    result = false;
                }
                break;
            }
            else
            {
                if (parser.TryParse(s[..index], provider, out var resultValue))
                {
                    item.Add(resultValue);
                }
                else
                {
                    result = false;
                }
            }

            s = s[(index + separator.Length)..];
        }
        SetValue(item);
        return result;
    }
}