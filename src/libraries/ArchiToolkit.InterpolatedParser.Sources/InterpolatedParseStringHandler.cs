using System.Collections;
using System.Data;
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
    private readonly Queue<Func<IFormatProvider?, string>> _replacements;
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
        _replacements = new Queue<Func<IFormatProvider?, string>>(2 * formattedCount + 1);
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
        AppendRegex(_replacements.Count == 0 && !regex.StartsWith("^")
            ? _ => "^" + regex
            : _ => regex);
    }

    private void AppendRegex(Func<IFormatProvider?, string> regexCreator)
    {
        _replacements.Enqueue(regexCreator);
    }

    #region Format

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [OverloadResolutionPriority(-1)]
    // ReSharper disable once MethodOverloadWithOptionalParameter
    public void AppendFormatted(object t, string format, [CallerArgumentExpression(nameof(t))] string callerName = "")
    {
        AppendObject(t, format, callerName);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [OverloadResolutionPriority(-1)]
    public void AppendFormatted(object t, [CallerArgumentExpression(nameof(t))] string callerName = "")
    {
        AppendObject(t, null, callerName);
    }

    private void AppendObject(object t, string? format, string callerName)
    {
        var option = _options[callerName];
        if (IsInput(t, option, format)) return;
        throw new NotImplementedException(
            "The method or operation is not implemented. Please check the source generator.");
    }

    #endregion

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SetFormat(ref IParser? parser, string? format)
    {
        if (parser is null) return;
        parser.Format = format;
    }

    #region Append

    public void AppendCollection<TCollection, TValue>(in TCollection t, string? format, string callerName,
        IParser? collectionParser, IParser? itemParser)
        where TCollection : ICollection<TValue>, new()
    {
        var option = _options[callerName];
        var type = option.TrimType ?? _options.TrimType;
        if (option.DataType is not DataType.List)
        {
            var realParser = GetParser(in t, format, collectionParser, option) ?? FindParser<TValue>();
            SetFormat(ref realParser, format);
            AppendObjectRaw(in t, realParser, type);
        }
        else
        {
            var realParser = GetParser(in t, format, itemParser, option) ?? FindParser<TCollection>();
            SetFormat(ref realParser, format);
            AppendCollectionRaw<TCollection, TValue>(in t, realParser, option.Separator, type);
        }
    }

    public void AppendObject<T>(in T t, string? format, string callerName, IParser? parser)
    {
        var option = _options[callerName];
        var type = option.TrimType ?? _options.TrimType;
        var realParser = GetParser(in t, format, parser, option) ?? FindParser<T>();
        SetFormat(ref realParser, format);
        AppendObjectRaw(in t, realParser, type);
    }

    private IParser? FindParser<T>()
    {
        foreach (var parser in _options.Parsers)
            if (parser.TargetType == typeof(T))
                return parser;

        foreach (var parser in _options.Parsers)
            if (parser.TargetType.IsAssignableFrom(typeof(T)))
                return parser;

        return null;
    }

    private void AppendCollectionRaw<TCollection, TValue>(in TCollection t, IParser? parser, string separator,
        TrimType type)
        where TCollection : ICollection<TValue>, new()
    {
        if (string.IsNullOrEmpty(separator))
            throw new ArgumentNullException(nameof(separator), "Separator cannot be empty.");
        switch (parser)
        {
#if NETCOREAPP
            case ISpanParser<TValue> spanParser:
                _items.Enqueue(
                    new CollectionSpanParseItem<TCollection, TValue>(in t, _replacements.Count, spanParser, separator,
                        type));
                break;
#endif
            case IStringParser<TValue> stringParser:
                _items.Enqueue(new CollectionStringParseItem<TCollection, TValue>(in t, _replacements.Count,
                    stringParser, separator, type));
                break;
            case not null:
                throw new InvalidDataException($"Invalid parser type, which is {parser.GetType()}.");
        }
    }

    private void AppendObjectRaw<T>(in T t, IParser? parser, TrimType type)
    {
        switch (parser)
        {
#if NETCOREAPP
            case ISpanParser<T> spanParser:
                _items.Enqueue(new SpanParseItem<T>(in t, _replacements.Count, spanParser, type));
                break;
#endif
            case IStringParser<T> stringParser:
                _items.Enqueue(new StringParseItem<T>(in t, _replacements.Count, stringParser, type));
                break;
            case not null:
                throw new InvalidDataException($"Invalid parser type, which is {parser.GetType()}.");
        }
    }

    private IParser? GetParser<T>(in T t, string? format, IParser? parser, ParseItemOptions option)
    {
        if (IsInput(t, option, format)) return null;
        var realParser = option.Parser ?? parser;
        if (realParser is null) throw new NullReferenceException("Parser is null!");
        return realParser;
    }

    private bool IsInput<T>(T t, ParseItemOptions option, string? format)
    {
        if (option.ParseType is not ParseType.In) return false;
        if (option.DataType is DataType.List && t is IEnumerable list)
            AppendRegex(p => "^" + string.Join(option.Separator,
                from object? item in list select option.FormatToString(item, format, p)));
        else
            AppendRegex(p => "^" + option.FormatToString(t, format, p));

        return true;
    }

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
                    result.Add(si.TryParse(TrimString(subString, i.TrimType), provider));
                    break;
#if NETCOREAPP
                case ISpanParseItem si:
                    var subSpan = l.HasValue ? t.AsSpan(s, l.Value) : t.AsSpan(s);
                    result.Add(si.TryParse(TrimString(subSpan, i.TrimType), provider));
                    break;
#endif
                default:
                    throw new StrongTypingException("Unexpected IParseItem");
            }
        }, provider);
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
                    si.Parse(TrimString(subString, i.TrimType), provider);
                    break;
#if NETCOREAPP
                case ISpanParseItem si:
                    var subSpan = l.HasValue ? t.AsSpan(s, l.Value) : t.AsSpan(s);
                    si.Parse(TrimString(subSpan, i.TrimType), provider);
                    break;
#endif
                default:
                    throw new StrongTypingException("Unexpected IParseItem");
            }
        }, provider);
    }

    private static string TrimString(string s, TrimType type)
    {
        return type switch
        {
            TrimType.Trim => s.Trim(),
            TrimType.TrimStart => s.TrimStart(),
            TrimType.TrimEnd => s.TrimEnd(),
            _ => s
        };
    }
#if NETCOREAPP

    private static ReadOnlySpan<char> TrimString(ReadOnlySpan<char> s, TrimType type)
    {
        return type switch
        {
            TrimType.Trim => s.Trim(),
            TrimType.TrimStart => s.TrimStart(),
            TrimType.TrimEnd => s.TrimEnd(),
            _ => s
        };
    }
#endif

    private delegate void ParseDelegate(IParseItem item, string text, int start, int? length);

    private void Solve(string input, ParseDelegate action, IFormatProvider? provider)
    {
        var index = 0;
        var stringStart = 0;

        while (_items.TryDequeue(out var item))
        {
            var parsed = false;
            while (_replacements.TryDequeue(out var regexCreator))
            {
                var regex = new Regex(regexCreator(provider));
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