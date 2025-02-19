namespace ArchiToolkit.Fluent;

/// <summary>
///
/// </summary>
/// <param name="fluent"></param>
/// <typeparam name="TFluent"></typeparam>
/// <typeparam name="TResult"></typeparam>
/// <typeparam name="TValue"></typeparam>
public class DoResult<TFluent, TValue, TResult>(TFluent fluent, Func<TResult> getResult)
    where TFluent : Fluent<TValue>
{
    private readonly Lazy<TResult> _result = new(getResult);

    /// <summary>
    /// The return value
    /// </summary>
    public TResult ReturnValue
    {
        get
        {
            fluent.Execute();
            return _result.Value;
        }
    }

    /// <inheritdoc cref="Fluent{TTarget}.Result"/>
    public TValue Result => Continue.Result;

    /// <summary>
    /// Continue to do sth.
    /// </summary>
    public TFluent Continue
    {
        get
        {
            fluent.AddAction(() => getResult());
            return fluent;
        }
    }

    /// <summary>
    /// Continue when can continue.
    /// </summary>
    /// <param name="canContinue"></param>
    /// <returns></returns>
    public TFluent ContinueWhen(Predicate<TResult> canContinue)
    {
        fluent.AddAction(() => fluent.CanContinue = canContinue(_result.Value));
        return fluent;
    }

    /// <summary>
    /// Convert to the result.
    /// </summary>
    /// <param name="doResult"></param>
    /// <returns></returns>
    public static implicit operator TResult(DoResult<TFluent, TValue, TResult> doResult) => doResult.ReturnValue;
}