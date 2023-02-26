using System.Diagnostics;
using OpenTelemetry.Trace;
using Status = Sentry.Performance.Model.Status;

namespace Sentry.Performance.Service.OpenTelemetry;

public class PerformanceTrackerOtlp : IPerformanceTracker
{
    private readonly ActivitySource _activitySource;

    public PerformanceTrackerOtlp(ActivitySource activitySource)
    {
        _activitySource = activitySource;
    }

    public class Span : Model.ISpan
    {
        private readonly Activity _activity;

        public Span(Activity activity)
        {
            _activity = activity;
        }

        public void Dispose()
        {
            Finish(Model.Status.Ok);
            _activity.Dispose();
        }

        public Status? Status
        {
            get
            {
                return _activity.Status switch
                {
                    ActivityStatusCode.Unset => Model.Status.UnknownError,
                    ActivityStatusCode.Ok    => Model.Status.Ok,
                    ActivityStatusCode.Error => Model.Status.UnknownError,
                    _                        => throw new ArgumentOutOfRangeException()
                };
            }
        }

        public bool IsFinished => _activity.IsStopped;
        public string SpanId => _activity.Id ?? "N/A";

        public void Finish()
        {
            if (_activity.IsStopped)
            {
                return;
            }
            _activity.Stop();
        }

        public void Finish(Status status)
        {
            if (_activity.IsStopped)
            {
                return;
            }
            var statusActivity = status switch
            {
                Model.Status.Ok => ActivityStatusCode.Ok,
                _               => ActivityStatusCode.Error
            };
            _activity.SetTag("StatusDetail", status);
            _activity.SetStatus(statusActivity);
            Finish();
        }

        public void Finish(Exception exception, Status status)
        {
            _activity.RecordException(exception);
            Finish(status);
        }

        public void Finish(Exception exception)
        {
            _activity.RecordException(exception);
            Finish(Model.Status.InternalError);
        }
    }

    public Model.ISpan BeginNestedSpan(string operation, string description)
    {
        var activity = _activitySource.StartActivity(operation)!;
        activity.SetTag("Description", description);
        return new Span(activity);
    }
}