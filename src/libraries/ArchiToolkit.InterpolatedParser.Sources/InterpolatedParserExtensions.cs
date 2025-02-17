using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace ArchiToolkit.InterpolatedParser;

/// <summary>
/// The extensions
/// </summary>
internal static class InterpolatedParserExtensions
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="input"></param>
    /// <param name="template"></param>
    public static void Parse(this string input,
        [StringSyntax(StringSyntaxAttribute.Regex)]InterpolatedParseStringHandler template)
    {
        template.Parse(input);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="input"></param>
    /// <param name="inputs"></param>
    /// <param name="template"></param>
    public static void Parse(this string input, string[] inputs,
        [StringSyntax(StringSyntaxAttribute.Regex)][InterpolatedStringHandlerArgument(nameof(inputs))]InterpolatedParseStringHandler template)
    {
        template.Parse(input);
    }


    /// <summary>
    ///
    /// </summary>
    /// <param name="input"></param>
    /// <param name="template"></param>
    /// <returns></returns>
    public static bool[] TryParse(this string input,
        [StringSyntax(StringSyntaxAttribute.Regex)]InterpolatedParseStringHandler template)
    {
        return template.TryParse(input);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="input"></param>
    /// <param name="inputs"></param>
    /// <param name="template"></param>
    /// <returns></returns>
    public static bool[] TryParse(this string input, string[] inputs,
        [StringSyntax(StringSyntaxAttribute.Regex)][InterpolatedStringHandlerArgument(nameof(inputs))]InterpolatedParseStringHandler template)
    {
        return template.TryParse(input);
    }
}