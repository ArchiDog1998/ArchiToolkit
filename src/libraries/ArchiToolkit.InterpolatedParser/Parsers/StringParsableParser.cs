#if NET7_0_OR_GREATER

using System.Diagnostics.CodeAnalysis;

namespace ArchiToolkit.InterpolatedParser.Parsers;

public abstract class StringParsableParser<T> : Parser, IStringParser<T> where T : IParsable<T>
{
    public T Parse(string s, IFormatProvider? provider)
    {
        return T.Parse(s, provider);
    }

    public bool TryParse(string s, IFormatProvider? provider, [MaybeNullWhen(false)] out T result)
    {
        return T.TryParse(s, provider, out result);
    }
}
#endif
