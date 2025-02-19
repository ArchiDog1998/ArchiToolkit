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
    internal bool Executed;

    internal Fluent(in TTarget target, FluentType type)
    {
        _target = target;
        _type = type;
    }

    /// <summary>
    ///     Get the result of it, actually it is not necessary, because it already modified the original one for you.
    /// </summary>
    public TTarget Result
    {
        get
        {
            Execute();
            return _target;
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Execute();
        GC.SuppressFinalize(this);
    }

    private void Execute()
    {
        if (Executed) return;
        Executed = true;
        while (_canContinue && _actions.Count > 0) _actions.Dequeue().Invoke();
    }

    internal Fluent<TTarget> AddCondition(Func<bool> condition)
    {
        return AddAction(() => _canContinue = condition());
    }

    public Fluent<TTarget> AddProperty(PropertyDelegate<TTarget> property)
    {
        return AddAction(() => property(ref _target));
    }

    public DoResult<TTarget, TResult> InvokeMethod<TResult>(MethodDelegate<TTarget, TResult> method)
    {
        var actions = _actions.ToArray();
        _actions.Clear();
        var lazy = new Lazy<TResult>(() =>
        {
            foreach (var action in actions) action();
            return method(ref _target);
        });
        return new DoResult<TTarget, TResult>(AddAction(() => { _ = lazy.Value; }), lazy);
    }

    private Fluent<TTarget> AddAction(Action action)
    {
        if (Executed) throw new NotSupportedException("You cannot add more actions after executing.");

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