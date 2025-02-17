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
    /// <param name="provider"></param>
    public static void Parse(this string input,
        [StringSyntax(StringSyntaxAttribute.Regex)]
        InterpolatedParseStringHandler template,
        IFormatProvider? provider = null)
    {
        template.Parse(input, provider);
    }

    /// <summary>
    /// </summary>
    /// <param name="input"></param>
    /// <param name="options"></param>
    /// <param name="template"></param>
    /// <param name="provider"></param>
    public static void Parse(this string input, ParseOptions options,
        [StringSyntax(StringSyntaxAttribute.Regex)] [InterpolatedStringHandlerArgument(nameof(options))]
        InterpolatedParseStringHandler template,
        IFormatProvider? provider = null)
    {
        template.Parse(input, provider);
    }

    /// <summary>
    /// </summary>
    /// <param name="input"></param>
    /// <param name="template"></param>
    /// <param name="provider"></param>
    /// <returns></returns>
    public static bool[] TryParse(this string input,
        [StringSyntax(StringSyntaxAttribute.Regex)]
        InterpolatedParseStringHandler template,
        IFormatProvider? provider = null)
    {
        return template.TryParse(input, provider);
    }

    /// <summary>
    /// </summary>
    /// <param name="input"></param>
    /// <param name="options"></param>
    /// <param name="template"></param>
    /// <param name="provider"></param>
    /// <returns></returns>
    public static bool[] TryParse(this string input, ParseOptions options,
        [StringSyntax(StringSyntaxAttribute.Regex)] [InterpolatedStringHandlerArgument(nameof(options))]
        InterpolatedParseStringHandler template,
        IFormatProvider? provider = null)
    {
        return template.TryParse(input, provider);
    }
}