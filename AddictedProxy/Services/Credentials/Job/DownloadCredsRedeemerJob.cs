using AddictedProxy.Services.Job.Extensions;
using Job.Scheduler.Job;
using Job.Scheduler.Job.Action;
using Job.Scheduler.Job.Exception;

namespace AddictedProxy.Services.Credentials.Job;

public class DownloadCredsRedeemerJob : IRecurringJob
{
    private readonly ILogger<DownloadCredsRedeemerJob> _logger;
    private readonly ICredentialsService _credentialsService;

    public DownloadCredsRedeemerJob(ILogger<DownloadCredsRedeemerJob> logger, ICredentialsService credentialsService)
    {
        _logger = logger;
        _credentialsService = credentialsService;
    }

    public Task ExecuteAsync(CancellationToken cancellationToken)
    {
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