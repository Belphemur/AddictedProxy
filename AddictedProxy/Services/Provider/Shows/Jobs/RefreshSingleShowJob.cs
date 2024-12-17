
using AddictedProxy.Services.Job.Filter;
using AsyncKeyedLock;
using Hangfire;
using Locking;
using Performance.Model;
using Performance.Service;

namespace AddictedProxy.Services.Provider.Shows.Jobs;

public class RefreshSingleShowJob 
{
    private readonly ILogger<RefreshSingleShowJob> _logger;
    private readonly IShowRefresher _showRefresher;
    private readonly IPerformanceTracker _performanceTracker;
    private readonly static AsyncKeyedLocker<long> _asyncKeyedLocker = new(LockOptions.Default);

    public RefreshSingleShowJob(ILogger<RefreshSingleShowJob> logger, IShowRefresher showRefresher, IPerformanceTracker performanceTracker)
    {
        _logger = logger;
        _showRefresher = showRefresher;
        _performanceTracker = performanceTracker;
    }

    [UniqueJob(Order = 10, TtlFingerprintSeconds = 2 * TimeSpan.SecondsPerHour)]
    [AutomaticRetry(Attempts = 20)]
    [MaximumConcurrentExecutions(4)]
    [Queue("refresh-one-show")]
    public async Task ExecuteAsync(long showId, CancellationToken cancellationToken)
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(10));
        using var ctsLinked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cts.Token);
        var token = ctsLinked.Token;
        
        using var transaction = _performanceTracker.BeginNestedSpan("refresh", "refresh-specific-show");

        using var releaser = await _asyncKeyedLocker.LockOrNullAsync(showId, 0, token).ConfigureAwait(false);

        if (releaser is null)
        {
            _logger.LogInformation("Lock for {show} already taken", showId);
            transaction.Finish(Status.Unavailable);
            return;
        }

        _logger.LogInformation("Refreshing show: {Show}", showId);
        await _showRefresher.RefreshShowAsync(showId, token);
    }
}