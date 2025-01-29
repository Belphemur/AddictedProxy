using System.Diagnostics;
using OpenTelemetry.Trace;

namespace Performance.Model.OpenTelemetry;

public class SpanOtlp : ISpan
{
    private readonly Activity _activity;

    public SpanOtlp(Activity activity)
    {
        _activity = activity;
    }

    public void Dispose()
    {
        Finish(Performance.Model.Status.Ok);
        _activity.Dispose();
    }

    public Status? Status
    {
        get
        {
            return _activity.Status switch
            {
                ActivityStatusCode.Unset => Performance.Model.Status.UnknownError,
                ActivityStatusCode.Ok    => Performance.Model.Status.Ok,
                ActivityStatusCode.Error => Performance.Model.Status.UnknownError,
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
            Performance.Model.Status.Ok => ActivityStatusCode.Ok,
            _               => ActivityStatusCode.Error
        };
        if (status != Performance.Model.Status.Ok)
        {
            _activity.SetTag("fail.detail", status);
        }

        _activity.SetStatus(statusActivity);
        Finish();
    }

    public void Finish(Exception exception, Status status)
    {
        _activity.AddException(exception);
        Finish(status);
    }

    public void Finish(Exception exception)
    {
        _activity.AddException(exception);
        Finish(Performance.Model.Status.InternalError);
    }

    public void SetTag(string tag, object value)
    {
        _activity.SetTag(tag, value);
    }
}