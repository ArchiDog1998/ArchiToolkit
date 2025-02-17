#if NETCOREAPP

namespace ArchiToolkit.InterpolatedParser.ParseItems;

public interface ISpanParseItem : IParseItem
{
    void Parse(ReadOnlySpan<char> s, IFormatProvider? provider);

    bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider);
}
#endif