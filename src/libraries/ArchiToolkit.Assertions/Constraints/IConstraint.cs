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
    object?[] FailureReturnValue { get; set; }
}