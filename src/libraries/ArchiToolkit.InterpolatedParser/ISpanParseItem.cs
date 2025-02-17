namespace ArchiToolkit.InterpolatedParser.Parsers;

#if NETCOREAPP
public interface ISpanParseItem : IParseItem
{
    void Parse(ReadOnlySpan<char> s);

    bool TryParse(ReadOnlySpan<char> s);
}
#endif