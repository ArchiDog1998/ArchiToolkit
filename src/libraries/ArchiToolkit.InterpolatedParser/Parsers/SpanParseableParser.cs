#if NET7_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;

namespace ArchiToolkit.InterpolatedParser.Parsers;

public readonly struct SpanParseableParser<T> : ISpanParser<T> where T : ISpanParsable<T>
{
    public T Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        return T.Parse(s, provider);
    }

    public bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out T result)
    {
        return T.TryParse(s, provider, out result);
    }
}
#endif