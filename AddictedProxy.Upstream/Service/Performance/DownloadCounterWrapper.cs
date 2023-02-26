using Prometheus;

namespace AddictedProxy.Upstream.Service.Performance;

internal class DownloadCounterWrapper
{
    private readonly Counter _downloadCounter;

    internal enum SubtitleRequestResult
    {
        Downloaded,
        Deleted,
        DownloadLimitReached
    }

    public DownloadCounterWrapper()
    {
        _downloadCounter = Metrics.CreateCounter("upstream_subtitle_requests", "Nb of subtitles download request sent to Addic7ed", new[] { "result" }, new CounterConfiguration
        {
            ExemplarBehavior = ExemplarBehavior.NoExemplars()
        });
    }

    /// <summary>
    /// Increment the download request counter with the proper label
    /// </summary>
    /// <param name="requestResult"></param>
    public void Inc(SubtitleRequestResult requestResult)
    {
        _downloadCounter.Labels(requestResult.ToString()).Inc();
    }
}