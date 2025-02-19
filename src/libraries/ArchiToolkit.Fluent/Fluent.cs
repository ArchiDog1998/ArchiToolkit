using System.Runtime.CompilerServices;

namespace ArchiToolkit.Fluent;

/// <summary>
///     The basic fluent item
/// </summary>
/// <typeparam name="TTarget"></typeparam>
public unsafe class Fluent<TTarget> : IDisposable
{
    private readonly Queue<Action> _actions = new();
    private readonly void* _ptr;
    private readonly FluentType _type;
    private bool _executed;

    /// <summary>
    /// Create one.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="type"></param>
    public Fluent(in TTarget target, FluentType type)
    {
        ref var t = ref Unsafe.AsRef(in target);
        _ptr = Unsafe.AsPointer(ref t);
        _type = type;
    }

    /// <summary>
    ///     The target to modify
    /// </summary>
    protected ref TTarget Target => ref Unsafe.AsRef<TTarget>(_ptr);

    /// <summary>
    ///     Get the result of it, actually it is not necessary, because it already modified the original one for you.
    /// </summary>
    public TTarget Result
    {
        get
        {
            Execute();
            return Target;
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Execute();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Can continue
    /// </summary>
    public bool CanContinue { get; set; } = true;


    internal void Execute()
    {
        if (_executed) return;
        _executed = true;
        while (CanContinue && _actions.Count > 0) _actions.Dequeue().Invoke();
    }

    /// <summary>
    ///     Add an action to it.
    /// </summary>
    /// <param name="action"></param>
    protected internal void AddAction(Action action)
    {
        if (_executed)
        {
            throw new NotSupportedException("You cannot add more actions after executing.");
        }
        switch (_type)
        {
            case FluentType.Immediate:
                if (CanContinue) action.Invoke();
                break;
            case FluentType.Lazy:
                _actions.Enqueue(action);
                break;
            default:
                throw new NotSupportedException($"Unsupported fluent type {_type}");
        }
    }
}