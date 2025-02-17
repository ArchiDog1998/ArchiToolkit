using System.Runtime.CompilerServices;

namespace ArchiToolkit.InterpolatedParser.Parsers;

#if NET7_0_OR_GREATER
public readonly unsafe struct SpanParseItem<T> : ISpanParseItem where T : ISpanParsable<T>
{
    private readonly void* _ptr;
    private readonly IFormatProvider? _provider;
    public int RegexIndex { get; }

    public SpanParseItem(in T value, int index, IFormatProvider? provider = null)
    {
        ref var t = ref Unsafe.AsRef(in value);
        _ptr = Unsafe.AsPointer(ref t);
        _provider = provider;
        RegexIndex = index;
    }

    public void Parse(ReadOnlySpan<char> s)
    {
        Unsafe.AsRef<T>(_ptr) = T.Parse(s, _provider);
    }

    public bool TryParse(ReadOnlySpan<char> s)
    {
        if (!T.TryParse(s, _provider, out var result)) return false;
        Unsafe.AsRef<T>(_ptr) = result;
        return true;
    }
}
#endif