namespace ArchiToolkit.InterpolatedParser.ParseItems;

public interface IStringParseItem : IParseItem
{
    void Parse(string s, IFormatProvider? provider);

    bool TryParse(string s, IFormatProvider? provider);
}