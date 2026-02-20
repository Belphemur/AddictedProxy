using AddictedProxy.Services.Job.Filter;
using Hangfire;
using Hangfire.Console;
using Hangfire.Server;
using Performance.Service;

namespace AddictedProxy.Services.Credentials.Job;

public class ResetDownloadCredJob
{
    private readonly ICredentialsService _credentialsService;
    private readonly IPerformanceTracker _performanceTracker;
    private readonly ILogger<ResetDownloadCredJob> _logger;


    public ResetDownloadCredJob(ICredentialsService credentialsService, IPerformanceTracker performanceTracker, ILogger<ResetDownloadCredJob> logger)
    {
        _credentialsService = credentialsService;
        _performanceTracker = performanceTracker;
        _logger = logger;
    }

    [AutomaticRetry(Attempts = 10, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    [UniqueJob(TtlFingerprintSeconds = TimeSpan.SecondsPerHour)]
    [Queue("download-creds-checker")]
    [MaximumConcurrentExecutions(3)]
    public async Task CheckAndResetCredAsync(long credentialId, PerformContext context, CancellationToken token)
    {
        context.WriteLine($"Starting to check and reset credentials for ID {credentialId}");
        using var span = _performanceTracker.BeginNestedSpan(nameof(ResetDownloadCredJob), "check-download-cred");
        try
        {
            await _credentialsService.CheckAndResetCredentialsAsync(credentialId, token);
            context.WriteLine($"Successfully reset credentials for ID {credentialId}");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Couldn't reset credential {CredId}", credentialId);
            context.WriteLine($"Error: Failed to reset credentials for ID {credentialId}");
            span.Finish(e);
            throw;
        }
    }
}