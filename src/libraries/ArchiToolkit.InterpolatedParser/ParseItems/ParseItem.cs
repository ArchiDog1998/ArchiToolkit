using System.Runtime.CompilerServices;

namespace ArchiToolkit.InterpolatedParser.ParseItems;

public abstract unsafe class ParseItem<T> : IParseItem
{
    private readonly void* _ptr;

    protected ParseItem(in T value, int index)
    {
        ref var t = ref Unsafe.AsRef(in value);
        _ptr = Unsafe.AsPointer(ref t);
        RegexIndex = index;
    }

    public int RegexIndex { get; }

    protected void SetValue(in T value)
    {
        Unsafe.AsRef<T>(_ptr) = value;
    }
}