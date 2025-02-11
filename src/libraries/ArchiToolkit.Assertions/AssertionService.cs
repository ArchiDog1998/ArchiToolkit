using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using ArchiToolkit.Assertions.AssertionItems;
using ArchiToolkit.Assertions.Assertions;
using ArchiToolkit.Assertions.Exceptions;
using ArchiToolkit.Assertions.Execution;
using ArchiToolkit.Assertions.Utils;

namespace ArchiToolkit.Assertions;

[DebuggerNonUserCode]
file class DefaultPushStrategy(
    AssertionType minRaisingExceptionType,
    [StringSyntax(StringSyntaxAttribute.DateTimeFormat)]
    string timeFormat) : IAssertionStrategy
{
    public object? HandleFailure(AssertionScope scope, AssertionType assertionType, AssertionItem assertion,
        object? tag)
    {
        if (assertionType < minRaisingExceptionType) return null;
        var message = $"{assertion.Message}\nwhen [{assertion.Time.ToString(timeFormat)}]{scope.Context}";
        throw new AssertionException(message);
    }

    public object? HandleFailure(AssertionScope scope, IReadOnlyList<IAssertion> assertions)
    {
        return null;
    }
}

[DebuggerNonUserCode]
file class DefaultScopeStrategy(
    AssertionType minRaisingExceptionType,
    [StringSyntax(StringSyntaxAttribute.DateTimeFormat)]
    string timeFormat,
    Func<StackFrame, string>? stackFrameFormat) : IAssertionStrategy
{
    public object? HandleFailure(AssertionScope scope, IReadOnlyList<IAssertion> assertions)
    {
        var stringBuilder = new StringBuilder();
        var messageCount = 0;
        foreach (var assertion in assertions)
        {
            if (assertion.Type < minRaisingExceptionType) continue;
            foreach (var assertionItem in assertion.Items)
            {
                stringBuilder.AppendLine(
                    $"{++messageCount:D2}. [{assertionItem.Time.ToString(timeFormat)}]{assertionItem.Message}");
                var frame = assertionItem.StackFrame;
                if (frame is null) continue;
                stringBuilder.AppendLine(stackFrameFormat?.Invoke(frame) ?? $"  at {frame.GetString()}");
            }
        }

        if (messageCount is 0) return null;
        stringBuilder.Insert(0,
            $"[{DateTimeOffset.Now.ToString(timeFormat)}][{messageCount} message(s)]{scope.Context}\n");
        throw new AssertionException(stringBuilder.ToString());
    }

    public object? HandleFailure(AssertionScope scope, AssertionType assertionType, AssertionItem assertion,
        object? tag)
    {
        return null;
    }
}

/// <summary>
///     The service of the assertions.
/// </summary>
public readonly struct AssertionService(IAssertionStrategy pushStrategy, IAssertionStrategy scopeStrategy)
{
    private static readonly ConcurrentBag<AssertionService> CurrentServices = [];
    private static readonly AsyncLocal<MergedAssertionStrategy> MergedPushStrategyAsyncLocal = new();
    private static readonly AsyncLocal<MergedAssertionStrategy> MergedScopeStrategyAsyncLocal = new();

    internal static MergedAssertionStrategy MergedPushStrategy =>
        MergedPushStrategyAsyncLocal.Value ??= CalculateMergedPushStrategy();

    internal static MergedAssertionStrategy MergedScopeStrategy =>
        MergedScopeStrategyAsyncLocal.Value ??= CalculateMergedScopeStrategy();


    private static void CheckCurrentServices()
    {
        if (CurrentServices.Count > 0) return;
        CurrentServices.Add(new AssertionService((AssertionType)byte.MaxValue));
    }

    private static MergedAssertionStrategy CalculateMergedPushStrategy()
    {
        CheckCurrentServices();
        return new MergedAssertionStrategy([..CurrentServices.Select(s => s.PushStrategy)]);
    }

    private static MergedAssertionStrategy CalculateMergedScopeStrategy()
    {
        CheckCurrentServices();
        return new MergedAssertionStrategy([..CurrentServices.Select(s => s.ScopeStrategy)]);
    }

    /// <summary>
    ///     Create the service by the min raising exception type
    /// </summary>
    /// <param name="minRaisingExceptionType"></param>
    /// <param name="timeFormat"></param>
    /// <param name="stackFrameFormat"></param>
    public AssertionService(AssertionType minRaisingExceptionType, string timeFormat = "yyyy-MM-dd HH:mm:ss.fff zzz",
        Func<StackFrame, string>? stackFrameFormat = null)
        : this(new DefaultPushStrategy(minRaisingExceptionType, timeFormat),
            new DefaultScopeStrategy(minRaisingExceptionType, timeFormat, stackFrameFormat))
    {
    }

    /// <summary>
    ///     Set the service
    /// </summary>
    /// <param name="service"></param>
    public static void AddService(AssertionService service)
    {
        CurrentServices.Add(service);
        MergedPushStrategyAsyncLocal.Value = CalculateMergedPushStrategy();
        MergedScopeStrategyAsyncLocal.Value = CalculateMergedScopeStrategy();
    }

    /// <summary>
    ///     Clear the services
    /// </summary>
    public static void ClearServices()
    {
        while (CurrentServices.TryTake(out _))
        {
        }

        MergedPushStrategyAsyncLocal.Value = CalculateMergedPushStrategy();
        MergedScopeStrategyAsyncLocal.Value = CalculateMergedScopeStrategy();
    }

    /// <summary>
    ///     The default push strategy.
    /// </summary>
    public IAssertionStrategy PushStrategy { get; } = pushStrategy;

    /// <summary>
    ///     The default scope strategy
    /// </summary>
    public IAssertionStrategy ScopeStrategy { get; } = scopeStrategy;
}