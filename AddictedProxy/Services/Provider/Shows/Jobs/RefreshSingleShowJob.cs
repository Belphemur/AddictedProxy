
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

    [MaximumConcurrentExecutions(3)]
    [Queue("refresh-one-show")]
    public async Task ExecuteAsync(long showId, CancellationToken cancellationToken)
    {
        using var transaction = _performanceTracker.BeginNestedSpan("refresh", "refresh-specific-show");

        using var namedLock = Lock<RefreshSingleShowJob>.GetNamedLock(showId.ToString());
        if (!await namedLock.WaitAsync(TimeSpan.Zero, cancellationToken))
        {
            _logger.LogInformation("Lock for {show} already taken", showId);
            transaction.Finish(Status.Unavailable);
            return;
        }

        _logger.LogInformation("Refreshing show: {Show}", showId);
        await _showRefresher.RefreshShowAsync(showId, cancellationToken);
    }
}