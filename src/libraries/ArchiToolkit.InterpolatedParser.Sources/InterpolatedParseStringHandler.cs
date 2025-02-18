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

    public void AppendCollection<TCollection, TValue>(in TCollection t, string? format, string callerName,
        IParser? collectionParser, IParser? itemParser)
        where TCollection : ICollection<TValue>, new()
    {
        var option = _options[callerName];
        if (option.DataType is not DataType.List)
        {
            var realParser = GetParser(in t, format, collectionParser, option);
            AppendObjectRaw(in t, realParser);
        }
        else
        {
            var realParser = GetParser(in t, format, itemParser, option);
            AppendCollectionRaw<TCollection, TValue>(in t, realParser, option.Separator);
        }
    }

    public void AppendObject<T>(in T t, string? format, string callerName, IParser? parser)
    {
        var option = _options[callerName];
        var realParser = GetParser(in t, format, parser, option);
        AppendObjectRaw(in t, realParser);
    }

    private void AppendCollectionRaw<TCollection, TValue>(in TCollection t, IParser? parser, string separator)
        where TCollection : ICollection<TValue>, new()
    {
        if (string.IsNullOrEmpty(separator))
            throw new ArgumentNullException(nameof(separator), "Separator cannot be empty.");
        switch (parser)
        {
#if NETCOREAPP
            case ISpanParser<TValue> spanParser:
                _items.Enqueue(
                    new CollectionSpanParseItem<TCollection, TValue>(in t, _replacements.Count, spanParser, separator));
                break;
#endif
            case IStringParser<TValue> stringParser:
                _items.Enqueue(new CollectionStringParseItem<TCollection, TValue>(in t, _replacements.Count,
                    stringParser, separator));
                break;
            case not null:
                throw new InvalidDataException($"Invalid parser type, which is {parser.GetType()}.");
        }
    }

    private void AppendObjectRaw<T>(in T t, IParser? parser)
    {
        switch (parser)
        {
#if NETCOREAPP
            case ISpanParser<T> spanParser:
                _items.Enqueue(new SpanParseItem<T>(in t, _replacements.Count, spanParser));
                break;
#endif
            case IStringParser<T> stringParser:
                _items.Enqueue(new StringParseItem<T>(in t, _replacements.Count, stringParser));
                break;
            case not null:
                throw new InvalidDataException($"Invalid parser type, which is {parser.GetType()}.");
        }
    }

    private IParser? GetParser<T>(in T t, string? format, IParser? parser, ParseItemOptions option)
    {
        if (IsInput(in t, in option, format)) return null;
        var realParser = option.Parser ?? parser;
        if (realParser is null) throw new NullReferenceException("Parser is null!");
        return realParser;
    }

    private bool IsInput<T>(in T t, in ParseItemOptions option, string? format)
    {
        if (option.ParseType is not ParseType.In) return false;
        if (option.DataType is DataType.List && t is IEnumerable list)
        {
            List<string> stringItems = [];
            foreach (var item in list)
            {
                stringItems.Add(option.FormatToString(item, format));
            }

            AppendRegex("^" + string.Join(option.Separator, stringItems));
        }
        else
        {
            AppendRegex("^" + option.FormatToString(t, format));
        }

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

