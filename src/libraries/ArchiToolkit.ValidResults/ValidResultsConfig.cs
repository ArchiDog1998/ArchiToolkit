using FluentResults;
using FluentValidation;
using FluentValidation.Results;

namespace ArchiToolkit.ValidResults;

public static class ValidResultsConfig
{
    private static readonly Dictionary<Type, Func<object, ValidationResult>> Validators = new();

    public static Func<Exception, IError>? ExceptionHandler { get; set; } = ex => new ExceptionalError(ex.Message, ex);

    internal static ValidationResult Validate<T>(T value)
    {
        if (value is null) throw new ArgumentNullException(nameof(value));
        var validator = GetValidator<T>();
        return validator?.Invoke(value) ?? new ValidationResult();
    }

    private static Func<object, ValidationResult>? GetValidator<T>()
    {
        var type = typeof(T);

        if (Validators.TryGetValue(type, out var del))
            return del;

        foreach (var kvp in Validators.Where(kvp => kvp.Key.IsAssignableFrom(type)))
        {
            return kvp.Value;
        }

        return null;
    }

    public static void AddValidator<T>(IValidator<T> validator)
    {
        AddValidator<T>(validator.Validate);
    }

    public static void AddValidator<T>(Func<T, ValidationResult> validator)
    {
        Validators[typeof(T)] = o => validator((T)o);
    }

    public static void RemoveValidator<T>()
    {
        Validators.Remove(typeof(T));
    }
}