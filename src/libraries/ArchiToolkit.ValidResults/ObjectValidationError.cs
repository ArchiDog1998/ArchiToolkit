using FluentResults;
using FluentValidation.Results;

namespace ArchiToolkit.ValidResults;

public class ObjectValidationError : Error
{
    private const string ThisName = "this";
    public string CallerInfo { get; }
    public ValidationResult ValidationResult { get; }

    internal ObjectValidationError(ValidationResult result, string callerInfo = ThisName) :
        base(string.IsNullOrEmpty(callerInfo) ? "" : $"The [{callerInfo}] is Invalid! {result}")
    {
        CallerInfo = callerInfo;
        ValidationResult = result;
        Metadata[nameof(CallerInfo)] = callerInfo;
        Metadata[nameof(ValidationResult)] = result;
    }

    internal ObjectValidationError WithInstanceName(string instanceName)
    {
        if (CallerInfo is not ThisName) return this;
        return new ObjectValidationError(ValidationResult, instanceName);
    }

    public override string ToString()
    {
        if (!ValidResultsConfig.SimplifyObjectValidationErrorToString) return base.ToString();
        return new ReasonStringBuilder()
            .WithReasonType(GetType())
            .WithInfo(nameof(Message), Message)
            .Build();
    }
}