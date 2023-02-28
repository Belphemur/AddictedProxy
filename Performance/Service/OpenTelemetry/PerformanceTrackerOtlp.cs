using System.Diagnostics;
using Performance.Model;

namespace Performance.Service.OpenTelemetry;

public class PerformanceTrackerOtlp : IPerformanceTracker
{
    private readonly ActivitySource _activitySource;

    public PerformanceTrackerOtlp(ActivitySource activitySource)
    {
        _activitySource = activitySource;
    }

    public Model.ISpan BeginNestedSpan(string operation, string? description)
    {
        var activity = _activitySource.StartActivity(operation);
        if (activity == null)
        {
            return new EmptySpan();
        }

        if (description != null)
        {
            activity.SetTag("Description", description);
        }

        return new global::Performance.Model.OpenTelemetry.SpanOtlp(activity);
    }
}