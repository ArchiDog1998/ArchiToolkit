using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using ArchiToolkit.Assertions.AssertionItems;
using ArchiToolkit.Assertions.Constraints;
using ArchiToolkit.Assertions.Resources;

namespace ArchiToolkit.Assertions.Assertions.Extensions;

/// <summary>
///     For string extensions.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    ///     The expression
    /// </summary>
    /// <param name="assertion"></param>
    /// <param name="regularExpression"></param>
    /// <param name="reasonFormat"></param>
    /// <param name="reasonArgs"></param>
    /// <returns></returns>
    public static AndConstraint<string> MatchRegex(this ObjectAssertion<string> assertion,
        [StringSyntax(StringSyntaxAttribute.Regex)]
        string regularExpression,
        [StringSyntax(StringSyntaxAttribute.CompositeFormat)]
        string reasonFormat = "", params object?[] reasonArgs)
    {
        var regex = new Regex(regularExpression);

        return assertion.AssertCheck(
            regex.IsMatch(assertion.Subject), AssertionItemType.Match,
            AssertionLocalization.MatchAssertion,
            [
                new Argument("Expression", regularExpression)
            ],
            reasonFormat, reasonArgs);
    }

    /// <summary>
    ///     match regex.
    /// </summary>
    /// <param name="assertion"></param>
    /// <param name="regularExpression"></param>
    /// <param name="expectedMatchCount"></param>
    /// <param name="reasonFormat"></param>
    /// <param name="reasonArgs"></param>
    /// <returns></returns>
    public static AndConstraint<string> MatchRegex(this ObjectAssertion<string> assertion,
        [StringSyntax(StringSyntaxAttribute.Regex)]
        string regularExpression,
        int expectedMatchCount,
        [StringSyntax(StringSyntaxAttribute.CompositeFormat)]
        string reasonFormat = "", params object?[] reasonArgs)
    {
        var actualMatchCount = new Regex(regularExpression).Matches(assertion.Subject).Count;
        return assertion.AssertCheck(actualMatchCount == expectedMatchCount, AssertionItemType.Match,
            AssertionLocalization.MatchCountAssertion,
            [
                new Argument("Expression", regularExpression),
                new Argument("ActualMatchCount", actualMatchCount),
                new Argument("ExpectedMatchCount", expectedMatchCount)
            ],
            reasonFormat, reasonArgs);
    }
}