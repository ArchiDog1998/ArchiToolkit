using ArchiToolkit.InterpolatedParser.Options;
using ArchiToolkit.InterpolatedParser.Parsers;

namespace ArchiToolkit.InterpolatedParser.ParseItems;

/// <summary>
/// The collection parser.
/// </summary>
/// <param name="value"></param>
/// <param name="index"></param>
/// <param name="parser"></param>
/// <param name="separator"></param>
/// <param name="preModify"></param>
/// <typeparam name="TCollection"></typeparam>
/// <typeparam name="TValue"></typeparam>
public sealed class CollectionStringParseItem<TCollection, TValue>(
    in TCollection value,
    int index,
    IStringParser<TValue> parser,
    string separator,
    PreModifyOptions preModify)
    : ParseItem<TCollection>(in value, index, preModify), IStringParseItem
    where TCollection : ICollection<TValue>, new()
{
    /// <inheritdoc />

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

    /// <inheritdoc />

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

            s = s[(index + separator.Length)..];
        }

        SetValue(item);
        return result;
    }
}