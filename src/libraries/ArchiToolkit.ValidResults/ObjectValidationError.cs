using FluentResults;
using FluentValidation.Results;

namespace ArchiToolkit.ValidResults;

public class ObjectValidationError : Error
{
    private const string ThisName = "self";
    internal ObjectValidationError? Owner { get; }
    public string CallerInfo { get; }
    public ValidationResult ValidationResult { get; }

    internal ObjectValidationError(ValidationResult result, string callerInfo = ThisName,
        ObjectValidationError? owner = null) :
        base(string.IsNullOrEmpty(callerInfo) ? "" : $"The [{callerInfo}] is Invalid! {result}")
    {
        CallerInfo = callerInfo;
        ValidationResult = result;
        Metadata[nameof(CallerInfo)] = callerInfo;
        Metadata[nameof(ValidationResult)] = result;
        Owner = owner;
    }

    internal ObjectValidationError WithInstanceName(string callerInfo)
    {
        if (CallerInfo is not ThisName) return this;
        return new ObjectValidationError(ValidationResult, callerInfo, this);
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