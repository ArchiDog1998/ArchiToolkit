﻿using System.Diagnostics.CodeAnalysis;

namespace ArchiToolkit.InterpolatedParser.Parsers;

public abstract class SpanParser<T>: Parser, ISpanParser<T>
{
    /// <inheritdoc />
    public sealed override Type TargetType => typeof(T);

    /// <inheritdoc />
    public abstract T Parse(ReadOnlySpan<char> s, IFormatProvider? provider);

    /// <inheritdoc />
    public abstract bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out T result);
}