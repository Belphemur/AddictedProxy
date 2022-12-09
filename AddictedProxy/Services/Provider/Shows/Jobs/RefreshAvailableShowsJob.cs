#region

using Hangfire;
using Sentry.Performance.Service;

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

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var transaction = _performanceTracker.BeginNestedSpan("refresh", "refresh-show-list");
        await _showRefresher.RefreshShowsAsync(cancellationToken);
        BackgroundJob.Enqueue<MapShowTmdbJob>(job => job.ExecuteAsync(default));
    }
}