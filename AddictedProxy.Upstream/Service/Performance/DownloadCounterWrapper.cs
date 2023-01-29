using Prometheus;

namespace AddictedProxy.Upstream.Service.Performance;

internal class DownloadCounterWrapper
{
    private readonly Counter _downloadCounter;

    internal enum SubtitleState
    {
        Downloaded,
        Deleted,
        DownloadLimitReached
    }

    public DownloadCounterWrapper()
    {
        _downloadCounter = Metrics.CreateCounter("upstream_subtitle_requests", "Nb of subtitles download request sent to Addic7ed", "result");
    }

    /// <summary>
    /// Increment the state of the counter
    /// </summary>
    /// <param name="state"></param>
    public void Inc(SubtitleState state)
    {
        _downloadCounter.Labels(state.ToString()).Inc();
    }
}