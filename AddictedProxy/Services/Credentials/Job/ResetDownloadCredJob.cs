using AddictedProxy.Services.Job.Filter;
using Hangfire;
using Performance.Service;

namespace AddictedProxy.Services.Credentials.Job;

public class ResetDownloadCredJob
{
    private readonly ICredentialsService _credentialsService;
    private readonly IPerformanceTracker _performanceTracker;


    public ResetDownloadCredJob(ICredentialsService credentialsService, IPerformanceTracker performanceTracker)
    {
        _credentialsService = credentialsService;
        _performanceTracker = performanceTracker;
    }

    [AutomaticRetry(Attempts = 10, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    [UniqueJob(TtlFingerprintSeconds = TimeSpan.SecondsPerHour)]
    [Queue("download-creds-checker")]
    [MaximumConcurrentExecutions(3)]
    public async Task CheckAndResetCredAsync(long credentialId, CancellationToken token)
    {
        using var span = _performanceTracker.BeginNestedSpan(nameof(ResetDownloadCredJob), "check-download-cred");
        await _credentialsService.CheckAndResetCredentialsAsync(credentialId, token);
    }
}