using System.Runtime.CompilerServices;
using ArchiToolkit.InterpolatedParser.Options;

namespace ArchiToolkit.InterpolatedParser.ParseItems;

public abstract unsafe class ParseItem<T> : IParseItem
{
    private readonly void* _ptr;

    protected ParseItem(in T value, int index, TrimType trimType)
    {
        ref var t = ref Unsafe.AsRef(in value);
        _ptr = Unsafe.AsPointer(ref t);
        RegexIndex = index;
        TrimType = trimType;
    }

    public int RegexIndex { get; }

    public TrimType TrimType { get; }

    protected void SetValue(in T value)
    {
        Unsafe.AsRef<T>(_ptr) = value;
    }
}