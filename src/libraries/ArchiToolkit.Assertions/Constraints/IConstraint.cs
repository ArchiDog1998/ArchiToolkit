using ArchiToolkit.Assertions.Execution;

namespace ArchiToolkit.Assertions.Constraints;

/// <summary>
///     Just the Constraint
/// </summary>
public interface IConstraint;

/// <summary>
///     And Constraint
/// </summary>
public interface IAndConstraint : IConstraint
{
    /// <summary>
    ///     The failure return Value
    /// </summary>
    IDictionary<IAssertionStrategy, object>? FailureReturnValues { get; set; }
}