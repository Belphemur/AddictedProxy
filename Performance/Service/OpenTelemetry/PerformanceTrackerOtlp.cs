using System.Diagnostics;
using Performance.Model;
using ISpan = Performance.Model.ISpan;

namespace Performance.Service.OpenTelemetry;

public class PerformanceTrackerOtlp : IPerformanceTracker
{
    private readonly ActivitySource _activitySource;
    private static readonly ISpan EmptySpan = new EmptySpan();

    public PerformanceTrackerOtlp(ActivitySource activitySource)
    {
        _activitySource = activitySource;
    }

    public ISpan BeginNestedSpan(string operation, string? description)
    {
        var activity = _activitySource.StartActivity(operation);
        if (activity == null)
        {
            return EmptySpan;
        }

        if (description != null)
        {
            activity.SetTag("Description", description);
        }

        return new global::Performance.Model.OpenTelemetry.SpanOtlp(activity);
    }
}