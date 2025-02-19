namespace ArchiToolkit.Fluent;

/// <summary>
/// </summary>
/// <param name="fluent"></param>
/// <typeparam name="TResult"></typeparam>
/// <typeparam name="TValue"></typeparam>
public class DoResult<TValue, TResult>(Fluent<TValue> fluent, Lazy<TResult> lazy)
{
    /// <summary>
    ///     The return value
    /// </summary>
    public TResult ReturnValue
    {
        get
        {
            fluent.Executed = true;
            return lazy.Value;
        }
    }

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
        return fluent.AddCondition(() => canContinue(lazy.Value));
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