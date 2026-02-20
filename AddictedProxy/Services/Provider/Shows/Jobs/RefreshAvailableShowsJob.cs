#region

using Hangfire;
using Hangfire.Console;
using Hangfire.Server;
using Performance.Service;

#endregion

namespace AddictedProxy.Services.Provider.Shows.Jobs;

public class RefreshAvailableShowsJob
{
    private readonly IShowRefresher _showRefresher;
    private readonly IPerformanceTracker _performanceTracker;
    private readonly ILogger<RefreshAvailableShowsJob> _logger;

    public RefreshAvailableShowsJob(IShowRefresher showRefresher,
                                    IPerformanceTracker performanceTracker,
                                    ILogger<RefreshAvailableShowsJob> logger)
    {
        _showRefresher = showRefresher;
        _performanceTracker = performanceTracker;
        _logger = logger;
    }

    public async Task ExecuteAsync(PerformContext context, CancellationToken cancellationToken)
    {
        context.WriteLine("Starting to refresh available shows list...");
        using var transaction = _performanceTracker.BeginNestedSpan("refresh", "refresh-show-list");
        await _showRefresher.RefreshShowsAsync(cancellationToken);
        context.WriteLine("Finished refreshing shows, enqueueing downstream jobs...");
        var jobId = BackgroundJob.Enqueue<MapShowTmdbJob>(job => job.ExecuteAsync(null, default));
        context.WriteLine($"Enqueued MapShowTmdbJob (ID: {jobId})");
        jobId = BackgroundJob.ContinueJobWith<CleanDuplicateTmdbJob>(jobId, job => job.ExecuteAsync(null, default));
        context.WriteLine($"Enqueued CleanDuplicateTmdbJob (ID: {jobId})");
        jobId = BackgroundJob.ContinueJobWith<FetchMissingTvdbIdJob>(jobId, job => job.ExecuteAsync(null, default));
        context.WriteLine($"Enqueued FetchMissingTvdbIdJob (ID: {jobId})");
    }
}