using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace ArchiToolkit.InterpolatedParser;

/// <summary>
///     Just the handler
/// </summary>
[InterpolatedStringHandler]
public readonly struct InterpolatedParseStringHandler
{
    private readonly Queue<Regex> _replacements;
    private readonly Queue<IParseItem> _items;
    private readonly int _formattedCount;

    /// <summary>
    ///
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

    public void AppendLiteral([StringSyntax(StringSyntaxAttribute.Regex)] string s)
    {
        _replacements.Enqueue(new Regex(s));
    }


    public void AppendFormatted<T>(in T t, [StringSyntax(StringSyntaxAttribute.NumericFormat)]string format) where T : ISpanParsable<T>
    {
        _items.Enqueue(new Parser<T>(in t, _replacements.Count));
    }

    public void AppendFormatted<T>(in T t) where T : ISpanParsable<T>
    {
        _items.Enqueue(new Parser<T>(in t, _replacements.Count));
    }

    public bool[] TryParse(string input)
    {
        List<bool> result = new(_formattedCount);
        Solve(input, (i, s) => result.Add(i.TryParse(s)));
        return result.ToArray();
    }

    public void Parse(string input)
    {
        Solve(input, (i, s) => i.Parse(s));
    }

    private delegate void ParseDelegate(IParseItem item, ReadOnlySpan<char> buffer);

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
                        action(item, input.AsSpan(stringStart, match.Index - stringStart));
                        parsed = true;
                        break;
                    }
                }
                finally
                {
                    stringStart = match.Index + match.Length;
                }
            }

            if (!parsed) action(item, input.AsSpan(stringStart));
        }
    }
}

file readonly unsafe struct Parser<T> : IParseItem where T : ISpanParsable<T>
{
    private readonly void* _ptr;
    private readonly IFormatProvider? _provider;

    public Parser(in T value, int index, IFormatProvider? provider = null)
    {
        ref var t = ref Unsafe.AsRef(in value);
        _ptr = Unsafe.AsPointer(ref t);
        _provider = provider;
        RegexIndex = index;
    }

    public int RegexIndex { get; }

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

internal interface IParseItem
{
    public int RegexIndex { get; }
    void Parse(ReadOnlySpan<char> s);
    bool TryParse(ReadOnlySpan<char> s);
}