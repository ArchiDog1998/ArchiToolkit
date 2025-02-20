namespace ArchiToolkit.Fluent;

/// <summary>
///     The basic fluent item
/// </summary>
/// <typeparam name="TTarget"></typeparam>
public class Fluent<TTarget> : IDisposable
{
    private readonly Queue<Action> _actions = new();
    private readonly FluentType _type;
    private bool _canContinue = true;
    private TTarget _target;

    internal Fluent(in TTarget target, FluentType type)
    {
        _target = target;
        _type = type;
    }

    /// <summary>
    ///     Get the result of it, actually it is not necessary, because it already modified the original one for you.
    /// </summary>
    public TTarget Result => Execute();

    /// <inheritdoc />
    public void Dispose()
    {
        Execute();
        GC.SuppressFinalize(this);
    }

    private TTarget Execute()
    {
        _canContinue = true;
        while (_canContinue && _actions.Count > 0) _actions.Dequeue().Invoke();
        return _target;
    }

    internal Fluent<TTarget> AddCondition(Func<bool> condition)
    {
        return AddAction(() => _canContinue = condition());
    }

    /// <summary>
    ///     Add the property to this fluent
    /// </summary>
    /// <param name="property"></param>
    /// <returns></returns>
    public Fluent<TTarget> AddProperty(PropertyDelegate<TTarget> property)
    {
        return AddAction(() => property(ref _target));
    }

    /// <summary>
    ///     Invoke the method you want.
    /// </summary>
    /// <param name="method"></param>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    public DoResult<TTarget, TResult> InvokeMethod<TResult>(MethodDelegate<TTarget, TResult> method)
    {
        var lazy = InvokeMethod(b =>
        {
            if (b) return (b, method(ref _target));
            return (b, default!);
        });
        return new DoResult<TTarget, TResult>(AddAction(() => { _ = lazy.Value; }), lazy);
    }

    /// <summary>
    /// Invokes.
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    public DoResult<TTarget> InvokeMethod(MethodDelegate<TTarget> method)
    {
        var lazy = InvokeMethod(b =>
        {
            if (b) method(ref _target);
            return b;
        });
        return new DoResult<TTarget>(AddAction(() => { _ = lazy.Value; }), lazy);
    }

    private Lazy<T> InvokeMethod<T>(Func<bool, T> method)
    {
        var actions = _actions.ToArray();
        _actions.Clear();
        return new Lazy<T>(() =>
        {
            _canContinue = true;
            foreach (var action in actions)
            {
                if (_canContinue) action();
                else return method(false);
            }

            return method(true);
        });
    }

    private Fluent<TTarget> AddAction(Action action)
    {
        switch (_type)
        {
            case FluentType.Immediate:
                if (_canContinue) action.Invoke();
                break;
            case FluentType.Lazy:
                _actions.Enqueue(action);
                break;
            default:
                throw new NotSupportedException($"Unsupported fluent type {_type}");
        }

        return this;
    }
}