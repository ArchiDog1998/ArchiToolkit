using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using ArchiToolkit.InterpolatedParser.Options;

namespace ArchiToolkit.InterpolatedParser;

/// <summary>
///     The extensions
/// </summary>
internal static class InterpolatedParserExtensions
{
    /// <summary>
    ///     The basic Parse
    /// </summary>
    /// <param name="input"></param>
    /// <param name="template"></param>
    public static void Parse(this string input,
        [StringSyntax(StringSyntaxAttribute.Regex)][InterpolatedStringHandlerArgument(nameof(input))]
        InterpolatedParseStringHandler template)
    {
        template.Parse();
    }

    /// <summary>
    /// </summary>
    /// <param name="input"></param>
    /// <param name="options"></param>
    /// <param name="template"></param>
    public static void Parse(this string input, ParseOptions options,
        [StringSyntax(StringSyntaxAttribute.Regex)] [InterpolatedStringHandlerArgument(nameof(input), nameof(options))]
        InterpolatedParseStringHandler template)
    {
        template.Parse();
    }

    /// <summary>
    /// </summary>
    /// <param name="input"></param>
    /// <param name="template"></param>
    /// <returns></returns>
    public static ParseResult[] TryParse(this string input,
        [StringSyntax(StringSyntaxAttribute.Regex)][InterpolatedStringHandlerArgument(nameof(input))]
        InterpolatedParseStringHandler template)
    {
        return template.TryParse();
    }

    /// <summary>
    /// </summary>
    /// <param name="input"></param>
    /// <param name="options"></param>
    /// <param name="template"></param>
    /// <returns></returns>
    public static ParseResult[] TryParse(this string input, ParseOptions options,
        [StringSyntax(StringSyntaxAttribute.Regex)] [InterpolatedStringHandlerArgument(nameof(input), nameof(options))]
        InterpolatedParseStringHandler template)
    {
        return template.TryParse();
    }
}