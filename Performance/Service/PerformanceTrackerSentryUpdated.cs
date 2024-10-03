using Performance.Model.Sentry;
using Sentry;
using ISpan = Performance.Model.ISpan;

namespace Performance.Service;

public class PerformanceTrackerSentryUpdated : IPerformanceTracker
{
    public ISpan BeginNestedSpan(string operation, string? description = null)
    {
        var span = SentrySdk.GetSpan()?.StartChild(operation, description) ?? SentrySdk.StartTransaction(operation, operation, description);
        return new SpanSentry(span, null);
    }
}