using FluentValidation.Results;

namespace ArchiToolkit.ValidResults;

internal class ValidResultsValidator(Type type)
{
    public Type TargetType => type;

    private readonly Dictionary<Guid, (ValidResultsConfig.ShouldUseDelegate? shouldUse, Func<object, ValidationResult>
            validator)>
        _validators = [];

    public ValidationResult ValidateObject(object value)
    {
        return new ValidationResult(_validators.Values
            .Where(i => i.shouldUse is null)
            .Select(i => i.validator(value)));
    }

    public ValidationResult ValidateArgument(object value, string methodName, string methodArgumentName)
    {
        return new ValidationResult(_validators.Values
            .Where(i => i.shouldUse?.Invoke(methodName, methodArgumentName) ?? false)
            .Select(i => i.validator(value)));
    }

    public Guid AddValidator(Func<object, ValidationResult> validator, ValidResultsConfig.ShouldUseDelegate? shouldUse)
    {
        var key = Guid.NewGuid();
        _validators[key] = (shouldUse, validator);
        return key;
    }

    public bool RemoveValidator(Guid key)
    {
        return _validators.Remove(key);
    }
}