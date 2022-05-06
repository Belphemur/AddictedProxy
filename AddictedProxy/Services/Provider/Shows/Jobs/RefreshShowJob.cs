using AddictedProxy.Database.Model.Shows;
using AddictedProxy.Model.Dto;
using Job.Scheduler.Job;
using Job.Scheduler.Job.Action;
using Job.Scheduler.Job.Exception;

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
        _logger.LogInformation("Refreshing show: {Show}", Show.Name);
        await _showRefresher.RefreshShowAsync(Show, cancellationToken);
    }

    public Task OnFailure(JobException exception)
    {
        _logger.LogError(exception, "Couldn't refresh show {Show}", Show.Name);
        return Task.CompletedTask;
    }
}