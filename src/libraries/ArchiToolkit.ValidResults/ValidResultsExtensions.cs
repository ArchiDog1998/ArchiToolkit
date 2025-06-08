using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using FluentResults;

namespace ArchiToolkit.ValidResults;

public static class ValidResultsExtensions
{
    [Pure]
    [OverloadResolutionPriority(1)]
    public static ResultTracker<ValidResult<TValue>> Track<TValue>(this ValidResult<TValue> value,
        [CallerArgumentExpression(nameof(value))]
        string valueName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int fileLineNumber = 0)
    {
        return new ResultTracker<ValidResult<TValue>>(value, GetCallerInfo(valueName, filePath, fileLineNumber));
    }

    [Pure]
    public static ResultTracker<ValidResult<TValue>> Track<TValue>(this TValue value,
        [CallerArgumentExpression(nameof(value))]
        string valueName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int fileLineNumber = 0)
    {
        return new ResultTracker<ValidResult<TValue>>(ValidResult<TValue>.Data.Ok(value),
            GetCallerInfo(valueName, filePath, fileLineNumber));
    }

    [Pure]
    public static IEnumerable<IReason> GetReasons<TValue>(this ResultTracker<TValue> tracker) where TValue : IValidResult
    {
        return tracker.Value.Result.Reasons
            .Select(i => i is ObjectValidationError validation ? validation.WithInstanceName(tracker.CallerInfo) : i);
    }

    [Pure]
    public static IEnumerable<IReason> GetReasons<TValue>(TValue value,
        string valueName,
        string filePath,
        int fileLineNumber) where TValue : IValidResult
    {
        var callerInfo = GetCallerInfo(valueName, filePath, fileLineNumber);
        return value.Result.Reasons
            .Select(i => i is ObjectValidationError validation ? validation.WithInstanceName(callerInfo) : i);
    }

    [Pure]
    public static ResultTracker<TValue> AsTracker<TValue>(this TValue value,
        [CallerArgumentExpression(nameof(value))]
        string valueName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int fileLineNumber = 0)
    {
        return new ResultTracker<TValue>(value, GetCallerInfo(valueName, filePath, fileLineNumber));
    }

    [Pure]
    public static string GetCallerInfo(string valueName, string filePath, int fileLineNumber)
    {
        return $"The \"{valueName}\" at {Path.GetFileName(filePath)} ({fileLineNumber})";
    }
}