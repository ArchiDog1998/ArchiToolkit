﻿using System.Runtime.CompilerServices;
using ArchiToolkit.Assertions.Assertions;
using ArchiToolkit.Assertions.Constraints;

namespace ArchiToolkit.Assertions;

/// <summary>
///     Contains some assertions
/// </summary>
public static class AssertionExtensions
{
    private static void ThrowIfInvalid<T>(T value)
    {
        switch (value)
        {
            case IAssertion:
                throw new InvalidOperationException("You can't create an assertion by an assertion!");
            case IConstraint:
                throw new InvalidOperationException(
                    "You can't create an assertion by an constraint! Try to use its property!");
        }
    }

    /// <summary>
    ///     The must assertion
    /// </summary>
    /// <param name="value"></param>
    /// <param name="valueName"></param>
    /// <param name="memberName"></param>
    /// <param name="filePath"></param>
    /// <param name="lineNumber"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static ObjectAssertion<T> Must<T>(this T value,
        [CallerArgumentExpression(nameof(value))]
        string valueName = "",
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        ThrowIfInvalid(value);
        return new ObjectAssertion<T>(value, valueName, AssertionType.Must,
            new CallerInfo(memberName, filePath, lineNumber));
    }

    /// <summary>
    ///     The things should be.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="valueName"></param>
    /// <param name="memberName"></param>
    /// <param name="filePath"></param>
    /// <param name="lineNumber"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static ObjectAssertion<T> Should<T>(this T value,
        [CallerArgumentExpression(nameof(value))]
        string valueName = "",
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        ThrowIfInvalid(value);
        return new ObjectAssertion<T>(value, valueName, AssertionType.Should,
            new CallerInfo(memberName, filePath, lineNumber));
    }

    /// <summary>
    ///     The things could be.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="valueName"></param>
    /// <param name="memberName"></param>
    /// <param name="filePath"></param>
    /// <param name="lineNumber"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static ObjectAssertion<T> Could<T>(this T value,
        [CallerArgumentExpression(nameof(value))]
        string valueName = "",
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        ThrowIfInvalid(value);
        return new ObjectAssertion<T>(value, valueName, AssertionType.Could,
            new CallerInfo(memberName, filePath, lineNumber));
    }
}