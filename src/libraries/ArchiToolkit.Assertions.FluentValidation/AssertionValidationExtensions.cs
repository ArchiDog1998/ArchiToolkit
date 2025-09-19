using ArchiToolkit.Assertions.AssertionItems;
using ArchiToolkit.Assertions.Assertions;
using ArchiToolkit.Assertions.Constraints;
using ArchiToolkit.Assertions.FluentValidation.Resources;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Results;

namespace ArchiToolkit.Assertions.FluentValidation;

/// <summary>
///     The extension for the Fluent Validation
/// </summary>
public static class AssertionValidationExtensions
{
    /// <summary>
    ///     If it is valid by Fluent Validation
    /// </summary>
    /// <param name="assertion"></param>
    /// <param name="validator">The validator</param>
    /// <param name="option">Callback to configure additional options</param>
    /// <param name="assertionParams"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static AndConstraint<T> BeValidBy<T>(this ObjectAssertion<T> assertion, IValidator<T> validator,
        Action<ValidationStrategy<T>>? option = null,
        AssertionParams? assertionParams = null)
    {
        var result = option is null
            ? validator.Validate(assertion.Subject)
            : validator.Validate(assertion.Subject, option);

        return assertion.AssertCheck(result.IsValid, AssertionItemType.Valid,
            new AssertMessage(ValidationLocalization.ValidationAssertion,
                new Argument("Messages", string.Join("\n", result.Errors.Select(ToErrorMessage)))),
            assertionParams);
    }

    private static string ToErrorMessage(this ValidationFailure failure)
    {
        return failure.ErrorMessage;
    }
}