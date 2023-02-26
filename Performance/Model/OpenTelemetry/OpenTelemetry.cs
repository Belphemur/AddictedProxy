using System.Diagnostics;
using OpenTelemetry.Trace;

namespace Sentry.Performance.Model.OpenTelemetry;

public class SpanOtlp : ISpan
{
    private readonly Activity _activity;

    public SpanOtlp(Activity activity)
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
        ActivityExtensions.RecordException(_activity, exception);
        Finish(status);
    }

    public void Finish(Exception exception)
    {
        ActivityExtensions.RecordException(_activity, exception);
        Finish(Model.Status.InternalError);
    }

    public void SetTag(string tag, object value)
    {
        _activity.SetTag(tag, value);
    }
}