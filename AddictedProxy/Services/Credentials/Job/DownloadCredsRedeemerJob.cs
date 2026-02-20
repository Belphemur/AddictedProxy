using Hangfire.Console;
using Hangfire.Server;
using Performance.Service;

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

    public async Task ExecuteAsync(PerformContext context, CancellationToken cancellationToken)
    {
        context.WriteLine("Starting to redeem expired download credentials...");
        using var span = _performanceTracker.BeginNestedSpan(nameof(DownloadCredsRedeemerJob), "redeeming-expired-dl-creds"); 
        await _credentialsService.RedeemDownloadCredentialsAsync(DateTime.UtcNow, cancellationToken);
        context.WriteLine("Finished redeeming download credentials.");
    }
}