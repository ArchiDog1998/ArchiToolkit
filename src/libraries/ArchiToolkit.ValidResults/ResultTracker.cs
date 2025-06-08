namespace ArchiToolkit.ValidResults;

public class ResultTracker<TValue>
{
    public TValue Value { get; }
    public string CallerInfo { get; }
    protected internal ResultTracker(
        TValue value,
        string callerInfo)
    {
        Value = value;
        CallerInfo = callerInfo;
    }
}