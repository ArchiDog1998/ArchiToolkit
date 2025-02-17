#if NETCOREAPP
using System.Diagnostics.CodeAnalysis;

namespace ArchiToolkit.InterpolatedParser.Parsers;

public interface ISpanParser<T> : IParser
{
    T Parse(ReadOnlySpan<char> s, IFormatProvider? provider);
    bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out T result);
}
#endif