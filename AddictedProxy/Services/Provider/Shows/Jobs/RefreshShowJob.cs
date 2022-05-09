using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Model.Dto;
using AddictedProxy.Services.Provider.Subtitle.Jobs;
using Job.Scheduler.Job;
using Job.Scheduler.Job.Action;
using Job.Scheduler.Job.Exception;
using Locking;

namespace AddictedProxy.Services.Provider.Shows.Jobs;

public class RefreshShowJob : IJob
{
    private readonly ILogger<RefreshShowJob> _logger;
    private readonly IShowRefresher _showRefresher;

    public IRetryAction FailRule { get; } = new ExponentialBackoffRetry(TimeSpan.FromSeconds(30), 3);
    public TimeSpan? MaxRuntime { get; } = TimeSpan.FromMinutes(10);

    public TvShow Show { get; set; }

    public RefreshShowJob(ILogger<RefreshShowJob> logger, IShowRefresher showRefresher)
    {
        _logger = logger;
        _showRefresher = showRefresher;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var namedLock = Lock<RefreshShowJob>.GetNamedLock(Show.Id.ToString());
        if (!await namedLock.WaitAsync(TimeSpan.Zero, cancellationToken))
        {
            _logger.LogInformation("Lock for {show} already taken", Show.Id);
            return;
        }

        _logger.LogInformation("Refreshing show: {Show}", Show.Name);
        await _showRefresher.RefreshShowAsync(Show, cancellationToken);
    }

    public Task OnFailure(JobException exception)
    {
        _logger.LogError(exception, "Couldn't refresh show {Show}", Show.Name);
        return Task.CompletedTask;
    }
}