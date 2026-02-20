using AddictedProxy.Services.Credentials.Job;
using AddictedProxy.Services.Provider.Shows.Jobs;
using AddictedProxy.Services.Provider.SuperSubtitles.Config;
using AddictedProxy.Services.Provider.SuperSubtitles.Jobs;
using Hangfire;
using Microsoft.Extensions.Options;

namespace AddictedProxy.Services.Job.Service;

public class SchedulerHostedService : IHostedService
{
    private readonly SuperSubtitlesImportConfig _importConfig;

    public SchedulerHostedService(IOptions<SuperSubtitlesImportConfig> importConfig)
    {
        _importConfig = importConfig.Value;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        RecurringJob.AddOrUpdate<RefreshAvailableShowsJob>(nameof(RefreshAvailableShowsJob), job => job.ExecuteAsync(null, CancellationToken.None), "0 2 * * *");
        RecurringJob.AddOrUpdate<CheckCompletedTmdbJob>($"{nameof(CheckCompletedTmdbJob)}-not-completed", job => job.ExecuteAsync(false, null, CancellationToken.None), "50 2 * * *");
        RecurringJob.AddOrUpdate<CheckCompletedTmdbJob>($"{nameof(CheckCompletedTmdbJob)}-completed", job => job.ExecuteAsync(true, null, CancellationToken.None), "10 2 * * */2");
        RecurringJob.AddOrUpdate<DownloadCredsRedeemerJob>(nameof(DownloadCredsRedeemerJob), job => job.ExecuteAsync(null, CancellationToken.None), "11 * * * *");

        if (_importConfig.EnableRefresh)
        {
            RecurringJob.AddOrUpdate<RefreshSuperSubtitlesJob>(nameof(RefreshSuperSubtitlesJob), job => job.ExecuteAsync(null!, CancellationToken.None), "*/15 * * * *");
        }

        // One-time bulk import from SuperSubtitles (idempotent — skips if already imported)
        if (_importConfig.EnableImport)
        {
            BackgroundJob.Enqueue<ImportSuperSubtitlesJob>(job => job.ExecuteAsync(null!, CancellationToken.None));
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}