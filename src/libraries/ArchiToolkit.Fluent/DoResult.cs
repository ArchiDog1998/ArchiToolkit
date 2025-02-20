namespace ArchiToolkit.Fluent;

/// <summary>
/// The Do things result.
/// </summary>
/// <param name="fluent"></param>
/// <typeparam name="TResult"></typeparam>
/// <typeparam name="TValue"></typeparam>
public sealed class DoResult<TValue, TResult>(Fluent<TValue> fluent, Lazy<(bool, TResult)> lazy)
{
    /// <summary>
    /// Did it run the method.
    /// </summary>
    public bool DidIt  => lazy.Value.Item1;

    /// <summary>
    ///     The return value
    /// </summary>
    public TResult ReturnValue => lazy.Value.Item2;

    /// <inheritdoc cref="Fluent{TTarget}.Result" />
    public TValue Result => Continue.Result;

    /// <summary>
    ///     Continue to do sth.
    /// </summary>
    public Fluent<TValue> Continue => fluent;

    /// <summary>
    ///     Continue when can continue.
    /// </summary>
    /// <param name="canContinue"></param>
    /// <returns></returns>
    public Fluent<TValue> ContinueWhen(Predicate<TResult> canContinue)
    {
        return fluent.AddCondition(() => canContinue(lazy.Value.Item2));
    }

    /// <summary>
    ///     Convert to the result.
    /// </summary>
    /// <param name="doResult"></param>
    /// <returns></returns>
    public static implicit operator TResult(DoResult<TValue, TResult> doResult)
    {
        return doResult.ReturnValue;
    }
}