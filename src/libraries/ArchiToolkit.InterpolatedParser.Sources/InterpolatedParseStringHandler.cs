using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using ArchiToolkit.InterpolatedParser.Options;
using ArchiToolkit.InterpolatedParser.ParseItems;
using ArchiToolkit.InterpolatedParser.Parsers;

namespace ArchiToolkit.InterpolatedParser;

/// <summary>
///     Just the handler
/// </summary>
[InterpolatedStringHandler]
// ReSharper disable once PartialTypeWithSinglePart
internal readonly partial struct InterpolatedParseStringHandler
{
    private readonly Queue<Regex> _replacements;
    private readonly Queue<IParseItem> _items;
    private readonly int _formattedCount;
    private readonly ParseOptions _options = new();

    /// <summary>
    /// </summary>
    /// <param name="literalLength"></param>
    /// <param name="formattedCount"></param>
    // ReSharper disable once UnusedParameter.Local
    public InterpolatedParseStringHandler(int literalLength, int formattedCount)
    {
        _replacements = new Queue<Regex>(2 * formattedCount + 1);
        _items = new Queue<IParseItem>(formattedCount);
        _formattedCount = formattedCount;
    }

    /// <summary>
    /// </summary>
    /// <param name="literalLength"></param>
    /// <param name="formattedCount"></param>
    /// <param name="options"></param>
    public InterpolatedParseStringHandler(int literalLength, int formattedCount, ParseOptions options)
        : this(literalLength, formattedCount)
    {
        _options = options;
    }

    public void AppendLiteral([StringSyntax(StringSyntaxAttribute.Regex)] string regex)
    {
        AppendRegex(_replacements.Count == 0 && !regex.StartsWith("^") ? "^" + regex : regex);
    }

    private void AppendRegex([StringSyntax(StringSyntaxAttribute.Regex)] string regex)
    {
        _replacements.Enqueue(new Regex(regex));
    }

    private bool IsInput<T>(T t, in ParseItemOptions option, string? format)
    {
        if (option.Type is not ParseType.In) return false;
        if (option.CustomToString is { } toString)
            AppendRegex("^" + toString(t, format));
        if (t is IFormattable formattable)
            AppendRegex("^" + formattable.ToString(format, null));
        else
            AppendRegex("^" + (t?.ToString() ?? string.Empty));

        return true;
    }

    #region Format

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [OverloadResolutionPriority(-1)]
    // ReSharper disable once MethodOverloadWithOptionalParameter
    public void AppendFormatted(object t, string format, [CallerArgumentExpression(nameof(t))] string callerName = "")
        => AppendObject(t, format, callerName);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [OverloadResolutionPriority(-1)]
    public void AppendFormatted(object t, [CallerArgumentExpression(nameof(t))] string callerName = "")
        => AppendObject(t, null, callerName);

    private void AppendObject(object t, string? format, string callerName)
    {
        var option = _options[callerName];
        if (IsInput(t, in option, format)) return;
        throw new NotImplementedException(
            "The method or operation is not implemented. Please check the source generator.");
    }

    #endregion

    #region Append

    public void AppendStringObject<T>(in T t, string? format, string callerName, IStringParser<T> parser)
    {
        var option = _options[callerName];
        if (IsInput(t, in option, format)) return;
        if (option.Parser is IStringParser<T> customParser)
        {
            parser = customParser;
        }

        _items.Enqueue(new StringParseItem<T>(in t, _replacements.Count, parser));
    }

    public void AppendStringCollection<TCollection, TValue>(in TCollection t, string? format, string callerName,
        IStringParser<TValue> parser)
        where TCollection : ICollection<TValue>, new()
    {
        var option = _options[callerName];
        if (IsInput(t, in option, format)) return;
        if (string.IsNullOrEmpty(format)) throw new ArgumentException("Format cannot be empty.");
        if (option.Parser is IStringParser<TValue> customParser)
        {
            parser = customParser;
        }

        _items.Enqueue(new CollectionStringParseItem<TCollection, TValue>(in t, _replacements.Count, parser, format));
    }
#if NETCOREAPP
    public void AppendSpanObject<T>(in T t, string? format, string callerName, ISpanParser<T> parser)
    {
        var option = _options[callerName];
        if (IsInput(t, in option, format)) return;
        if (option.Parser is ISpanParser<T> customParser)
        {
            parser = customParser;
        }

        _items.Enqueue(new SpanParseItem<T>(in t, _replacements.Count, parser));
    }


    public void AppendSpanCollection<TCollection, TValue>(in TCollection t, string? format, string callerName,
        ISpanParser<TValue> parser)
        where TCollection : ICollection<TValue>, new()
    {
        var option = _options[callerName];
        if (IsInput(t, in option, format)) return;
        if (string.IsNullOrEmpty(format)) throw new ArgumentException("Format cannot be empty.");
        if (option.Parser is ISpanParser<TValue> customParser)
        {
            parser = customParser;
        }

        _items.Enqueue(new CollectionSpanParseItem<TCollection, TValue>(in t, _replacements.Count, parser, format));
    }
#endif

#if NET7_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendStringParsableObject<T>(in T t, string? format, string callerName)
        where T : IParsable<T>
        => AppendStringObject(in t, format, callerName, new StringParsableParser<T>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendSpanParsableObject<T>(in T t, string? format, string callerName)
        where T : ISpanParsable<T>
        => AppendSpanObject(in t, format, callerName, new SpanParseableParser<T>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendStringParsableCollection<TCollection, TValue>(in TCollection t, string? format, string callerName)
        where TCollection : ICollection<TValue>, new()
        where TValue : IParsable<TValue>
        => AppendStringCollection(in t, format, callerName, new StringParsableParser<TValue>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendSpanParsableCollection<TCollection, TValue>(in TCollection t, string? format, string callerName)
        where TCollection : ICollection<TValue>, new()
        where TValue : ISpanParsable<TValue>
        => AppendSpanCollection(in t, format, callerName, new SpanParseableParser<TValue>());
#endif

    #endregion

    #region Parse

    internal bool[] TryParse(string input, IFormatProvider? provider)
    {
        List<bool> result = new(_formattedCount);
        Solve(input, (i, t, s, l) =>
        {
            switch (i)
            {
                case IStringParseItem si:
                    var subString = l.HasValue ? t.Substring(s, l.Value) : t[s..];
                    result.Add(si.TryParse(subString, provider));
                    break;
#if NETCOREAPP
                case ISpanParseItem si:
                    var subSpan = l.HasValue ? t.AsSpan(s, l.Value) : t.AsSpan(s);
                    result.Add(si.TryParse(subSpan, provider));
                    break;
#endif
                default:
                    throw new StrongTypingException("Unexpected IParseItem");
            }
        });
        return result.ToArray();
    }

    internal void Parse(string input, IFormatProvider? provider)
    {
        Solve(input, (i, t, s, l) =>
        {
            switch (i)
            {
                case IStringParseItem si:
                    var subString = l.HasValue ? t.Substring(s, l.Value) : t[s..];
                    si.Parse(subString, provider);
                    break;
#if NETCOREAPP
                case ISpanParseItem si:
                    var subSpan = l.HasValue ? t.AsSpan(s, l.Value) : t.AsSpan(s);
                    si.Parse(subSpan, provider);
                    break;
#endif
                default:
                    throw new StrongTypingException("Unexpected IParseItem");
            }
        });
    }

    private delegate void ParseDelegate(IParseItem item, string text, int start, int? length);

    private void Solve(string input, ParseDelegate action)
    {
        var index = 0;
        var stringStart = 0;

        while (_items.TryDequeue(out var item))
        {
            var parsed = false;
            while (_replacements.TryDequeue(out var regex))
            {
                var match = regex.Match(input, stringStart);
                if (!match.Success) throw new FormatException("Invalid format to get the parsed string.");
                try
                {
                    if (index++ == item.RegexIndex)
                    {
                        action(item, input, stringStart, match.Index - stringStart);
                        parsed = true;
                        break;
                    }
                }
                finally
                {
                    stringStart = match.Index + match.Length;
                }
            }

            if (!parsed) action(item, input, stringStart, null);
        }
    }

    #endregion

    #region Test

#if NET9_0_OR_GREATER // Waiting for generating!
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted(in int t, string format, [CallerArgumentExpression(nameof(t))] string callerName = "")
        => AppendInt(in t, format, callerName);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted(in int t, [CallerArgumentExpression(nameof(t))] string callerName = "")
        => AppendInt(in t, null, callerName);

    private void AppendInt(in int i, string? format, string callerName)
    {
        AppendSpanParsableObject(in i, format, callerName);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted(in string t, string format,
        [CallerArgumentExpression(nameof(t))] string callerName = "")
        => AppendString(in t, format, callerName);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendFormatted(in string t, [CallerArgumentExpression(nameof(t))] string callerName = "")
        => AppendString(in t, null, callerName);

    private void AppendString(in string i, string? format, string callerName)
    {
        AppendSpanParsableObject(in i, format, callerName);
    }

#endif

    #endregion
}

#if NETFRAMEWORK || NETSTANDARD
file static class QueueExtensions
{
    public static bool TryDequeue<T>(this Queue<T> queue, [MaybeNullWhen(false)]out T value)
    {
        try
        {
            value = queue.Dequeue();
            return true;
        }
        catch
        {
            value = default;
            return false;
        }
    }
}
#endif