using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using ArchiToolkit.InterpolatedParser.Options;

namespace ArchiToolkit.InterpolatedParser.ParseItems;

/// <inheritdoc />
public abstract unsafe class ParseItem<T> : IParseItem
{
    private readonly void* _ptr;

    private protected ParseItem(in T value, int index, PreModifyOptions preModify)
    {
        ref var t = ref Unsafe.AsRef(in value);
        _ptr = Unsafe.AsPointer(ref t);
        RegexIndex = index;
        PreModification = preModify;
    }

    /// <inheritdoc />
    public int RegexIndex { get; }

    /// <inheritdoc />
    public PreModifyOptions PreModification { get; }

    private protected void SetValue(in T value)
    {
        Unsafe.AsRef<T>(_ptr) = value;
    }
}