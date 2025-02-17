namespace ArchiToolkit.InterpolatedParser.Parsers;

public interface IStringParseItem : IParseItem
{
    void Parse(string s);

    bool TryParse(string s);
}