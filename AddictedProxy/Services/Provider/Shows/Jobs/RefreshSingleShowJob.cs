
using AddictedProxy.Services.Job.Filter;
using Hangfire;
using Locking;
using Sentry.Performance.Model;
using Sentry.Performance.Service;

namespace AddictedProxy.Services.Provider.Shows.Jobs;

public class RefreshSingleShowJob 
{
    private readonly ILogger<RefreshSingleShowJob> _logger;
    private readonly IShowRefresher _showRefresher;
    private readonly IPerformanceTracker _performanceTracker;

    public RefreshSingleShowJob(ILogger<RefreshSingleShowJob> logger, IShowRefresher showRefresher, IPerformanceTracker performanceTracker)
    {
        _logger = logger;
        _showRefresher = showRefresher;
        _performanceTracker = performanceTracker;
    }

    [DisableMultipleQueuedItemsFilter(Order = 10)]
    [AutomaticRetry(Attempts = 20)]
    [MaximumConcurrentExecutions(4)]
    [Queue("refresh-one-show")]
    public async Task ExecuteAsync(long showId, CancellationToken cancellationToken)
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(8));
        using var ctsLinked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cts.Token);
        var token = ctsLinked.Token;
        
        using var transaction = _performanceTracker.BeginNestedSpan("refresh", "refresh-specific-show");

        using var lockReleaser = Lock<RefreshSingleShowJob>.GetLockReleaser(showId.ToString());
        if (lockReleaser.SemaphoreSlim.CurrentCount == 0)
        {
            _logger.LogInformation("Lock for {show} already taken", showId);
            transaction.Finish(Status.Unavailable);
            return;
        }
        await lockReleaser.SemaphoreSlim.WaitAsync(token);

        _logger.LogInformation("Refreshing show: {Show}", showId);
        await _showRefresher.RefreshShowAsync(showId, token);
    }
}