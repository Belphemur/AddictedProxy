namespace Sentry.Performance.Model;

public class EmptySpan : ISpan
{
    public void Dispose()
    {
    }

    public Status? Status => Model.Status.Ok;
    public bool IsFinished => true;
    public string SpanId => "N/A";

    public void Finish()
    {
    }

    public void Finish(Status status)
    {
    }

    public void Finish(Exception exception, Status status)
    {
    }

    public void Finish(Exception exception)
    {
    }
}