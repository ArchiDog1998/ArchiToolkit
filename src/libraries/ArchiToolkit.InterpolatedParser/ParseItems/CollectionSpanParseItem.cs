#if NETCOREAPP
using ArchiToolkit.InterpolatedParser.Options;
using ArchiToolkit.InterpolatedParser.Parsers;

namespace ArchiToolkit.InterpolatedParser.ParseItems;

/// <summary>
/// The parse item for collections.
/// </summary>
/// <param name="value"></param>
/// <param name="index"></param>
/// <param name="parser"></param>
/// <param name="separator"></param>
/// <param name="preModify"></param>
/// <typeparam name="TCollection"></typeparam>
/// <typeparam name="TValue"></typeparam>
public sealed class CollectionSpanParseItem<TCollection, TValue>(
    in TCollection value,
    int index,
    ISpanParser<TValue> parser,
    string separator,
    PreModifyOptions preModify)
    : ParseItem<TCollection>(in value, index, preModify), ISpanParseItem
    where TCollection : ICollection<TValue>, new()
{
    /// <inheritdoc />
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

    /// <inheritdoc />
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