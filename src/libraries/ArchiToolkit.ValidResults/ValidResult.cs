using System.Diagnostics.Contracts;
using FluentResults;

namespace ArchiToolkit.ValidResults;

public class ValidResult(ValidResult.Data data) : IValidResult
{
    public record Data(Result Result)
    {
        [Pure]
        public static Data Ok(Data value) => value;

        [Pure]
        public static Data Ok(params IReadOnlyCollection<ISuccess> successes)
        {
            var result = Result.Ok().WithSuccesses(successes);
            return new Data(result);
        }

        [Pure]
        public static Data? Fail(IReadOnlyCollection<IReason> reasons)
        {
            return reasons.OfType<IError>().Any() ? new Data(new Result().WithReasons(reasons)) : null;
        }

        [Pure]
        public static implicit operator Data(Result result) => new(result);
    }

    public Result Result => data.Result;

    [Pure]
    public static implicit operator ValidResult(Data data) => new(data);

    public static bool operator true(ValidResult message) => message.Result.IsSuccess;
    public static bool operator false(ValidResult message) => message.Result.IsFailed;
}

public class ValidResult<TValue>(ValidResult<TValue>.Data data) : IValidResult<TValue>
{
    public record Data(Result Result, TValue ValueOrDefault)
    {
        [Pure]
        public static Data Ok(Data value) => value;

        [Pure]
        public static Data Ok(TValue value, params IReadOnlyCollection<ISuccess> successes)
        {
            if (value is null) throw new ArgumentNullException(nameof(value));
            var validationResult = ValidResultsConfig.ValidateObject(value);
            var result = Result.Ok().WithSuccesses(successes);

            return validationResult.IsValid
                ? new Data(result, value)
                : new Data(result.WithError(new ObjectValidationError(validationResult)), default!);
        }

        [Pure]
        public static Data? Fail(IReadOnlyCollection<IReason> reasons)
        {
            return reasons.OfType<IError>().Any() ? new Data(new Result().WithReasons(reasons), default!) : null;
        }

        [Pure]
        public static implicit operator Data(TValue value) => Ok(value);

        [Pure]
        public static implicit operator Data(Result result) => new(result, default!);
    }

    [Pure]
    public static implicit operator ValidResult<TValue>(Data data) => new(data);

    [Pure]
    public static implicit operator ValidResult<TValue>(TValue value) => Data.Ok(value);

    public Result Result => data.Result;
    public TValue ValueOrDefault => data.ValueOrDefault;

    public TValue Value
    {
        get
        {
            if (Result.IsFailed)
            {
                throw new InvalidOperationException("This is failed, you can't get the value.");
            }

            return ValueOrDefault;
        }
    }

    public override string ToString()
    {
        if (Result.IsFailed) return Result.ToString();
        return Result + " : " + ValueOrDefault;
    }

    public static bool operator true(ValidResult<TValue> message) => message.Result.IsSuccess;
    public static bool operator false(ValidResult<TValue> message) => message.Result.IsFailed;

    protected ValidResult<T>.Data GetProperty<T>(Func<T> getter)
    {
        return GetProperty(() => new ValidResult<T>.Data(Result, getter()));
    }

    protected ValidResult<T>.Data GetProperty<T>(Func<ValidResult<T>.Data> getter)
    {
        if (ValidResultsConfig.ExceptionHandler is not { } handle)
            return getter();
        try
        {
            return getter();
        }
        catch (Exception ex)
        {
            return new ValidResult<T>.Data(Result.Fail(handle(ex)), default!);
        }
    }

    protected void SetProperty<T>(ValidResult<T> value, Action<T> setter)
    {
        if (Result.IsFailed) return;
        if (value.Result.IsFailed) return;
        setter(value.Value);
    }

    object? IValidObjectResult.ValueOrDefault => ValueOrDefault;
}