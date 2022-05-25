namespace Sentry.Performance.Model.Sentry;

internal class SpanSentry : ISpan
{
    internal global::Sentry.ISpan InternalSpan { get; }
    internal SpanSentry? Parent { get; }
    public event EventHandler<TransactionFinishedEvent> OnSpanFinished = null!;

    /// <summary>
    /// Id of the span
    /// </summary>
    public string SpanId => InternalSpan.SpanId.ToString();

    /// <summary>
    /// Status of the span
    /// </summary>
    public Status? Status
    {
        get => (Status?)InternalSpan.Status;
        set => InternalSpan.Status = (SpanStatus?)value;
    }

    /// <summary>
    /// Is the span finished
    /// </summary>
    public bool IsFinished => InternalSpan.IsFinished;

    public SpanSentry(global::Sentry.ISpan internalSpan, SpanSentry? parent)
    {
        InternalSpan = internalSpan;
        Parent = parent;
    }

    /// <summary>
    /// Create a child span
    /// </summary>
    /// <param name="operation"></param>
    /// <param name="description"></param>
    /// <returns></returns>
    internal SpanSentry StartChild(string operation, string description)
    {
        return new SpanSentry(InternalSpan.StartChild(operation, description), this);
    }

    /// <summary>
    /// Finishes the span.
    /// </summary>
    public void Finish()
    {
        InternalSpan.Finish();
        OnFinished();
    }

    /// <summary>
    /// Finishes the span with the specified status.
    /// </summary>
    public void Finish(Status status)
    {
        InternalSpan.Finish((SpanStatus)status);
        OnFinished();
    }

    /// <summary>
    /// Finishes the span with the specified exception and status.
    /// </summary>
    public void Finish(Exception exception, Status status)
    {
        InternalSpan.Finish(exception, (SpanStatus)status);
        OnFinished();
    }

    /// <summary>
    /// Finishes the span with the specified exception and automatically inferred status.
    /// </summary>
    public void Finish(Exception exception)
    {
        InternalSpan.Finish(exception);
        OnFinished();
    }


    internal record TransactionFinishedEvent(SpanSentry SpanSentry);


    private void OnFinished()
    {
        OnSpanFinished(this, new TransactionFinishedEvent(this));
    }


    public void Dispose()
    {
        if (InternalSpan.IsFinished)
        {
            return;
        }

        Finish(Model.Status.Ok);
    }
}