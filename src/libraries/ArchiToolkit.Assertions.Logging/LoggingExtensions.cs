using System.Diagnostics;
using ArchiToolkit.Assertions.AssertionItems;
using ArchiToolkit.Assertions.Assertions;
using ArchiToolkit.Assertions.Execution;
using ArchiToolkit.Assertions.Utils;
using Microsoft.Extensions.Logging;

namespace ArchiToolkit.Assertions.Logging;

[DebuggerNonUserCode]
file class LoggerStrategy(AssertionLogOptions options) : IAssertionStrategy
{
    public object? HandleFailure(AssertionScope scope, IReadOnlyList<IAssertion> assertions)
    {
        return null;
    }

    public object? HandleFailure(AssertionScope scope, AssertionType assertionType, AssertionItem assertion,
        object? tag)
    {
        if (scope.Tag is not ILogger logger) return null;

        var format = assertion.Message.StructuredFormat;
        IEnumerable<object?> arguments = assertion.Message.Arguments;

        if (options.ShowContext && !string.IsNullOrEmpty(scope.Context))
        {
            format += "\nContext: {Context}";
            arguments = arguments.Append(scope.Context);
        }

        if (options.ShowTag && tag is not null)
        {
            format += "\nTag: {Tag}";
            arguments = arguments.Append(tag);
        }

        if (options.ShowFrame && assertion.StackFrame is { } frame)
        {
            format += "\nStackFrame: {StackFrame}";
            arguments = arguments.Append(options.StackFrameFormat?.Invoke(frame) ?? frame.GetString());
        }

        logger.Log(assertionType switch
        {
            AssertionType.Must => LogLevel.Error,
            AssertionType.Should => LogLevel.Warning,
            AssertionType.Could => LogLevel.Information,
            _ => LogLevel.Debug
        }, format, [..arguments]);
        return null;
    }
}

/// <summary>
///     The extensions for the logging
/// </summary>
public static class LoggingExtensions
{
    /// <summary>
    ///     Add the logging scope
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public static AssertionScope CreateScope(this ILogger logger, string context = "")
    {
        return new AssertionScope(context, logger);
    }

    /// <summary>
    ///     Create the scope with the strategy.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="strategy"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public static AssertionScope CreateScope(this ILogger logger, IAssertionStrategy strategy, string context = "")
    {
        return new AssertionScope(strategy, context, logger);
    }

    /// <summary>
    ///     Add the assertions to the <see cref="ILogger" />.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="changeOptions"></param>
    /// <returns></returns>
    public static ILoggingBuilder AddArchiToolkitAssertion(this ILoggingBuilder builder,
        Func<AssertionLogOptions, AssertionLogOptions>? changeOptions = null)
    {
        var defaultOptions = new AssertionLogOptions(true, true, true);
        var loggerStrategy = new LoggerStrategy(changeOptions?.Invoke(defaultOptions) ?? defaultOptions);
        AssertionService.AddService(new AssertionService(loggerStrategy, loggerStrategy));

        //Exceptions.
        AssertionService.AddService(new AssertionService(AssertionType.Must));
        return builder;
    }
}