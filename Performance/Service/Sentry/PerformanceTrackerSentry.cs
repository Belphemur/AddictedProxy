using Sentry.Performance.Model.Sentry;

namespace Sentry.Performance.Service.Sentry;

public class PerformanceTrackerSentry : IPerformanceTracker
{
    private readonly IHub _sentryHub;

    public PerformanceTrackerSentry(IHub sentryHub)
    {
        _sentryHub = sentryHub;
    }

    /// <summary>
    /// Start a transaction.
    /// <remarks>If there is already a ongoing transaction in the scope, create a child from it.</remarks>
    /// </summary>
    public Model.ISpan BeginNestedSpan(string operation, string description)
    {
        return new SpanSentry(_sentryHub.GetSpan()?.StartChild(operation, description) ?? _sentryHub.StartTransaction(operation, description));
    }
}