using Sentry.Performance.Service;

namespace AddictedProxy.Services.Credentials.Job;

public class DownloadCredsRedeemerJob
{
    private readonly ICredentialsService _credentialsService;
    private readonly IPerformanceTracker _performanceTracker;

    public DownloadCredsRedeemerJob(ICredentialsService credentialsService, IPerformanceTracker performanceTracker)
    {
        _credentialsService = credentialsService;
        _performanceTracker = performanceTracker;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var span = _performanceTracker.BeginNestedSpan(nameof(DownloadCredsRedeemerJob), "redeeming-expired-dl-creds"); 
        await _credentialsService.RedeemDownloadCredentialsAsync(DateTime.UtcNow, cancellationToken);
    }
}