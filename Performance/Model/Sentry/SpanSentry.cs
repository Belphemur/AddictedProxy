using Sentry;

namespace Performance.Model.Sentry;

internal class SpanSentry : ISpan
{
    private global::Sentry.ISpan InternalSpan { get; }
    public SpanSentry? Parent { get; private set; }

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
    internal SpanSentry StartChild(string operation, string? description)
    {
        return new SpanSentry(InternalSpan.StartChild(operation, description), this);
    }

    /// <summary>
    /// Finishes the span.
    /// </summary>
    public void Finish()
    {
        InternalSpan.Finish();
    }

    /// <summary>
    /// Finishes the span with the specified status.
    /// </summary>
    public void Finish(Status status)
    {
        InternalSpan.Finish((SpanStatus)status);
    }

    /// <summary>
    /// Finishes the span with the specified exception and status.
    /// </summary>
    public void Finish(Exception exception, Status status)
    {
        InternalSpan.Finish(exception, (SpanStatus)status);
    }

    /// <summary>
    /// Finishes the span with the specified exception and automatically inferred status.
    /// </summary>
    public void Finish(Exception exception)
    {
        InternalSpan.Finish(exception);
    }

    public void SetTag(string tag, object value)
    {
        InternalSpan.SetTag(tag, value.ToString() ?? string.Empty);
    }


    public void Dispose()
    {
        if (InternalSpan.IsFinished)
        {
            return;
        }

        Finish(Model.Status.Ok);
        Parent = null;
    }
}