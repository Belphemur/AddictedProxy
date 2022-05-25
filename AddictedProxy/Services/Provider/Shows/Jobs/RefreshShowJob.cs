using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Model.Dto;
using AddictedProxy.Services.Job.Extensions;
using AddictedProxy.Services.Provider.Subtitle.Jobs;
using Job.Scheduler.Job;
using Job.Scheduler.Job.Action;
using Job.Scheduler.Job.Exception;
using Locking;
using Sentry.Performance.Service;

namespace AddictedProxy.Services.Provider.Shows.Jobs;

public class RefreshShowJob : IJob
{
    private readonly ILogger<RefreshShowJob> _logger;
    private readonly IShowRefresher _showRefresher;
    private readonly IPerformanceTracker _performanceTracker;

    public IRetryAction FailRule { get; } = new ExponentialBackoffRetry(TimeSpan.FromSeconds(30), 3);
    public TimeSpan? MaxRuntime { get; } = TimeSpan.FromMinutes(10);

    public TvShow Show { get; set; }

    /// <summary>
    /// Id of the user that made the request
    /// </summary>
    public string? ConnectionId { get; set; }

    public RefreshShowJob(ILogger<RefreshShowJob> logger, IShowRefresher showRefresher, IPerformanceTracker performanceTracker)
    {
        _logger = logger;
        _showRefresher = showRefresher;
        _performanceTracker = performanceTracker;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var namedLock = Lock<RefreshShowJob>.GetNamedLock(Show.Id.ToString());
        if (!await namedLock.WaitAsync(TimeSpan.Zero, cancellationToken))
        {
            _logger.LogInformation("Lock for {show} already taken", Show.Id);
            return;
        }

        using var _ = _performanceTracker.BeginNestedSpan(nameof(RefreshShowJob), $"refresh-show {Show.Name}");

        _logger.LogInformation("Refreshing show: {Show}", Show.Name);
        await _showRefresher.RefreshShowAsync(Show, cancellationToken);
    }

    public Task OnFailure(JobException exception)
    {
        _logger.LogJobException(exception, "Couldn't refresh show {Show}", Show.Name);
        return Task.CompletedTask;
    }
}