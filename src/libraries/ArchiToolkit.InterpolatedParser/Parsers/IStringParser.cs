using System.Diagnostics.CodeAnalysis;

namespace ArchiToolkit.InterpolatedParser.Parsers;

public interface IStringParser<T> : IParser
{
    T Parse(string s, IFormatProvider? provider);
    bool TryParse(string s, IFormatProvider? provider, [MaybeNullWhen(false)] out T result);
}