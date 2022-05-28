using AddictedProxy.Services.Job.Extensions;
using Job.Scheduler.Job;
using Job.Scheduler.Job.Action;
using Job.Scheduler.Job.Exception;
using Sentry.Performance.Service;

namespace AddictedProxy.Services.Credentials.Job;

public class DownloadCredsRedeemerJob : IRecurringJob
{
    private readonly ILogger<DownloadCredsRedeemerJob> _logger;
    private readonly ICredentialsService _credentialsService;
    private readonly IPerformanceTracker _performanceTracker;

    public DownloadCredsRedeemerJob(ILogger<DownloadCredsRedeemerJob> logger, ICredentialsService credentialsService, IPerformanceTracker performanceTracker)
    {
        _logger = logger;
        _credentialsService = credentialsService;
        _performanceTracker = performanceTracker;
    }

    public Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var span = _performanceTracker.BeginNestedSpan(nameof(DownloadCredsRedeemerJob), "redeeming-expired-dl-creds"); 
        return _credentialsService.RedeemDownloadCredentialsAsync(DateTime.UtcNow, cancellationToken);
    }

    public Task OnFailure(JobException exception)
    {
        _logger.LogJobException(exception, "Couldn't clean the download credentials");
        return Task.CompletedTask;
    }

    public IRetryAction FailRule { get; } = new AlwaysRetry();
    public TimeSpan? MaxRuntime { get; }
    public TimeSpan Delay { get; } = TimeSpan.FromHours(1);
}