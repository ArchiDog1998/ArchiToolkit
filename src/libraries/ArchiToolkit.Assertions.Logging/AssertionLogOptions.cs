using System.Diagnostics;

namespace ArchiToolkit.Assertions.Logging;

/// <summary>
///     The assertion log options
/// </summary>
/// <param name="ShowTag"></param>
/// <param name="ShowFrame"></param>
/// <param name="StackFrameFormat"></param>
public readonly record struct AssertionLogOptions(
    bool ShowTag,
    bool ShowFrame,
    Func<StackFrame, string>? StackFrameFormat = null);