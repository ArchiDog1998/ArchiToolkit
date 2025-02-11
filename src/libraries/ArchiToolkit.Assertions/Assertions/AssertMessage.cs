using System.Collections;
using System.Diagnostics.CodeAnalysis;
using ArchiToolkit.Assertions.Utils;

namespace ArchiToolkit.Assertions.Assertions;

/// <summary>
/// </summary>
/// <param name="structuredFormat">
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
/// <param name="additionalArguments"></param>
public readonly struct AssertMessage(
    [StringSyntax(StringSyntaxAttribute.CompositeFormat)]
    string structuredFormat,
    params Argument[] additionalArguments)
{

    /// <summary>
    /// The Structured Format
    /// </summary>
    public string StructuredFormat => structuredFormat;

    /// <summary>
    /// The general format
    /// </summary>
    public string Format
    {
        get
        {
            var index = 0;
            return additionalArguments.Aggregate(structuredFormat,
                (current, argument) => current.ReplacePlaceHolder(argument.Name, (index++).ToString()));
        }
    }

    /// <summary>
    /// The structured Arguments
    /// </summary>
    internal Argument[] StructuredArguments => additionalArguments;

    /// <summary>
    /// The argument
    /// </summary>
    public object?[] Arguments => StructuredArguments.Select(a => a.Value).ToArray();

    /// <summary>
    ///     convert by the default string.
    /// </summary>
    /// <param name="reason"></param>
    /// <returns></returns>
    public static implicit operator AssertMessage([StringSyntax(StringSyntaxAttribute.CompositeFormat)]string reason)
    {
        return new AssertMessage(reason);
    }

    /// <inheritdoc />
    public override string ToString() => string.Format(Format, [..Arguments.Select(a => a.GetObjectString())]);
}