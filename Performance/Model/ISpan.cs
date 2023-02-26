namespace Sentry.Performance.Model;

public interface ISpan : IDisposable
{
    /// <summary>
    /// Status of the span
    /// </summary>
    Status? Status { get; }

    /// <summary>
    /// Is the span finished
    /// </summary>
    bool IsFinished { get; }

    /// <summary>
    /// Id of the span
    /// </summary>
    string SpanId { get; }

    /// <summary>
    /// Finishes the span.
    /// </summary>
    void Finish();

    /// <summary>
    /// Finishes the span with the specified status.
    /// </summary>
    void Finish(Status status);

    /// <summary>
    /// Finishes the span with the specified exception and status.
    /// </summary>
    void Finish(Exception exception, Status status);

    /// <summary>
    /// Finishes the span with the specified exception and automatically inferred status.
    /// </summary>
    void Finish(Exception exception);
}