using System.Diagnostics;
using Sentry.Performance.Model;

namespace Sentry.Performance.Service.OpenTelemetry;

public class PerformanceTrackerOtlp : IPerformanceTracker
{
    private readonly ActivitySource _activitySource;

    public PerformanceTrackerOtlp(ActivitySource activitySource)
    {
        _activitySource = activitySource;
    }

    public Model.ISpan BeginNestedSpan(string operation, string description)
    {
        var activity = _activitySource.StartActivity(operation);
        if (activity == null)
        {
            return new EmptySpan();
        }
        activity.SetTag("Description", description);
        return new Model.OpenTelemetry.SpanOtlp(activity);
    }
}