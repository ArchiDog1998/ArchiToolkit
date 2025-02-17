using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
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
    private readonly string[] _inputs = [];

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
    /// <param name="inputs"></param>
    public InterpolatedParseStringHandler(int literalLength, int formattedCount, string[] inputs)
        : this(literalLength, formattedCount)
    {
        _inputs = inputs;
    }

    public void AppendLiteral([StringSyntax(StringSyntaxAttribute.Regex)] string regex)
    {
        AppendRegex(_replacements.Count == 0 ? "^" + regex : regex);
    }

    private void AppendRegex([StringSyntax(StringSyntaxAttribute.Regex)] string regex)
    {
        _replacements.Enqueue(new Regex(regex));
    }

    private bool IsInput<T>(T t, string? format, string callerName)
    {
        if (!_inputs.Contains(callerName)) return false;
        if (t is IFormattable formattable)
            AppendRegex("^" + formattable.ToString(format, null));
        else
            AppendRegex("^" + (t?.ToString() ?? string.Empty));

        return true;
    }

    #region Format

#if NET7_0_OR_GREATER
    // ReSharper disable once MethodOverloadWithOptionalParameter
    public void AppendFormatted<T>(in T t, string format, [CallerArgumentExpression(nameof(t))] string callerName = "")
        where T : ISpanParsable<T>
    {
        if (IsInput(t, format, callerName)) return;
        _items.Enqueue(new SpanParseItem<T>(in t, _replacements.Count));
    }

    public void AppendFormatted<T>(in T t, [CallerArgumentExpression(nameof(t))] string callerName = "")
        where T : ISpanParsable<T>
    {
        if (IsInput(t, null, callerName)) return;
        _items.Enqueue(new SpanParseItem<T>(in t, _replacements.Count));
    }
#endif

    [OverloadResolutionPriority(-1)]
    // ReSharper disable once MethodOverloadWithOptionalParameter
    public void AppendFormatted(object t, string format, [CallerArgumentExpression(nameof(t))] string callerName = "")
    {
        if (IsInput(t, format, callerName)) return;
        throw new NotImplementedException(
            "The method or operation is not implemented. Please check the source generator.");
    }

    [OverloadResolutionPriority(-1)]
    public void AppendFormatted(object t, [CallerArgumentExpression(nameof(t))] string callerName = "")
    {
        if (IsInput(t, null, callerName)) return;
        throw new NotImplementedException(
            "The method or operation is not implemented. Please check the source generator.");
    }

    #endregion


    #region Parse

    internal bool[] TryParse(string input)
    {
        List<bool> result = new(_formattedCount);
        Solve(input, (i, t, s, l) =>
        {
            switch (i)
            {
                case IStringParseItem si:
                    var subString = l.HasValue ? t.Substring(s, l.Value) : t[s..];
                    result.Add(si.TryParse(subString));
                    break;
#if NETCOREAPP
                case ISpanParseItem si:
                    var subSpan = l.HasValue ? t.AsSpan(s, l.Value) : t.AsSpan(s);
                    result.Add(si.TryParse(subSpan));
                    break;
#endif
                default:
                    throw new StrongTypingException("Unexpected IParseItem");
            }
        });
        return result.ToArray();
    }

    internal void Parse(string input)
    {
        Solve(input, (i, t, s, l) =>
        {
            switch (i)
            {
                case IStringParseItem si:
                    var subString = l.HasValue ? t.Substring(s, l.Value) : t[s..];
                    si.Parse(subString);
                    break;
#if NETCOREAPP
                case ISpanParseItem si:
                    var subSpan = l.HasValue ? t.AsSpan(s, l.Value) : t.AsSpan(s);
                    si.Parse(subSpan);
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

#if NETCOREAPP
#else
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