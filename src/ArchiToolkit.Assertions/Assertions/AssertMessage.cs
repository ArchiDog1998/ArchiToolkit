using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace ArchiToolkit.Assertions.Assertions;

/// <summary>
/// </summary>
/// <param name="Format">
/// <list type="table">
///   <listheader>
///     <term>Key Name</term>
///     <description>Description</description>
///   </listheader>
///   <item>
///     <term>Subject</term>
///     <description>just the subject with <see cref="object.ToString()"/></description>
///   </item>
///   <item>
///     <term>Subjects</term>
///     <description>the subject with <see cref="IEnumerable"/> format string</description>
///   </item>
///   <item>
///     <term>SubjectName</term>
///     <description>the name of the Subject</description>
///   </item>
///   <item>
///     <term>AssertionType</term>
///     <description>the assertion type typed named with <see cref="AssertionType"/> format string</description>
///   </item>
///   <item>
///     <term>Not</term>
///     <description>The Reverse string</description>
///   </item>
/// </list>
/// </param>
/// <param name="AdditionalArguments"></param>
public readonly record struct AssertMessage(
    [StringSyntax(StringSyntaxAttribute.CompositeFormat)]
    string Format,
    params Argument[] AdditionalArguments)
{
    /// <summary>
    ///     convert by the default string.
    /// </summary>
    /// <param name="reason"></param>
    /// <returns></returns>
    public static implicit operator AssertMessage([StringSyntax(StringSyntaxAttribute.CompositeFormat)]string reason)
    {
        return new AssertMessage(reason);
    }
}