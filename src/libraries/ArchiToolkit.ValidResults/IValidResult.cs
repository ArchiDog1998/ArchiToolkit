using FluentResults;

namespace ArchiToolkit.ValidResults;

public interface IValidResult<out TValue> : IValidResult
{
    TValue Value { get; }
}

public interface IValidResult
{
    Result Result { get; }
}