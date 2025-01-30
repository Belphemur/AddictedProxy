using System.Diagnostics;
using OpenTelemetry;

namespace Performance.Service.OpenTelemetry.Processor;

public class TailRecordErrorProcessor : BaseProcessor<Activity>
{
    public override void OnEnd(Activity data)
    {
        if (data.Status == ActivityStatusCode.Error)
        {
            data.ActivityTraceFlags |= ActivityTraceFlags.Recorded;
        }

        base.OnEnd(data);
    }
}