using System.Diagnostics;
using System.Text;
using ArchiToolkit.Assertions.AssertionItems;
using ArchiToolkit.Assertions.Assertions;
using ArchiToolkit.Assertions.Exceptions;
using ArchiToolkit.Assertions.Execution;

namespace ArchiToolkit.Assertions;

[DebuggerNonUserCode]
file class DefaultPushStrategy : IAssertionStrategy
{
    public object HandleFailure(string context, AssertionType assertionType, AssertionItem assertion, object? tag)
    {
        var message = $"{assertion.Message}\nwhen [{assertion.Time:yyyy-MM-dd HH:mm:ss.fff zzz}]{context}";
        throw new AssertionException(message);
    }

    public object? HandleFailure(string context, IReadOnlyList<IAssertion> assertions)
    {
        return null;
    }
}

[DebuggerNonUserCode]
file class DefaultScopeStrategy : IAssertionStrategy
{
    public object? HandleFailure(string context, IReadOnlyList<IAssertion> assertions)
    {
        var stringBuilder = new StringBuilder();
        var messageCount = 0;
        foreach (var assertion in assertions)
        foreach (var assertionItem in assertion.Items)
        {
            stringBuilder.AppendLine(
                $"{++messageCount:D2}. [{assertionItem.Time:yyyy-MM-dd HH:mm:ss.fff zzz}]{assertionItem.Message}");
            var frame = assertionItem.StackTrace.GetFrames()[0];
            stringBuilder.AppendLine(
                $"  at {frame.GetMethod()} in {frame.GetFileName()}:line {frame.GetFileLineNumber()}");
        }

        if (messageCount is 0) return null;
        stringBuilder.Insert(0,
            $"[{DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss.fff zzz}][{messageCount} message(s)]{context}\n");
        throw new AssertionException(stringBuilder.ToString());
    }

    public object? HandleFailure(string context, AssertionType assertionType, AssertionItem assertion, object? tag)
    {
        return null;
    }
}

/// <summary>
///     The service of the assertions.
/// </summary>
public static class AssertionService
{
    /// <summary>
    ///     The default push strategy.
    /// </summary>
    public static IAssertionStrategy DefaultPushStrategy { get; set; } = new DefaultPushStrategy();

    /// <summary>
    ///     The default scope strategy
    /// </summary>
    public static IAssertionStrategy DefaultScopeStrategy { get; set; } = new DefaultScopeStrategy();
}