using System.Text;
using ArchiToolkit.Assertions.AssertionItems;
using ArchiToolkit.Assertions.Assertions;
using ArchiToolkit.Assertions.Exceptions;
using ArchiToolkit.Assertions.Execution;

namespace ArchiToolkit.Assertions;

file class DefaultPushStrategy : IAssertionStrategy
{
    public void HandleFailure(string context, AssertionType assertionType, AssertionItem assertion)
    {
        var message = $"[{assertion.Time:yyyy-MM-dd HH:mm:ss.fff zzz}]{context}\n{assertion.Message}";
        throw new AssertionException(message);
    }

    public void HandleFailure(string context, IReadOnlyList<IAssertion> assertions)
    {
    }
}

file class DefaultScopeStrategy : IAssertionStrategy
{
    public void HandleFailure(string context, IReadOnlyList<IAssertion> assertions)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine($"[{DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss.fff zzz}]{context}");
        foreach (var assertion in assertions)
        {
            foreach (var assertionItem in assertion.Items)
            {
                stringBuilder.AppendLine($"-[{assertionItem.Time:yyyy-MM-dd HH:mm:ss.fff zzz}]{assertionItem.Message}");
                var frame = assertionItem.StackTrace.GetFrames()[0];
                stringBuilder.AppendLine($"-at {frame.GetMethod()?.ToString()} in {frame.GetFileName()}:line {frame.GetFileLineNumber()}");
            }
        }
        throw new AssertionException(stringBuilder.ToString());
    }

    public void HandleFailure(string context, AssertionType assertionType, AssertionItem assertion)
    {
    }
}

/// <summary>
/// The service of the assertions.
/// </summary>
public static class AssertionService
{
    /// <summary>
    /// The default push strategy.
    /// </summary>
    public static IAssertionStrategy DefaultPushStrategy { get; set; } = new DefaultPushStrategy();

    /// <summary>
    /// The default scope strategy
    /// </summary>
    public static IAssertionStrategy DefaultScopeStrategy { get; set; } = new DefaultScopeStrategy();
}