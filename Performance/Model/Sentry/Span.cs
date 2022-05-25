namespace Sentry.Performance.Model.Sentry;

internal class Span : ISpan
{
    private readonly global::Sentry.ISpan _span;

    /// <summary>
    /// Status of the span
    /// </summary>
    public Status? Status
    {
        get => (Status?)_span.Status;
        set => _span.Status = (SpanStatus?)value;
    }

    public Span(global::Sentry.ISpan span)
    {
        _span = span;
    }

    /// <summary>
    /// Create a child span
    /// </summary>
    /// <param name="operation"></param>
    /// <param name="description"></param>
    /// <returns></returns>
    public ISpan StartChild(string operation, string description)
    {
        return new Span(_span.StartChild(operation, description));
    }
    /// <summary>
    /// Finishes the span.
    /// </summary>
    public void Finish()
    {
        _span.Finish();
    }

    /// <summary>
    /// Finishes the span with the specified status.
    /// </summary>
    public void Finish(Status status)
    {
        _span.Finish((SpanStatus)status);
    }

    /// <summary>
    /// Finishes the span with the specified exception and status.
    /// </summary>
    public void Finish(Exception exception, Status status)
    {
        _span.Finish(exception, (SpanStatus)status);
    }

    /// <summary>
    /// Finishes the span with the specified exception and automatically inferred status.
    /// </summary>
    public void Finish(Exception exception)
    {
        _span.Finish(exception);
    }

    public void Dispose()
    {
        if (_span.IsFinished)
        {
            return;
        }

        _span.Finish();
    }
}