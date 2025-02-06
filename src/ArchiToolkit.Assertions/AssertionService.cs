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
    public void HandleFailure(string context, AssertionType assertionType, AssertionItem assertion)
    {
        var message = $"{assertion.Message}\nwhen [{assertion.Time:yyyy-MM-dd HH:mm:ss.fff zzz}]{context}";
        throw new AssertionException(message);
    }

    public void HandleFailure(string context, IReadOnlyList<IAssertion> assertions)
    {
    }
}

[DebuggerNonUserCode]
file class DefaultScopeStrategy : IAssertionStrategy
{
    public void HandleFailure(string context, IReadOnlyList<IAssertion> assertions)
    {
        var stringBuilder = new StringBuilder();
        var messageCount = 0;
        foreach (var assertion in assertions)
        foreach (var assertionItem in assertion.Items)
        {
            stringBuilder.AppendLine($"{++messageCount:D2}. [{assertionItem.Time:yyyy-MM-dd HH:mm:ss.fff zzz}]{assertionItem.Message}");
            var frame = assertionItem.StackTrace.GetFrames()[0];
            stringBuilder.AppendLine(
                $"  at {frame.GetMethod()} in {frame.GetFileName()}:line {frame.GetFileLineNumber()}");
        }

        if (messageCount is 0) return;
        stringBuilder.Insert(0, $"[{DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss.fff zzz}][{messageCount} message(s)]{context}\n");
        throw new AssertionException(stringBuilder.ToString());
    }

    public void HandleFailure(string context, AssertionType assertionType, AssertionItem assertion)
    {
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