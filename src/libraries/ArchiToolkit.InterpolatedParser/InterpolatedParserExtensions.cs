using System.Diagnostics.CodeAnalysis;

namespace ArchiToolkit.InterpolatedParser;

/// <summary>
/// The extensions
/// </summary>
public static class InterpolatedParserExtensions
{
    public static void Parse(this string input,
        [StringSyntax(StringSyntaxAttribute.Regex)]InterpolatedParseStringHandler template)
    {
        template.Parse(input);
    }

    public static bool[] TryParse(this string input,
        [StringSyntax(StringSyntaxAttribute.Regex)] InterpolatedParseStringHandler template)
    {
        return template.TryParse(input);
    }
}