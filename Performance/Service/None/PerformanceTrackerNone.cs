using Performance.Model;
using ISpan = Performance.Model.ISpan;

namespace Performance.Service.None;

public class PerformanceTrackerNone : IPerformanceTracker
{
    private static readonly ISpan None = new SpanNone();
    private class SpanNone : ISpan
    {
        public void Dispose()
        {
        }

        public Status? Status => Model.Status.Ok;
        public bool IsFinished => true;
        public string SpanId { get; } = Guid.NewGuid().ToString();

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

        public void SetTag(string tag, object value)
        {
        }
    }

    public ISpan BeginNestedSpan(string operation, string? description = null)
    {
        return None;
    }
}