﻿using System.Diagnostics.Contracts;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace ArchiToolkit.RoslynHelper.Extensions;

/// <summary>
///     The extensions for the <see cref="Microsoft.CodeAnalysis.CSharp.SyntaxFactory" />
/// </summary>
public static class SyntaxExtensions
{
    /// <summary>
    ///     Add the comment
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
    /// <param name="node"></param>
    /// <param name="comment"></param>
    /// <returns></returns>
    public static TNode WithXmlComment<TNode>(this TNode node, string comment)
        where TNode : SyntaxNode
    {
        return node.WithLeadingTrivia(TriviaList([Comment(comment)]));
    }

    /// <summary>
    ///     Generate a node by the string.
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
    /// <param name="code"></param>
    /// <returns></returns>
    public static TNode ParseSyntax<TNode>(string code) where TNode : MemberDeclarationSyntax
    {
        return (TNode)((CompilationUnitSyntax)ParseSyntaxTree(code).GetRoot()).Members[0];
    }

    /// <summary>
    ///     Generate the basic namespace declaration.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="comment"></param>
    /// <returns></returns>
    public static BaseNamespaceDeclarationSyntax NamespaceDeclaration(string name, string? comment = null)
    {
        return MakeGenerated( FileScopedNamespaceDeclaration(ParseName(name)), comment);
    }

    /// <summary>
    /// Make the node as the auto generated.
    /// </summary>
    /// <param name="syntax"></param>
    /// <param name="comment"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T MakeGenerated<T>(T syntax, string? comment = null) where T : SyntaxNode
    {
        var text = "// <auto-generated/>";
        if (comment != null)
            text = "// <auto-generated>"
                   + "\n" + comment.Leading("// ")
                   + "\n// </auto-generated>";

        return syntax.WithLeadingTrivia(TriviaList(
                Comment(text),
                Trivia(PragmaWarningDirectiveTrivia(Token(SyntaxKind.DisableKeyword), true)),
                Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword), true))));
    }

    #region Attribute

    /// <summary>
    /// </summary>
    /// <param name="generator"></param>
    /// <returns></returns>
    public static AttributeListSyntax GeneratedCodeAttribute(Type generator)
    {
        return AttributeList([
            Attribute(IdentifierName("global::System.CodeDom.Compiler.GeneratedCode"))
                .AddArgumentListArguments(
                    AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression,
                        Literal(generator.FullName ?? generator.Name))),
                    AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression,
                        Literal(generator.Assembly.GetName().Version?.ToString() ?? "1.0.0"))))
        ]);
    }

    /// <summary>
    ///     No user code attribute.
    /// </summary>
    /// <returns></returns>
    public static AttributeSyntax NonUserCodeAttribute()
    {
        return Attribute(IdentifierName("global::System.Diagnostics.DebuggerNonUserCode"));
    }

    /// <summary>
    ///    Obsolete Attribute
    /// </summary>
    /// <returns></returns>
    public static AttributeSyntax ObsoleteAttribute()
    {
        return Attribute(IdentifierName("global::System.Obsolete"));
    }

    /// <summary>
    /// Pure attribute.
    /// </summary>
    /// <returns></returns>
    public static AttributeSyntax PureAttribute()
    {
        return Attribute(IdentifierName("global::System.Diagnostics.Contracts.Pure"));
    }

    /// <summary>
    /// </summary>
    /// <param name="description"></param>
    /// <returns></returns>
    public static AttributeSyntax DescriptionAttribute(string description)
    {
        var attributeArgument =
            AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(description)));
        return Attribute(IdentifierName("global::System.ComponentModel.Description"),
            AttributeArgumentList(SingletonSeparatedList(attributeArgument)));
    }

    #endregion

    #region Enum Member

    /// <summary>
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static EnumMemberDeclarationSyntax EnumMember(string name, byte value)
    {
        return EnumMemberDeclaration(name)
            .WithEqualsValue(EqualsValueClause(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(value))));
    }

    /// <summary>
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static EnumMemberDeclarationSyntax EnumMember(string name, ushort value)
    {
        return EnumMemberDeclaration(name)
            .WithEqualsValue(EqualsValueClause(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(value))));
    }

    /// <summary>
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static EnumMemberDeclarationSyntax EnumMember(string name, uint value)
    {
        return EnumMemberDeclaration(name)
            .WithEqualsValue(EqualsValueClause(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(value))));
    }

    #endregion
}